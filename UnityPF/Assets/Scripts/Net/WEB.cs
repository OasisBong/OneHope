﻿/*
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
		CMD_WEB_AUTHORIZE(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				SWebAuthorizeGsToCl tRData = (SWebAuthorizeGsToCl)kCommand_.GetData(typeof(SWebAuthorizeGsToCl));
				MESSAGE("CMD_ID_AUTHORIZE: OK: key: " + tRData.key + ", aid: " + tRData.aid + ", name: " + ConvertToString(tRData.GetName()) + ", tick: " + tRData.tick);
			} else {
				MESSAGE("CMD_ID_AUTHORIZE: FAIL: ");
			}
			return true;
		}

		public static bool
		CMD_WEB_QUIT(CCommand kCommand_) {
			MESSAGE("CMD_WEB_QUIT: ");
			return true;
		}

		public static bool
		CMD_WEB_CHECK(CCommand kCommand_) {
			SWebCheckGsToCl tRData = (SWebCheckGsToCl)kCommand_.GetData(typeof(SWebCheckGsToCl));
			MESSAGE(ConvertToString(tRData.GetContent()));
			return true;
		}

		public static void
		InitializeWebCommand() {
			g_bfNativeLauncher[(INT)(PROTOCOL.WEB_AUTHORIZE)] = new NativeLauncher(CMD_WEB_AUTHORIZE);
			g_bfNativeLauncher[(INT)(PROTOCOL.WEB_QUIT)] = new NativeLauncher(CMD_WEB_QUIT);
			g_bfNativeLauncher[(INT)(PROTOCOL.WEB_CHECK)] = new NativeLauncher(CMD_WEB_CHECK);
		}
	}
}

/* EOF */