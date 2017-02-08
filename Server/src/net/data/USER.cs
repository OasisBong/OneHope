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
		// USER_CHAT
		// Extra : NONE, CHECK, NEW
		// Option : size
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SUserChatClToGs {
			public SUserChatClToGs(bool o) : this()	{ if(o) { content = new CHAR[iMAX_CHAT_LEN+1]; } }

			public CHAR[]	GetContent()				{ INT iOffset = 0; for(UINT i = 0; i < content.Length; ++i) { ++iOffset; if((CHAR)('\0') == content[i]) break; } Array.Clear(content, iOffset, content.Length-iOffset); return content; }
			public void		SetContent(CHAR[] o)		{ INT iLength = o.Length; if(iMAX_CHAT_LEN < iLength) { iLength = iMAX_CHAT_LEN; } Array.Copy(o, 0, content, 0, iLength); content[iMAX_CHAT_LEN] = (CHAR)('\0'); }

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iMAX_CHAT_LEN+1)]
			public CHAR[]	content;
		}

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        public struct SUserMoveClToGs
        {
            public SUserMoveClToGs(bool o) : this() { if (o) { Key = 0; } }

            public UINT32 Key;
        }
    }
}

/* EOF */
