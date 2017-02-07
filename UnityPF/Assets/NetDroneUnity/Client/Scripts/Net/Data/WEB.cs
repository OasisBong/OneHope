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
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SWebAuthorizeGsToCl {
			public SWebAuthorizeGsToCl(bool o) : this()	{ key = 0; aid = 0; tick = 0; name = new CHAR[iUNIT_NAME_LEN+1]; }

			public CHAR[]	GetName()					{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void		SetName(CHAR[] o)			{ INT iLength = o.Length; if(iUNIT_NAME_LEN < iLength) { iLength = iUNIT_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iUNIT_NAME_LEN] = (CHAR)('\0'); }

			public UINT32	key;
			public UINT32	aid;

			public UINT64	tick;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iUNIT_NAME_LEN+1)]
			public CHAR[]	name;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SWebAuthorizeClToGs {
			public SWebAuthorizeClToGs(bool o) : this()	{ major_version = 0; minor_version = 0; login_id = new CHAR[iLOGIN_ID_LEN+1]; }

			public CHAR[]	GetLoginId()				{ INT iOffset = 0; for(UINT i = 0; i < login_id.Length; ++i) { ++iOffset; if((CHAR)('\0') == login_id[i]) break; } Array.Clear(login_id, iOffset, login_id.Length-iOffset); return login_id; }
			public void		SetLoginId(CHAR[] o)		{ INT iLength = o.Length; if(iLOGIN_ID_LEN < iLength) { iLength = iLOGIN_ID_LEN; } Array.Copy(o, 0, login_id, 0, iLength); login_id[iLOGIN_ID_LEN] = (CHAR)('\0'); }

			public UINT16	major_version;
			public UINT16	minor_version;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iLOGIN_ID_LEN+1)]
			public CHAR[]	login_id;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SWebCheckGsToCl {
			public SWebCheckGsToCl(bool o) : this()	{ if(o) { content = new CHAR[iCOMMAND_DATA_SIZE]; } }

			public CHAR[]	GetContent()				{ INT iOffset = 0; for(UINT i = 0; i < content.Length; ++i) { ++iOffset; if((CHAR)('\0') == content[i]) break; } Array.Clear(content, iOffset, content.Length-iOffset); return content; }
			public void		SetContent(CHAR[] o)		{ INT iLength = o.Length; if(iCOMMAND_DATA_SIZE < iLength) { iLength = iCOMMAND_DATA_SIZE; } Array.Copy(o, 0, content, 0, iLength); content[iCOMMAND_DATA_SIZE] = (CHAR)('\0'); }

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iCOMMAND_DATA_SIZE)]
			public CHAR[]	content;
		}
	}
}

/* EOF */
