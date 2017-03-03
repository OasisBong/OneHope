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
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack =4)]
        public struct SUserMoveClToGs
        {
            public SUserMoveClToGs(bool o) : this() { if (o) { Key = 0; x = 0f; y = 0f; z = 0f; rw = 0f; rx = 0f; ry = 0f; rz = 0f; } }

            public UINT32   GetKey() { return Key; }
            public void     SetKet(UINT32 o) { Key = o; }

            public float GetX() { return x; }
            public float GetY() { return y; }
            public float GetZ() { return z; }
            public void SetPosition(float _x, float _y, float _z) { x = _x; y = _y; z = _z; }

            public float GetrW() { return rw; }
            public float GetrX() { return rx; }
            public float GetrY() { return ry; }
            public float GetrZ() { return rz; }
            public void SetRoteta(float _w, float _x, float _y, float _z) { rw = _w; rx = _x; ry = _y; rz = _z; }

            //==넘겨줄 데이터 선언==//
            UINT32 Key;
            float x;
            float y;
            float z;

            float rw;
            float rx;
            float ry;
            float rz;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        public struct SUserRotationClToGs
        {
            public SUserRotationClToGs(bool o) : this() { if (o) { Key = 0; rw = 0f; rx = 0f; ry = 0f; rz = 0f; } }

            public UINT32 GetKey() { return Key; }
            public void SetKet(UINT32 o) { Key = o; }

            public float GetrW() { return rw; }
            public float GetrX() { return rx; }
            public float GetrY() { return ry; }
            public float GetrZ() { return rz; }
            public void SetRoteta(float _w, float _x, float _y, float _z) { rw = _w; rx = _x; ry = _y; rz = _z; }

            //==넘겨줄 데이터 선언==//
            UINT32 Key;

            float rw;
            float rx;
            float ry;
            float rz;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        public struct SUserCreateObj
        {
            public SUserCreateObj(bool o) : this() { if (o) { name = new CHAR[iMAX_CHAT_LEN + 1]; x = 0f; y = 0f; z = 0f; } }

            public CHAR[]   GetName() { INT iOffset = 0; for (UINT i = 0; i < name.Length; ++i) { ++iOffset; if ((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length - iOffset); return name; }
            public void     SetName(CHAR[] o) { INT iLength = o.Length; if (iMAX_CHAT_LEN < iLength) { iLength = iMAX_CHAT_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iMAX_CHAT_LEN] = (CHAR)('\0'); }

            public float    GetX() { return x; }
            public float    GetY() { return y; }
            public float    GetZ() { return z; }
            public void     SetPosition(float _x, float _y, float _z) { x = _x; y = _y; z = _z; }
            //==넘겨줄 데이터 선언==//
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = iMAX_CHAT_LEN + 1)]
            CHAR[] name;

            float x;
            float y;
            float z;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        public struct sUserAnimator
        {
            public sUserAnimator(bool o) : this() { if (o) { name = new CHAR[iMAX_CHAT_LEN + 1]; Index = 0; } }

            public CHAR[] GetName() { INT iOffset = 0; for (UINT i = 0; i < name.Length; ++i) { ++iOffset; if ((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length - iOffset); return name; }
            public void SetName(CHAR[] o) { INT iLength = o.Length; if (iMAX_CHAT_LEN < iLength) { iLength = iMAX_CHAT_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iMAX_CHAT_LEN] = (CHAR)('\0'); }

            public ANIME_NUM GetAniIndex() { return Index; }
            public void SetAniIndex(ANIME_NUM o) { Index = o; }
            //==넘겨줄 데이터 선언==//
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = iMAX_CHAT_LEN + 1)]
            CHAR[] name;

            ANIME_NUM Index;
        }
    }
}

/* EOF */