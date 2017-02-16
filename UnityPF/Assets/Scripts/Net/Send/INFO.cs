/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
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

	public partial class GameFramework {
		public static bool
		SEND_INFO_USER_LIST() {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.INFO_USER_LIST);
			kCommand.SetExtra((UINT)EXTRA.NONE);

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_INFO_USER_LIST: sending failed: ");
				return false;
			}
			MESSAGE("SEND_INFO_USER_LIST: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}

		public static bool
		SEND_INFO_CHANNEL() {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.INFO_CHANNEL);
			kCommand.SetExtra((UINT)EXTRA.CHANGE);
			kCommand.SetOption((UINT)g_kUnitMgr.GetMainPlayer().GetChannelIndex());

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_INFO_CHANNEL: sending failed: ");
				return false;
			}
			MESSAGE("SEND_INFO_CHANNEL: CHANGE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}
	}
}

/* EOF */
