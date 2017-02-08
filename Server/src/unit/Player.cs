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
		public class CPlayer : CUnit {
			public CPlayer() {}
			~CPlayer() {}

			public override bool
			Initialize() {
				m_tPlayerData = new SPlayerData(true);
				m_tPlayerSums = new SPlayerData(true);

				if(false == isptr(m_kRoomHandler)) {
					m_kRoomHandler = new CRoomHandler(this);
				}

				m_tPlayerData.Clear();
				m_tPlayerSums.Clear();

				return true;
			}

			public override bool
			Release() {
				Disconnect();

				SAFE_DELETE(ref m_kRoomHandler);

				m_tPlayerSums.Clear();
				m_tPlayerData.Clear();

				return true;
			}

			public override void
			Clear() {
				base.Clear();

				m_tPlayerData.Clear();
				m_tPlayerSums.Clear();

				m_iChannelIndex = -1;
				m_iUserIndex = -1;

				Array.Clear(m_szLoginId, 0, m_szLoginId.Length);
			}

			public override void
			Disconnect() {
				Clear();
			}

			public override INT
			Launcher(CCommand o, INT p =0, PACKET_TYPE q =PACKET_TYPE.PACKET_THROW, CRYPT_TYPE r =CRYPT_TYPE.CRYPT_NONE) {
				return -1;
			}

			public override CUnit
			GetTypeAs(UNIT_TYPE o) {
				if(isptr(base.GetTypeAs(o)) == false) {
					if(UNIT_TYPE.UNIT_PLAYER == o) {
						return this;
					}
					return null;
				}
				return this;
			}

			public override bool
			ReCalculateData() {
				return true;
			}

			public INT					GetChannelIndex()			{ return m_iChannelIndex; }
			public void					SetChannelIndex(INT o)		{ m_iChannelIndex = o; }

			public INT					GetUserIndex()				{ return m_iUserIndex; }
			public void					SetUserIndex(INT o)			{ m_iUserIndex = o; }

			public override UINT		GetAid()					{ return m_tPlayerData.GetAid(); }
			public void					SetAid(UINT o)				{ m_tPlayerData.SetAid(o); }

			public override UINT		GetKey()					{ return m_tPlayerData.GetKey(); }
			public override void		SetKey(UINT o)				{ m_tPlayerData.SetKey(o); }

			public override STATUS_TYPE	GetStatus()					{ return m_tPlayerData.GetStatus(); }
			public override void		SetStatus(STATUS_TYPE o)	{ m_tPlayerData.SetStatus(o); }

			public CHAR[]				GetLoginId()				{ return m_szLoginId; }
			public void					SetLoginId(CHAR[] o)		{ INT iLength = o.Length; if(iLOGIN_ID_LEN < iLength) { iLength = iLOGIN_ID_LEN; } Array.Copy(o, 0, m_szLoginId, 0, iLength); m_szLoginId[iLOGIN_ID_LEN] = (CHAR)('\0'); }

			public override CHAR[]		GetName()					{ return m_tPlayerData.GetName(); }
			public override void		SetName(CHAR[] o)			{ m_tPlayerData.SetName(o); }

			private INT		m_iUserIndex = -1;
			private INT		m_iChannelIndex = -1;

			private CHAR[]	m_szLoginId = new CHAR[iLOGIN_ID_LEN + 1];
		}
	}
}

/* EOF */
