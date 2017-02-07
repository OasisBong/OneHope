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
		public class CUnit {
			public CUnit() {}
			~CUnit() {}

			public virtual bool			Initialize()				{ return false; }
			public virtual bool			Release()					{ return false; }

			public virtual bool			Create()					{ return true; }
			public virtual bool			Terminate()					{ return true; }

			public virtual bool
			Update() {
				return true;
			}

			public virtual void
			Clear() {
				m_tkPlayingTick = 0;
			}

			public virtual void			Disconnect()				{}

			public virtual bool			ReCalculateData()			{ return true; }

			public virtual INT
			Launcher(CCommand o, INT p =0, PACKET_TYPE q =PACKET_TYPE.PACKET_THROW, CRYPT_TYPE r =CRYPT_TYPE.CRYPT_NONE) {
				return -1;
			}

			public virtual bool
			Broadcast(CCommand kCommand_, INT iSize_ =0, CUnit kFrom_ =null, CUnit kTo_ =null) {
				if(isptr(m_kRoomHandler)) {
					if(m_kRoomHandler.InRoom()) {
						return m_kRoomHandler.Broadcast(kCommand_, iSize_, kFrom_, kTo_);
					} else {
						TRACE("user is not in room: ");
					}
				}
				return false;
			}

			public virtual CUnit
			GetTypeAs(UNIT_TYPE o) {
				if(UNIT_TYPE.UNIT_SELF == o) {
					return this;
				}
				return null;
			}

			public CNetIO				GetNetIO()					{ return m_kNetIO; }

			public CRoomHandler			GetRoomHandler()			{ return m_kRoomHandler; }

			public virtual UINT			GetAid()					{ return 0; }

			public virtual UINT			GetKey()					{ return 0; }
			public virtual void			SetKey(UINT o)				{}

			public virtual STATUS_TYPE	GetStatus()					{ return STATUS_TYPE.STATUS_EXIT; }
			public virtual void			SetStatus(STATUS_TYPE o)	{}

			public virtual CHAR[]		GetName()					{ return null; }
			public virtual void			SetName(CHAR[] o)			{}

			public SPlayerData			GetPlayerData()				{ return m_tPlayerData; }
			public SPlayerData			GetPlayerSums()				{ return m_tPlayerSums; }

			public SNpcData				GetNpcData()				{ return m_tNpcData; }
			public SNpcData				GetNpcSums()				{ return m_tNpcSums; }

			public void					ResetPlayingTick()			{ m_tkPlayingTick = 0; }
			public void					SetPlayingTick()			{ m_tkPlayingTick = g_kTick.GetTick(); }
			public tick_t				GetPlayingTick()			{ if((0 < m_tkPlayingTick) && (g_kTick.GetTick() > m_tkPlayingTick)) return (g_kTick.GetTick() - m_tkPlayingTick); else return 0; }

			protected SPlayerData		m_tPlayerData;
			protected SPlayerData		m_tPlayerSums;

			protected SNpcData			m_tNpcData;
			protected SNpcData			m_tNpcSums;

			protected CNetIO			m_kNetIO = null;

			protected CRoomHandler		m_kRoomHandler = null;

			private tick_t				m_tkPlayingTick = 0;
		}
	}
}

/* EOF */
