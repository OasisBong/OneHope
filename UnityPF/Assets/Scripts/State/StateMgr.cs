/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;

// | ------- Offline ------- || --------------- Online -------------- |
//                            | ------------- KeepAlive ------------- |
//                            | ------------ Ping / Pong ------------ |
// State:Empty - State:Login - State:Channel - State:Lobby - State:Room
//

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
		public static CStateMgr	g_kStateMgr	{ get { return CStateMgr.GetInstance(); } }

		public class CStateMgr : CSingleton<CStateMgr> {
			public CStateMgr() {}
			~CStateMgr() { /* Release(); */ }

			public bool
			Initialize() {
				if(false == m_bInitialized) {
					GameObject kMainStateObject = new GameObject("Main State");
					m_kMainStateText = kMainStateObject.AddComponent<Text>();
					DontDestroyOnLoad(kMainStateObject);

					m_kList[(INT)(STATE_TYPE.STATE_EMPTY)] = new CStateEmpty();
					m_kList[(INT)(STATE_TYPE.STATE_LOGIN)] = new CStateLogin();
					m_kList[(INT)(STATE_TYPE.STATE_CHANNEL)] = new CStateChannel();
					m_kList[(INT)(STATE_TYPE.STATE_LOBBY)] = new CStateLobby();
					m_kList[(INT)(STATE_TYPE.STATE_ROOM)] = new CStateRoom();

					for(INT i = 0; i < (INT)(STATE_TYPE.STATE_MAX); ++i) {
						if(isptr(m_kList[i])) {
							m_kList[i].Initialize();
						}
					}

					SetTransition(STATE_TYPE.STATE_EMPTY);

					m_bInitialized = true;
					return true;
				}
				return false;
			}

			public bool
			Release() {
				if(m_bInitialized) {
					SetTransition(STATE_TYPE.STATE_EMPTY);

					if(isptr(m_kMainStateText)) {
						Destroy(m_kMainStateText.gameObject);
					}

					m_eCurrentType = STATE_TYPE.STATE_NONE;
					m_eReservationType = STATE_TYPE.STATE_NONE;
					m_kState = null;

					for(INT i = 0; i < (INT)(STATE_TYPE.STATE_MAX); ++i) {
						SAFE_DELETE_RELEASE(ref m_kList[i]);
					}

					m_bInitialized = false;
					return true;
				}
				return false;
			}

			public STATE_TYPE	GetTransitionType()				{ return m_eCurrentType; }
			public STATE_TYPE	GetReservationType()			{ return m_eReservationType; }

			public void
			SetReservation(STATE_TYPE eType_) {
				m_eReservationType = eType_;
			}

			public CState
			GetTransition(STATE_TYPE o) {
				if(o < STATE_TYPE.STATE_MAX) {
					return m_kList[(INT)(o)];
				}
				return null;
			}

			public void
			SetTransition(STATE_TYPE eType_) {
				m_eReservationType = STATE_TYPE.STATE_NONE;		// 예약 된 STATE가 있다면 취소.

				if(m_eCurrentType == eType_) {
					TRACE("set transition: overcalled: " + m_eCurrentType);
				} else {
					if(isptr(m_kState)) {
						m_kState.PostProcess(eType_);
					}

					if(eType_ < STATE_TYPE.STATE_MAX) {
						CState kState = m_kList[(INT)(eType_)];
						if(isptr(kState)) {
							kState.PreProcess(m_eCurrentType);

							m_kState = kState;
							m_eLastType = m_eCurrentType;
							m_eCurrentType = eType_;

							if(isptr(m_kMainStateText)) {
								m_kMainStateText.gameObject.name = "Main State (" + eType_.ToString().Substring((eType_.ToString().IndexOf("_") + 1)) + ")";
							}
						}
					}
				}
			}

			public bool
			Update() {
				if(m_eReservationType != STATE_TYPE.STATE_NONE) {
					SetTransition(m_eReservationType);
					m_eReservationType = STATE_TYPE.STATE_NONE;
				}

				if(isptr(m_kState)) {
					return m_kState.Update();
				}
				return true;
			}

			public STATE_TYPE	m_eCurrentType = STATE_TYPE.STATE_NONE;
			public STATE_TYPE	m_eLastType = STATE_TYPE.STATE_NONE;
			public STATE_TYPE	m_eReservationType = STATE_TYPE.STATE_NONE;

			public CState		m_kState = null;
			public CState[]		m_kList = new CState[(INT)(STATE_TYPE.STATE_MAX)];

			private Text		m_kMainStateText = null;

			private bool		m_bInitialized = false;

		}
	}
}

/* EOF */
