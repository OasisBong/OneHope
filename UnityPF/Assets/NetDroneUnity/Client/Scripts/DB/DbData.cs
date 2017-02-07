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
		public struct SStageInfo {
			public SStageInfo(bool o) : this() {
				if(o) {
					id = 0;
					stage_type = STAGE_TYPE.STAGE_NONE;
					name = "";
					desc = "";
				}
			}

			public void
			Clear() {
				id = 0;
				stage_type = STAGE_TYPE.STAGE_NONE;
				name = "";
				desc = "";
			}

			public UINT				GetId()					{ return id; }
			public STAGE_TYPE		GetStageType()			{ return stage_type; }

			public string			GetName()				{ return name; }
			public void				SetName(string o)		{ name = o; }

			public string			GetDesc()				{ return desc; }
			public void				SetDesc(string o)		{ desc = o; }

			public UINT				id;
			public STAGE_TYPE		stage_type;

			public string			name;
			public string			desc;
		}

		public struct SChannelInfo {
			public SChannelInfo(bool o) : this() {
				if(o) {
					id = 0;
					index = 0;
					user = 0;
					name = "";
				}
			}

			public void
			Clear() {
				id = 0;
				index = 0;
				user = 0;
				name = "";
			}

			public UINT		GetId()				{ return id; }
			public UINT		GetIndex()			{ return index; }
			public UINT		GetUser()			{ return user; }

			public string	GetName()			{ return name; }
			public void		SetName(string o)	{ name = o; }

			public UINT		id;
			public UINT		index;
			public UINT		user;
			public string	name;
		}
	}
}

/* EOF */
