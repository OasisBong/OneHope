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

	public class ExampleRoom : GameFramework {
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

			m_kRoomNameInputField.text = "enjoy your time";
			m_kRoomStageIdInputField.text = "1";
			m_kRoomMaxUserInputField.text = iMAX_ROOM_MEMBERS.ToString();

			g_kTextMgr.Clear();
			g_kTextMgr.SetChannelTransform(m_kChannelPanel);
			g_kTextMgr.SetUserListTransform(m_kUserPanel);
			g_kTextMgr.SetRoomListTransform(m_kRoomPanel);
			g_kTextMgr.SetMemberTransform(m_kMemberPanel);
			g_kTextMgr.SetMessageTransform(m_kMessagePanel);

			REFRESH_CHANNEL_LIST();
			REFRESH_USER_LIST();
			REFRESH_ROOM_LIST();
			REFRESH_MEMBER_LIST();

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

			if(STATE_TYPE.STATE_ROOM == g_kStateMgr.GetTransitionType()) {
				if(isptr(g_kChannelMgr.GetMainRoom().GetLeader())) {
					if(g_kUnitMgr.GetMainPlayer() == g_kChannelMgr.GetMainRoom().GetLeader()) {
						if(g_kChannelMgr.GetMainRoom().IsDoing()) {
							m_kGameButton.GetComponentInChildren<Text>().text = "Stop";
							m_kGameButton.interactable = true;
						} else {
							m_kGameButton.GetComponentInChildren<Text>().text = "Start";

							bool bCheck = true;
							if(1 < g_kChannelMgr.GetMainRoom().GetTopCount()) {
								for(UINT i=0; i < (UINT)g_kChannelMgr.GetMainRoom().GetTopCount(); ++i) {
									GamePlayer kPlayer = g_kChannelMgr.GetMainRoom().GetMember(i);
									if(isptr(kPlayer)) {
										if(STATUS_TYPE.STATUS_WAITING == kPlayer.GetStatus()) {
											bCheck = false;
											break;
										}
									}
								}
							} else {
								bCheck = false;
							}

							if(bCheck) {
								m_kGameButton.interactable = true;
							} else {
								m_kGameButton.interactable = false;
							}
						}
					} else {
						if(g_kChannelMgr.GetMainRoom().IsDoing()) {
							if(STATUS_TYPE.STATUS_WAITING == g_kUnitMgr.GetMainPlayer().GetStatus()) {
								m_kGameButton.GetComponentInChildren<Text>().text = "Start";
								m_kGameButton.interactable = true;

							} else {
								m_kGameButton.GetComponentInChildren<Text>().text = "Play";
								m_kGameButton.interactable = false;
							}
						} else {
							if(STATUS_TYPE.STATUS_READY == g_kUnitMgr.GetMainPlayer().GetStatus()) {
								m_kGameButton.GetComponentInChildren<Text>().text = "Waiting";
							} else {
								m_kGameButton.GetComponentInChildren<Text>().text = "Ready";
							}
							m_kGameButton.interactable = true;
						}
					}
				}
			} else {
				m_kGameButton.GetComponentInChildren<Text>().text = "Game";
				m_kGameButton.interactable = false;
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
											//szRemoteIpAddr = "192.168.1.32";
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
		OnRoomList() {
			if(STATE_TYPE.STATE_LOBBY <= g_kStateMgr.GetTransitionType()) {
				SEND_ROOM_LIST();
				MESSAGE("refreshing room list.");
			}
		}

		public void
		OnMemberList() {
			if(STATE_TYPE.STATE_ROOM <= g_kStateMgr.GetTransitionType()) {
				SEND_ROOM_MEMBER_LIST();
				MESSAGE("refreshing room member list.");
			}
		}

		public void
		OnCreate() {
			if(STATE_TYPE.STATE_LOBBY <= g_kStateMgr.GetTransitionType()) {
				if(0 < m_kRoomNameInputField.text.Length) {
					if(0 < m_kRoomStageIdInputField.text.Length) {
						UINT uiRoomStageId = Convert.ToUInt32(m_kRoomStageIdInputField.text);
						if(0 < uiRoomStageId) {
							SStageInfo tStageInfo = g_kStageInfoList.Find(uiRoomStageId);
							if(0 < tStageInfo.GetId()) {
								if(0 < m_kRoomMaxUserInputField.text.Length) {
									UINT uiRoomMaxUser = Convert.ToUInt32(m_kRoomMaxUserInputField.text);
									if((0 < uiRoomMaxUser) && (uiRoomMaxUser <= (UINT)iMAX_ROOM_MEMBERS)) {
										if(0 < g_kChannelMgr.GetMainRoom().GetId()) {
											g_kChannelMgr.GetMainRoom().Clear();
										}

										g_kChannelMgr.GetMainRoom().SetStageId(uiRoomStageId);
										g_kChannelMgr.GetMainRoom().SetMaxUser(uiRoomMaxUser);
										g_kChannelMgr.GetMainRoom().SetName(ConvertToBytes(m_kRoomNameInputField.text));

										SEND_ROOM_CREATE();
									} else {
										MESSAGE("error: please enter an max user greater than zero: max: " + iMAX_ROOM_MEMBERS);
									}
								} else {
									MESSAGE("error: max user is empty.");
								}
							} else {
								MESSAGE("error: stage id was not found.");
							}
						} else {
							MESSAGE("error: please enter an stage id greater than zero.");
						}
					} else {
						MESSAGE("error: stage id is empty.");
					}
				} else {
					MESSAGE("error: room name is empty.");
				}
			}
		}

		public void
		OnJoin() {
			if(STATE_TYPE.STATE_LOBBY <= g_kStateMgr.GetTransitionType()) {
				if(0 < m_kRoomIdInputField.text.Length) {
					UINT uiRoomId = Convert.ToUInt32(m_kRoomIdInputField.text);
					if(0 < uiRoomId) {
						UINT uiRoomChannelId = (UINT)((Convert.ToUInt16(uiRoomId) >> 10) & 0x3F);
						SChannelInfo tChannelInfo = g_kChannelInfoList.Find(uiRoomChannelId);
						if(0 < tChannelInfo.GetId()) {
							if((INT)tChannelInfo.GetIndex() != g_kUnitMgr.GetMainPlayer().GetChannelIndex()) {
								g_kUnitMgr.GetMainPlayer().SetChannelIndex((INT)tChannelInfo.GetIndex());
								m_kChannelIdInputField.text = tChannelInfo.GetId().ToString();
							}

							if(0 < g_kChannelMgr.GetMainRoom().GetId()) {
								g_kChannelMgr.GetMainRoom().Clear();
							}

							SEND_ROOM_JOIN(uiRoomId);
						} else {
							MESSAGE("error: room id is not valid.");
						}
					} else {
						MESSAGE("error: please enter an room id greater than zero.");
					}
				} else {
					MESSAGE("error: room id is empty.");
				}
			}
		}

		public void
		OnLeave() {
			if(STATE_TYPE.STATE_ROOM <= g_kStateMgr.GetTransitionType()) {
				if(g_kChannelMgr.GetMainRoom().IsAvailable()) {
					SEND_ROOM_LEAVE();
				}
			}
		}

		public void
		OnGame() {
			if(STATE_TYPE.STATE_ROOM <= g_kStateMgr.GetTransitionType()) {
				if(isptr(g_kChannelMgr.GetMainRoom().GetLeader())) {
					if(g_kUnitMgr.GetMainPlayer() == g_kChannelMgr.GetMainRoom().GetLeader()) {
						if(g_kChannelMgr.GetMainRoom().IsDoing()) {
							SEND_ROOM_STOP();
						} else {
							SEND_ROOM_START();
						}
					} else {
						if(g_kChannelMgr.GetMainRoom().IsDoing()) {
							if(STATUS_TYPE.STATUS_WAITING == g_kUnitMgr.GetMainPlayer().GetStatus()) {
								SEND_ROOM_START();
							}
						} else {
							if(STATUS_TYPE.STATUS_READY == g_kUnitMgr.GetMainPlayer().GetStatus()) {
								SEND_USER_STATUS_WAITING();
							} else {
								SEND_USER_STATUS_READY();
							}
						}
					}
				}
			}
		}

		public void
		OnDisconnect() {
			if(g_kNetMgr.IsConnected()) {
				g_kNetMgr.DisconnectAll();

				if(g_kCfgMgr.IsAutoConnected()) {
					MESSAGE("please wait before reconnecting: tick: " + iMAX_RECONNECT_LATENCY_TICK);
				} else {
					MESSAGE("auto reconnecting switch was not checked.");
				}
			} else {
				MESSAGE("network manager was not connected.");
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

		public Button		m_kCreateButton;
		public Button		m_kJoinButton;

		public Button		m_kGameButton;
		public Button		m_kLeaveButton;

		public Button		m_kMenuButton;

		public InputField	m_kLoginIdInputField;
		public InputField	m_kChannelIdInputField;
		public InputField	m_kRoomNameInputField;
		public InputField	m_kRoomStageIdInputField;
		public InputField	m_kRoomMaxUserInputField;
		public InputField	m_kRoomIdInputField;

		public Transform	m_kIdentityPanel;
		public Transform	m_kChannelPanel;
		public Transform	m_kUserPanel;
		public Transform	m_kLobbyPanel;
		public Transform	m_kRoomPanel;
		public Transform	m_kMemberPanel;
		public Transform	m_kControlPanel;
		public Transform	m_kMessagePanel;

	}
}

/* EOF */
