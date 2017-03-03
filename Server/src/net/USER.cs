/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
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
		CMD_USER_STATUS(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					CRoomHandler kRoomHandler = kPlayer.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						CRoom kRoom = kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							kPlayer.SetStatus((STATUS_TYPE)kCommand_.GetOption());

							kCommand_.SetExtra((UINT)EXTRA.OK);
							kPlayer.Launcher(kCommand_);

							TRACE("OK: key: " + kPlayer.GetKey() + ", aid: " + kPlayer.GetAid() + ", room id: " + kRoom.GetId() + ", status: " + kPlayer.GetStatus());

							kCommand_.SetOrder((UINT)PROTOCOL.OTHER_STATUS);
							SOtherStatusGsToCl tSData = new SOtherStatusGsToCl(true);
							tSData.actor = kPlayer.GetKey();

							INT iSize = Marshal.SizeOf(tSData);
							kCommand_.SetData(tSData, iSize);

							kRoom.Broadcast(kCommand_, iSize);
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
		CMD_USER_CHAT(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					SUserChatClToGs tRData = (SUserChatClToGs)kCommand_.GetData(typeof(SUserChatClToGs));
					tRData.content[kCommand_.GetOption()] = (CHAR)'\0';

					CCommand kCommand = new CCommand();
					kCommand.SetOrder((UINT)PROTOCOL.OTHER_CHAT);
					kCommand.SetExtra(kCommand_.GetExtra());
					kCommand.SetOption(kCommand_.GetOption());

					SOtherChatGsToCl tSData = new SOtherChatGsToCl(true);
					tSData.SetName(kPlayer.GetName());
					tSData.SetContent(tRData.content);

					//TRACE("bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(typeof(SOtherChatGsToCl)) - (iMAX_CHAT_LEN + 1)) + kCommand.GetOption()));

					FILELOG(LOG_EVENT_TYPE.LE_CHAT_CHANNEL, LOG_FIELD_TYPE.LF_AID, kPlayer.GetAid(), LOG_FIELD_TYPE.LF_LOGIN_ID, ConvertToString(kPlayer.GetLoginId()), LOG_FIELD_TYPE.LF_CHANNEL_ID, (kPlayer.GetChannelIndex() + 1), LOG_FIELD_TYPE.LF_STRING, ConvertToString(tSData.GetContent()));

					INT iSize = Marshal.SizeOf(typeof(SOtherChatGsToCl)) - (iMAX_CHAT_LEN + 1) + (INT)kCommand.GetOption();
					kCommand.SetData(tSData, iSize);

					TRACE("channel: " + (kPlayer.GetChannelIndex() + 1) + ", name: " + ConvertToString(tSData.GetName()) + ", message: " + ConvertToString(tSData.GetContent()) + ", size: " + iSize);

					g_kChannelMgr.BroadcastChannel((UINT)kPlayer.GetChannelIndex(), kCommand, iSize);
					return true;
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
        CMD_USER_MOVE(CUnit kActor_, CCommand kCommand_)
        {
            if(isptr(kActor_))
            {
                if(0 < kActor_.GetAid())
                {
                    SUserMoveClToGs tRData = (SUserMoveClToGs)kCommand_.GetData(typeof(SUserMoveClToGs));
                    CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
                    if(isptr(kRoomHandler))
                    {
                        CRoom kRoom = kRoomHandler.GetRoom();
                        if(isptr(kRoom))
                        {
                            INT iSize = Marshal.SizeOf(tRData);
                            kCommand_.SetData(tRData, iSize);
                            kRoom.Broadcast(kCommand_, iSize, kActor_);
                            return true;
                        }
                        else
                        {
                            OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Room is null: ");
                        }
                    }
                    else
                    {
                        OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] RoomHandler is null: ");
                    }
                }
                else
                {
                    OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Aid is 0: ");
                }
            }
            else
            {
                OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
            }

            kActor_.Launcher(kCommand_);
            kActor_.Disconnect();
            return true;
        }

        public static bool
        CME_USER_ROTATION(CUnit kActor_, CCommand kCommand_)
        {
            if (isptr(kActor_))
            {
                if (0 < kActor_.GetAid())
                {
                    SUserRotationClToGs tRData = (SUserRotationClToGs)kCommand_.GetData(typeof(SUserRotationClToGs));
                    CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
                    if (isptr(kRoomHandler))
                    {
                        CRoom kRoom = kRoomHandler.GetRoom();
                        if (isptr(kRoom))
                        {
                            INT iSize = Marshal.SizeOf(tRData);
                            kCommand_.SetData(tRData, iSize);
                            kRoom.Broadcast(kCommand_, iSize, kActor_);
                            return true;
                        }
                        else
                        {
                            OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Room is null: ");
                        }
                    }
                    else
                    {
                        OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] RoomHandler is null: ");
                    }
                }
                else
                {
                    OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Aid is 0: ");
                }
            }
            else
            {
                OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
            }

            kActor_.Launcher(kCommand_);
            kActor_.Disconnect();
            return true;
        }

        public static bool
        CME_USER_CREATE_OBJ(CUnit kActor_, CCommand kCommand_)
        {
            if (isptr(kActor_))
            {
                if (0 < kActor_.GetAid())
                {
                    SUserCreateObj tRData = (SUserCreateObj)kCommand_.GetData(typeof(SUserCreateObj));
                    CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
                    if (isptr(kRoomHandler))
                    {
                        CRoom kRoom = kRoomHandler.GetRoom();
                        if (isptr(kRoom))
                        {
                            INT iSize = Marshal.SizeOf(tRData);
                            kCommand_.SetData(tRData, iSize);
                            kRoom.Broadcast(kCommand_, iSize, kActor_);
                            return true;
                        }
                        else
                        {
                            OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Room is null: ");
                        }
                    }
                    else
                    {
                        OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] RoomHandler is null: ");
                    }
                }
                else
                {
                    OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Aid is 0: ");
                }
            }
            else
            {
                OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
            }

            kActor_.Launcher(kCommand_);
            kActor_.Disconnect();
            return true;
        }

        public static bool
        CME_USER_ANIMATOR(CUnit kActor_, CCommand kCommand_)
        {
            if (isptr(kActor_))
            {
                if (0 < kActor_.GetAid())
                {
                    sUserAnimator tRData = (sUserAnimator)kCommand_.GetData(typeof(sUserAnimator));
                    CRoomHandler kRoomHandler = kActor_.GetRoomHandler();
                    if (isptr(kRoomHandler))
                    {
                        CRoom kRoom = kRoomHandler.GetRoom();
                        if (isptr(kRoom))
                        {
                            INT iSize = Marshal.SizeOf(tRData);
                            kCommand_.SetData(tRData, iSize);
                            kRoom.Broadcast(kCommand_, iSize, kActor_);
                            return true;
                        }
                        else
                        {
                            OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Room is null: ");
                        }
                    }
                    else
                    {
                        OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] RoomHandler is null: ");
                    }
                }
                else
                {
                    OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] Aid is 0: ");
                }
            }
            else
            {
                OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
            }

            kActor_.Launcher(kCommand_);
            kActor_.Disconnect();
            return true;
        }


        public static void
		InitializeUserCommand() {
			g_bfNativeLauncher[(INT)(PROTOCOL.USER_STATUS)] = new NativeLauncher(CMD_USER_STATUS);
			g_bfNativeLauncher[(INT)(PROTOCOL.USER_CHAT)] = new NativeLauncher(CMD_USER_CHAT);
            g_bfNativeLauncher[(INT)(PROTOCOL.USER_MOVE)] = new NativeLauncher(CMD_USER_MOVE);
            g_bfNativeLauncher[(INT)(PROTOCOL.USER_CREATE_OBJ)] = new NativeLauncher(CME_USER_CREATE_OBJ);
            g_bfNativeLauncher[(INT)(PROTOCOL.USER_ROTATION)] = new NativeLauncher(CME_USER_ROTATION);
            g_bfNativeLauncher[(INT)(PROTOCOL.USER_ANIMATOR)] = new NativeLauncher(CME_USER_ANIMATOR);
        }
	}
}

/* EOF */
