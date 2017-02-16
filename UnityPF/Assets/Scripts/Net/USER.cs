/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;

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
		CMD_USER_STATUS(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				STATUS_TYPE eType = (STATUS_TYPE)kCommand_.GetOption();
				g_kUnitMgr.GetMainPlayer().SetStatus(eType);

				REFRESH_MEMBER_LIST();

				MESSAGE("CMD_USER_STATUS: OK: status: " + eType.ToString().Substring((eType.ToString().IndexOf("_") + 1)) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				MESSAGE("CMD_USER_STATUS: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_USER_CHAT(CCommand kCommand_) {
			if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				MESSAGE("CMD_USER_CHAT: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

        public static bool
        CMD_USER_MOVE(CCommand kCommand_)
        {
            SUserMoveClToGs tRData = (SUserMoveClToGs)kCommand_.GetData(typeof(SUserMoveClToGs));
            Vector3 temp = new Vector3();
            temp.x = tRData.GetX();
            temp.y = tRData.GetY();
            temp.z = tRData.GetZ();
            g_kUnitMgr.GetPlayer(tRData.GetKey()).inner.GetGameObject().transform.position = temp;
            return true;
        }

		public static void
		InitializeUserCommand() {
			g_bfNativeLauncher[(INT)(PROTOCOL.USER_STATUS)] = new NativeLauncher(CMD_USER_STATUS);
			g_bfNativeLauncher[(INT)(PROTOCOL.USER_CHAT)] = new NativeLauncher(CMD_USER_CHAT);
            g_bfNativeLauncher[(INT)(PROTOCOL.USER_MOVE)] = new NativeLauncher(CMD_USER_MOVE);
		}
	}
}

/* EOF */
