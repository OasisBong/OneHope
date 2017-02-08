/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Linq;
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
		// INFO_SERVER
		// Extra : NONE
		// Option : not used
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SInfoServerClToGs {
			public SInfoServerClToGs(bool o) : this() { if(o) { major_version = 0; minor_version = 0; } }

			public UINT16	major_version;
			public UINT16	minor_version;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SInfoServerGsToCl {
			public SInfoServerGsToCl(bool o) : this() { if(o) { serial = 0; key = 0; } }

			public INT32	serial;
			public UINT32	key;
		}

		// INFO_CHANNEL
		// EXtra : IN, OUT
		// Option : channel index
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SInfoChannelInGsToCl {
			public SInfoChannelInGsToCl(bool o) : this()	{ if(o) { aid = 0; key = 0; name = new CHAR[iUNIT_NAME_LEN+1]; } }

			public CHAR[]	GetName()					{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)			{ INT iLength = o.Length; if(iUNIT_NAME_LEN < iLength) { iLength = iUNIT_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iUNIT_NAME_LEN] = (CHAR)('\0'); }

			public UINT32	aid;
			public UINT32	key;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iUNIT_NAME_LEN+1)]
			public CHAR[]	name;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SInfoChannelOutGsToCl {
			public SInfoChannelOutGsToCl(bool o) : this() { if(o) { key = 0; } }

			public UINT32	key;
		}

		// INFO_USER_LIST
		// Extra : NEW, CHANGE
		// Option : count
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SInfoUserData {
			public SInfoUserData(bool o) : this()		{ if(o) { aid = 0; key = 0; name = new CHAR[iUNIT_NAME_LEN+1]; } }

			public CHAR[]	GetName()					{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)			{ INT iLength = o.Length; if(iUNIT_NAME_LEN < iLength) { iLength = iUNIT_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iUNIT_NAME_LEN] = (CHAR)('\0'); }

			public UINT32	aid;
			public UINT32	key;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iUNIT_NAME_LEN+1)]
			public CHAR[]	name;
		}

		// iMAX_PACKET_INFO_USER_LIST = 32
		public static readonly INT iMAX_PACKET_INFO_USER_LIST = iCOMMAND_DATA_SIZE / Marshal.SizeOf(typeof(SInfoUserData));

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SInfoUserListGsToCl {
			public SInfoUserListGsToCl(bool o) : this() { if(o) { list = (new SInfoUserData[iMAX_PACKET_INFO_USER_LIST]).Select(x => new SInfoUserData(o)).ToArray(); } }

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public SInfoUserData[]	list;
		}
	}
}

/* EOF */
