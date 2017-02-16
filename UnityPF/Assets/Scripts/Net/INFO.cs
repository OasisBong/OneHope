/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
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
		CMD_INFO_SERVER(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				CNetIO kNetIO = g_kUnitMgr.GetMainPlayer().GetNetIO();
				if(isptr(kNetIO)) {
					SInfoServerGsToCl tRData = (SInfoServerGsToCl)kCommand_.GetData(typeof(SInfoServerGsToCl));

					kNetIO.GetConnector().SetSendKey(tRData.key);
					kNetIO.GetConnector().SetSerialKey(tRData.serial);

					MESSAGE("CMD_INFO_SERVER: OK: serial: " + tRData.serial + ", key: " + tRData.key + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));
				}
			} else {
				MESSAGE("CMD_INFO_SERVER: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_INFO_CHANNEL(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				MESSAGE("CMD_INFO_CHANNEL: OK: channel index: " + kCommand_.GetOption() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));

				if(STATE_TYPE.STATE_LOBBY > g_kStateMgr.GetTransitionType()) {
					g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOBBY);
				}
			} else if(EXTRA.IN == (EXTRA)kCommand_.GetExtra()) {
				SInfoChannelInGsToCl tRData = (SInfoChannelInGsToCl)kCommand_.GetData(typeof(SInfoChannelInGsToCl));

				SUserInfo tUserInfo = new SUserInfo(true);
				tUserInfo.aid = tRData.aid;
				tUserInfo.key = tRData.key;
				tUserInfo.SetName(ConvertToString(tRData.GetName()));

				MESSAGE("CMD_INFO_CHANNEL: IN: key: " + tUserInfo.key + ", aid: " + tUserInfo.aid + ", name: " + tUserInfo.GetName() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));

				g_kChannelMgr.AddUserList(tUserInfo);
				REFRESH_USER_LIST();
			} else if(EXTRA.OUT == (EXTRA)kCommand_.GetExtra()) {
				SInfoChannelOutGsToCl tRData = (SInfoChannelOutGsToCl)kCommand_.GetData(typeof(SInfoChannelOutGsToCl));

				SUserInfo tUserInfo = g_kChannelMgr.FindUserList(tRData.key);
				MESSAGE("CMD_INFO_CHANNEL: OUT: key: " + tUserInfo.key + ", aid: " + tUserInfo.aid + ", name: " + tUserInfo.GetName() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));

				g_kChannelMgr.RemoveUserList(tRData.key);
				REFRESH_USER_LIST();
			} else if(EXTRA.DENY == (EXTRA)kCommand_.GetExtra()) {
				MESSAGE("CMD_INFO_CHANNEL: DENY: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else {
				MESSAGE("CMD_INFO_CHANNEL: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_INFO_NOTIFY(CCommand kCommand_) {
			MESSAGE("CMD_INFO_NOTIFY: order: " + kCommand_.GetOrder() + ", extra: " + kCommand_.GetExtra() + ", option: " + kCommand_.GetOption() + ", mission: " + kCommand_.GetMission());

			return true;
		}

		public static bool
		CMD_INFO_USER_LIST(CCommand kCommand_) {
			if(EXTRA.NEW == (EXTRA)kCommand_.GetExtra()) {
				// 유저목록 정보 도착.
				g_kChannelMgr.ClearUserList();

				SInfoUserListGsToCl tRData = (SInfoUserListGsToCl)kCommand_.GetData(typeof(SInfoUserListGsToCl));
				for(UINT i = 0; i < kCommand_.GetOption(); ++i) {
					SUserInfo tUserInfo = new SUserInfo(true);
					tUserInfo.aid = tRData.list[i].aid;
					tUserInfo.key = tRData.list[i].key;
					tUserInfo.SetName(ConvertToString(tRData.list[i].GetName()));

					g_kChannelMgr.AddUserList(tUserInfo);

					MESSAGE("CMD_INFO_USER_LIST: NEW: key: " + tUserInfo.key + ", aid: " + tUserInfo.aid + ", name: " + tUserInfo.GetName() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData.list[i])));
				}
				MESSAGE("CMD_INFO_USER_LIST: NEW: count: " + kCommand_.GetOption());
			} else if(EXTRA.CHANGE == (EXTRA)kCommand_.GetExtra()) {
				// 유저목록 끊어서 받음.
				SInfoUserListGsToCl tRData = (SInfoUserListGsToCl)kCommand_.GetData(typeof(SInfoUserListGsToCl));
				for(UINT i = 0; i < kCommand_.GetOption(); ++i) {
					SUserInfo tUserInfo = new SUserInfo(true);
					tUserInfo.aid = tRData.list[i].aid;
					tUserInfo.key = tRData.list[i].key;
					tUserInfo.SetName(ConvertToString(tRData.list[i].GetName()));

					g_kChannelMgr.AddUserList(tUserInfo);

					MESSAGE("CMD_INFO_USER_LIST: CHANGE: key: " + tUserInfo.key + ", aid: " + tUserInfo.aid + ", name: " + tUserInfo.GetName() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData.list[i])));
				}
				MESSAGE("CMD_INFO_USER_LIST: CHANGE: count: " + kCommand_.GetOption());
			} else if(EXTRA.DONE == (EXTRA)kCommand_.GetExtra()) {
				// 유저목록 전송 완료.

				REFRESH_USER_LIST();

				MESSAGE("CMD_INFO_USER_LIST: DONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.EMPTY == (EXTRA)kCommand_.GetExtra()) {
				// 유저목록 없음.
				g_kChannelMgr.ClearUserList();

				REFRESH_USER_LIST();

				MESSAGE("CMD_INFO_USER_LIST: EMPTY: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 서버 오류.
				MESSAGE("CMD_INFO_USER_LIST: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_INFO_OTHER(CCommand kCommand_) {
			MESSAGE("CMD_INFO_OTHER: order: " + kCommand_.GetOrder() + ", extra: " + kCommand_.GetExtra() + ", option: " + kCommand_.GetOption() + ", mission: " + kCommand_.GetMission());

			return true;
		}

		public static void
		InitializeInfoCommand() {
			g_bfNativeLauncher[(INT)(PROTOCOL.INFO_SERVER)] = new NativeLauncher(CMD_INFO_SERVER);
			g_bfNativeLauncher[(INT)(PROTOCOL.INFO_CHANNEL)] = new NativeLauncher(CMD_INFO_CHANNEL);
			g_bfNativeLauncher[(INT)(PROTOCOL.INFO_NOTIFY)] = new NativeLauncher(CMD_INFO_NOTIFY);
			g_bfNativeLauncher[(INT)(PROTOCOL.INFO_USER_LIST)] = new NativeLauncher(CMD_INFO_USER_LIST);
			g_bfNativeLauncher[(INT)(PROTOCOL.INFO_OTHER)] = new NativeLauncher(CMD_INFO_OTHER);
		}
	}
}

/* EOF */
