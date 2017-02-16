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
		SEND_ROOM_CREATE() {
			if(0 >= ConvertToString(g_kChannelMgr.GetMainRoom().GetName()).Length) {
				MESSAGE("SEND_ROOM_CREATE: room name is empty: ");
				return false;
			}

			if(0 >= g_kChannelMgr.GetMainRoom().GetStageId()) {
				MESSAGE("SEND_ROOM_CREATE: stage id is zero: ");
				return false;
			}

			if((0 < g_kChannelMgr.GetMainRoom().GetMaxUser()) && (g_kChannelMgr.GetMainRoom().GetMaxUser() <= (UINT)iMAX_ROOM_MEMBERS)) {
				CCommand kCommand = new CCommand();
				kCommand.SetOrder((UINT)PROTOCOL.ROOM_CREATE);
				kCommand.SetExtra((UINT)EXTRA.NONE);

				SRoomCreateClToGs tSData = new SRoomCreateClToGs(true);
				tSData.SetName(g_kChannelMgr.GetMainRoom().GetName());
				tSData.stage_id = g_kChannelMgr.GetMainRoom().GetStageId();
				tSData.max = g_kChannelMgr.GetMainRoom().GetMaxUser();

				INT iSize = Marshal.SizeOf(tSData);
				kCommand.SetData(tSData, iSize);

				if(0 > g_kNetMgr.Send(kCommand, iSize)) {
					MESSAGE("SEND_ROOM_CREATE: sending failed: ");
					return false;
				}
				MESSAGE("SEND_ROOM_CREATE: NONE: stage id: " + tSData.stage_id + ", max: " + tSData.max + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + iSize));
				return true;
			} else {
				MESSAGE("SEND_ROOM_CREATE: max user is not valid: ");
				return false;
			}
		}

		public static bool
		SEND_ROOM_JOIN(UINT uiRoomId_) {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ROOM_JOIN);
			kCommand.SetExtra((UINT)EXTRA.NONE);

			SRoomJoinClToGs tSData = new SRoomJoinClToGs(true);
			tSData.id = uiRoomId_;

			INT iSize = Marshal.SizeOf(tSData);
			kCommand.SetData(tSData, iSize);

			if(0 > g_kNetMgr.Send(kCommand, iSize)) {
				MESSAGE("SEND_ROOM_JOIN: sending failed: ");
				return false;
			}
			MESSAGE("SEND_ROOM_JOIN: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}

		public static bool
		SEND_ROOM_LEAVE() {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ROOM_LEAVE);
			kCommand.SetExtra((UINT)EXTRA.NONE);

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_ROOM_LEAVE: sending failed: ");
				return false;
			}
			MESSAGE("SEND_ROOM_LEAVE: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}

		public static bool
		SEND_ROOM_START() {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ROOM_START);
			kCommand.SetExtra((UINT)EXTRA.NONE);

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_ROOM_START: sending failed: ");
				return false;
			}
			MESSAGE("SEND_ROOM_START: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}

		public static bool
		SEND_ROOM_STOP() {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ROOM_STOP);
			kCommand.SetExtra((UINT)EXTRA.NONE);

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_ROOM_STOP: sending failed: ");
				return false;
			}
			MESSAGE("SEND_ROOM_STOP: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}

		public static bool
		SEND_ROOM_LIST() {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ROOM_LIST);
			kCommand.SetExtra((UINT)EXTRA.NONE);
			kCommand.SetMission(0);

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_ROOM_LIST: sending failed: ");
				return false;
			}
			MESSAGE("SEND_ROOM_LIST: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}

		public static bool
		SEND_ROOM_MEMBER_LIST() {
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ROOM_LIST);
			kCommand.SetExtra((UINT)EXTRA.NONE);
			kCommand.SetMission(1);

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_ROOM_MEMBER_LIST: sending failed: ");
				return false;
			}
			MESSAGE("SEND_ROOM_MEMBER_LIST: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}
	}
}

/* EOF */
