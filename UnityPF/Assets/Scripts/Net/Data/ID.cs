/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Linq;
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
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SIdAuthorizeGsToCl {
			public SIdAuthorizeGsToCl(bool o) : this()	{ key = 0; aid = 0; public_ip = 0; public_port = 0; tick = 0; name = new CHAR[iUNIT_NAME_LEN+1]; }

			// 포인터로 읽어와 아래처럼 정리하지 않으면 쓰레기값이 붙어나옴.
			// 서버에서는 문자열값 뒤에 항상 '\0'을 붙여 잘못 날아오는 경우가 없음.
			public CHAR[]	GetName()					{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)			{ INT iLength = o.Length; if(iUNIT_NAME_LEN < iLength) { iLength = iUNIT_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iUNIT_NAME_LEN] = (CHAR)('\0'); }

			// 아래는 4바이트 패킷 하나로 9개의 값을 전달하는 예제임.
			// 2: 0000 0000 0000 0000 0000 0111 1111 1111, 16: 0x7FF
			public UINT32	padding1					{ get { return (UINT32)((bits) & 0x7FF); } set { bits = (UINT32)(bits & ~(0x7FF) | (value)); } }
			// 2: 0000 0000 0000 0000 0000 1000 0000 0000, 16: 0x1
			public UINT32	padding2					{ get { return (UINT32)((bits >> 11) & 0x1); } set { bits = (UINT32)(bits & ~(0x1 << 11) | (value << 11)); } }
			// 2: 0000 0000 0000 0000 0001 0000 0000 0000, 16: 0x1
			public UINT32	padding3					{ get { return (UINT32)((bits >> 12) & 0x1); } set { bits = (UINT32)(bits & ~(0x1 << 12) | (value << 12)); } }
			// 2: 0000 0000 0000 0000 0010 0000 0000 0000, 16: 0x1
			public UINT32	padding4					{ get { return (UINT32)((bits >> 13) & 0x1); } set { bits = (UINT32)(bits & ~(0x1 << 13) | (value << 13)); } }
			// 2: 0000 0000 0000 0111 1100 0000 0000 0000, 16: 0x1F
			public UINT32	padding5					{ get { return (UINT32)((bits >> 14) & 0x1F); } set { bits = (UINT32)(bits & ~(0x1F << 14) | (value << 14)); } }
			// 2: 0000 0000 0000 1000 0000 0000 0000 0000, 16: 0x1
			public UINT32	padding6					{ get { return (UINT32)((bits >> 19) & 0x1); } set { bits = (UINT32)(bits & ~(0x1 << 19) | (value << 19)); } }
			// 2: 0000 0000 0001 0000 0000 0000 0000 0000, 16: 0x1
			public UINT32	padding7					{ get { return (UINT32)((bits >> 20) & 0x1); } set { bits = (UINT32)(bits & ~(0x1 << 20) | (value << 20)); } }
			// 2: 0000 0000 0010 0000 0000 0000 0000 0000, 16: 0x1
			public UINT32	padding8					{ get { return (UINT32)((bits >> 21) & 0x1); } set { bits = (UINT32)(bits & ~(0x1 << 21) | (value << 21)); } }
			// 2: 1111 1111 1100 0000 0000 0000 0000 0000, 16: 0x3FF
			public UINT32	padding9					{ get { return (UINT32)((bits >> 22) & 0x3FF); } set { bits = (UINT32)(bits & ~(0x3FF << 22) | (value << 22)); } }

			public UINT32	key;
			public UINT32	aid;
			// 계산법이 조금 어렵지만 C/C++ 구조체 비트와 오버헤드 없이 호환되는 방법임.
			// 비트 계산기를 활용하면 편함. (C/C++ 구조체 코드를 넣으면 자동변환 되도록 툴 작성중).
			// 4 byte 만으로 9종류의 값을 주고받는 예제임.
			public UINT32	bits;		// bit field: padding1:11, padding2:1, padding3:1, padding4:1, padding5:5, padding6:1, padding7:1, padding8:1, padding9:10

			public UINT32	public_ip;
			public UINT16	public_port;
			public UINT16	padding;

			public UINT64	tick;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iUNIT_NAME_LEN+1)]
			public CHAR[]	name;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SIdAuthorizeClToGs {
			public SIdAuthorizeClToGs(bool o) : this()	{ major_version = 0; minor_version = 0; channel_index = 0; local_ip = 0; local_port = 0; login_id = new CHAR[iLOGIN_ID_LEN+1]; }

			public CHAR[]	GetLoginId()				{ INT iOffset = 0; for(UINT i = 0; i < login_id.Length; ++i) { ++iOffset; if((CHAR)('\0') == login_id[i]) break; } Array.Clear(login_id, iOffset, login_id.Length-iOffset); return login_id; }
			public void		SetLoginId(CHAR[] o)		{ INT iLength = o.Length; if(iLOGIN_ID_LEN < iLength) { iLength = iLOGIN_ID_LEN; } Array.Copy(o, 0, login_id, 0, iLength); login_id[iLOGIN_ID_LEN] = (CHAR)('\0'); }

			public UINT16	major_version;
			public UINT16	minor_version;
			public UINT32	channel_index;
			public UINT32	local_ip;
			public UINT16	local_port;
			public UINT16	padding;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iLOGIN_ID_LEN+1)]
			public CHAR[]	login_id;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SIdPongGsToCl {
			public SIdPongGsToCl(bool o) : this()		{ tick = 0; }

			public UINT64	tick;
		}
	}
}

/* EOF */
