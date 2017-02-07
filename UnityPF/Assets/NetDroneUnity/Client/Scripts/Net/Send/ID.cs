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
		SEND_ID_AUTHORIZE() {
			// 인증.
			if(0 >= ConvertToString(g_kUnitMgr.GetMainPlayer().GetLoginId()).Length) {
				MESSAGE("SEND_ID_AUTHORIZE: login id is empty: ");
				return false;
			}

			if(0 > g_kUnitMgr.GetMainPlayer().GetChannelIndex()) {
				MESSAGE("SEND_ID_AUTHORIZE: channel id is empty: ");
				return false;
			}

			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ID_AUTHORIZE);
			if(0 < g_kUnitMgr.GetMainPlayer().GetAid()) {
				// Reconnect 할 경우만 이렇게 보냄.
				kCommand.SetExtra((UINT)EXTRA.TIMEOUT);
			} else {
				kCommand.SetExtra((UINT)EXTRA.NONE);
			}
//			kCommand.SetOption(0);

			SIdAuthorizeClToGs tSData = new SIdAuthorizeClToGs(true);
			tSData.major_version = iSERVICE_MAJOR_VERSION;
			tSData.minor_version = iSERVICE_MINOR_VERSION;
			tSData.channel_index = (UINT)(g_kUnitMgr.GetMainPlayer().GetChannelIndex());

			tSData.SetLoginId(g_kUnitMgr.GetMainPlayer().GetLoginId());
			//TRACE("login id: " + ConvertToString(tSData.GetLoginId()));

			if(isptr(Dns.GetHostName())) {
				IPAddress[] kLocalAddr = Dns.GetHostAddresses(Dns.GetHostName());
				foreach(IPAddress kAddr in kLocalAddr) {
					if(kAddr.AddressFamily == AddressFamily.InterNetwork) {
						if(false == IPAddress.IsLoopback(kAddr)) {
							g_kNetMgr.GetCurrentConnector().SetLocalAddress(ConvertToBytes(kAddr.ToString()), 0);
						}
					}
				}
			}

			tSData.local_ip = g_kNetMgr.GetCurrentConnector().GetLocalSinAddress();
			tSData.local_port = (UINT16)g_kNetMgr.GetCurrentConnector().GetLocalPort();

			INT iSize = Marshal.SizeOf(tSData);
			kCommand.SetData(tSData, iSize);

			if(0 > g_kNetMgr.Send(kCommand, iSize)) {
				MESSAGE("SEND_ID_AUTHORIZE: sending failed: ");
				return false;
			}

			MESSAGE("SEND_ID_AUTHORIZE: " + (EXTRA)kCommand.GetExtra() + ", local ip: " + tSData.local_ip + ", local port: " + tSData.local_port + ": bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + iSize));
			return true;
		}

		public static bool
		SEND_ID_QUIT() {
			// 종료.
			// 게임 종료시 보냄.
			CCommand kCommand = new CCommand();
			kCommand.SetOrder((UINT)PROTOCOL.ID_QUIT);
			kCommand.SetExtra((UINT)EXTRA.NONE);

			if(0 > g_kNetMgr.Send(kCommand)) {
				MESSAGE("SEND_ID_QUIT: sending failed: ");
				return false;
			}
			MESSAGE("SEND_ID_QUIT: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			return true;
		}

		public static bool
		SEND_ID_PING() {
			// 서버로 보내는 PING.
			if(0 < g_kUnitMgr.GetMainPlayer().GetKey()) {
				CCommand kCommand = new CCommand();
				kCommand.SetOrder((UINT)PROTOCOL.ID_PING);
				kCommand.SetExtra((UINT)EXTRA.NONE);

				if(0 > g_kNetMgr.Send(kCommand)) {
					MESSAGE("SEND_ID_PING: sending failed: ");
					return false;
				}
				MESSAGE("SEND_ID_PING: NONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}
	}
}

/* EOF */
