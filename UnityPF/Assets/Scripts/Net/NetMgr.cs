/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

	public partial class GameFramework {
		public static CNetMgr		g_kNetMgr		{ get { return CNetMgr.GetInstance(); } }

		public class CNetMgr : CSingleton<CNetMgr> {
			public CNetMgr() {}
			~CNetMgr() { /* Release(); */ }

			public bool
			Initialize(INT iMaxUser_) {
				if(false == m_bInitialized) {
					GameObject kNetworksObject = new GameObject("Networks");
					m_kCheckProtocolText = kNetworksObject.AddComponent<Text>();
					DontDestroyOnLoad(kNetworksObject);

					GameObject kMasterObject = new GameObject("Master");
					m_kMasterNetwork = kMasterObject.AddComponent<GameNetwork>();
					kMasterObject.transform.SetParent(kNetworksObject.transform);
					DontDestroyOnLoad(kMasterObject);

					GameObject kSlaveObject = new GameObject("Slave");
					m_kSlaveNetwork = kSlaveObject.AddComponent<GameNetwork>();
					kSlaveObject.transform.SetParent(kNetworksObject.transform);
					DontDestroyOnLoad(kSlaveObject);

					GameObject kMasterQueueObject = new GameObject("Queue (Native)");
					m_kMasterQueueText = kMasterQueueObject.AddComponent<Text>();
					kMasterQueueObject.transform.SetParent(kMasterObject.transform);
					DontDestroyOnLoad(kMasterQueueObject);

					GameObject kSlaveQueueObject = new GameObject("Queue (Native)");
					m_kSlaveQueueText = kSlaveQueueObject.AddComponent<Text>();
					kSlaveQueueObject.transform.SetParent(kSlaveObject.transform);
					DontDestroyOnLoad(kSlaveQueueObject);

					GameObject kMasterMainConnectorObject = new GameObject("Connector (Main:TCP)");
					m_kMasterMainConnectorText = kMasterMainConnectorObject.AddComponent<Text>();
					kMasterMainConnectorObject.transform.SetParent(kMasterObject.transform);
					DontDestroyOnLoad(kMasterMainConnectorObject);

					GameObject kMasterSubConnectorObject = new GameObject("Connector (Sub:TCP)");
					m_kMasterSubConnectorText = kMasterSubConnectorObject.AddComponent<Text>();
					kMasterSubConnectorObject.transform.SetParent(kMasterObject.transform);
					DontDestroyOnLoad(kMasterSubConnectorObject);

					GameObject kSlaveMainConnectorObject = new GameObject("Connector (Main:UDP)");
					m_kSlaveMainConnectorText = kSlaveMainConnectorObject.AddComponent<Text>();
					kSlaveMainConnectorObject.transform.SetParent(kSlaveObject.transform);
					DontDestroyOnLoad(kSlaveMainConnectorObject);

					GameObject kSlaveMainListenerObject = new GameObject("Listener (Main:UDP)");
					m_kSlaveMainListenerText = kSlaveMainListenerObject.AddComponent<Text>();
					kSlaveMainListenerObject.transform.SetParent(kSlaveObject.transform);
					DontDestroyOnLoad(kSlaveMainListenerObject);

					bool bCheck = true;

					g_kConnectorProvider.Register((INT)NETWORK_TYPE.NETWORK_UDP_CLIENT, new CreateCallback(CReliableConnector.New));
					if(false == Action("initializing slave network", GetSlaveNetwork().Initialize(SENSOR_TYPE.SENSOR_POLL, COMMAND_TYPE.COMMAND_NATIVE, (UINT)iMaxUser_ + 5, NETWORK_TYPE.NETWORK_UDP_CLIENT))) {
						bCheck = false;
					}

					g_kConnectorProvider.Register((INT)NETWORK_TYPE.NETWORK_TCP_CLIENT, new CreateCallback(CDefaultConnector.New));
					if(false == Action("initializing master network", GetMasterNetwork().Initialize(SENSOR_TYPE.SENSOR_POLL, COMMAND_TYPE.COMMAND_NATIVE, 5, NETWORK_TYPE.NETWORK_TCP_CLIENT))) {
						bCheck = false;
					}

					if(bCheck) {
						if(Action("creating connector for master service", isptr(m_kMasterSubConnector = GetMasterNetwork().Create(ConvertToBytes("0.0.0.0"), 0, NETWORK_TYPE.NETWORK_TCP_CLIENT)))) {
							if(g_kCfgMgr.IsHeaderCrypt()) {
								m_kMasterSubConnector.SetHeaderCrypt(true);
							}

							m_kMasterMainConnector = (CConnector)g_kConnectorProvider.Create((INT)NETWORK_TYPE.NETWORK_TCP_CLIENT);
							m_kMasterMainConnector.CreateSocket();
							if(GetMasterNetwork().ChangeConnector(m_kMasterMainConnector)) {
								if(g_kCfgMgr.IsHeaderCrypt()) {
									m_kMasterMainConnector.SetHeaderCrypt(true);
								}

								if(Action("creating listener for slave service", isptr(m_kSlaveMainListener = GetSlaveNetwork().Create(ConvertToBytes("0.0.0.0"), 0, NETWORK_TYPE.NETWORK_UDP_SERVER)))) {
									IDispatcherList kDispatcherList = GetSlaveNetwork().GetDispatcherList();
									if(isptr(kDispatcherList)) {
										if(g_kCfgMgr.IsHeaderCrypt()) {
											kDispatcherList.SetHeaderCrypt(true);
										}

										m_kSlaveMainConnector = GetSlaveNetwork().Empty();
										if(isptr(m_kSlaveMainConnector)) {
											m_kSlaveMainConnector.SetSocket(m_kSlaveMainListener.GetSocket());
										}

										m_bInitialized = true;
										return true;
									}
								}
							}
						}
					}
				}
				return false;
			}

			public bool
			Release() {
				if(m_bInitialized) {
					if(isptr(m_kSlaveMainListenerText)) {
						Destroy(m_kSlaveMainListenerText.gameObject);
					}

					if(isptr(m_kSlaveMainConnectorText)) {
						Destroy(m_kSlaveMainConnectorText.gameObject);
					}

					if(isptr(m_kMasterSubConnectorText)) {
						Destroy(m_kMasterSubConnectorText.gameObject);
					}

					if(isptr(m_kMasterMainConnectorText)) {
						Destroy(m_kMasterMainConnectorText.gameObject);
					}

					if(isptr(m_kSlaveQueueText)) {
						Destroy(m_kSlaveQueueText.gameObject);
					}

					if(isptr(m_kMasterQueueText)) {
						Destroy(m_kMasterQueueText.gameObject);
					}

					if(isptr(m_kSlaveNetwork)) {
						Destroy(m_kSlaveNetwork.gameObject);
					}

					if(isptr(m_kMasterNetwork)) {
						Destroy(m_kMasterNetwork.gameObject);
					}

					if(isptr(m_kCheckProtocolText)) {
						Destroy(m_kCheckProtocolText.gameObject);
					}

					m_kCookieContainer = null;

					m_kSlaveMainListener = null;
					m_kSlaveMainConnector = null;
					m_kMasterSubConnector = null;
					m_kMasterMainConnector = null;

					m_kSlaveNetwork = null;
					m_kMasterNetwork = null;

					m_iConnectingStep = 0;
					m_bInitialized = false;

					return true;
				}
				return false;
			}

			public bool
			Update() {
				if(GetMasterMainConnector().IsConnected()) {
					SetMasterMainConnectorText("Connected: ip: " + ConvertToString(GetMasterMainConnector().GetRemoteAddress()) + ", port: " + GetMasterMainConnector().GetRemotePort());
				} else {
					SetMasterMainConnectorText("Disconnected");
				}

				if(GetMasterSubConnector().IsConnected()) {
					SetMasterSubConnectorText("Connected: ip: " + ConvertToString(GetMasterSubConnector().GetRemoteAddress()) + ", port: " + GetMasterSubConnector().GetRemotePort());
				} else {
					SetMasterSubConnectorText("Disconnected");
				}

				if(GetSlaveMainConnector().IsConnected()) {
					SetSlaveMainConnectorText("Connected: ip: " + ConvertToString(GetSlaveMainConnector().GetRemoteAddress()) + ", port: " + GetSlaveMainConnector().GetRemotePort());
				} else {
					SetSlaveMainConnectorText("Disconnected");
				}

				GetMasterNetwork().WaitEvent(100);
				GetSlaveNetwork().WaitEvent(100);

				GetMasterNetwork().SendQueue();
				GetSlaveNetwork().SendQueue();

				if(false == GetMasterNetwork().GetCommandQueue().Empty()) {
					SetMasterQueueText(GetMasterNetwork().GetCommandQueue().Size(), "Taking");

					CNativeCommand kNativeCommand = null;
					INT iCount = 0;
					while(isptr(kNativeCommand = (CNativeCommand)(GetMasterNetwork().GetCommandQueue().Take()))) {
						GetMasterNetwork().ParseCommand(kNativeCommand);
						SAFE_DELETE(ref kNativeCommand);
						++iCount;
						if(200 < iCount) {
							break;
						}
					}
				} else {
					SetMasterQueueText(GetMasterNetwork().GetCommandQueue().Size(), "Empty");
				}

				if(false == GetSlaveNetwork().GetCommandQueue().Empty()) {
					SetSlaveQueueText(GetSlaveNetwork().GetCommandQueue().Size(), "Taking");

					CNativeCommand kNativeCommand = null;
					INT iCount = 0;
					while(isptr(kNativeCommand = (CNativeCommand)(GetSlaveNetwork().GetCommandQueue().Take()))) {
						GetSlaveNetwork().ParseCommand(kNativeCommand);
						SAFE_DELETE(ref kNativeCommand);
						++iCount;
						if(200 < iCount) {
							break;
						}
					}
				} else {
					SetSlaveQueueText(GetSlaveNetwork().GetCommandQueue().Size(), "Empty");
				}
				return true;
			}

			public INT
			Send(CCommand kCommand_, INT iSize_ = 0, PACKET_TYPE ePacketType_ =PACKET_TYPE.PACKET_TRUST, CRYPT_TYPE eCryptType_ =CRYPT_TYPE.CRYPT_NONE) {
				switch(g_kCfgMgr.GetSendType()) {
				case SEND_TYPE.SEND_MASTER_SUB:
					return SendMasterSub(kCommand_, iSize_, ePacketType_, eCryptType_);
				case SEND_TYPE.SEND_SLAVE_MAIN:
					// Reliable UDP
					return SendSlaveMain(kCommand_, iSize_, ePacketType_, eCryptType_);
				default:
					return SendMasterMain(kCommand_, iSize_, ePacketType_, eCryptType_);
				}
			}

			public HttpStatusCode
			Send(string szUrl_, string szData_ ="", bool bPost_ =false) {
				return SendOverHttp(szUrl_, szData_, bPost_);
			}

			private INT
			SendMasterMain(CCommand kCommand_, INT iSize_ = 0, PACKET_TYPE ePacketType_ =PACKET_TYPE.PACKET_TRUST, CRYPT_TYPE eCryptType_ =CRYPT_TYPE.CRYPT_NONE) {
				if(isptr(GetMasterMainConnector())) {
					INT iRet = GetMasterMainConnector().Send(kCommand_, iSize_, ePacketType_, eCryptType_);
					if(0 > iRet) {
						GetMasterMainConnector().SetRelay(-1);
						DisconnectAll();
					}
					return iRet;
				}
				return -1;
			}

			private INT
			SendMasterSub(CCommand kCommand_, INT iSize_ = 0, PACKET_TYPE ePacketType_ =PACKET_TYPE.PACKET_TRUST, CRYPT_TYPE eCryptType_ =CRYPT_TYPE.CRYPT_NONE) {
				if(isptr(GetMasterSubConnector())) {
					INT iRet = GetMasterSubConnector().Send(kCommand_, iSize_, ePacketType_, eCryptType_);
					//TRACE("send: master sub: size: " + iSize_ + ", ret: " + iRet + ", packet type: " + ePacketType_ + ", crypt type: " + eCryptType_);
					if(0 > iRet) {
						GetMasterSubConnector().SetRelay(-1);
						GetMasterSubConnector().Disconnect(GetMasterSubConnector().GetSocket());
					}
					return iRet;
				}
				return -1;
			}

			public INT
			SendSlaveMain(CCommand kCommand_, INT iSize_ = 0, PACKET_TYPE ePacketType_ =PACKET_TYPE.PACKET_TRUST, CRYPT_TYPE eCryptType_ =CRYPT_TYPE.CRYPT_NONE) {
				if(isptr(GetSlaveMainConnector())) {
					INT iRet = GetSlaveMainConnector().Send(kCommand_, iSize_, ePacketType_, eCryptType_);
					//TRACE("send: slave main: size: " + iSize_ + ", ret: " + iRet + ", packet type: " + ePacketType_ + ", crypt type: " + eCryptType_);
					if(0 > iRet) {
						GetSlaveMainConnector().SetRelay(-1);
						GetSlaveMainConnector().Disconnect(GetSlaveMainConnector().GetSocket());
					}
					return iRet;
				}
				return -1;
			}

			public HttpStatusCode
			SendOverHttp(string szUrl_, string szData_, bool bPost_ =false) {
				HttpWebRequest kRequest = null;
				HttpStatusCode eStatus = HttpStatusCode.InternalServerError;

				try {
#if !DEBUG
					if(0 < szData_.Length) {
						if(bPost_) {
							szUrl_ = szUrl_ + "?release";
						} else {
							szUrl_ = szUrl_ + "?release&" + szData_;
						}
					} else {
						szUrl_ = szUrl_ + "?release";
					}
#else
					if(0 < szData_.Length) {
						if(bPost_) {
							szUrl_ = szUrl_ + "?debug";
						} else {
							szUrl_ = szUrl_ + "?debug" + szData_;
						}
					} else {
						szUrl_ = szUrl_ + "?debug";
					}
#endif

					if(0 < szData_.Length) {
						if(bPost_) {
							BYTE[] byData = ConvertToBytes(szData_);

							kRequest = (HttpWebRequest)WebRequest.Create(szUrl_);
							kRequest.CookieContainer = m_kCookieContainer;
							kRequest.Method = "POST";
							kRequest.ContentType = "application/x-www-form-urlencoded";
							kRequest.ContentLength = byData.Length;

							Stream kStream = kRequest.GetRequestStream();
							kStream.Write(byData, 0, byData.Length);
							kStream.Close();
						} else {
							kRequest = (HttpWebRequest)WebRequest.Create(szUrl_);
							kRequest.CookieContainer = m_kCookieContainer;
							kRequest.Method = "GET";
							TRACE("GET: data: url: " + szUrl_);
						}
					} else {
						kRequest = (HttpWebRequest)WebRequest.Create(szUrl_);
						kRequest.CookieContainer = m_kCookieContainer;
						kRequest.Method = "GET";
						TRACE("GET: no data: url: " + szUrl_);
					}

					if(isptr(kRequest)) {
						using(HttpWebResponse kResponse = (HttpWebResponse)kRequest.GetResponse()) {
							eStatus = kResponse.StatusCode;
							INT64 liLength = kResponse.ContentLength;

							if(0 < kResponse.Cookies.Count) {
								foreach(Cookie kCookie in kResponse.Cookies) {
									MESSAGE("cookie: name: " + kCookie.Name + ", value: " + kCookie.Value + ", path: " + kCookie.Path + ", domain: " + kCookie.Domain + ", expires: " + kCookie.Expires);
								}
							}

							Stream kStream = kResponse.GetResponseStream();
							StreamReader kReader = new StreamReader(kStream, Encoding.UTF8);

							string szProtocol = "NDP over HTTP";
							string szKey = "";
							string szValue = "";

							string[] szToken = null;
							bool bCheck = false;

							string szBuffer = "";
							//INT iCount = 0;
							while((szBuffer = kReader.ReadLine()) != null) {
								if(0 < szBuffer.Length) {
									if(false == bCheck) {
										if(szBuffer.ToCharArray()[0] == '@') {
											szBuffer = szBuffer.Substring(szBuffer.IndexOf("@") + 1).Trim();
											szKey = szBuffer.Substring(0, szBuffer.LastIndexOf(" ")).Trim();
											if(0 == String.Compare(szKey, szProtocol)) {
												szValue = szBuffer.Replace(szProtocol, "").Trim();

												bCheck = true;
												MESSAGE("protocol: " + szKey + ", version: " + szValue + ", length: " + liLength);
											}
										} else {
											MESSAGE(szBuffer);
											//MESSAGE(iCount + ": " + szBuffer);
										}
									} else {
										if((CHAR)'#' == szBuffer.ToCharArray()[0]) {
											TRACE(szBuffer.Substring(szBuffer.IndexOf("#") + 1).Trim());
										} else if((CHAR)'-' == szBuffer.ToCharArray()[0]) {
											OUTPUT(szBuffer.Substring(szBuffer.IndexOf("-") + 1).Trim());
										} else if((CHAR)'+' == szBuffer.ToCharArray()[0]) {
											string szClear = szBuffer.Substring(szBuffer.IndexOf("+") + 1);
											szClear = szClear.Replace("\n", "");
											szClear = szClear.Replace("\r", "");
											szClear = szClear.Trim();

											string szHeader = "";
											string szData = "";
											szToken = szClear.Split(new char[] {'&'});
											if(isptr(szToken)) {
												if(0 < szToken.Length) {
													szHeader = szToken[0];
													if(0 == String.Compare("header=", 0, szHeader, 0, 7)) {
														szHeader = szHeader.Replace("header=", "").Trim();
													}

													if(1 < szToken.Length) {
														szData = szToken[1];
														if(0 == String.Compare("data=", 0, szData, 0, 5)) {
															szData = szData.Replace("data=", "").Trim();
														}
													}
												}
											}

											MESSAGE("header: " + szHeader + ", data: " + szData);

											if(0 < szHeader.Length) {
												CCommand kCommand = new CCommand();

												CHAR[] acHeader = Base64Decode(szHeader);
												kCommand.SetHeader(acHeader);

												INT iDataSize = 0;
												if(0 < szData.Length) {
													CHAR[] acData = Base64Decode(szData);
													iDataSize = acData.Length;
													kCommand.SetData(acData, iDataSize);
												}

												MESSAGE("order: " + kCommand.GetOrder() + ", extra: " + kCommand.GetExtra() + ", option: " + kCommand.GetOption() + ", mission: " + kCommand.GetMission());

												CConnector kConnector = GetCurrentConnector();
												if(isptr(kConnector)) {
													kConnector.Queuing(kCommand, iDataSize, CRYPT_TYPE.CRYPT_NONE);
												}
											}
										} else if((CHAR)'?' == szBuffer.ToCharArray()[0]) {
											ERROR(szBuffer.Substring(szBuffer.IndexOf("?") + 1).Trim());
										} else {
											MESSAGE(szBuffer);
											//MESSAGE(iCount + ": " + szBuffer);
										}
									}
								}
								//++iCount;
							}
							kReader.Close();
							kStream.Close();
						}
					}
				} catch(WebException e) {
					HttpWebResponse kResponse = (HttpWebResponse)e.Response;
					if(isptr(kResponse)) {
						eStatus = kResponse.StatusCode;
					}
				}
				return eStatus;
			}

			public bool
			Login() {
				if(SEND_TYPE.SEND_SLAVE_MAIN == g_kCfgMgr.GetSendType()) {
					GetSlaveNetwork().StartLogin();
				} else {
					GetMasterNetwork().StartLogin();
				}
				return true;
			}

			public bool
			Relogin() {
				if(SEND_TYPE.SEND_SLAVE_MAIN == g_kCfgMgr.GetSendType()) {
					GetSlaveNetwork().StartRelogin();
				} else {
					GetMasterNetwork().StartRelogin();
				}
				return true;
			}

			public bool
			Reconnect() {
				if(SEND_TYPE.SEND_SLAVE_MAIN == g_kCfgMgr.GetSendType()) {
					GetSlaveNetwork().StartReconnect();
				} else {
					GetMasterNetwork().StartReconnect();
				}
				return true;
			}

			public void
			DisconnectAll() {
				TRACE("disconnect: all: ");
				// Relay는 아직 사용 안합니다.
				// 서버에서 1로 임시 고정.
				if(isptr(m_kMasterMainConnector)) {
					if(m_kMasterMainConnector.IsConnected()) {
						m_kMasterMainConnector.Disconnect(m_kMasterMainConnector.GetSocket());
					}
					m_kMasterMainConnector.SetRelay(-1);
					m_kMasterMainConnector.SetSerialKey(0);
				}

				if(isptr(m_kMasterSubConnector)) {
					if(m_kMasterSubConnector.IsConnected()) {
						m_kMasterSubConnector.Disconnect(m_kMasterSubConnector.GetSocket());
					}
					m_kMasterSubConnector.SetRelay(-1);
					m_kMasterSubConnector.SetSerialKey(0);
				}

				if(isptr(m_kSlaveMainConnector)) {
					if(m_kSlaveMainConnector.IsConnected()) {
						m_kSlaveMainConnector.Disconnect(m_kSlaveMainConnector.GetSocket());

						if(isptr(m_kSlaveMainListener)) {
							if(isptr(m_kSlaveMainListener.GetSocket())) {
								m_kSlaveMainListener.GetSocket().Close();
							}
							m_kSlaveMainListener.Disconnect(m_kSlaveMainListener.GetSocket());
							m_kSlaveMainListener.Listen(ConvertToBytes("0.0.0.0"), 0);
							m_kSlaveMainConnector.SetSocket(m_kSlaveMainListener.GetSocket());
						}
					}
					m_kSlaveMainConnector.SetRelay(-1);
					m_kSlaveMainConnector.SetSerialKey(0);
				}

				Thread.Sleep(0);
			}

			public bool
			IsConnected() {
				switch(g_kCfgMgr.GetSendType()) {
				case SEND_TYPE.SEND_MASTER_SUB:
					return GetMasterSubConnector().IsConnected();
				case SEND_TYPE.SEND_SLAVE_MAIN:
					return GetSlaveMainConnector().IsConnected();
				default:
					return GetMasterMainConnector().IsConnected();
				}
			}

			public bool
			CheckProtocol() {
				InitializeCommand();

				if(isptr(m_kCheckProtocolText)) {
					bool bCheck = true;

					StringBuilder kBuilder = new StringBuilder();
					kBuilder.AppendLine("iMAX_PACKET_SIZE: " + iMAX_PACKET_SIZE);
					kBuilder.AppendLine("iTCP_PACKET_HEAD_SIZE: " + iTCP_PACKET_HEAD_SIZE);
					kBuilder.AppendLine("iUDP_PACKET_HEAD_SIZE: " + iUDP_PACKET_HEAD_SIZE);
					kBuilder.AppendLine("iCOMMAND_HEAD_SIZE: " + iCOMMAND_HEAD_SIZE);
					kBuilder.AppendLine("iCOMMAND_DATA_SIZE: " + iCOMMAND_DATA_SIZE);
					kBuilder.AppendLine("iPACKET_DATA_SIZE: " + iPACKET_DATA_SIZE);
					kBuilder.AppendLine("iMAX_PACKET_INFO_USER_LIST: " + iMAX_PACKET_INFO_USER_LIST);
					kBuilder.AppendLine("iMAX_PACKET_ROOM_LIST: " + iMAX_PACKET_ROOM_LIST);
					kBuilder.AppendLine("iMAX_PACKET_ROOM_MEMBER_LIST: " + iMAX_PACKET_ROOM_MEMBER_LIST);

					kBuilder.Append("SIdAuthorizeGsToCl: " + Marshal.SizeOf(typeof(SIdAuthorizeGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SIdAuthorizeGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SIdAuthorizeClToGs: " + Marshal.SizeOf(typeof(SIdAuthorizeClToGs)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SIdAuthorizeClToGs))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SIdPongGsToCl: " + Marshal.SizeOf(typeof(SIdPongGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SIdPongGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SInfoServerClToGs: " + Marshal.SizeOf(typeof(SInfoServerClToGs)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SInfoServerClToGs))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SInfoServerGsToCl: " + Marshal.SizeOf(typeof(SInfoServerGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SInfoServerGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SInfoChannelInGsToCl: " + Marshal.SizeOf(typeof(SInfoChannelInGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SInfoChannelInGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SInfoChannelOutGsToCl: " + Marshal.SizeOf(typeof(SInfoChannelOutGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SInfoChannelOutGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SInfoUserData: " + Marshal.SizeOf(typeof(SInfoUserData)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SInfoUserData))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SInfoUserListGsToCl: " + Marshal.SizeOf(typeof(SInfoUserListGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SInfoUserListGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SUserChatClToGs: " + Marshal.SizeOf(typeof(SUserChatClToGs)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SUserChatClToGs))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SOtherChatGsToCl: " + Marshal.SizeOf(typeof(SOtherChatGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SOtherChatGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomCreateClToGs: " + Marshal.SizeOf(typeof(SRoomCreateClToGs)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomCreateClToGs))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomCreateGsToCl: " + Marshal.SizeOf(typeof(SRoomCreateGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomCreateGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomJoinClToGs: " + Marshal.SizeOf(typeof(SRoomJoinClToGs)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomJoinClToGs))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomJoinGsToCl: " + Marshal.SizeOf(typeof(SRoomJoinGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomJoinGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomInfo: " + Marshal.SizeOf(typeof(SRoomInfo)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomInfo))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomListGsToCl: " + Marshal.SizeOf(typeof(SRoomListGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomListGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomMember: " + Marshal.SizeOf(typeof(SRoomMember)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomMember))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomMemberListGsToCl: " + Marshal.SizeOf(typeof(SRoomMemberListGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomMemberListGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomMemberLeaderGsToCl: " + Marshal.SizeOf(typeof(SRoomMemberLeaderGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomMemberLeaderGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomJoinOtherGsToCl: " + Marshal.SizeOf(typeof(SRoomJoinOtherGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomJoinOtherGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SRoomLeaveOtherGsToCl: " + Marshal.SizeOf(typeof(SRoomLeaveOtherGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SRoomLeaveOtherGsToCl))) {
						kBuilder.Append(" (overflow)");
						bCheck = false;
					}
					kBuilder.AppendLine();

					kBuilder.Append("SWebAuthorizeGsToCl: " + Marshal.SizeOf(typeof(SWebAuthorizeGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SWebAuthorizeGsToCl))) {
						kBuilder.Append(" (overflow)");
					}
					kBuilder.AppendLine();

					kBuilder.Append("SWebAuthorizeClToGs: " + Marshal.SizeOf(typeof(SWebAuthorizeClToGs)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SWebAuthorizeClToGs))) {
						kBuilder.Append(" (overflow)");
					}
					kBuilder.AppendLine();

					kBuilder.Append("SWebCheckGsToCl: " + Marshal.SizeOf(typeof(SWebCheckGsToCl)));
					if(iCOMMAND_DATA_SIZE < Marshal.SizeOf(typeof(SWebCheckGsToCl))) {
						kBuilder.Append(" (overflow)");
					}
					kBuilder.AppendLine();

					if(0 < kBuilder.Length) {
						m_kCheckProtocolText.text = kBuilder.ToString();
					}
					return bCheck;
				}
				return false;
			}

			public INT			GetConnectingStep()			{ return m_iConnectingStep; }
			public void			SetConnectingStep(INT o)	{ m_iConnectingStep = o; }

			public GameNetwork	GetMasterNetwork()			{ return m_kMasterNetwork; }
			public GameNetwork	GetSlaveNetwork()			{ return m_kSlaveNetwork; }

			public CConnector	GetMasterMainConnector()	{ return m_kMasterMainConnector; }
			public CConnector	GetMasterSubConnector()		{ return m_kMasterSubConnector; }
			public CConnector	GetSlaveMainConnector()		{ return m_kSlaveMainConnector; }
			public CConnector	GetSlaveMainListener()		{ return m_kSlaveMainListener; }

			public CConnector
			GetCurrentConnector() {
				switch(g_kCfgMgr.GetSendType()) {
				case SEND_TYPE.SEND_MASTER_SUB:
					TRACE("current connector: type: master sub: ");
					return GetMasterSubConnector();
				case SEND_TYPE.SEND_SLAVE_MAIN:
					TRACE("current connector: type: slave main: ");
					return GetSlaveMainConnector();
				default:
					TRACE("current connector: type: master main: ");
					return GetMasterMainConnector();
				}
			}

			public void
			SetMasterMainConnectorText(string o) {
				if(isptr(m_kMasterMainConnectorText)) {
					m_kMasterMainConnectorText.text = o;
				}
			}

			public void
			SetMasterSubConnectorText(string o) {
				if(isptr(m_kMasterSubConnectorText)) {
					m_kMasterSubConnectorText.text = o;
				}
			}

			public void
			SetSlaveMainConnectorText(string o) {
				if(isptr(m_kSlaveMainConnectorText)) {
					m_kSlaveMainConnectorText.text = o;
				}
			}

			public void
			SetSlaveMainListenerText(string o) {
				if(isptr(m_kSlaveMainListenerText)) {
					m_kSlaveMainListenerText.text = o;
				}
			}

			public void
			SetMasterQueueText(size_t o, string p) {
				if(isptr(m_kMasterQueueText)) {
					m_kMasterQueueText.gameObject.name = "Queue (Native:" + o + ")";
					m_kMasterQueueText.text = p;
				}
			}

			public void
			SetSlaveQueueText(size_t o, string p) {
				if(isptr(m_kSlaveQueueText)) {
					m_kSlaveQueueText.gameObject.name = "Queue (Native:" + o + ")";
					m_kSlaveQueueText.text = p;
				}
			}

			private GameNetwork		m_kMasterNetwork = null;
			private GameNetwork		m_kSlaveNetwork = null;

			private CConnector		m_kMasterMainConnector = null;
			private CConnector		m_kMasterSubConnector = null;

			private CConnector		m_kSlaveMainConnector = null;
			private CConnector		m_kSlaveMainListener = null;

			private CookieContainer	m_kCookieContainer = new CookieContainer();

			private Text			m_kCheckProtocolText = null;

			private Text			m_kMasterMainConnectorText = null;
			private Text			m_kMasterSubConnectorText = null;
			private Text			m_kSlaveMainConnectorText = null;
			private Text			m_kSlaveMainListenerText = null;

			private Text			m_kMasterQueueText = null;
			private Text			m_kSlaveQueueText = null;

			private INT				m_iConnectingStep = 0;

			private bool			m_bInitialized = false;

		}
	}
}

/* EOF */
