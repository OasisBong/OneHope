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
		CMD_INFO_USER_LIST(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					g_kChannelMgr.GetUserList(kPlayer);
					TRACE("OK: USER LIST: aid: " + kPlayer.GetAid() + ", key: " + kActor_.GetKey());
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
		CMD_INFO_CHANNEL(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					if(EXTRA.CHANGE == (EXTRA)kCommand_.GetExtra()) {
						CRoomHandler kRoomHandler = kPlayer.GetRoomHandler();
						if(isptr(kRoomHandler)) {
							CRoom kRoom = kRoomHandler.GetRoom();
							if(isptr(kRoom)) {
								CCommand kCommnad = new CCommand();
								kCommnad.SetOrder((UINT)PROTOCOL.ROOM_LEAVE);
								kCommnad.SetExtra((UINT)EXTRA.NONE);

								ServerLauncher(kActor_, kCommnad);
							}
						}

#if DEBUG
						INT iPrevChannelIndex = kPlayer.GetChannelIndex();
						INT iNextChannelIndex = -1;
#endif

						if(g_kChannelMgr.InUser(kCommand_.GetOption(), kPlayer)) {
#if DEBUG
							iNextChannelIndex = kPlayer.GetChannelIndex();
#endif

							kCommand_.SetExtra((UINT)EXTRA.OK);
							kPlayer.Launcher(kCommand_);

							g_kChannelMgr.GetUserList(kPlayer);

#if DEBUG
							TRACE("OK: aid: " + kPlayer.GetAid() + ", key: " + kActor_.GetKey() + ", name: " + ConvertToString(kPlayer.GetName()) + ", channel id: " + (iPrevChannelIndex + 1) + " -> " + (iNextChannelIndex + 1));
#endif
							return true;
						}

						kCommand_.SetExtra((UINT)EXTRA.DENY);
						kActor_.Launcher(kCommand_);
						return true;
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] unknown extra: " + kCommand_.GetExtra());
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
		CMD_INFO_SERVER(CExtendCommand kCommand_) {
			CCommand kSelfCommand = kCommand_.GetCommand();
			CConnector kConnector = kCommand_.GetConnector();

			if(isptr(kConnector)) {
				kSelfCommand.SetExtra((UINT)EXTRA.OK);

				SInfoServerGsToCl tSData = new SInfoServerGsToCl(true);
				tSData.serial = kConnector.GetSerialKey();
				tSData.key = (UINT)kConnector.GetRegisterIndex();

				TRACE("serial: " + tSData.serial + ", key: " + tSData.key);

				INT iSize = Marshal.SizeOf(tSData);
				kCommand_.SetData(tSData, iSize);

				if(0 > kConnector.Send(kSelfCommand, iSize, PACKET_TYPE.PACKET_TRUST, CRYPT_TYPE.CRYPT_RC6)) {
					TRACE("FAILED: " + ConvertToString(kConnector.GetRemoteAddress()) + " : " + kConnector.GetRemotePort());
				} else {
					TRACE("OK: " + ConvertToString(kConnector.GetRemoteAddress()) + " : " + kConnector.GetRemotePort());
				}
			}
			return true;
		}

		public static void
		InitializeInfoCommand() {
			g_bfNativeLauncher[(INT)(PROTOCOL.INFO_USER_LIST)] = new NativeLauncher(CMD_INFO_USER_LIST);
			g_bfNativeLauncher[(INT)(PROTOCOL.INFO_CHANNEL)] = new NativeLauncher(CMD_INFO_CHANNEL);

			g_bfExtendLauncher[(INT)(PROTOCOL.INFO_SERVER)] = new ExtendLauncher(CMD_INFO_SERVER);
		}
	}
}

/* EOF */
