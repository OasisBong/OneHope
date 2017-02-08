/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Linq;
using System.Collections.Generic;

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
		public static CUnitMgr	g_kUnitMgr		{ get { return CUnitMgr.GetInstance(); } }

		public class CUnitMgr : CSingleton<CUnitMgr> {
			public CUnitMgr() {}
			~CUnitMgr() { /* Release(); */ }

			public bool
			Initialize() {
				if(false == m_bInitialized) {
					GameObject kMainPlayerObject = new GameObject("Main Player");
					m_kMainPlayer = kMainPlayerObject.AddComponent<GamePlayer>();
					m_kMainPlayer.Initialize();
					m_kMainPlayer.Create();

					DontDestroyOnLoad(kMainPlayerObject);

					m_bInitialized = true;
					return true;
				}
				return false;
			}

			public bool
			Release() {
				if(m_bInitialized) {
					for(INT i = 0; i < m_kPlayerBox.Count; ++i) {
						GamePlayer kPlayer = m_kPlayerBox.ElementAt(i).Value;
						if(isptr(kPlayer)) {
							kPlayer.Release();

							Destroy(kPlayer.gameObject);
						}
					}
					m_kPlayerBox.Clear();

					if(g_kNetMgr.IsConnected()) {
						SEND_ID_QUIT();
					}

					m_kMainPlayer.Release();

					Destroy(m_kMainPlayer.gameObject);

					m_kMainPlayer = null;
					m_kPlayerBox = null;

					m_bInitialized = false;
					return true;
				}
				return false;
			}

			public void
			Clear() {
				for(INT i = 0; i < m_kPlayerBox.Count; ++i) {
					GamePlayer kPlayer = m_kPlayerBox.ElementAt(i).Value;
					if(isptr(kPlayer)) {
						kPlayer.Release();

						Destroy(kPlayer.gameObject);
					}
				}
				m_kPlayerBox.Clear();

				//m_kMainPlayer.Clear();
			}

			//public bool
			//Update() {
			//	m_kMainPlayer.Update();

			//	for(INT i = 0; i < m_kPlayerBox.Count; ++i) {
			//		GamePlayer kPlayer = m_kPlayerBox.ElementAt(i).Value;
			//		if(isptr(kPlayer)) {
			//			kPlayer.Update();
			//		}
			//	}

			//	return true;
			//}

			public GamePlayer
			AddPlayer(UINT uiKey_) {
				if(0 < uiKey_) {
					if(m_kMainPlayer.GetKey() != uiKey_) {
						if(false == m_kPlayerBox.ContainsKey(uiKey_)) {
							GameObject kOtherPlayerObject = new GameObject("Other Player");
							GamePlayer kPlayer = kOtherPlayerObject.AddComponent<GamePlayer>();
							if(isptr(g_kChannelMgr.GetMainRoom())) {
								kOtherPlayerObject.transform.SetParent(g_kChannelMgr.GetMainRoom().gameObject.transform);
							}
							kPlayer.Initialize();
							kPlayer.Create();

							CNetIO kNetIO = kPlayer.GetNetIO();
							if(isptr(kNetIO)) {
								CConnector kConnector = g_kNetMgr.GetSlaveNetwork().Empty();
								if(isptr(kConnector)) {
									kConnector.SetSocket(g_kNetMgr.GetSlaveMainConnector().GetSocket());
									kNetIO.SetConnector(kConnector);
								}
							}

							DontDestroyOnLoad(kOtherPlayerObject);

							kPlayer.SetKey(uiKey_);

							m_kPlayerBox.Add(uiKey_, kPlayer);
							return kPlayer;
						} else {
							return m_kPlayerBox[uiKey_];
						}
					} else {
						return m_kMainPlayer;
					}

				} else {
					TRACE("critical error: unit key is 0: ");
				}
				return null;
			}

			public void
			RemovePlayer(UINT uiKey_) {
				if(m_kMainPlayer.GetKey() != uiKey_) {
					if(false == EmptyPlayer()) {
						if(0 < uiKey_) {
							if(m_kPlayerBox.ContainsKey(uiKey_)) {
								m_kPlayerBox[uiKey_].Release();

								Destroy(m_kPlayerBox[uiKey_].gameObject);

								m_kPlayerBox.Remove(uiKey_);
							}
						}
					}
				}
			}

			public GamePlayer
			GetPlayer(UINT uiKey_) {
				if(m_kMainPlayer.GetKey() != uiKey_) {
					if(0 < uiKey_) {
						if(m_kPlayerBox.ContainsKey(uiKey_)) {
							return m_kPlayerBox[uiKey_];
						}
					}
				} else {
					return m_kMainPlayer;
				}
				return null;
			}

			public GamePlayer
			FindPlayer(CHAR[] szName_) {
				for(INT i = 0; i < m_kPlayerBox.Count; ++i) {
					GamePlayer kPlayer = m_kPlayerBox.ElementAt(i).Value;
					if(isptr(kPlayer)) {
						if(0 == String.Compare(ConvertToString(kPlayer.GetName()), ConvertToString(szName_))) {
							return kPlayer;
						}
					}
				}
				return null;
			}

			public GamePlayer
			FindPlayer(UINT uiAid_) {
				for(INT i = 0; i < m_kPlayerBox.Count; ++i) {
					GamePlayer kPlayer = m_kPlayerBox.ElementAt(i).Value;
					if(isptr(kPlayer)) {
						if(kPlayer.GetAid() == uiAid_) {
							return kPlayer;
						}
					}
				}
				return null;
			}

			public size_t		SizePlayer()	{ return (size_t)(m_kPlayerBox.Count); }
			public bool			EmptyPlayer()	{ return (0 >= m_kPlayerBox.Count); }

			public GamePlayer	GetMainPlayer()	{ return m_kMainPlayer; }

			private GamePlayer						m_kMainPlayer = null;
			private Dictionary<UINT, GamePlayer>	m_kPlayerBox = new Dictionary<UINT, GamePlayer>();

			private bool							m_bInitialized = false;
		}
	}
}

/* EOF */
