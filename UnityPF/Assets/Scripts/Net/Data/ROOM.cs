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
		// ROOM_CREATE
		// Extra : OK, NOT_ENGOUGH
		// Option : channel
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomCreateClToGs {
			public SRoomCreateClToGs(bool o) : this()	{ if(o) { stage_id = 0; max = 0; name = new CHAR[iROOM_NAME_LEN+1]; } }

			public CHAR[]	GetName()					{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)			{ INT iLength = o.Length; if(iROOM_NAME_LEN < iLength) { iLength = iROOM_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iROOM_NAME_LEN] = (CHAR)('\0'); }

			// 2: 0000 0000 0000 0000 0000 0000 0000 0001, 16: 0x1
			public UINT32 padding {
				get { return (UINT32)(bits & 0x1); }
				set { bits = (UINT32)(bits & ~(0x1) | (value)); }
			}

			// 2: 0000 0000 0000 0000 0000 0000 0111 1110, 16: 0x3F
			public UINT32 max {
				get { return (UINT32)((bits >> 1) & 0x3F); }
				set { bits = (UINT32)(bits & ~(0x3F << 1) | (value << 1)); }
			}

			// 2: 1111 1111 1111 1111 1111 1111 1000 0000, 16: 0x1FFFFFF
			public UINT32 stage_id {
				get { return (UINT32)((bits >> 7) & 0x1FFFFFF); }
				set { bits = (UINT32)(bits & ~(0x1FFFFFF << 7) | (value << 7)); }
			}

			public UINT32		bits;	// bit field: padding:1, max:6, stage_id:25

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iROOM_NAME_LEN+1)]
			public CHAR[]	name;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomCreateGsToCl {
			public SRoomCreateGsToCl(bool o) : this()	{ if(o) { id = 0; } }

			public UINT32	id;
		}

		// ROOM_JOIN
		// Extra : OK, DENY, FAIL, FULL
		// Option : not used
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomJoinClToGs {
			public SRoomJoinClToGs(bool o) : this()	{ if(o) { id = 0; } }

			public UINT32	id;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomJoinGsToCl {
			public SRoomJoinGsToCl(bool o) : this()		{ if(o) { id = 0; max = 0; offset = 0; name = new CHAR[iROOM_NAME_LEN+1]; } }

			public CHAR[]	GetName()				{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)		{ INT iLength = o.Length; if(iROOM_NAME_LEN < iLength) { iLength = iROOM_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iROOM_NAME_LEN] = (CHAR)('\0'); }

			// 2: 0000 0000 0000 0000 0000 0000 0000 0001, 16: 0x1
			public UINT32 offset {
				get { return (UINT32)(bits & 0x1); }
				set { bits = (UINT32)(bits & ~(0x1) | (value)); }
			}

			// 2: 0000 0000 0000 0000 0000 0000 0111 1110, 16: 0x3F
			public UINT32 max {
				get { return (UINT32)((bits >> 1) & 0x3F); }
				set { bits = (UINT32)(bits & ~(0x3F << 1) | (value << 1)); }
			}

			// 2: 1111 1111 1111 1111 1111 1111 1000 0000, 16: 0x1FFFFFF
			public UINT32 stage_id {
				get { return (UINT32)((bits >> 7) & 0x1FFFFFF); }
				set { bits = (UINT32)(bits & ~(0x1FFFFFF << 7) | (value << 7)); }
			}

			public UINT32	id;

			public UINT32	bits;	// bit field: offset:1, max:6, stage_id:25

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iROOM_NAME_LEN+1)]
			public CHAR[]	name;
		}

		// ROOM_INFO
		// Extra : IN, OUT
		// Option : not used
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomInfo {
			public SRoomInfo(bool o) : this()		{ if(o) { id = 0; max = 0; offset = 0; name = new CHAR[iROOM_NAME_LEN+1]; } }

			public CHAR[]	GetName()				{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)		{ INT iLength = o.Length; if(iROOM_NAME_LEN < iLength) { iLength = iROOM_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iROOM_NAME_LEN] = (CHAR)('\0'); }

			// 2: 0000 0000 0000 0000 0000 0000 0000 0001, 16: 0x1
			public UINT32 offset {
				get { return (UINT32)(bits & 0x1); }
				set { bits = (UINT32)(bits & ~(0x1) | (value)); }
			}

			// 2: 0000 0000 0000 0000 0000 0000 0111 1110, 16: 0x3F
			public UINT32 max {
				get { return (UINT32)((bits >> 1) & 0x3F); }
				set { bits = (UINT32)(bits & ~(0x3F << 1) | (value << 1)); }
			}

			// 2: 1111 1111 1111 1111 1111 1111 1000 0000, 16: 0x1FFFFFF
			public UINT32 stage_id {
				get { return (UINT32)((bits >> 7) & 0x1FFFFFF); }
				set { bits = (UINT32)(bits & ~(0x1FFFFFF << 7) | (value << 7)); }
			}

			public UINT32	id;

			public UINT32	bits;	// bit field: offset:1, max:6, stage_id:25

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iROOM_NAME_LEN+1)]
			public CHAR[]	name;
		}

		// ROOM_LIST
		// Extra : NEW, CHANGE, DONE, EMPTY
		// Option : count
		// iMAX_PACKET_ROOM_LIST = 32
		public static readonly INT iMAX_PACKET_ROOM_LIST = iCOMMAND_DATA_SIZE / Marshal.SizeOf(typeof(SRoomInfo));

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomListGsToCl {
			public SRoomListGsToCl(bool o) : this()	{ if(o) { list = (new SRoomInfo[iMAX_PACKET_ROOM_LIST]).Select(x => new SRoomInfo(o)).ToArray(); } }

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public SRoomInfo[]	list;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomMember {
			public SRoomMember(bool o) : this()	{ if(o)	{ actor = 0; public_ip = 0; local_ip = 0; public_port = 0; local_port = 0; status = 0; name = new CHAR[iUNIT_NAME_LEN+1]; } }

			public CHAR[]	GetName()			{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)	{ INT iLength = o.Length; if(iUNIT_NAME_LEN < iLength) { iLength = iUNIT_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iUNIT_NAME_LEN] = (CHAR)('\0'); }

			public UINT32	actor;
			public UINT32	public_ip;
			public UINT32	local_ip;
			public UINT16	public_port;
			public UINT16	local_port;

			public UINT32	status;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iUNIT_NAME_LEN+1)]
			public CHAR[]	name;
		}

		// iMAX_PACKET_ROOM_MEMBER_LIST = 25
		public static readonly INT iMAX_PACKET_ROOM_MEMBER_LIST = iCOMMAND_DATA_SIZE / Marshal.SizeOf(typeof(SRoomMember));

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomMemberListGsToCl {
			public SRoomMemberListGsToCl(bool o) : this()	{ if(o) { list = (new SRoomMember[iMAX_PACKET_ROOM_MEMBER_LIST]).Select(x => new SRoomMember(o)).ToArray(); } }

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
			public SRoomMember[]	list;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomMemberLeaderGsToCl {
			public SRoomMemberLeaderGsToCl(bool o) : this()	{ if(o) { leader = 0; } }

			public UINT32	leader;
		}

		// ROOM_JOIN_OTHER
		// Extra : NONE
		// Option : not used
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomJoinOtherGsToCl {
			public SRoomJoinOtherGsToCl(bool o) : this()	{ if(o)	{ actor = 0; public_ip = 0; local_ip = 0; public_port = 0; local_port = 0; name = new CHAR[iUNIT_NAME_LEN+1]; } }

			public CHAR[]	GetName()			{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)	{ INT iLength = o.Length; if(iUNIT_NAME_LEN < iLength) { iLength = iUNIT_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iUNIT_NAME_LEN] = (CHAR)('\0'); }

			public UINT32	actor;

			public UINT32	public_ip;
			public UINT32	local_ip;
			public UINT16	public_port;
			public UINT16	local_port;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iUNIT_NAME_LEN+1)]
			public CHAR[]	name;
		}

		// ROOM_LEAVE_OTHER
		// Extra : OK, CHECK
		// Option : not used
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomLeaveOtherGsToCl {
			public SRoomLeaveOtherGsToCl(bool o) : this()	{ if(o) { actor = 0; leader = 0; } }

			public UINT32	actor;
			public UINT32	leader;
		}

		// ROOM_START_OTHER
		// Extra : OK
		// Option : not used
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomStartOtherGsToCl {
			public SRoomStartOtherGsToCl(bool o) : this()	{ if(o) { actor = 0; } }

			public UINT32	actor;
		}

		// ROOM_STOP_OTHER
		// Extra : OK
		// Option : not used
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SRoomStopOtherGsToCl {
			public SRoomStopOtherGsToCl(bool o) : this()	{ if(o) { leader = 0; } }

			public UINT32	leader;
		}

	}
}

/* EOF */
