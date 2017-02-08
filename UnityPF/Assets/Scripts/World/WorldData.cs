/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Linq;

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
		public struct SUserInfo {
			public SUserInfo(bool o) : this() {
				if(o) {
					aid = 0;
					key = 0;
					name = "";
				}
			}

			public void
			Clear() {
				aid = 0;
				key = 0;
				name = "";
			}

			public UINT		GetAid()			{ return aid; }
			public UINT		GetKey()			{ return key; }

			public string	GetName()			{ return name; }
			public void		SetName(string o)	{ name = o; }

			public UINT		aid;
			public UINT		key;
			public string	name;
		}
	}
}

/* EOF */
