/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Net;
using System.Collections;

namespace UnityEngine {
	#region User-Defined Types
	using UINT = System.UInt32;
	using BYTE = System.Byte;
	using SBYTE = System.SByte;
	using WORD = System.UInt16;
	using DWORD = System.UInt32;
	using QWORD = System.UInt64;
	using ULONG = System.UInt32;
	using ULONG32 = System.UInt32;
	using ULONG64 = System.UInt64;
	using CHAR = System.Byte;
	using INT = System.Int32;
	using INT16 = System.Int16;
	using INT32 = System.Int32;
	using INT64 = System.Int64;
	using UINT16 = System.UInt16;
	using UINT32 = System.UInt32;
	using UINT64 = System.UInt64;
	using LONG32 = System.Int32;
	using LONG64 = System.Int64;
	using FLOAT = System.Single;
	using DOUBLE = System.Double;
	using tick_t = System.UInt64;
	using time_t = System.UInt64;
	using size_t = System.UInt64;
	using wchar_t = System.Char;
	#endregion

	using UnityEngine.UI;

	public class ExampleChat : GameFramework {
		public void
		Awake() {
#if UNITY_STANDALONE
			Screen.SetResolution(360, 640, false);
#else
			Screen.SetResolution(360, 640, true);
#endif

			g_kFramework.Initialize();
		}

		public void
		OnApplicationQuit() {
			g_kFramework.Release();
		}

		public void
		Start() {
			if(0 < ConvertToString(g_kUnitMgr.GetMainPlayer().GetLoginId()).Length) {
				m_kLoginIdInputField.text = ConvertToString(g_kUnitMgr.GetMainPlayer().GetLoginId());
			} else {
				m_kLoginIdInputField.text = DefaultLoginId;
			}

			if(0 <= g_kUnitMgr.GetMainPlayer().GetChannelIndex()) {
				m_kChannelIdInputField.text = (g_kUnitMgr.GetMainPlayer().GetChannelIndex() + 1).ToString();
			} else {
				m_kChannelIdInputField.text = DefaultChannelId.ToString();
			}

			g_kTextMgr.Clear();
			g_kTextMgr.SetChannelTransform(m_kChannelPanel);
			g_kTextMgr.SetUserListTransform(m_kUserPanel);
			g_kTextMgr.SetMessageTransform(m_kMessagePanel);

			REFRESH_CHANNEL_LIST();
			REFRESH_USER_LIST();

			g_kCfgMgr.SetAutoConnect(true);
			if(EnableMainPort) {
				if(EnableReliableUdp) {
					g_kCfgMgr.SetSendType(SEND_TYPE.SEND_SLAVE_MAIN);
				} else {
					g_kCfgMgr.SetSendType(SEND_TYPE.SEND_MASTER_MAIN);
				}
			} else {
				g_kCfgMgr.SetSendType(SEND_TYPE.SEND_MASTER_SUB);
			}

			CNetIO kNetIO = g_kUnitMgr.GetMainPlayer().GetNetIO();
			if(isptr(kNetIO)) {
				kNetIO.SetConnector(g_kNetMgr.GetCurrentConnector());
			}

			Application.runInBackground = true;

			if(STATE_TYPE.STATE_EMPTY == g_kStateMgr.GetTransitionType()) {
				g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
			}
		}

		public void
		Update() {
			g_kStateMgr.Update();
		}

		public void
		OnGUI() {
			if(STATE_TYPE.STATE_LOGIN == g_kStateMgr.GetTransitionType()) {
				m_kAuthorizeButton.GetComponentInChildren<Text>().text = "Login";
			} else {
				m_kAuthorizeButton.GetComponentInChildren<Text>().text = "Relogin";
			}
		}

		public void
		OnAuthorize() {
			CConnector kConnector = g_kNetMgr.GetCurrentConnector();
			if(isptr(kConnector)) {
				GamePlayer kMainPlayer = g_kUnitMgr.GetMainPlayer();
				if(isptr(kMainPlayer)) {
					if(0 < RemoteIpOrDomain.Length) {
						if((0 < MasterMainTcp) && (0 < MasterSubTcp) && (0 < SlaveMainUdp)) {
							if(0 < m_kLoginIdInputField.text.Length) {
								if(0 < m_kChannelIdInputField.text.Length) {
									INT iChannelId = Convert.ToInt32(m_kChannelIdInputField.text);
									if(0 < iChannelId) {
										SChannelInfo tChannelInfo = g_kChannelInfoList.Find((UINT)iChannelId);
										if(0 < tChannelInfo.GetId()) {
											string szRemoteIpAddr = "";
											if(IPAddress.Any == g_kNetMgr.GetCurrentConnector().GetRemoteAddressIn().Address) {
												if(EnableDNS) {
													szRemoteIpAddr = Dns.GetHostAddresses(RemoteIpOrDomain)[0].ToString();
													MESSAGE("enable dns: domain: " + RemoteIpOrDomain + ", ip: " + szRemoteIpAddr);
												} else {
													szRemoteIpAddr = RemoteIpOrDomain;
												}
											} else {
												szRemoteIpAddr = ConvertToString(g_kNetMgr.GetCurrentConnector().GetRemoteAddress());
											}

											INT iPort = 0;
											if(0 < g_kNetMgr.GetCurrentConnector().GetRemotePort()) {
												iPort = g_kNetMgr.GetCurrentConnector().GetRemotePort();
											} else {
												if(g_kCfgMgr.GetSendType() == SEND_TYPE.SEND_MASTER_SUB) {
													iPort = MasterSubTcp;
												} else if(g_kCfgMgr.GetSendType() == SEND_TYPE.SEND_SLAVE_MAIN) {
													iPort = SlaveMainUdp;
												} else {
													iPort = MasterMainTcp;
												}
											}

											//szRemoteIpAddr = "192.168.1.22";
											string szSendType = g_kCfgMgr.GetSendType().ToString();
											MESSAGE("connecting: ip: " + szRemoteIpAddr + ", port: " + iPort + ", type: " + szSendType.Substring(szSendType.IndexOf("_") + 1).Replace("_", ": "));

											if(0 < szRemoteIpAddr.Length) {
												kMainPlayer.SetLoginId(ConvertToBytes(m_kLoginIdInputField.text));
												kMainPlayer.SetChannelIndex((INT)tChannelInfo.GetIndex());

												kConnector.SetRemoteAddress(ConvertToBytes(szRemoteIpAddr), iPort);

												if(0 < g_kChannelMgr.GetMainRoom().GetId()) {
													g_kChannelMgr.GetMainRoom().Clear();
												}

												if(0 < g_kUnitMgr.GetMainPlayer().GetAid()) {
													g_kNetMgr.Relogin();
												} else {
													g_kNetMgr.Login();
												}
												return;
											} else {
												MESSAGE("critical error: server ip address is empty.");
											}
										} else {
											MESSAGE("error: channel id was not found.");
										}
									} else {
										MESSAGE("error: please enter an channel id greater than zero.");
									}
								} else {
									MESSAGE("error: channel id is empty.");
								}
							} else {
								MESSAGE("error: login Id is empty.");
							}
						} else {
							MESSAGE("error: server port is empty.");
						}
					} else {
						MESSAGE("error: server domain is empty.");
					}
				} else {
					MESSAGE("critical error: main player is null.");
				}
			} else {
				MESSAGE("critical error: main connector is null.");
			}
		}

		public void
		OnChange() {
			if(STATE_TYPE.STATE_CHANNEL <= g_kStateMgr.GetTransitionType()) {
				GamePlayer kMainPlayer = g_kUnitMgr.GetMainPlayer();
				if(isptr(kMainPlayer)) {
					if(0 < m_kChannelIdInputField.text.Length) {
						INT iChannelId = Convert.ToInt32(m_kChannelIdInputField.text);
						if(0 < iChannelId) {
							SChannelInfo tChannelInfo = g_kChannelInfoList.Find((UINT)iChannelId);
							if(0 < tChannelInfo.GetId()) {
								if(STATE_TYPE.STATE_CHANNEL != g_kStateMgr.GetTransitionType()) {
									g_kStateMgr.SetTransition(STATE_TYPE.STATE_CHANNEL);
								}

								if(0 < g_kChannelMgr.GetMainRoom().GetId()) {
									g_kChannelMgr.GetMainRoom().Clear();
								}

								kMainPlayer.SetChannelIndex((INT)tChannelInfo.GetIndex());

								SEND_INFO_CHANNEL();
							} else {
								MESSAGE("error: channel id was not found.");
							}
						} else {
							MESSAGE("error: please enter an channel id greater than zero.");
						}
					} else {
						MESSAGE("error: channel id is empty.");
					}
				} else {
					MESSAGE("critical error: main player is null.");
				}
			}
		}

		public void
		OnChannelList() {
			if(STATE_TYPE.STATE_CHANNEL <= g_kStateMgr.GetTransitionType()) {
				REFRESH_CHANNEL_LIST();
				MESSAGE("refreshing channel list.");
			}
		}

		public void
		OnUserList() {
			if(STATE_TYPE.STATE_CHANNEL <= g_kStateMgr.GetTransitionType()) {
				SEND_INFO_USER_LIST();
				MESSAGE("refreshing user list.");
			}
		}

		public void
		OnSend() {
			if(STATE_TYPE.STATE_CHANNEL <= g_kStateMgr.GetTransitionType()) {
				if(0 < m_kChatInputField.text.Length) {
					if(iMAX_CHAT_LEN > ConvertToBytes(m_kChatInputField.text).Length) {
						if(0 < g_kUnitMgr.GetMainPlayer().GetKey()) {
							SEND_USER_CHAT(CHAT_TYPE.CHAT_NORMAL, m_kChatInputField.text);
						}
					} else {
						MESSAGE("chat messages too long.");
					}
				} else {
					MESSAGE("please enter your messages.");
				}
			}
		}

		public void
		OnMenu() {
			Application.LoadLevel((INT)SCENE_TYPE.SCENE_MENU);
		}

		//
		// Unity Editor Variables
		//
		public bool			EnableDNS;
		public string		RemoteIpOrDomain;

		public INT			MasterMainTcp;
		public INT			MasterSubTcp;
		public INT			SlaveMainUdp;

		public bool			EnableMainPort;
		public bool			EnableReliableUdp;

		public string		DefaultLoginId;
		public INT			DefaultChannelId;

		//
		// Member Variables
		//
		public Button		m_kAuthorizeButton;
		public Button		m_kChangeButton;

		public Button		m_kMenuButton;

		public InputField	m_kLoginIdInputField;
		public InputField	m_kChannelIdInputField;
		public InputField	m_kChatInputField;

		public Transform	m_kIdentityPanel;
		public Transform	m_kChannelPanel;
		public Transform	m_kUserPanel;
		public Transform	m_kMessagePanel;
		public Transform	m_kChatPanel;

	}
}

/* EOF */
