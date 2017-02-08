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
		public class CPlayerEx : CPlayer {
			public CPlayerEx() {}
			~CPlayerEx() {}

			public override bool
			Initialize() {
				if(base.Initialize()) {
					if(false == isptr(m_kNetIO)) {
						m_kNetIO = new CNetIO();
					}
					return true;
				}
				return false;
			}

			public override bool
			Release() {
				if(base.Release()) {
					SAFE_DELETE(ref m_kNetIO);

					return true;
				}
				return false;
			}

			public override void
			Clear() {
				if(0 < GetAid()) {
					if(isptr(m_kRoomHandler)) {
						CRoom kRoom = m_kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							CCommand kCommand = new CCommand();
							kCommand.SetOrder((UINT)PROTOCOL.ROOM_LEAVE);
							kCommand.SetExtra((UINT)EXTRA.NONE);

							ServerLauncher(this, kCommand);
						}
					}

					g_kChannelMgr.OutUser((UINT)GetChannelIndex(), this);
					g_kChannelMgr.DecreasedUser();

					TRACE("SERVER: OUT: id: " + g_kChannelMgr.GetServerInfo().GetId() + ", max user: " + g_kChannelMgr.GetServerInfo().GetMaxUser() + ", user: " + g_kChannelMgr.GetServerInfo().GetUser());
				}

				if(isptr(m_kNetIO)) {
					m_kNetIO.Clear();
				}

				base.Clear();
			}

			public override void
			Disconnect() {
				CCommand kCommand = new CCommand();
				kCommand.SetOrder((UINT)PROTOCOL.ID_QUIT);
				kCommand.SetExtra((UINT)EXTRA.CLOSE);

				Launcher(kCommand);

				base.Disconnect();
			}

			public override INT
			Launcher(CCommand o, INT p =0, PACKET_TYPE q =PACKET_TYPE.PACKET_THROW, CRYPT_TYPE r =CRYPT_TYPE.CRYPT_NONE) {
				if(isptr(m_kNetIO)) {
					return m_kNetIO.Send(o, p, q, r);
				}
				return -1;
			}

			public override bool
			ReCalculateData() {
				if(base.ReCalculateData()) {
					return true;
				}
				return false;
			}

			public static object	New()	{ return new CPlayerEx(); }
		}
	}
}

/* EOF */
