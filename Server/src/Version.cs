/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Reflection;
using System.IO;

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
		public const INT iSERVICE_MAJOR_VERSION = 1;
		public const INT iSERVICE_MINOR_VERSION = 2;
		public const INT iSERVICE_PATCH_VERSION = 3;

		public static string
		GetServiceVersion() {
			StringBuilder kBuilder = new StringBuilder();
			kBuilder.Append(Assembly.GetExecutingAssembly().GetName().Name);
			kBuilder.Append(" ");
			kBuilder.Append(iSERVICE_MAJOR_VERSION + "." + iSERVICE_MINOR_VERSION + "." + iSERVICE_PATCH_VERSION);
			kBuilder.Append(" (");
			kBuilder.Append((new FileInfo(Assembly.GetExecutingAssembly().Location)).CreationTime);
			kBuilder.Append(")");

			return kBuilder.ToString();
		}
	}
}

/* EOF */
