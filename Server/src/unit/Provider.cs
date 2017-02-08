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

	public partial class GameFramework {
		public static CPlayerProvider	g_kPlayerProvider { get { return CPlayerProvider.GetInstance(); } }

		public class CPlayerProvider : CSingleton< CPlayerProvider > {
			public CPlayerProvider() { inner.Clear(); }
			~CPlayerProvider() { inner.Clear(); }

			public virtual void
			Clear() {
				inner.Clear();
			}

			public object
			Create() {
				return inner.Create();
			}

			public object
			Create(INT iTypeId_) {
				return inner.Create(iTypeId_);
			}

			public bool
			Register(CreateCallback ofCreate_) {
				return inner.Register(ofCreate_);
			}

			public bool
			Register(INT iTypeId_, CreateCallback ofCreate_) {
				return inner.Register(iTypeId_, ofCreate_);
			}

			public bool
			Unregister(INT iTypeId_) {
				return inner.Unregister(iTypeId_);
			}

			public bool
			Working() {
				return inner.Working();
			}

			protected CProviding	inner = new CProviding();

		}
	}
}

/* EOF */
