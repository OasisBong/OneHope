/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
		CMD_ROOM_CREATE(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						CRoom kRoom = kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							CCommand kCommand = new CCommand();
							kCommand.SetOrder((UINT)PROTOCOL.ROOM_LEAVE);
							kCommand.SetExtra((UINT)EXTRA.NONE);

							ServerLauncher(kActor_, kCommand);
						}

						kRoom = g_kChannelMgr.NewRoom(kPlayer.GetChannelIndex());
						if(isptr(kRoom)) {
							SRoomCreateClToGs tRData = (SRoomCreateClToGs)kCommand_.GetData(typeof(SRoomCreateClToGs));
							kRoom.SetName(tRData.name);
							kRoom.SetMaxUser(tRData.max);
							kRoom.SetStageId(tRData.stage_id);

							if(kRoom.Create(kPlayer)) {
								kCommand_.SetExtra((UINT)EXTRA.OK);
								SRoomCreateGsToCl tSData = new SRoomCreateGsToCl(true);
								tSData.id = kRoom.GetId();

								INT iSize = Marshal.SizeOf(tSData);
								kCommand_.SetData(tSData, iSize);

								kActor_.Launcher(kCommand_, iSize);

								FILELOG(LOG_EVENT_TYPE.LE_ROOM_CREATE, LOG_FIELD_TYPE.LF_AID, kActor_.GetAid(), LOG_FIELD_TYPE.LF_ROOM_ID, kRoom.GetId(), LOG_FIELD_TYPE.LF_STAGE_ID, kRoom.GetStageId(), LOG_FIELD_TYPE.LF_STRING, ConvertToString(kRoom.GetName()));

								TRACE("OK: key: " + kActor_.GetKey() + ", name: " + ConvertToString(kPlayer.GetName()) + ", room id: " + kRoom.GetId() + ", room stage id: " + kRoom.GetStageId() + ", room max user: " + kRoom.GetMaxUser() + ", room name: " + ConvertToString(kRoom.GetName()));
								return true;
							} else {
								OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room create failed: ");

								INT iLine = 0;
								StackTrace kTrace = new StackTrace(true);
								if(0 < kTrace.FrameCount) iLine = kTrace.GetFrame(1).GetFileLineNumber();
								HACKLOG(LOG_EVENT_TYPE.LE_BUG, LOG_FIELD_TYPE.LF_AID, kActor_.GetAid(), LOG_FIELD_TYPE.LF_DEBUG, iLine, LOG_FIELD_TYPE.LF_STRING, " ROOM_CREATE: room create failed");
							}

							kCommand_.SetExtra((UINT)EXTRA.DENY);
							g_kChannelMgr.DeleteRoom((UINT16)kRoom.GetId());
						} else {
							kCommand_.SetExtra((UINT)EXTRA.NOT_ENOUGH);
							kActor_.Launcher(kCommand_);

							INT iLine = 0;
							StackTrace kTrace = new StackTrace(true);
							if(0 < kTrace.FrameCount) iLine = kTrace.GetFrame(1).GetFileLineNumber();
							HACKLOG(LOG_EVENT_TYPE.LE_BUG, LOG_FIELD_TYPE.LF_AID, kActor_.GetAid(), LOG_FIELD_TYPE.LF_DEBUG, iLine, LOG_FIELD_TYPE.LF_STRING, " ROOM_CREATE: room is not enough:");
						}
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room handler is null: " + kActor_.GetKey());
						kCommand_.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] aid is 0: ");
					kCommand_.SetExtra((UINT)EXTRA.FAIL);
				}
			} else {
				OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
				kCommand_.SetExtra((UINT)EXTRA.FAIL);
			}

			kActor_.Launcher(kCommand_);
			kActor_.Disconnect();
			return true;
		}

		public static bool
		CMD_ROOM_JOIN(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					CRoomHandler kRoomHandler = kPlayer.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						CRoom kRoom = kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							CCommand kCommnad = new CCommand();
							kCommnad.SetOrder((UINT)PROTOCOL.ROOM_LEAVE);
							kCommnad.SetExtra((UINT)EXTRA.NONE);

							ServerLauncher(kActor_, kCommnad);
						}

						SRoomJoinClToGs tRData = (SRoomJoinClToGs)kCommand_.GetData(typeof(SRoomJoinClToGs));

						kRoom = g_kChannelMgr.FindRoom((UINT16)tRData.id);
						if(isptr(kRoom)) {
							if(kRoom.Join(kPlayer)) {
								TRACE("OK: key: " + kPlayer.GetKey() + ", room id: " + tRData.id);
							} else {
								if(1 == kRoom.GetTopCount()) {
									if(isptr(kRoom.GetLeader())) {
										if(kRoom != kRoom.GetLeader().GetRoomHandler().GetRoom()) {
											OUTPUT("critical error: unit is not in this room: ");
											g_kChannelMgr.DeleteRoom((UINT16)kRoom.GetId());
										}
									}
								}
								TRACE("error: join failed: " + tRData.id);
							}
							return true;
						} else {
							TRACE("error: join failed: EMPTY");
							kCommand_.SetExtra((UINT)EXTRA.EMPTY);
							kActor_.Launcher(kCommand_);
							return true;
						}
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room handler is null: " + kActor_.GetKey());

						INT iLine = 0;
						StackTrace kTrace = new StackTrace(true);
						if(0 < kTrace.FrameCount) iLine = kTrace.GetFrame(1).GetFileLineNumber();
						HACKLOG(LOG_EVENT_TYPE.LE_BUG, LOG_FIELD_TYPE.LF_AID, kActor_.GetAid(), LOG_FIELD_TYPE.LF_DEBUG, iLine, LOG_FIELD_TYPE.LF_STRING, " ROOM_JOIN: room handler is null:");
						kCommand_.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] aid is 0: ");

					INT iLine = 0;
					StackTrace kTrace = new StackTrace(true);
					if(0 < kTrace.FrameCount) iLine = kTrace.GetFrame(1).GetFileLineNumber();
					HACKLOG(LOG_EVENT_TYPE.LE_BUG, LOG_FIELD_TYPE.LF_AID, kActor_.GetAid(), LOG_FIELD_TYPE.LF_DEBUG, iLine, LOG_FIELD_TYPE.LF_STRING, " ROOM_JOIN: aid is 0:");
					kCommand_.SetExtra((UINT)EXTRA.FAIL);
				}
			} else {
				OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");

				INT iLine = 0;
				StackTrace kTrace = new StackTrace(true);
				if(0 < kTrace.FrameCount) iLine = kTrace.GetFrame(1).GetFileLineNumber();
				HACKLOG(LOG_EVENT_TYPE.LE_BUG, LOG_FIELD_TYPE.LF_AID, kActor_.GetAid(), LOG_FIELD_TYPE.LF_DEBUG, iLine, LOG_FIELD_TYPE.LF_STRING, " ROOM_JOIN: player is null:");
				kCommand_.SetExtra((UINT)EXTRA.FAIL);
			}

			kActor_.Launcher(kCommand_);
			kActor_.Disconnect();
			return true;
		}

		public static bool
		CMD_ROOM_LEAVE(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						CRoom kRoom = kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							if(kRoomHandler.InRoom()) {
								kRoom.Leave(kPlayer);
							}

							kCommand_.SetExtra((UINT)EXTRA.OK);
							kPlayer.Launcher(kCommand_);

							return true;
						} else {
							OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room is null: " + kActor_.GetKey());
							kCommand_.SetExtra((UINT)EXTRA.FAIL);
						}
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room handler is null: " + kActor_.GetKey());
						kCommand_.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] aid is 0: ");
					kCommand_.SetExtra((UINT)EXTRA.FAIL);
				}
			} else {
				OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
				kCommand_.SetExtra((UINT)EXTRA.FAIL);
			}

			kActor_.Launcher(kCommand_);
			kActor_.Disconnect();
			return true;
		}

		public static bool
		CMD_ROOM_START(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						CRoom kRoom = kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							if(kPlayer == kRoom.GetLeader()) {
								if(false == kRoom.IsDoing()) {
									bool bCheck = true;
									for(UINT i = 0; i < kRoom.GetTopCount(); ++i) {
										CPlayer kMemeber = kRoom.GetMemeber(i);
										if(isptr(kMemeber)) {
											if(STATUS_TYPE.STATUS_WAITING == kMemeber.GetStatus()) {
												bCheck = false;
												break;
											}
										}
									}

									if(bCheck) {
										kRoom.SetDoing(true);

										for(UINT i = 0; i < kRoom.GetTopCount(); ++i) {
											CPlayer kMember = kRoom.GetMemeber(i);
											if(isptr(kMember)) {
												kMember.SetStatus(STATUS_TYPE.STATUS_NORMAL);
											}
										}

										kCommand_.SetExtra((UINT)EXTRA.OK);
										kPlayer.Launcher(kCommand_);
										TRACE("OK: key: " + kPlayer.GetKey() + ", aid: " + kPlayer.GetAid() + ", room id: " + kRoom.GetId());

										kCommand_.SetOrder((UINT)PROTOCOL.ROOM_START_OTHER);

										SRoomStartOtherGsToCl tSData = new SRoomStartOtherGsToCl(true);
										tSData.actor = kPlayer.GetKey();

										INT iSize = Marshal.SizeOf(tSData);
										kCommand_.SetData(tSData, iSize);

										kRoom.Broadcast(kCommand_, iSize);

										return true;
									}

									kCommand_.SetExtra((UINT)EXTRA.CANT_DO);
									kActor_.Launcher(kCommand_);
								} else {
									kCommand_.SetExtra((UINT)EXTRA.NO_PERMISSION);
									kActor_.Launcher(kCommand_);
								}
								return true;
							} else {
								if(kRoom.IsDoing()) {
									kPlayer.SetStatus(STATUS_TYPE.STATUS_NORMAL);

									kCommand_.SetExtra((UINT)EXTRA.NEW);
									kPlayer.Launcher(kCommand_);
									TRACE("NEW: key: " + kPlayer.GetKey() + ", aid: " + kPlayer.GetAid() + ", room id: " + kRoom.GetId());

									kCommand_.SetOrder((UINT)PROTOCOL.ROOM_START_OTHER);

									SRoomStartOtherGsToCl tSData = new SRoomStartOtherGsToCl(true);
									tSData.actor = kPlayer.GetKey();

									INT iSize = Marshal.SizeOf(tSData);
									kCommand_.SetData(tSData, iSize);

									kRoom.Broadcast(kCommand_, iSize);

									return true;
								} else {
									kCommand_.SetExtra((UINT)EXTRA.NO_PERMISSION);
									kActor_.Launcher(kCommand_);
								}
								return true;
							}
						} else {
							OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room is null: " + kActor_.GetKey());
							kCommand_.SetExtra((UINT)EXTRA.FAIL);
						}
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room handler is null: " + kActor_.GetKey());
						kCommand_.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] aid is 0: ");
					kCommand_.SetExtra((UINT)EXTRA.FAIL);
				}
			} else {
				OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
				kCommand_.SetExtra((UINT)EXTRA.FAIL);
			}

			kActor_.Launcher(kCommand_);
			kActor_.Disconnect();
			return true;
		}

		public static bool
		CMD_ROOM_STOP(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						CRoom kRoom = kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							if(kPlayer == kRoom.GetLeader()) {
								if(kRoom.IsDoing()) {
									kRoom.SetDoing(false);

									for(UINT i = 0; i < kRoom.GetTopCount(); ++i) {
										CPlayer kMemeber = kRoom.GetMemeber(i);
										if(isptr(kMemeber)) {
											if(kRoom.GetLeader() == kMemeber) {
												kMemeber.SetStatus(STATUS_TYPE.STATUS_READY);
											} else {
												kMemeber.SetStatus(STATUS_TYPE.STATUS_WAITING);
											}
										}
									}

									kCommand_.SetExtra((UINT)EXTRA.OK);
									kPlayer.Launcher(kCommand_);

									TRACE("OK: key: " + kPlayer.GetKey() + ", aid: " + kPlayer.GetAid() + ", room id: " + kRoom.GetId());

									kCommand_.SetOrder((UINT)PROTOCOL.ROOM_STOP_OTHER);
									SRoomStopOtherGsToCl tSData = new SRoomStopOtherGsToCl(true);
									tSData.leader = kPlayer.GetKey();

									INT iSize = Marshal.SizeOf(tSData);
									kCommand_.SetData(tSData, iSize);

									kRoom.Broadcast(kCommand_, iSize);
								}
							} else {
								kCommand_.SetExtra((UINT)EXTRA.NO_PERMISSION);
								kActor_.Launcher(kCommand_);
							}
							return true;
						} else {
							OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room is null: " + kActor_.GetKey());
							kCommand_.SetExtra((UINT)EXTRA.FAIL);
						}
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room handler is null: " + kActor_.GetKey());
						kCommand_.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] aid is 0: ");
					kCommand_.SetExtra((UINT)EXTRA.FAIL);
				}
			} else {
				OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
				kCommand_.SetExtra((UINT)EXTRA.FAIL);
			}

			kActor_.Launcher(kCommand_);
			kActor_.Disconnect();
			return true;
		}

		public static bool
		CMD_ROOM_LIST(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						if(1 == kCommand_.GetMission()) {
							g_kChannelMgr.GetRoomMemberList(kPlayer);
							TRACE("OK: ROOM LIST: NONE: member: aid: " + kPlayer.GetAid() + ", key: " + kActor_.GetKey());
						} else {
							g_kChannelMgr.GetRoomList(kPlayer);
							TRACE("OK: ROOM LIST: NONE: aid: " + kPlayer.GetAid() + ", key: " + kActor_.GetKey());
						}
						return true;
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] room handler is null: " + kActor_.GetKey());
						kCommand_.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] aid is 0: ");
					kCommand_.SetExtra((UINT)EXTRA.FAIL);
				}
			} else {
				OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
				kCommand_.SetExtra((UINT)EXTRA.FAIL);
			}

			kActor_.Launcher(kCommand_);
			kActor_.Disconnect();
			return true;
		}

		public static void
		InitializeRoomCommand() {
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_CREATE)] = new NativeLauncher(CMD_ROOM_CREATE);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_JOIN)] = new NativeLauncher(CMD_ROOM_JOIN);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_LEAVE)] = new NativeLauncher(CMD_ROOM_LEAVE);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_START)] = new NativeLauncher(CMD_ROOM_START);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_STOP)] = new NativeLauncher(CMD_ROOM_STOP);
			g_bfNativeLauncher[(INT)(PROTOCOL.ROOM_LIST)] = new NativeLauncher(CMD_ROOM_LIST);
		}
	}
}

/* EOF */
