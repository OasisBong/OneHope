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
		public const INT iMAX_RECONNECT_RETRY_COUNT = 8;
		public const INT iMAX_RECONNECT_RETRY_TICK = 100;

		public const INT iMAX_RECONNECT_LATENCY_TICK = 500;	// 5 sec
		public const INT iMAX_TIMEOUT_LATENCY_TICK = 2000;	// 20 sec

		public const INT iLOGIN_ID_LEN = 32;
		public const INT iUNIT_NAME_LEN = 32;

		public const INT iMAX_CHAT_LEN = 255;

		public const INT iMAX_ROOM_MEMBERS = 16;
		public const INT iROOM_NAME_LEN = 32;

		public const INT iMAX_CHANNEL_LIST = 10;
		public const INT iMAX_CHANNEL_USERS = 500;
		public const INT iMAX_ROOMS = 100;
	}
}

/* EOF */
