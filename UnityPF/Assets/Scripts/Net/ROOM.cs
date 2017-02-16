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
		CMD_ROOM_CREATE(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				SRoomCreateGsToCl tRData = (SRoomCreateGsToCl)kCommand_.GetData(typeof(SRoomCreateGsToCl));

				g_kChannelMgr.GetMainRoom().SetId(tRData.id);
				g_kChannelMgr.GetMainRoom().Create(g_kUnitMgr.GetMainPlayer());

				REFRESH_MEMBER_LIST();

				MESSAGE("CMD_ROOM_CREATE: OK: room id: " + tRData.id + ", channel index: " + g_kChannelMgr.GetMainRoom().GetChannelIndex() + ", room index: " + g_kChannelMgr.GetMainRoom().GetIndex() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));

				if(STATE_TYPE.STATE_ROOM > g_kStateMgr.GetTransitionType()) {
					g_kStateMgr.SetTransition(STATE_TYPE.STATE_ROOM);
				}
			} else if(EXTRA.DENY == (EXTRA)kCommand_.GetExtra()) {
				// 방생성 실패.
				MESSAGE("CMD_ROOM_CREATE: DENY: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.NOT_ENOUGH == (EXTRA)kCommand_.GetExtra()) {
				// 잘못된 정보 오류.
				MESSAGE("CMD_ROOM_CREATE: NOT_ENOUGH: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 서버 오류.
				MESSAGE("CMD_ROOM_CREATE: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_ROOM_JOIN(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				SRoomJoinGsToCl tRData = (SRoomJoinGsToCl)kCommand_.GetData(typeof(SRoomJoinGsToCl));

				g_kChannelMgr.GetMainRoom().SetId(tRData.id);
				g_kChannelMgr.GetMainRoom().SetName(tRData.GetName());
				g_kChannelMgr.GetMainRoom().SetStageId(tRData.stage_id);
				g_kChannelMgr.GetMainRoom().SetMaxUser(tRData.max);
				if(0 == tRData.offset) {
					g_kChannelMgr.GetMainRoom().SetDoing(false);
				} else {
					g_kChannelMgr.GetMainRoom().SetDoing(true);
				}
				g_kChannelMgr.GetMainRoom().Join(g_kUnitMgr.GetMainPlayer());

				REFRESH_MEMBER_LIST();

				MESSAGE("CMD_ROOM_JOIN: OK: room id: " + tRData.id + ", channel index: " + g_kChannelMgr.GetMainRoom().GetChannelIndex() + ", room index: " + g_kChannelMgr.GetMainRoom().GetIndex() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));

				if(STATE_TYPE.STATE_ROOM > g_kStateMgr.GetTransitionType()) {
					g_kStateMgr.SetTransition(STATE_TYPE.STATE_ROOM);
				}
			} else if(EXTRA.FULL == (EXTRA)kCommand_.GetExtra()) {
				// 더이상 못들어감.
				MESSAGE("CMD_ROOM_JOIN: FULL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.DENY == (EXTRA)kCommand_.GetExtra()) {
				// 이미 다른방에 있음.
				MESSAGE("CMD_ROOM_JOIN: DENY: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.EMPTY == (EXTRA)kCommand_.GetExtra()) {
				// 방이 없음.
				MESSAGE("CMD_ROOM_JOIN: EMPTY: ");
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 서버 오류.
				MESSAGE("CMD_ROOM_JOIN: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_ROOM_LEAVE(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				MESSAGE("CMD_ROOM_LEAVE: OK: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));

				g_kChannelMgr.GetMainRoom().Leave(g_kUnitMgr.GetMainPlayer());
				g_kUnitMgr.Clear();

				if(STATE_TYPE.STATE_LOBBY < g_kStateMgr.GetTransitionType()) {
					g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOBBY);
				}
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 서버 오류.
				MESSAGE("CMD_ROOM_LEAVE: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_ROOM_START(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				// 방장.
				g_kChannelMgr.GetMainRoom().SetDoing(true);

				for(UINT i = 0; i < (UINT)g_kChannelMgr.GetMainRoom().GetTopCount(); ++i) {
					GamePlayer kPlayer = g_kChannelMgr.GetMainRoom().GetMember(i);
					if(isptr(kPlayer)) {
						kPlayer.SetStatus(STATUS_TYPE.STATUS_NORMAL);
					}
				}

				REFRESH_MEMBER_LIST();

				MESSAGE("CMD_ROOM_START: OK: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));

                g_kStateMgr.SetTransition(STATE_TYPE.STATE_GAME);
                Application.LoadLevel((INT)SCENE_TYPE.SCENE_MAIN_GAME);
            } else if(EXTRA.NEW == (EXTRA)kCommand_.GetExtra()) {
				// 난입 플레이어 본인.
				g_kUnitMgr.GetMainPlayer().SetStatus(STATUS_TYPE.STATUS_NORMAL);

				REFRESH_MEMBER_LIST();

				MESSAGE("CMD_ROOM_START: NEW: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.CANT_DO == (EXTRA)kCommand_.GetExtra()) {
				// Ready 안누른 맴버가 있음.
				MESSAGE("CMD_ROOM_START: CANT_DO: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.NO_PERMISSION == (EXTRA)kCommand_.GetExtra()) {
				// 방장이 아니거나 방이 시작상태가 아닐때.
				MESSAGE("CMD_ROOM_START: NO_PERMISSION: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 서버 오류.
				MESSAGE("CMD_ROOM_START: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_ROOM_STOP(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				// 방장.
				g_kChannelMgr.GetMainRoom().SetDoing(false);

				for(UINT i = 0; i < (UINT)g_kChannelMgr.GetMainRoom().GetTopCount(); ++i) {
					GamePlayer kPlayer = g_kChannelMgr.GetMainRoom().GetMember(i);
					if(isptr(kPlayer)) {
						if(kPlayer == g_kChannelMgr.GetMainRoom().GetLeader()) {
							kPlayer.SetStatus(STATUS_TYPE.STATUS_READY);
						} else {
							kPlayer.SetStatus(STATUS_TYPE.STATUS_WAITING);
						}
					}
				}

				REFRESH_MEMBER_LIST();

				MESSAGE("CMD_ROOM_STOP: OK: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 서버 오류.
				MESSAGE("CMD_ROOM_STOP: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_ROOM_INFO(CCommand kCommand_) {
			if(EXTRA.IN == (EXTRA)kCommand_.GetExtra()) {
				SRoomInfo tRData = (SRoomInfo)kCommand_.GetData(typeof(SRoomInfo));

				SRoomInfo tRoomInfo = new SRoomInfo(true);
				tRoomInfo.id = tRData.id;
				tRoomInfo.stage_id = tRData.stage_id;
				tRoomInfo.max = tRData.max;
				tRoomInfo.offset = tRData.offset;

				tRoomInfo.SetName(tRData.GetName());

				MESSAGE("CMD_ROOM_INFO: IN: id: " + tRoomInfo.id + ", stage id: " + tRoomInfo.stage_id + ", offset: " + tRoomInfo.offset + ", max: " + tRoomInfo.max + ", name: " + ConvertToString(tRoomInfo.GetName()) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));

				g_kChannelMgr.AddRoomList(tRoomInfo);
				REFRESH_ROOM_LIST();
			} else if(EXTRA.OUT == (EXTRA)kCommand_.GetExtra()) {
				SRoomInfo tRData = (SRoomInfo)kCommand_.GetData(typeof(SRoomInfo));

				SRoomInfo tRoomInfo = g_kChannelMgr.FindRoomList(tRData.id);
				MESSAGE("CMD_ROOM_INFO: OUT: id: " + tRoomInfo.id + ", stage id: " + tRoomInfo.stage_id + ", offset: " + tRoomInfo.offset + ", max: " + tRoomInfo.max + ", name: " + ConvertToString(tRoomInfo.GetName()) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(typeof(UINT32))));

				g_kChannelMgr.RemoveRoomList(tRData.id);
				REFRESH_ROOM_LIST();
			}
			return true;
		}

		public static bool
		CMD_ROOM_LIST(CCommand kCommand_) {
			if(EXTRA.NEW == (EXTRA)kCommand_.GetExtra()) {
				if(0 == kCommand_.GetMission()) {
					// 방목록 정보 도착.
					g_kChannelMgr.ClearRoomList();

					SRoomListGsToCl tRData = (SRoomListGsToCl)kCommand_.GetData(typeof(SRoomListGsToCl));
					for(UINT i = 0; i < kCommand_.GetOption(); ++i) {
						SRoomInfo tRoomInfo = new SRoomInfo(true);
						tRoomInfo.id = tRData.list[i].id;
						tRoomInfo.stage_id = tRData.list[i].stage_id;
						tRoomInfo.max = tRData.list[i].max;
						tRoomInfo.offset = tRData.list[i].offset;
						tRoomInfo.SetName(tRData.list[i].GetName());

						g_kChannelMgr.AddRoomList(tRoomInfo);

						MESSAGE("CMD_ROOM_LIST: NEW: id: " + tRoomInfo.id + ", stage id: " + tRoomInfo.stage_id + ", offset: " + tRoomInfo.offset + ", max: " + tRoomInfo.max + ", name: " + ConvertToString(tRoomInfo.GetName()) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData.list[i])));
					}
					MESSAGE("CMD_ROOM_LIST: NEW: count: " + kCommand_.GetOption());
				} else if(1 == kCommand_.GetMission()) {
					// 맴버 목록 정보 도착.
					SRoomMemberListGsToCl tRData = (SRoomMemberListGsToCl)kCommand_.GetData(typeof(SRoomMemberListGsToCl));
					for(UINT i = 0; i < kCommand_.GetOption(); ++i) {
						SRoomMember tRoomMember = tRData.list[i];

						GamePlayer kPlayer = g_kUnitMgr.AddPlayer(tRoomMember.actor);
						if(isptr(kPlayer)) {
							kPlayer.SetName(tRoomMember.GetName());
							kPlayer.SetStatus((STATUS_TYPE)tRoomMember.status);

							kPlayer.GetNetIO().GetConnector().SetLocalAddress(tRoomMember.local_ip, tRoomMember.local_port);
							kPlayer.GetNetIO().GetConnector().SetPublicAddress(tRoomMember.public_ip, tRoomMember.public_port);

							g_kChannelMgr.GetMainRoom().Join(kPlayer);

							MESSAGE("CMD_ROOM_LIST: NEW: key: " + kPlayer.GetKey() + ", aid: " + kPlayer.GetAid() + ", name: " + ConvertToString(kPlayer.GetName()) + ", public ip: " + ConvertToString(kPlayer.GetNetIO().GetConnector().GetPublicAddress()) + ", public port: " + kPlayer.GetNetIO().GetConnector().GetPublicPort() + ", local ip: " + ConvertToString(kPlayer.GetNetIO().GetConnector().GetLocalAddress()) + ", local port: " + kPlayer.GetNetIO().GetConnector().GetLocalPort() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData.list[i])));
						}
					}
					MESSAGE("CMD_ROOM_LIST: NEW: count: " + kCommand_.GetOption());
				}
			} else if(EXTRA.CHANGE == (EXTRA)kCommand_.GetExtra()) {
				if(0 == kCommand_.GetMission()) {
					// 방목록 끊어서 받음.
					SRoomListGsToCl tRData = (SRoomListGsToCl)kCommand_.GetData(typeof(SRoomListGsToCl));
					for(UINT i = 0; i < kCommand_.GetOption(); ++i) {
						SRoomInfo tRoomInfo = new SRoomInfo(true);
						tRoomInfo.id = tRData.list[i].id;
						tRoomInfo.stage_id = tRData.list[i].stage_id;
						tRoomInfo.max = tRData.list[i].max;
						tRoomInfo.offset = tRData.list[i].offset;
						tRoomInfo.SetName(tRData.list[i].GetName());

						g_kChannelMgr.AddRoomList(tRoomInfo);

						MESSAGE("CMD_ROOM_LIST: CHANGE: id: " + tRoomInfo.id + ", stage id: " + tRoomInfo.stage_id + ", offset: " + tRoomInfo.offset + ", max: " + tRoomInfo.max + ", name: " + ConvertToString(tRoomInfo.GetName()) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData.list[i])));
					}
					MESSAGE("CMD_ROOM_LIST: CHANGE: count: " + kCommand_.GetOption());
				} else if(1 == kCommand_.GetMission()) {
					// 맴버 목록 끊어서 받음.
					SRoomMemberListGsToCl tRData = (SRoomMemberListGsToCl)kCommand_.GetData(typeof(SRoomMemberListGsToCl));
					for(UINT i = 0; i < kCommand_.GetOption(); ++i) {
						SRoomMember tRoomMember = tRData.list[i];

						GamePlayer kPlayer = g_kUnitMgr.AddPlayer(tRoomMember.actor);
						if(isptr(kPlayer)) {
							kPlayer.SetName(tRoomMember.GetName());
							kPlayer.SetStatus((STATUS_TYPE)tRoomMember.status);

							kPlayer.GetNetIO().GetConnector().SetLocalAddress(tRoomMember.local_ip, tRoomMember.local_port);
							kPlayer.GetNetIO().GetConnector().SetPublicAddress(tRoomMember.public_ip, tRoomMember.public_port);

							g_kChannelMgr.GetMainRoom().Join(kPlayer);

							MESSAGE("CMD_ROOM_LIST: CHANGE: key: " + kPlayer.GetKey() + ", aid: " + kPlayer.GetAid() + ", name: " + ConvertToString(kPlayer.GetName()) + ", public ip: " + ConvertToString(kPlayer.GetNetIO().GetConnector().GetPublicAddress()) + ", public port: " + kPlayer.GetNetIO().GetConnector().GetPublicPort() + ", local ip: " + ConvertToString(kPlayer.GetNetIO().GetConnector().GetLocalAddress()) + ", local port: " + kPlayer.GetNetIO().GetConnector().GetLocalPort() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData.list[i])));
						}
					}
					MESSAGE("CMD_ROOM_LIST: CHANGE: count: " + kCommand_.GetOption());
				}
			} else if(EXTRA.DONE == (EXTRA)kCommand_.GetExtra()) {
				if(0 == kCommand_.GetMission()) {
					// 방목록 전송 완료.

					REFRESH_ROOM_LIST();

					MESSAGE("CMD_ROOM_LIST: DONE: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
				} else if(1 == kCommand_.GetMission()) {
					// 맴버 목록 전송 완료.
					SRoomMemberLeaderGsToCl tRData = (SRoomMemberLeaderGsToCl)kCommand_.GetData(typeof(SRoomMemberLeaderGsToCl));

					GamePlayer kLeader = g_kChannelMgr.GetMainRoom().GetLeader();
					if(isptr(kLeader)) {
						if(kLeader.GetKey() == tRData.leader) {
							MESSAGE("CMD_ROOM_LIST: DONE: leader key : " + kLeader.GetKey() + ", leader aid: " + kLeader.GetAid() + ", leader name: " + ConvertToString(kLeader.GetName()) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
						} else {
							MESSAGE("CMD_ROOM_LIST: DONE: leader was not found: leader: [" + tRData.leader + ":" + kLeader.GetKey() + "] bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
						}
					} else {
						MESSAGE("CMD_ROOM_LIST: DONE: leader is null: leader: [" + tRData.leader + ":0] bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
					}
					REFRESH_MEMBER_LIST();
				}
			} else if(EXTRA.EMPTY == (EXTRA)kCommand_.GetExtra()) {
				if(0 == kCommand_.GetMission()) {
					// 방목록 없음.
					g_kChannelMgr.ClearRoomList();

					REFRESH_ROOM_LIST();

					MESSAGE("CMD_ROOM_LIST: EMPTY: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
				} else if(1 == kCommand_.GetMission()) {
					// 맴버 아무도 없음.
					// 이건 오면 안됨.
					//REFRESH_MEMBER_LIST();

					MESSAGE("CMD_ROOM_LIST: EMPTY: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
				}
			} else if(EXTRA.FAIL == (EXTRA)kCommand_.GetExtra()) {
				// 서버 오류.
				MESSAGE("CMD_ROOM_LIST: FAIL: bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE));
			}
			return true;
		}

		public static bool
		CMD_ROOM_JOIN_OTHER(CCommand kCommand_) {
			if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
				if(g_kChannelMgr.GetMainRoom().IsAvailable()) {
					SRoomJoinOtherGsToCl tRData = (SRoomJoinOtherGsToCl)kCommand_.GetData(typeof(SRoomJoinOtherGsToCl));

					GamePlayer kPlayer = g_kUnitMgr.AddPlayer(tRData.actor);
					if(isptr(kPlayer)) {
						CNetIO kNetIO = kPlayer.GetNetIO();
						if(isptr(kNetIO)) {
							kPlayer.SetName(tRData.GetName());
							kPlayer.SetStatus(STATUS_TYPE.STATUS_WAITING);

							kNetIO.GetConnector().SetLocalAddress(tRData.local_ip, tRData.local_port);
							kNetIO.GetConnector().SetPublicAddress(tRData.public_ip, tRData.public_port);

							g_kChannelMgr.GetMainRoom().Join(kPlayer);

							MESSAGE("CMD_ROOM_JOIN_OTHER: OK: key: " + kPlayer.GetKey() + ", aid: " + kPlayer.GetAid() + ", name: " + ConvertToString(kPlayer.GetName()) + ", public ip: " + ConvertToString(kNetIO.GetConnector().GetPublicAddress()) + ", public port: " + kNetIO.GetConnector().GetPublicPort() + ", local ip: " + ConvertToString(kNetIO.GetConnector().GetLocalAddress()) + ", local port: " + kNetIO.GetConnector().GetLocalPort() + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));
						}
					}
				}
				REFRESH_MEMBER_LIST();
			}
			return true;
		}

		public static bool
		CMD_ROOM_LEAVE_OTHER(CCommand kCommand_) {
			if(g_kChannelMgr.GetMainRoom().IsAvailable()) {
				if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
					SRoomLeaveOtherGsToCl tRData = (SRoomLeaveOtherGsToCl)kCommand_.GetData(typeof(SRoomLeaveOtherGsToCl));

					GamePlayer kPlayer = g_kUnitMgr.GetPlayer(tRData.actor);
					if(isptr(kPlayer)) {
						g_kChannelMgr.GetMainRoom().Leave(kPlayer);
						g_kUnitMgr.RemovePlayer(kPlayer.GetKey());

						MESSAGE("CMD_ROOM_LEAVE_OTHER: OK: actor: " + tRData.actor + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tRData) - Marshal.SizeOf(typeof(UINT32)))));
					} else {
						MESSAGE("CMD_ROOM_LEAVE_OTHER: OK: player is null: actor: " + tRData.actor + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tRData) - Marshal.SizeOf(typeof(UINT32)))));
					}
					REFRESH_MEMBER_LIST();
				} else if(EXTRA.CHECK == (EXTRA)kCommand_.GetExtra()) {
					// 리더 변경정보 포함.
					SRoomLeaveOtherGsToCl tRData = (SRoomLeaveOtherGsToCl)kCommand_.GetData(typeof(SRoomLeaveOtherGsToCl));

					GamePlayer kPlayer = g_kUnitMgr.GetPlayer(tRData.actor);
					if(isptr(kPlayer)) {
						g_kChannelMgr.GetMainRoom().Leave(kPlayer);
						g_kUnitMgr.RemovePlayer(kPlayer.GetKey());

						GamePlayer kLeader = g_kChannelMgr.GetMainRoom().GetLeader();
						if(isptr(kLeader)) {
							if(kLeader.GetKey() == tRData.leader) {
								kLeader.SetStatus(STATUS_TYPE.STATUS_READY);
								MESSAGE("CMD_ROOM_LEAVE_OTHER: CHECK: actor: " + tRData.actor + ", leader: " + tRData.leader + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));
							} else {
								MESSAGE("CMD_ROOM_LEAVE_OTHER: CHECK: leader was not found: actor: " + tRData.actor + ", leader: [" + tRData.leader + ":" + kLeader.GetKey() + "], bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));
							}
						} else {
							MESSAGE("CMD_ROOM_LEAVE_OTHER: CHECK: leader is null: actor: " + tRData.actor + ", leader: [" + tRData.leader + ":0], bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));
						}
					} else {
						MESSAGE("CMD_ROOM_LEAVE_OTHER: CHECK: player is null: actor: " + tRData.actor + ", leader: " + tRData.leader + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + Marshal.SizeOf(tRData)));
					}
					REFRESH_MEMBER_LIST();
				}
			}
			return true;
		}

		public static bool
		CMD_ROOM_START_OTHER(CCommand kCommand_) {
			if(g_kChannelMgr.GetMainRoom().IsAvailable()) {
				if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
					SRoomStartOtherGsToCl tRData = (SRoomStartOtherGsToCl)kCommand_.GetData(typeof(SRoomStartOtherGsToCl));
					if(g_kChannelMgr.GetMainRoom().GetLeader() == g_kUnitMgr.GetPlayer(tRData.actor)) {
						// 처음 시작은 방장만 가능.
						g_kChannelMgr.GetMainRoom().SetDoing(true);

						for(UINT i = 0; i < (UINT)g_kChannelMgr.GetMainRoom().GetTopCount(); ++i) {
							GamePlayer kPlayer = g_kChannelMgr.GetMainRoom().GetMember(i);
							if(isptr(kPlayer)) {
								kPlayer.SetStatus(STATUS_TYPE.STATUS_NORMAL);
							}
						}

                        g_kStateMgr.SetTransition(STATE_TYPE.STATE_GAME);
                        Application.LoadLevel((INT)SCENE_TYPE.SCENE_MAIN_GAME);

                        REFRESH_MEMBER_LIST();

						MESSAGE("CMD_ROOM_START_OTHER: OK: leader: " + tRData.actor + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tRData))));
					}
				} else if(EXTRA.NEW == (EXTRA)kCommand_.GetExtra()) {
					// 난입 플레이어 처리.
					SRoomStartOtherGsToCl tRData = (SRoomStartOtherGsToCl)kCommand_.GetData(typeof(SRoomStartOtherGsToCl));

					GamePlayer kPlayer = g_kUnitMgr.GetPlayer(tRData.actor);
					if(isptr(kPlayer)) {
						kPlayer.SetStatus(STATUS_TYPE.STATUS_NORMAL);
					}

					REFRESH_MEMBER_LIST();

					MESSAGE("CMD_ROOM_START_OTHER: NEW: actor: " + tRData.actor + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tRData))));
				}
			}
			return true;
		}

		public static bool
		CMD_ROOM_STOP_OTHER(CCommand kCommand_) {
			if(g_kChannelMgr.GetMainRoom().IsAvailable()) {
				if(EXTRA.OK == (EXTRA)kCommand_.GetExtra()) {
					SRoomStopOtherGsToCl tRData = (SRoomStopOtherGsToCl)kCommand_.GetData(typeof(SRoomStopOtherGsToCl));
					if(g_kChannelMgr.GetMainRoom().GetLeader() == g_kUnitMgr.GetPlayer(tRData.leader)) {
						// 방장만 중단 가능.
						g_kChannelMgr.GetMainRoom().SetDoing(false);

						for(UINT i = 0; i < (UINT)g_kChannelMgr.GetMainRoom().GetTopCount(); ++i) {
							GamePlayer kPlayer = g_kChannelMgr.GetMainRoom().GetMember(i);
							if(isptr(kPlayer)) {
								if(kPlayer == g_kChannelMgr.GetMainRoom().GetLeader()) {
									kPlayer.SetStatus(STATUS_TYPE.STATUS_READY);
								} else {
									kPlayer.SetStatus(STATUS_TYPE.STATUS_WAITING);
								}
							}
						}

						REFRESH_MEMBER_LIST();

						MESSAGE("CMD_ROOM_STOP_OTHER: OK: leader: " + tRData.leader + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tRData))));
					}
				}
			}
			return true;
		}

		public static void
		InitializeRoomCommand() {
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_CREATE)] = new NativeLauncher(CMD_ROOM_CREATE);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_JOIN)] = new NativeLauncher(CMD_ROOM_JOIN);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_LEAVE)] = new NativeLauncher(CMD_ROOM_LEAVE);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_START)] = new NativeLauncher(CMD_ROOM_START);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_STOP)] = new NativeLauncher(CMD_ROOM_STOP);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_INFO)] = new NativeLauncher(CMD_ROOM_INFO);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_LIST)] = new NativeLauncher(CMD_ROOM_LIST);

			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_JOIN_OTHER)] = new NativeLauncher(CMD_ROOM_JOIN_OTHER);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_LEAVE_OTHER)] = new NativeLauncher(CMD_ROOM_LEAVE_OTHER);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_START_OTHER)] = new NativeLauncher(CMD_ROOM_START_OTHER);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_STOP_OTHER)] = new NativeLauncher(CMD_ROOM_STOP_OTHER);
		}
	}
}

/* EOF */
