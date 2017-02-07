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
		public delegate bool	NativeLauncher(CCommand o);
		public delegate bool	ExtendLauncher(CExtendCommand o);

		public static NativeLauncher[]	g_bfNativeLauncher = new NativeLauncher[(INT)(PROTOCOL.PROTOCOL_MAX)];
		public static ExtendLauncher[]	g_bfExtendLauncher = new ExtendLauncher[(INT)PROTOCOL.PROTOCOL_MAX];

		public static void
		ClientLauncher(CCommand kCommand_) {
			if((0 < kCommand_.GetOrder()) && (kCommand_.GetOrder() < (UINT)PROTOCOL.PROTOCOL_MAX)) {
				NativeLauncher kLauncher = g_bfNativeLauncher[kCommand_.GetOrder()];
				if(isptr(kLauncher)) {
					kLauncher(kCommand_);
				} else {
					MESSAGE("[" + g_kTick.GetTime() + "] error: order is none: " + kCommand_.GetOrder());
				}
			} else {
				MESSAGE("[" + g_kTick.GetTime() + "] error: order range over: " + kCommand_.GetOrder());
			}
		}

		public static void
		ClientLauncher(CExtendCommand kCommand_) {
			CCommand kCommand = kCommand_.GetCommand();
			if((0 < kCommand.GetOrder()) && (kCommand.GetOrder() < (UINT)PROTOCOL.PROTOCOL_MAX)) {
				ExtendLauncher kLauncher = g_bfExtendLauncher[kCommand.GetOrder()];
				if(isptr(kLauncher)) {
					kLauncher(kCommand_);
				} else {
					MESSAGE("[" + g_kTick.GetTime() + "] error: order is none: " + kCommand.GetOrder());
				}
			} else {
				MESSAGE("[" + g_kTick.GetTime() + "] error: order range over: " + kCommand.GetOrder());
			}
		}

		public static void
		InitializeCommand()	{
			InitializeIdCommand();
			InitializeInfoCommand();
			InitializeRoomCommand();
			InitializeUserCommand();
			InitializeOtherCommand();
			InitializeWebCommand();
		}
	}
}

/* EOF */
