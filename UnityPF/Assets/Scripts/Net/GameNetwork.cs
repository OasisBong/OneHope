/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Collections;
using System.Text;
using System.Net.Sockets;

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

	public class GameNetwork : GameFramework {
		public GameNetwork() {}
		~GameNetwork() {}

		public bool
		Initialize(SENSOR_TYPE eSensorType_, COMMAND_TYPE eCommandType_, UINT uiMaxSize_ =1, NETWORK_TYPE eNetworkType_ =NETWORK_TYPE.NETWORK_TCP_SERVER, UINT uiDispatcherSize_ =0) {
			inner = new CNetworkEx();
			inner.outer = this;

			return inner.Initialize(eSensorType_, eCommandType_, uiMaxSize_, eNetworkType_, uiDispatcherSize_);
		}

		public bool
		Release() {
			if(isptr(inner)) {
				if(inner.IsInitialized()) {
					if(inner.Release()) {
						inner.outer = null;
						inner = null;
						return true;
					}
					inner.outer = null;
					inner = null;
				}
			}
			return false;
		}

		public void
		Clear() {
			if(isptr(inner)) {
				inner.Clear();
			}
		}

		public bool
		CreateWaitEventThread(UINT uiWaitTime_ =100, UINT uiSleepTime_ =0) {
			if(isptr(inner)) {
				return inner.CreateWaitEventThread(uiWaitTime_, uiSleepTime_);
			}
			return false;
		}

		public bool
		CreateSenderThread(UINT uiThreadNumber_ =1, UINT uiSleepTime_ =0) {
			if(isptr(inner)) {
				return inner.CreateSenderThread(uiThreadNumber_, uiSleepTime_);
			}
			return false;
		}

		public bool
		StartThread() {
			if(isptr(inner)) {
				return inner.StartThread();
			}
			return false;
		}

		public bool
		CancelThread() {
			if(isptr(inner)) {
				return inner.CancelThread();
			}
			return false;
		}

		public bool		ParseCommand(CNativeCommand o)	{ if(isptr(inner)) { return inner.ParseCommand(o); } return false; }
		public bool		ParseCommand(CExtendCommand o)	{ if(isptr(inner)) { return inner.ParseCommand(o); } return false; }

		public object	Read()							{ if(isptr(inner)) { return inner.Read(); } return null; }
		public void		Send()							{ if(isptr(inner)) { inner.Send(); } }
		public INT		WaitEvent(INT o)				{ if(isptr(inner)) { return inner.WaitEvent(o); } return -1; }
		public void		SendQueue(UINT o =0)			{ if(isptr(inner)) { inner.SendQueue(o); } }

		public ICommandQueue
		GetCommandQueue() {
			if(isptr(inner)) {
				return inner.GetCommandQueue();
			}
			return null;
		}

		public ISensor
		GetSensor() {
			if(isptr(inner)) {
				return inner.GetSensor();
			}
			return null;
		}

		public bool
		SetSensor(SENSOR_TYPE eType_, UINT uiMaxSize_) {
			if(isptr(inner)) {
				return inner.SetSensor(eType_, uiMaxSize_);
			}
			return false;
		}

		public bool
		ChangeConnector(CConnector kConnector_) {
			if(isptr(inner)) {
				return inner.ChangeConnector(kConnector_);
			}
			return false;
		}

		public CConnector
		Create(CHAR[] szAddr_, INT iPort_, NETWORK_TYPE eType_) {
			if(isptr(inner)) {
				return inner.Create(szAddr_, iPort_, eType_);
			}
			return null;
		}

		public CConnector
		Empty() {
			if(isptr(inner)) {
				return inner.Empty();
			}
			return null;
		}

		public IDispatcherList
		GetDispatcherList() {
			if(isptr(inner)) {
				return inner.GetDispatcherList();
			}
			return null;
		}

		public void
		SetInitialize(bool o) {
			if(isptr(inner)) {
				inner.SetInitialize(o);
			}
		}

		public bool
		IsInitialized() {
			if(isptr(inner)) {
				return inner.IsInitialized();
			}
			return false;
		}

		public void		StartLogin()					{ StartCoroutine(WaitLogin()); }
		public void		StartRelogin()					{ StartCoroutine(WaitRelogin()); }
		public void		StartReconnect()				{ StartCoroutine(WaitReconnect()); }

		IEnumerator
		WaitLogin() {
			CConnector kConnector = g_kNetMgr.GetCurrentConnector();
			if(isptr(kConnector)) {
				if(false == kConnector.IsConnected()) {
					TRACE("wait login: connect: connected");

					if(false == kConnector.Connect(kConnector.GetRemoteAddress(), kConnector.GetRemotePort())) {
						g_kNetMgr.SetConnectingStep(-1);
					} else {
						g_kNetMgr.SetConnectingStep(1);
					}

					bool bCheck = true;

					g_kTick.Update();
					m_tkSendRetryTick = g_kTick.GetTick() + iMAX_RECONNECT_RETRY_TICK;
					m_uiSendRetryCount = 0;

					while(0 >= kConnector.GetSerialKey()) {
						yield return null;

						if(g_kTick.GetTick() > m_tkSendRetryTick) {
							g_kNetMgr.DisconnectAll();
							TRACE("wait login: connect: timeout");

							if(iMAX_RECONNECT_RETRY_COUNT > m_uiSendRetryCount) {
								if(false == kConnector.Connect(kConnector.GetRemoteAddress(), kConnector.GetRemotePort())) {
									g_kNetMgr.SetConnectingStep(-1);
									TRACE("wait login: connect: timeout: connecting failed");
									bCheck = false;
								} else {
									bCheck = true;
								}
								g_kNetMgr.SetConnectingStep(2 + (INT)m_uiSendRetryCount);
								m_tkSendRetryTick = g_kTick.GetTick() + iMAX_RECONNECT_RETRY_TICK;

								++m_uiSendRetryCount;
							} else {
								g_kNetMgr.SetConnectingStep(-1);
								g_kNetMgr.DisconnectAll();
								TRACE("wait login: connect: retry: connecting failed");
								bCheck = false;
								break;
							}
						}

						g_kTick.Update();
					}

					if(bCheck) {
						g_kNetMgr.SetConnectingStep(0);
						if(false == SEND_ID_AUTHORIZE()) {
							if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
								g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
							}
						}
					} else {
						if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
							g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
						}
					}
				}
			}
			yield break;
		}

		IEnumerator
		WaitRelogin() {
			CConnector kConnector = g_kNetMgr.GetCurrentConnector();
			if(isptr(kConnector)) {
				if(kConnector.IsConnected()) {
					SEND_ID_QUIT();

					TRACE("wait relogin: SEND_ID_QUIT");

					g_kTick.Update();
					m_tkTimeoutReloginTick = g_kTick.GetTick() + iMAX_TIMEOUT_LATENCY_TICK;

					while(true) {
						yield return null;

						if(g_kTick.GetTick() < m_tkTimeoutReloginTick) {
							if(false == kConnector.IsConnected()) {
								TRACE("wait relogin: disconnected");
								break;
							}
						} else {
							TRACE("wait relogin: disconnect: timeout");
							break;
						}
					}
				}

				if(false == kConnector.IsConnected()) {
					TRACE("wait relogin: connect: reset: aid: " + g_kUnitMgr.GetMainPlayer().GetAid());

					if(false == kConnector.Connect(kConnector.GetRemoteAddress(), kConnector.GetRemotePort())) {
						g_kNetMgr.SetConnectingStep(-1);
					} else {
						g_kNetMgr.SetConnectingStep(1);
					}

					bool bCheck = true;

					g_kTick.Update();
					m_tkSendRetryTick = g_kTick.GetTick() + iMAX_RECONNECT_RETRY_TICK;
					m_uiSendRetryCount = 0;

					while(0 >= kConnector.GetSerialKey()) {
						yield return null;

						if(g_kTick.GetTick() > m_tkSendRetryTick) {
							g_kNetMgr.DisconnectAll();
							TRACE("wait relogin: connect: timeout");

							if(iMAX_RECONNECT_RETRY_COUNT > m_uiSendRetryCount) {
								if(false == kConnector.Connect(kConnector.GetRemoteAddress(), kConnector.GetRemotePort())) {
									g_kNetMgr.SetConnectingStep(-1);
									TRACE("wait relogin: connect: timeout: connecting failed");
									bCheck = false;
								} else {
									bCheck = true;
								}
								g_kNetMgr.SetConnectingStep(2 + (INT)m_uiSendRetryCount);
								m_tkSendRetryTick = g_kTick.GetTick() + iMAX_RECONNECT_RETRY_TICK;

								++m_uiSendRetryCount;
							} else {
								g_kNetMgr.SetConnectingStep(-1);
								g_kNetMgr.DisconnectAll();
								TRACE("wait relogin: connect: retry: connecting failed");
								bCheck = false;
								break;
							}
						}

						g_kTick.Update();
					}

					if(bCheck) {
						g_kNetMgr.SetConnectingStep(0);
						if(false == SEND_ID_AUTHORIZE()) {
							if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
								g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
							}
						}
					} else {
						if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
							g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
						}
					}
				} else {
					if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
						g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
					}
				}
			}
			yield break;
		}

		IEnumerator
		WaitReconnect() {
			CConnector kConnector = g_kNetMgr.GetCurrentConnector();
			if(kConnector.IsConnected()) {
				g_kNetMgr.DisconnectAll();
			}

			if(isptr(kConnector)) {
				if(false == kConnector.IsConnected()) {
					TRACE("wait reconnect: connect: reset: aid: " + g_kUnitMgr.GetMainPlayer().GetAid());

					if(false == kConnector.Connect(kConnector.GetRemoteAddress(), kConnector.GetRemotePort())) {
						g_kNetMgr.SetConnectingStep(-1);
					} else {
						g_kNetMgr.SetConnectingStep(1);
					}

					bool bCheck = true;

					g_kTick.Update();
					m_tkSendRetryTick = g_kTick.GetTick() + iMAX_RECONNECT_RETRY_TICK;
					m_uiSendRetryCount = 0;

					while(0 >= kConnector.GetSerialKey()) {
						yield return null;

						if(g_kTick.GetTick() > m_tkSendRetryTick) {
							g_kNetMgr.DisconnectAll();
							TRACE("wait reconnect: connect: timeout");

							if(iMAX_RECONNECT_RETRY_COUNT > m_uiSendRetryCount) {
								if(false == kConnector.Connect(kConnector.GetRemoteAddress(), kConnector.GetRemotePort())) {
									g_kNetMgr.SetConnectingStep(-1);
									TRACE("wait reconnect: connect: timeout: connecting failed");
									bCheck = false;
								} else {
									bCheck = true;
								}
								g_kNetMgr.SetConnectingStep(2 + (INT)m_uiSendRetryCount);
								m_tkSendRetryTick = g_kTick.GetTick() + iMAX_RECONNECT_RETRY_TICK;

								++m_uiSendRetryCount;
							} else {
								g_kNetMgr.SetConnectingStep(-1);
								g_kNetMgr.DisconnectAll();
								TRACE("wait reconnect: connect: retry: connecting failed");
								bCheck = false;
								break;
							}
						}

						g_kTick.Update();
					}

					if(bCheck) {
						g_kNetMgr.SetConnectingStep(0);
						if(false == SEND_ID_AUTHORIZE()) {
							if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
								g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
							}
						}
					} else {
						if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
							g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
						}
					}
				}
			}
			yield break;
		}

		public CNetworkEx	inner = null;

		private tick_t		m_tkTimeoutReloginTick = 0;

		private tick_t		m_tkSendRetryTick = 0;
		private UINT		m_uiSendRetryCount = 0;
	}
}

/* EOF */
