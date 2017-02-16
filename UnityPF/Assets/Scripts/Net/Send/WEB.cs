/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
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
		public static bool
		SEND_WEB_AUTHORIZE() {
			if(0 >= ConvertToString(g_kUnitMgr.GetMainPlayer().GetLoginId()).Length) {
				MESSAGE("SEND_ID_AUTHORIZE: login id is empty: ");
				return false;
			}

			//
			// Command Header.
			//
			SCommandHeader tSHeader = new SCommandHeader(true);
			tSHeader.order = (UINT16)PROTOCOL.WEB_AUTHORIZE;
			tSHeader.extra = (BYTE)EXTRA.NONE;

			IntPtr pHeader = Marshal.AllocHGlobal(iCOMMAND_HEAD_SIZE);
			Marshal.StructureToPtr(tSHeader, pHeader, false);

			CHAR[] acHeader = new CHAR[iCOMMAND_HEAD_SIZE];
			Marshal.Copy(pHeader, acHeader, 0, iCOMMAND_HEAD_SIZE);
			Marshal.FreeHGlobal(pHeader);

			//
			// Command Data.
			//
			SWebAuthorizeClToGs tSData = new SWebAuthorizeClToGs(true);
			tSData.major_version = iSERVICE_MAJOR_VERSION;
			tSData.minor_version = iSERVICE_MINOR_VERSION;
			tSData.SetLoginId(g_kUnitMgr.GetMainPlayer().GetLoginId());

			INT iSize = Marshal.SizeOf(tSData);
			IntPtr pData = Marshal.AllocHGlobal(iSize);
			Marshal.StructureToPtr(tSData, pData, false);

			CHAR[] acData = new CHAR[iSize];
			Marshal.Copy(pData, acData, 0, iSize);
			Marshal.FreeHGlobal(pData);

			string szData = "header=" + Base64Encode(acHeader, (UINT)iCOMMAND_HEAD_SIZE) + "&data=" + Base64Encode(acData, (UINT)iSize);

			HttpStatusCode eStatus = g_kNetMgr.Send(g_kCfgMgr.GetRequestUrl(), szData, true);
			if(HttpStatusCode.OK != eStatus) {
				MESSAGE("SEND_ID_AUTHORIZE: sending failed: " + eStatus);
				return false;
			}

			//MESSAGE("SEND_ID_AUTHORIZE: request url: " + g_kCfgMgr.GetRequestUrl() + ", data: " + szData);
			return true;
		}

		public static bool
		SEND_WEB_QUIT() {
			//
			// Command Header.
			//
			SCommandHeader tSHeader = new SCommandHeader(true);
			tSHeader.order = (UINT16)PROTOCOL.WEB_QUIT;
			tSHeader.extra = (BYTE)EXTRA.NONE;

			IntPtr pHeader = Marshal.AllocHGlobal(iCOMMAND_HEAD_SIZE);
			Marshal.StructureToPtr(tSHeader, pHeader, false);

			CHAR[] acHeader = new CHAR[iCOMMAND_HEAD_SIZE];
			Marshal.Copy(pHeader, acHeader, 0, iCOMMAND_HEAD_SIZE);
			Marshal.FreeHGlobal(pHeader);

			string szData = "header=" + Base64Encode(acHeader, (UINT)iCOMMAND_HEAD_SIZE);

			HttpStatusCode eStatus = g_kNetMgr.Send(g_kCfgMgr.GetRequestUrl(), szData);
			if(HttpStatusCode.OK != eStatus) {
				MESSAGE("SEND_WEB_QUIT: sending failed: " + eStatus);
				return false;
			}

			//MESSAGE("SEND_WEB_QUIT: request url: " + g_kCfgMgr.GetRequestUrl() + ", data: " + szData);
			return true;
		}

		public static bool
		SEND_WEB_CHECK() {
			MESSAGE("SEND_WEB_CHECK: request url: " + g_kCfgMgr.GetRequestUrl());

			HttpStatusCode eStatus = g_kNetMgr.Send(g_kCfgMgr.GetRequestUrl());
			if(HttpStatusCode.OK != eStatus) {
				MESSAGE("SEND_WEB_CHECK: sending failed: " + eStatus);
				return false;
			}
			return true;
		}
	}
}

/* EOF */
