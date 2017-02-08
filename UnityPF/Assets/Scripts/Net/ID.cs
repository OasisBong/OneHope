/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Collections.Generic;
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
		CMD_ID_AUTHORIZE(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				//kCommand_.GetOption();	// Login Type

				CNetIO kNetIO = g_kUnitMgr.GetMainPlayer().GetNetIO();
				if(isptr(kNetIO)) {
					SIdAuthorizeGsToCl tRData = (SIdAuthorizeGsToCl)kCommand_.GetData(typeof(SIdAuthorizeGsToCl));

					g_kUnitMgr.GetMainPlayer().SetKey(tRData.key);
					g_kUnitMgr.GetMainPlayer().SetAid(tRData.aid);
					g_kUnitMgr.GetMainPlayer().SetName(tRData.GetName());

					kNetIO.GetConnector().SetPublicAddress(tRData.public_ip, tRData.public_port);

					g_kTick.Reset(tRData.tick * 10);

					kNetIO.SetSendPingServerTick(g_kTick.GetTick());

					MESSAGE("CMD_ID_AUTHORIZE: OK: key: " + g_kUnitMgr.GetMainPlayer().GetKey() + ", aid: " + g_kUnitMgr.GetMainPlayer().GetAid() + ", name: " + ConvertToString(g_kUnitMgr.GetMainPlayer().GetName()) + ", public ip: " + ConvertToString(kNetIO.GetConnector().GetPublicAddress()) + ", public port: " + kNetIO.GetConnector().GetPublicPort() + ", local ip: " + ConvertToString(kNetIO.GetConnector().GetLocalAddress()) + ", local port: " + kNetIO.GetConnector().GetLocalPort() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));

					if(STATE_TYPE.STATE_CHANNEL > g_kStateMgr.GetTransitionType()) {
						g_kStateMgr.SetTransition(STATE_TYPE.STATE_CHANNEL);
					}
					return true;
				}
			} else if(EXTRA.TIMEOUT == (EXTRA)kCommand_.GetExtra()) {
				// 자동 재접속.

				CNetIO kNetIO = g_kUnitMgr.GetMainPlayer().GetNetIO();
				if(isptr(kNetIO)) {
					SIdAuthorizeGsToCl tRData = (SIdAuthorizeGsToCl)kCommand_.GetData(typeof(SIdAuthorizeGsToCl));

					g_kUnitMgr.GetMainPlayer().SetKey(tRData.key);
					g_kUnitMgr.GetMainPlayer().SetAid(tRData.aid);
					g_kUnitMgr.GetMainPlayer().SetName(tRData.GetName());

					kNetIO.GetConnector().SetPublicAddress(tRData.public_ip, tRData.public_port);

					g_kTick.Reset(tRData.tick * 10);

					kNetIO.SetSendPingServerTick(g_kTick.GetTick());

					MESSAGE("CMD_ID_AUTHORIZE: TIMEOUT: key: " + g_kUnitMgr.GetMainPlayer().GetKey() + ", aid: " + g_kUnitMgr.GetMainPlayer().GetAid() + ", name: " + ConvertToString(g_kUnitMgr.GetMainPlayer().GetName()) + ", public ip: " + ConvertToString(kNetIO.GetConnector().GetPublicAddress()) + ", public port: " + kNetIO.GetConnector().GetPublicPort() + ", local ip: " + ConvertToString(kNetIO.GetConnector().GetLocalAddress()) + ", local port: " + kNetIO.GetConnector().GetLocalPort() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));

					if(STATE_TYPE.STATE_CHANNEL > g_kStateMgr.GetTransitionType()) {
						g_kStateMgr.SetTransition(STATE_TYPE.STATE_CHANNEL);
					} else {
						// 자동 Join 시키고 싶으면 서버쪽으로 방정보 모두를 요청하여 받아서 처리하면 됨.
						// 이렇게하면 다른 플레이어들은 해당 플레이어가 잠시 사라졌다 나타나는 정도로만 보임.

						// 지금은 로비로만 이동, 나중에 시간날때 추가할께요.
						g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOBBY);
					}
					return true;
				}
			} else if(EXTRA.BUSY == (EXTRA)kCommand_.GetExtra()) {
				if(kCommand_.GetOption() == 0) {
					// 중복 접속.
					MESSAGE("CMD_ID_AUTHORIZE: BUSY: duplicate name, bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
				} else if(kCommand_.GetOption() == 1) {
					// 채널 꽉참
					MESSAGE("CMD_ID_AUTHORIZE: BUSY: channel is full, bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
				}
			} else if(EXTRA.DENY == (EXTRA)kCommand_.GetExtra()) {
				// 서버에서 연결 거부.
				MESSAGE("CMD_ID_AUTHORIZE: DENY: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.DATA_ERROR == (EXTRA)kCommand_.GetExtra()) {
				// 다른 버전.
				MESSAGE("CMD_ID_AUTHORIZE: DATA_ERROR: version is not valid: " + iSERVICE_MAJOR_VERSION + "." + iSERVICE_MINOR_VERSION + "." + iSERVICE_PATCH_VERSION + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 인증 실패.
				MESSAGE("CMD_ID_AUTHORIZE: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_ID_PONG(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				// 스테이지 플레이 중이 아닐 경우만 서버 Tick 동기화.
				SIdPongGsToCl tRData = (SIdPongGsToCl)kCommand_.GetData(typeof(SIdPongGsToCl));
				if(0 < tRData.tick) {
					g_kTick.Reset(tRData.tick * 10);
				}

				FLOAT fDelaySec = 0;
				CNetIO kNetIO = g_kUnitMgr.GetMainPlayer().GetNetIO();
				if(isptr(kNetIO)) {
					if(0 == kNetIO.GetDelayPingTick()) {
						kNetIO.SetDelayPingTick(g_kTick.GetTick());
					} else {
						fDelaySec = (FLOAT)((FLOAT)(g_kTick.GetTick() - kNetIO.GetDelayPingTick()) - (FLOAT)(iMAX_PING_LATENCY_TICK)) / 100;
						kNetIO.SetDelayPingTick(g_kTick.GetTick());
					}
				}

				MESSAGE("CMD_ID_PONG: OK: tick: " + tRData.tick + ", delay: " + String.Format("{0:F2}", fDelaySec) + " sec, bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));
			}
			return true;
		}

		public static bool
		CMD_ID_QUIT(CCommand kCommand_) {
			MESSAGE("CMD_ID_QUIT: order: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));

			g_kNetMgr.DisconnectAll();

			if(STATE_TYPE.STATE_LOGIN != g_kStateMgr.GetTransitionType()) {
				g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
			}
			return true;
		}

		public static void
		InitializeIdCommand()	{
			g_bfNativeLauncher[(INT)(PROTOCOL.ID_AUTHORIZE)] = new NativeLauncher(CMD_ID_AUTHORIZE);
			g_bfNativeLauncher[(INT)(PROTOCOL.ID_PONG)] = new NativeLauncher(CMD_ID_PONG);
			g_bfNativeLauncher[(INT)(PROTOCOL.ID_QUIT)] = new NativeLauncher(CMD_ID_QUIT);
		}
	}
}

/* EOF */
