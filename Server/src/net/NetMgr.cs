/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;

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

	public partial class GameFramework {
		public static CNetMgr	g_kNetMgr { get { return CNetMgr.GetInstance(); } }

		public class CNetMgr : CSingleton< CNetMgr > {
			public CNetMgr() {}
			~CNetMgr() { Release(); }

			public bool
			Initialize(INT iMaxUser_) {
				bool bCheck = true;

				if(0 >= g_kCfgMgr.GetTcpDefaultPort()) {
					OUTPUT("[" + g_kTick.GetTime() + "] tcp default port is null: ");
					return false;
				}

				if(0 >= g_kCfgMgr.GetTcpBackbonePort()) {
					OUTPUT("[" + g_kTick.GetTime() + "] tcp backbone port is null: ");
					return false;
				}

				g_kConnectorProvider.Register((INT)NETWORK_TYPE.NETWORK_UDP_CLIENT, new CreateCallback(CReliableConnector.New));
				if(false == Action("initializing slave network", GetSlaveNetwork().Initialize(SENSOR_TYPE.SENSOR_POLL, COMMAND_TYPE.COMMAND_EXTEND, (UINT)iMaxUser_ + 15, NETWORK_TYPE.NETWORK_UDP_SERVER))) {
					bCheck = false;
				}

				g_kConnectorProvider.Register((INT)NETWORK_TYPE.NETWORK_TCP_CLIENT, new CreateCallback(CDefaultConnector.New));
				if(false == Action("initializing master network", GetMasterNetwork().Initialize(SENSOR_TYPE.SENSOR_POLL, COMMAND_TYPE.COMMAND_EXTEND, (UINT)iMaxUser_ + 15, NETWORK_TYPE.NETWORK_TCP_SERVER))) {
					bCheck = false;
				}

				if(bCheck) {
#if _THREAD
					GetMasterNetwork().CreateWaitEventThread(500, 1);

					INT iCPUNums = Environment.ProcessorCount;
					if(4 < iCPUNums) {
						GetMasterNetwork().CreateSenderThread(4, 1);
					} else if(2 < iCPUNums) {
						GetMasterNetwork().CreateSenderThread(2, 1);
					} else {
						GetMasterNetwork().CreateSenderThread(1, 1);
					}

					GetSlaveNetwork().CreateWaitEventThread(500, 1);
					GetSlaveNetwork().CreateSenderThread(1, 1);
#else
					TRACE("thread is not initialized");
#endif
					if(Action("creating listener for default service", isptr(GetMasterNetwork().Create(ConvertToBytes("0.0.0.0"), g_kCfgMgr.GetTcpDefaultPort(), NETWORK_TYPE.NETWORK_TCP_SERVER)))) {
						g_kConnectorProvider.Register((INT)NETWORK_TYPE.NETWORK_TCP_CLIENT, new CreateCallback(CBackboneConnector.New));
						g_kConnectorProvider.Register((INT)NETWORK_TYPE.NETWORK_TCP_SERVER, new CreateCallback(CBackboneListener.New));
						if(Action("creating listener for backbone service", isptr(GetMasterNetwork().Create(g_kCfgMgr.GetTcpBackboneAddress(), g_kCfgMgr.GetTcpBackbonePort(), NETWORK_TYPE.NETWORK_TCP_SERVER)))) {
							IDispatcherList kDispatcherList = GetMasterNetwork().GetDispatcherList();
							if(isptr(kDispatcherList)) {
								// 패킷 해더의 암호화를 항상 하려면 true로 설정.
								// 클라이언트도 true로 설정되어야 서로 해석할수 있음.
								// 패킷 데이터의 암호화는 별개이며 Send 할때마다 설정할수 있음.
								if(g_kCfgMgr.IsHeaderCrypt()) {
									kDispatcherList.SetHeaderCrypt(true);
								}

								if(Action("creating listener for reliable service", isptr(GetSlaveNetwork().Create(ConvertToBytes("0.0.0.0"), g_kCfgMgr.GetUdpReliablePort(), NETWORK_TYPE.NETWORK_UDP_SERVER)))) {
									kDispatcherList = GetSlaveNetwork().GetDispatcherList();
									if(isptr(kDispatcherList)) {
										if(g_kCfgMgr.IsHeaderCrypt()) {
											kDispatcherList.SetHeaderCrypt(true);
										}

										m_bInitialized = true;
										return true;
									} else {
										OUTPUT("[" + g_kTick.GetTime() + "] dispatcher list is null");
									}
								}
							} else {
								OUTPUT("[" + g_kTick.GetTime() + "] dispatcher list is null");
							}
						}
					}
				} else {
					TRACE("error: network manager is not initialized");
				}
				return false;
			}

			public bool
			Release() {
				if(m_bInitialized) {
					return true;
				}
				return false;
			}

			public bool
			Update() {
#if !_THREAD
				GetMasterNetwork().WaitEvent(100);
				GetSlaveNetwork().WaitEvent(100);

				GetMasterNetwork().SendQueue();
				GetSlaveNetwork().SendQueue();
#endif

				if(false == GetMasterNetwork().GetCommandQueue().Empty()) {
					CExtendCommand kExtendCommand = null;
					INT iCount = 0;
					while(isptr(kExtendCommand = (CExtendCommand)(GetMasterNetwork().GetCommandQueue().Take()))) {
						GetMasterNetwork().ParseCommand(kExtendCommand);
						SAFE_DELETE(ref kExtendCommand);
						++iCount;
						if(200 < iCount) {
							break;
						}
					}
				}

				if(false == GetSlaveNetwork().GetCommandQueue().Empty()) {
					CExtendCommand kExtendCommand = null;
					INT iCount = 0;
					while(isptr(kExtendCommand = (CExtendCommand)(GetSlaveNetwork().GetCommandQueue().Take()))) {
						GetSlaveNetwork().ParseCommand(kExtendCommand);
						SAFE_DELETE(ref kExtendCommand);
						++iCount;
						if(200 < iCount) {
							break;
						}
					}
				}

//				if(false == GetSlaveNetwork().GetCommandQueue().Empty()) {
//					INT iCount = 0;
//					CExtendCommand kExtendCommand = null;
//					while(isptr(kExtendCommand = (CExtendCommand)(GetSlaveNetwork().GetCommandQueue().Take()))) {
//						CCommand kCommand = kExtendCommand.GetCommand();
//						if((kCommand.GetOrder() == (UINT)PROTOCOL.ID_AUTHORIZE) && (kCommand.GetOrder() == (UINT)PROTOCOL.INFO_SERVER)) {
//							ExtendLauncher kLauncher = g_bfExtendLauncher[kCommand.GetOrder()];
//							if(isptr(kLauncher)) {
//								kLauncher(kExtendCommand);
//							} else {
//								OUTPUT("[" + g_kTick.GetTime() + "] error: order is none: " + kCommand.GetOrder());
//							}
//						} else {
//							OUTPUT("[" + g_kTick.GetTime() + "] error: order range over: " + kCommand.GetOrder());
//						}
//						SAFE_DELETE(ref kExtendCommand);
//						++iCount;
//						if(100 < iCount) {
//							break;
//						}
//					}
//				}
				return true;
			}

			public bool
			StartThread() {
#if _THREAD
				GetMasterNetwork().StartThread();
				GetSlaveNetwork().StartThread();
				return true;
#else
				return false;
#endif
			}

			public bool
			CancelThread() {
#if _THREAD
				GetMasterNetwork().CancelThread();
				GetSlaveNetwork().CancelThread();
				return true;
#else
				return false;
#endif
			}

			public CNetwork		GetMasterNetwork()		{ return m_kMasterNetwork; }
			public CNetwork		GetSlaveNetwork()		{ return m_kSlaveNetwork; }

			public bool
			CheckProtocol() {
				InitializeCommand();

				return true;
			}

			private CNetworkEx	m_kMasterNetwork = new CNetworkEx();
			private CNetworkEx	m_kSlaveNetwork = new CNetworkEx();

			private bool		m_bInitialized = false;
		}
	}
}

/* EOF */
