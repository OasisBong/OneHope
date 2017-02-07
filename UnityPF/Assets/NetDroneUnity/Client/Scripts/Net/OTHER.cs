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
		CMD_OTHER_STATUS(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				SOtherStatusGsToCl tRData = (SOtherStatusGsToCl)kCommand_.GetData(typeof(SOtherStatusGsToCl));
				GamePlayer kPlayer = g_kUnitMgr.GetPlayer(tRData.actor);
				if(isptr(kPlayer)) {
					kPlayer.SetStatus((STATUS_TYPE)kCommand_.GetOption());
				}

				REFRESH_MEMBER_LIST();

				MESSAGE("CMD_OTHER_STATUS: actor: " + tRData.actor + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tRData))));
			}
			return true;
		}

		public static bool
		CMD_OTHER_CHAT(CCommand kCommand_) {
//			CHAT_TYPE eType = CHAT_TYPE.CHAT_MAX;
//			if(EXTRA.NEW == (EXTRA)kCommand_.GetExtra()) {
//				eType = CHAT_TYPE.CHAT_CHEAT;
//			} else if(EXTRA.CHECK == (EXTRA)kCommand_.GetExtra()) {
//				eType = CHAT_TYPE.CHAT_SYSTEM;
//			} else {
//				eType = CHAT_TYPE.CHAT_NORMAL;
//			}

			SOtherChatGsToCl tRData = (SOtherChatGsToCl)kCommand_.GetData(typeof(SOtherChatGsToCl));

			MESSAGE(ConvertToString(tRData.GetName()) + ": " + ConvertToString(tRData.GetContent()) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tRData) - (iMAX_CHAT_LEN+1)) + (INT)kCommand_.GetOption()));
			return true;
		}

		public static void
		InitializeOtherCommand()	{
			g_bfNativeLauncher[(INT)(PROTOCOL.OTHER_STATUS)] = new NativeLauncher(CMD_OTHER_STATUS);
			g_bfNativeLauncher[(INT)(PROTOCOL.OTHER_CHAT)] = new NativeLauncher(CMD_OTHER_CHAT);
		}
	}
}

/* EOF */
