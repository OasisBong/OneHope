/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
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
		public const INT iLOGIN_ID_LEN = 32;
		public const INT iUNIT_NAME_LEN = 32;

		public const INT iMAX_CHAT_LEN = 255;

		public const INT iMAX_ROOM_MEMBERS = 16;
		public const INT iROOM_NAME_LEN = 32;

		public const INT iMAX_SERVER_LIST = 20;
		public const INT iMAX_CHANNEL_LIST = 10;
		public const INT iMAX_CHANNEL_USERS = 500;
		public const INT iMAX_ROOMS = 100;

		public const INT iMAX_UNIT_CAPACITY = 60000;
		public const INT iMAX_TRUNK_CAPACITY = 10;

#if DEBUG
		// 클라이언트 디버깅(breakpoint)시 응답대기 시간을 지연 시키려면 아래 Tick 값을 길게 잡으면 됩니다.
		public const INT iRECV_PING_TICK = 3200;				// 32 sec
#else
		public const INT iRECV_PING_TICK = 1200;
#endif

		public const INT iMAX_OPTIONS = 256;
	}
}

/* EOF */
