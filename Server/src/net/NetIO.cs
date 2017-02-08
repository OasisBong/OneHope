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
		public class CNetIO {
			public CNetIO() {}
			~CNetIO() {}

			public virtual void
			Clear() {
				if(isptr(m_kConnector)) {
					m_kConnector.Disconnect();
					m_kConnector = null;
				}
				m_tkDelayPingTick = 0;
			}

			public virtual INT
			Send(CCommand kCommand_, INT iSize_, PACKET_TYPE eType_ =PACKET_TYPE.PACKET_THROW, CRYPT_TYPE eCrypt_ =CRYPT_TYPE.CRYPT_NONE) {
				if(isptr(m_kConnector)) {
					return m_kConnector.Send(kCommand_, iSize_, eType_, eCrypt_);
				}
				return -1;
			}

			public void			SetConnector(CConnector o)	{ m_kConnector = o; }
			public CConnector	GetConnector()				{ return m_kConnector; }

			public void			SetDelayPingTick(tick_t o)	{ m_tkDelayPingTick = o; }
			public tick_t		GetDelayPingTick()			{ return m_tkDelayPingTick; }

			private CConnector	m_kConnector = null;
			private tick_t		m_tkDelayPingTick = 0;
		}
	}
}

/* EOF */
