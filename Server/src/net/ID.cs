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
		CMD_ID_AUTHORIZE(CExtendCommand kCommand_) {
			CCommand kSelfCommand = kCommand_.GetCommand();
			CConnector kConnector = kCommand_.GetConnector();

			if(isptr(kConnector)) {
				CUnit kActor = g_kUnitMgr.GetUnit((UINT)UNIT_TYPE.UNIT_PLAYER, (UINT)kCommand_.GetKey());
				if(isptr(kActor)) {
					CPlayerEx kPlayer = (CPlayerEx)kActor.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
					if(isptr(kPlayer)) {
						if(0 == kPlayer.GetAid()) {
							CNetIO kNetIO = kPlayer.GetNetIO();
							if(isptr(kNetIO)) {
								UINT uiKey = GenerateBogoKey((UINT16)UNIT_TYPE.UNIT_PLAYER, (UINT16)kCommand_.GetKey());
								kPlayer.SetKey(uiKey);

								kNetIO.SetConnector(kConnector);

								if(EXTRA.NONE != (EXTRA)kSelfCommand.GetExtra()) {
									if(EXTRA.TIMEOUT != (EXTRA)kSelfCommand.GetExtra()) {
										kSelfCommand.SetExtra((UINT)EXTRA.DENY);
										kActor.Launcher(kSelfCommand);
										kActor.Disconnect();
										return true;
									}
								}

								UINT uiChannelIndex = 0;

								SIdAuthorizeClToGs tRData = (SIdAuthorizeClToGs)kSelfCommand.GetData(typeof(SIdAuthorizeClToGs));
								if((tRData.major_version == iSERVICE_MAJOR_VERSION) && (tRData.minor_version == iSERVICE_MINOR_VERSION)) {
									uiChannelIndex = tRData.channel_index;

									string szTempLoginId = ConvertToString(tRData.GetLoginId());
									string szTempName = szTempLoginId;

									CPlayer kTargetPlayer = g_kUnitMgr.FindPlayer((UINT)UNIT_TYPE.UNIT_PLAYER, ConvertToBytes(szTempName));
									if(isptr(kTargetPlayer)) {
										// 접속 중인 유저가 있을 때.
										TRACE("BUSY: " + ConvertToString(kPlayer.GetName()) + " : " + kPlayer.GetKey());

										// 같은 서버 일 때는 이렇게 해결 되지만 다른 서버 일 때는 즉시 종료가 안됨.
										// 이럴 땐 Client로 종료 대기 중인 서버 채널 정보를 알려 주고 그쪽으로 접속 할 수 있도록 해야 함.
										CCommand kCommand = new CCommand();
										kCommand.SetOrder((UINT)PROTOCOL.ID_AUTHORIZE);
										kCommand.SetExtra((UINT)EXTRA.BUSY);
										kCommand.SetOption(0);
										kTargetPlayer.Launcher(kCommand);
										kTargetPlayer.Disconnect();
									}

									kPlayer.SetLoginId(ConvertToBytes(szTempLoginId));
									kPlayer.SetName(ConvertToBytes(szTempName));
									kPlayer.SetAid((UINT)kCommand_.GetKey());	// DB 인증을 따로 하고있지 않아 인증되었다는 표시를 임의값으로 지정합니다.

									if(0 < tRData.local_ip) {
										kPlayer.GetNetIO().GetConnector().SetLocalAddress((ULONG)tRData.local_ip, (INT)tRData.local_port);
									}
									kPlayer.GetNetIO().GetConnector().SetPublicAddress(kPlayer.GetNetIO().GetConnector().GetRemoteSinAddress(), kPlayer.GetNetIO().GetConnector().GetRemotePort());

									g_kChannelMgr.IncreasedUser();

									TRACE("SERVER: IN: id: " + g_kChannelMgr.GetServerInfo().GetId() + ", max user: " + g_kChannelMgr.GetServerInfo().GetMaxUser() + ", user: " + g_kChannelMgr.GetServerInfo().GetUser());
									TRACE("SERVER: IN: public ip: " + ConvertToString(kPlayer.GetNetIO().GetConnector().GetPublicAddress()) + ", public port: " + kPlayer.GetNetIO().GetConnector().GetPublicPort());
									TRACE("SERVER: IN: local ip: " + ConvertToString(kPlayer.GetNetIO().GetConnector().GetLocalAddress()) + ", local port: " + kPlayer.GetNetIO().GetConnector().GetLocalPort());

									if(false == g_kChannelMgr.InUser(uiChannelIndex, kPlayer)) {
										CCommand kCommand = new CCommand();
										kCommand.SetOrder((UINT)PROTOCOL.ID_AUTHORIZE);
										kCommand.SetExtra((UINT)EXTRA.BUSY);
										kCommand.SetOption(1);
										kActor.Launcher(kSelfCommand);
										kActor.Disconnect();
										return true;
									}

									SIdAuthorizeGsToCl tSData = new SIdAuthorizeGsToCl(true);
									tSData.key = kPlayer.GetKey();
									tSData.aid = kPlayer.GetAid();
									tSData.SetName(kPlayer.GetName());
									tSData.tick = g_kTick.GetTick();

									tSData.public_ip = kPlayer.GetNetIO().GetConnector().GetPublicSinAddress();
									tSData.public_port = (UINT16)kPlayer.GetNetIO().GetConnector().GetPublicPort();

									if(EXTRA.TIMEOUT == (EXTRA)kSelfCommand.GetExtra()) {
										TRACE("TIMEOUT: aid: " + kPlayer.GetAid() + ", key: " + kPlayer.GetKey() + ", name: " + ConvertToString(tSData.GetName()));
										kSelfCommand.SetExtra((UINT)EXTRA.TIMEOUT);
									} else {
										TRACE("OK: aid: " + kPlayer.GetAid() + ", key: " + kPlayer.GetKey() + ", name: " + ConvertToString(tSData.GetName()));
										kSelfCommand.SetExtra((UINT)EXTRA.OK);
									}

									INT iSize = Marshal.SizeOf(tSData);
									kSelfCommand.SetData(tSData, iSize);

									kPlayer.Launcher(kSelfCommand, iSize);

									kSelfCommand.SetOrder((UINT)PROTOCOL.INFO_CHANNEL);
									kSelfCommand.SetExtra((UINT)EXTRA.OK);
									kPlayer.Launcher(kSelfCommand);

									return true;
								} else {
									TRACE("error: version is not valid: aid: " + kPlayer.GetAid() + ", key: " + kActor.GetKey() + ", client version: " + tRData.major_version + "." + tRData.minor_version + ", server version: " + iSERVICE_MAJOR_VERSION + "." + iSERVICE_MINOR_VERSION);
									kSelfCommand.SetExtra((UINT)EXTRA.DATA_ERROR);
								}
							} else {
								OUTPUT("[" + g_kTick.GetTime() + ":" + kActor.GetAid() + "] netio is null: ");
								kSelfCommand.SetExtra((UINT)EXTRA.FAIL);
							}
						} else {
							OUTPUT("[" + g_kTick.GetTime() + ":" + kActor.GetAid() + "] aid is not 0: ");
							kSelfCommand.SetExtra((UINT)EXTRA.FAIL);
						}
					} else {
						OUTPUT("[" + g_kTick.GetTime() + ":" + kActor.GetAid() + "] player is null: ");
						kSelfCommand.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					OUTPUT("[" + g_kTick.GetTime() + "] unit is null: ");
					kSelfCommand.SetExtra((UINT)EXTRA.FAIL);
				}

				kConnector.Send(kSelfCommand);
				kConnector.Disconnect();
			} else {
				OUTPUT("[" + g_kTick.GetTime() + "] connector is null: ");
			}
			return true;
		}

		public static bool
		CMD_ID_PING(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
//					INT iDelayTick = 0;
//		
//					if (isptr(kActor_.GetNetIO())) {
//						if (0 == kActor_.GetNetIO().GetDelayPingTick()) {
//							kActor_.GetNetIO().SetDelayPingTick(g_kTick.GetTick());
//						} else {
//							iDelayTick = (INT)(g_kTick.GetTick() - kActor_.GetNetIO().GetDelayPingTick());
//							kActor_.GetNetIO().SetDelayPingTick(g_kTick.GetTick());
//						}
//					}

					kCommand_.SetOrder((UINT)PROTOCOL.ID_PONG);
					kCommand_.SetExtra((UINT)EXTRA.OK);

					SIdPongGsToCl tSData = new SIdPongGsToCl(true);
					tSData.tick = g_kTick.GetTick();

//					TRACE("OK: aid: " + kPlayer.GetAid() + ", key: " + kPlayer.GetKey() + ", tick: " + tSData.tick + ", delay: " + iDelayTick);

					INT iSize = Marshal.SizeOf(tSData);
					kCommand_.SetData(tSData, iSize);

					kPlayer.Launcher(kCommand_, iSize);

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
		CMD_ID_QUIT(CUnit kActor_, CCommand kCommand_) {
			CPlayer kPlayer = (CPlayer)kActor_.GetTypeAs(UNIT_TYPE.UNIT_PLAYER);
			if(isptr(kPlayer)) {
				if(0 < kPlayer.GetAid()) {
					TRACE("OK: aid: " + kPlayer.GetAid() + ", key: " + kPlayer.GetKey());
				} else {
					OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] aid is 0: ");
				}
			} else {
				OUTPUT("[" + g_kTick.GetTime() + ":" + kActor_.GetAid() + "] player is null: ");
			}

			kActor_.Disconnect();
			return true;
		}

		public static void
		InitializeIdCommand()	{
			g_bfExtendLauncher[(INT)(PROTOCOL.ID_AUTHORIZE)] = new ExtendLauncher(CMD_ID_AUTHORIZE);

			g_bfNativeLauncher[(INT)(PROTOCOL.ID_PING)] = new NativeLauncher(CMD_ID_PING);
			g_bfNativeLauncher[(INT)(PROTOCOL.ID_QUIT)] = new NativeLauncher(CMD_ID_QUIT);
		}
	}
}

/* EOF */
