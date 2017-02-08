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
		public struct SUnitSelfData {
			public SUnitSelfData(bool o) : this()				{ if(o) { key = 0; } }
			public SUnitSelfData(SUnitSelfData o) : this()		{ key = o.GetKey(); }
			public SUnitSelfData(SPlayerData o) : this()		{ key = o.GetKey(); }
			public SUnitSelfData(SNpcData o) : this()			{ key = o.GetKey(); }

			public void
			Clear() {
				key = 0;
			}

			public	UINT				GetKey()				{ return key; }
			public void					SetKey(UINT o)			{ key = o; }

			public UINT32	key;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SUnitOtherData {
			public SUnitOtherData(bool o) : this()				{ if(o) { name = new CHAR[iUNIT_NAME_LEN+1]; status = 0; } }
			public SUnitOtherData(SUnitOtherData o) : this()	{ name = new CHAR[iUNIT_NAME_LEN+1]; Array.Copy(o.GetName(), 0, name, 0, o.GetName().Length); status = (UINT)o.GetStatus(); }
			public SUnitOtherData(SPlayerData o) : this()		{ name = new CHAR[iUNIT_NAME_LEN+1]; Array.Copy(o.GetName(), 0, name, 0, o.GetName().Length); status = (UINT)o.GetStatus(); }
			public SUnitOtherData(SNpcData o) : this()			{ name = new CHAR[iUNIT_NAME_LEN+1]; Array.Copy(o.GetName(), 0, name, 0, o.GetName().Length); status = (UINT)o.GetStatus(); }

			public void
			Clear() {
				Array.Clear(name, 0, name.Length);
				status = 0;
			}

			public STATUS_TYPE		GetStatus()					{ return (STATUS_TYPE)(status); }
			public void				SetStatus(STATUS_TYPE o)	{ status = (UINT32)(o); }

			public CHAR[]			GetName()					{ INT iOffset = 0; for(UINT i = 0; i < name.Length; ++i) { ++iOffset; if((CHAR)('\0') == name[i]) break; } Array.Clear(name, iOffset, name.Length-iOffset); return name; }
			public void				SetName(CHAR[] o)			{ INT iLength = o.Length; if(iUNIT_NAME_LEN < iLength) { iLength = iUNIT_NAME_LEN; } Array.Copy(o, 0, name, 0, iLength); name[iUNIT_NAME_LEN] = (CHAR)('\0'); }

			// 2: 0000 0000 0000 1111 1111 1111 1111 1111, 16: 0xFFFFF
			public UINT32			padding1					{ get { return (UINT32)((bits >> 12) & 0xFFFFF); } set { bits = (UINT32)(bits & ~(0xFFFFF << 12) | (value << 12)); } }
			// 2: 0000 0000 0000 0000 0000 0000 0000 1111, 16: 0xF
			public UINT32			padding2					{ get { return (UINT32)((bits >> 8) & 0xF); } set { bits = (UINT32)(bits & ~(0xF << 8) | (value << 8)); } }
			// 2: 0000 0000 0000 0000 0000 0000 0000 1111, 16: 0xF
			public UINT32			padding3					{ get { return (UINT32)((bits >> 4) & 0xF); } set { bits = (UINT32)(bits & ~(0xF << 4) | (value << 4)); } }
			// 2: 0000 0000 0000 0000 0000 0000 0000 1111, 16: 0xF
			public UINT32			status						{ get { return (UINT32)((bits) & 0xF); } set { bits = (UINT32)(bits & ~(0xF) | (value)); } }

			// 서버 구조체를 기준으로 비트 제한자가 있을 경우 역순으로 정의해야 올바로 작동함.
			public UINT32			bits;	// padding1:20, padding2:4, padding3:4, status:4

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = iUNIT_NAME_LEN+1)]
			public CHAR[]			name;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SPlayerData {
			public SPlayerData(bool o) : this() {
				if(o) {
					self = new SUnitSelfData(o);
					other = new SUnitOtherData(o);
					aid = 0;
				}
			}

			public SPlayerData(SPlayerData o) : this() {
				self = new SUnitSelfData(o);
				other = new SUnitOtherData(o);
				aid = o.aid;
			}

			public void
			Reset(SPlayerData o) {
				self.SetKey(o.GetKey());
				other.SetName(o.GetName());
				other.SetStatus(o.GetStatus());
				aid = o.aid;
			}

			public void
			ClearUnitSelfData() {
				self.Clear();
			}

			public void
			ClearUnitOtherData() {
				other.Clear();
			}

			public void
			ClearPlayerData() {
				aid = 0;
			}

			public void
			Clear() {
				self.Clear();
				other.Clear();
				aid = 0;
			}

			public SUnitSelfData	GetUnitSelfData()			{ return self; }
			public SUnitOtherData	GetUnitOtherData()			{ return other; }
			public SPlayerData		GetPlayerData()				{ return this; }

			#region SUnitSelfData
			public	UINT			GetKey()					{ return self.GetKey(); }
			public void				SetKey(UINT o)				{ self.SetKey(o); }

			SUnitSelfData			self;
			#endregion

			#region SUnitOtherData
			public STATUS_TYPE		GetStatus()					{ return other.GetStatus(); }
			public void				SetStatus(STATUS_TYPE o)	{ other.SetStatus(o); }

			public CHAR[]			GetName()					{ return other.GetName(); }
			public void				SetName(CHAR[] o)			{ other.SetName(o); }

			SUnitOtherData			other;
			#endregion

			#region SPlayerData
			public UINT				GetAid()					{ return aid; }
			public void				SetAid(UINT o)				{ aid = o; }

			public UINT32			aid;
			#endregion
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack=4)]
		public struct SNpcData {
			public SNpcData(bool o) : this() {
				if(o) {
					self = new SUnitSelfData(o);
					other = new SUnitOtherData(o);
				}
			}

			public SNpcData(SNpcData o) : this() {
				self = new SUnitSelfData(o);
				other = new SUnitOtherData(o);
			}

			public void
			Reset(SNpcData o) {
				self.SetKey(o.GetKey());
				other.SetName(o.GetName());
				other.SetStatus(o.GetStatus());
			}

			public void
			ClearUnitSelfData() {
				self.Clear();
			}

			public void
			ClearUnitOtherData() {
				other.Clear();
			}

			public void
			ClearNpcData() {
			}

			public void
			Clear() {
				self.Clear();
				other.Clear();
			}

			public SUnitSelfData	GetUnitSelfData()			{ return self; }
			public SUnitOtherData	GetUnitOtherData()			{ return other; }
			public SNpcData			GetNpcData()				{ return this; }

			#region SUnitSelfData
			public	UINT			GetKey()					{ return self.GetKey(); }
			public void				SetKey(UINT o)				{ self.SetKey(o); }

			SUnitSelfData			self;
			#endregion

			#region SUnitOtherData
			public STATUS_TYPE		GetStatus()					{ return other.GetStatus(); }
			public void				SetStatus(STATUS_TYPE o)	{ other.SetStatus(o); }

			public CHAR[]			GetName()					{ return other.GetName(); }
			public void				SetName(CHAR[] o)			{ other.SetName(o); }

			SUnitOtherData			other;
			#endregion

			#region SNpcData

			#endregion
		}
	}
}

/* EOF */
