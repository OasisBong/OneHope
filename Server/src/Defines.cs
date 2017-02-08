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

	public partial class GameFramework : AutoFramework {
		public const string KEY_DEFAULT_INTERFACE = "DefaultInterface";
		public const string KEY_DEFAULT_PORT = "DefaultPort";
		public const string KEY_BACKBONE_INTERFACE = "BackboneInterface";
		public const string KEY_BACKBONE_PORT ="BackbonePort";
		public const string KEY_RELIABLE_INTERFACE = "ReliableInterface";
		public const string KEY_RELIABLE_PORT = "ReliablePort";
		public const string KEY_MAX_CONNECTION = "MaxConnection";
		public const string KEY_HEADER_CRYPT = "HeaderCrypt";
		public const string KEY_LOG_PATH = "LogPath";
		public const string	KEY_INFO_PATH = "InfoPath";

		public static UINT	Max(UINT x, UINT y)					{ return (x > y) ? x : y; }
		public static UINT	Min(UINT x, UINT y)					{ return (x < y) ? x : y; }

//		public static void	SAFE_DELETE(ref SPlayerData o)		{ o = null; }
//		public static void	SAFE_DELETE(ref SNpcData o)			{ o = null; }
		public static void	SAFE_DELETE(ref CUnit o)			{ o = null; }
		public static void	SAFE_DELETE(ref CRoomHandler o)		{ o = null; }
		public static void	SAFE_DELETE(ref CNetIO o)			{ o = null; }

		public static void	SAFE_DELETE_RELEASE(ref CRoom o)	{ if(isptr(o)) { o.Release(); } o = null; }
	}
}

/* EOF */
