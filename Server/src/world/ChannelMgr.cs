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
		public static CChannelMgr	g_kChannelMgr { get { return CChannelMgr.GetInstance(); } }

		public class CChannelMgr : CSingleton< CChannelMgr > {
			public CChannelMgr() {
				for(UINT i = 0; i < (UINT)(iMAX_CHANNEL_LIST); ++i) {
					for(UINT j = 0; j < (UINT)(iMAX_CHANNEL_USERS); ++j) {
						m_kUserList[i, j] = null;
						m_iEmptyUserList[i, j] = -1;
					}

					m_iUserMaxCount[i] = 0;

					m_iUserTopIndex[i] = 0;
					m_iEmptyUserCount[i] = 0;

					m_iUsedRoomCount[i] = 0;

					m_iRoomTopIndex[i] = 0;
					m_iEmptyRoomCount[i] = 0;
				}
			}

			~CChannelMgr() {
				for(UINT i = 0; i < (UINT)(iMAX_CHANNEL_LIST); ++i) {
					for(UINT j = 0; j < (UINT)(iMAX_ROOMS); ++j) {
						SAFE_DELETE_RELEASE(ref m_kRoomList[(INT)i, (INT)j]);
					}
				}
			}

			public bool
			Initialize() {
				for(UINT i = 0; i < (UINT)(iMAX_CHANNEL_LIST); ++i) {
					for(UINT j = 0; j < (UINT)(iMAX_ROOMS); ++j) {
						m_iEmptyRoomList[i, j] = -1;
					}
				}

				for(UINT i = 0; i < (UINT)(iMAX_CHANNEL_LIST); ++i) {
					for(UINT j = 0; j < (UINT)(iMAX_ROOMS); ++j) {
						m_iEmptyRoomList[i, j] = -1;
						m_kRoomList[i, j] = (CRoom)g_kRoomProvider.Create();
						if(isptr(m_kRoomList[i, j])) {
							if(m_kRoomList[i, j].Initialize()) {
								m_kRoomList[i, j].SetId((UINT)GenerateRoomKey((UINT16)i, (UINT16)j));
							} else {
								OUTPUT("[" + i + "][" + j + "] critical error: room created: failed: system memory is not enough");
								for(UINT k = 0; k <= i; ++k) {
									for(UINT l = 0; l < j; ++l) {
										SAFE_DELETE_RELEASE(ref m_kRoomList[k, l]);
									}
								}
								return false;
							}
						} else {
							OUTPUT("[" + i + "][" + j + "] critical error: room created failed: system memory is not enough");
							for(UINT k = 0; k <= i; ++k) {
								for(UINT l = 0; l < j; ++l) {
									SAFE_DELETE_RELEASE(ref m_kRoomList[k, l]);
								}
							}
							return false;
						}
					}
				}

				return true;
			}

			public bool
			Release() {
				for(UINT i = 0; i < (UINT)(iMAX_CHANNEL_LIST); ++i) {
					for(UINT j = 0; j < (UINT)(iMAX_ROOMS); ++j) {
						m_iEmptyRoomList[i, j] = -1;
						SAFE_DELETE_RELEASE(ref m_kRoomList[i, j]);
					}
				}
				return true;
			}

			public CRoom
			NewRoom(INT o) {
				if((0 <= o) && (o < (INT)GetMaxChannel())) {
					if(0 < m_iEmptyRoomCount[o]) {
						--m_iEmptyRoomCount[o];
						INT iIndex = m_iEmptyRoomList[o, m_iEmptyRoomCount[o]];
						m_iEmptyRoomList[o, m_iEmptyRoomCount[o]] = -1;

						if((0 <= iIndex) && (m_iRoomTopIndex[o] > iIndex)) {
							++m_iUsedRoomCount[o];
							TRACE("new room[" + iIndex + "]: OK: count: " + m_iUsedRoomCount[o] + ", id: " + m_kRoomList[o, iIndex].GetId() + " : " + m_kRoomList[o, iIndex].GetTopCount());
							return m_kRoomList[o, iIndex];
						}
					}

					if((0 <= m_iRoomTopIndex[o]) && (iMAX_ROOMS > m_iRoomTopIndex[o])) {
						INT iIndex = m_iRoomTopIndex[o];

						++m_iRoomTopIndex[o];
						++m_iUsedRoomCount[o];

						return m_kRoomList[o, iIndex];
					} else {
						OUTPUT("critical error: room is full: U: " + m_iUsedRoomCount[o] + " - T: " + m_iRoomTopIndex[o] + " - E: " + m_iEmptyRoomCount[o]);
					}
				} else {
					OUTPUT("critical error: channel info of the user is wrong: (" + o + ")");
				}
				return null;
			}

			public bool
			DeleteRoom(UINT16 o) {
				SRoomKey tKey = new SRoomKey(o);

				if(tKey.channel < GetMaxChannel()) {
					if(tKey.index < m_iRoomTopIndex[tKey.channel]) {
						if(tKey.index == (UINT16)(m_iRoomTopIndex[tKey.channel] - 1)) {
							--m_iRoomTopIndex[tKey.channel];
						} else {
							m_iEmptyRoomList[tKey.channel, m_iEmptyRoomCount[tKey.channel]] = tKey.index;
							++m_iEmptyRoomCount[tKey.channel];
						}

						--m_iUsedRoomCount[tKey.channel];
						TRACE("room is deleted: OK: ");

						m_kRoomList[tKey.channel, tKey.index].Clear();
						return true;
					} else {
						OUTPUT("room delete: failed: room index is wrong: (" + tKey.index + ") - U: " + m_iUsedRoomCount[tKey.channel] + " - T: " + m_iRoomTopIndex[tKey.channel] + " - E: " + m_iEmptyRoomCount[tKey.channel]);
					}
				} else {
					OUTPUT("room delete: failed: room index is wrong: (" + tKey.channel + ":" + tKey.index + ")");
				}
				return false;
			}

			public CRoom
			FindRoom(UINT16 usId_) {
				if(0 < usId_) {
					SRoomKey tKey = new SRoomKey(usId_);

					if(tKey.channel < GetMaxChannel()) {
						if(tKey.index < iMAX_ROOMS) {
							return m_kRoomList[tKey.channel, tKey.index];
						} else {
							TRACE("error: invalid condition: id: " + usId_ + ", channel: " + tKey.channel + ", index: " + tKey.index);
						}
					} else {
						TRACE("critical error: range over");
					}
				}
				return null;
			}

			public bool
			InUser(UINT o, CPlayer kPlayer_) {
				if((INT)o == kPlayer_.GetChannelIndex()) {
					return true;
				} else {
					if(o < GetMaxChannel()) {
						if(GetChannelInfo(o).GetUser() < GetChannelInfo(o).GetMaxUser()) {
							if(0 <= kPlayer_.GetChannelIndex()) {
								OutUser((UINT)kPlayer_.GetChannelIndex(), kPlayer_);
							}

							if(0 < m_iEmptyUserCount[o]) {
								if(0 <= m_iEmptyUserList[o, m_iEmptyUserCount[o] - 1]) {
									m_kUserList[o, m_iEmptyUserList[o, m_iEmptyUserCount[o] - 1]] = kPlayer_;
									kPlayer_.SetUserIndex(m_iEmptyUserList[o, m_iEmptyUserCount[o] - 1]);
									m_iEmptyUserList[o, m_iEmptyUserCount[o] - 1] = -1;
									--m_iEmptyUserCount[o];
								} else {
									OUTPUT("critical error: index is wrong: ");
									return false;
								}
							} else {
								m_kUserList[o, m_iUserTopIndex[o]] = kPlayer_;
								kPlayer_.SetUserIndex(m_iUserTopIndex[o]);
								++m_iUserTopIndex[o];
							}
							kPlayer_.SetChannelIndex((INT)o);
							m_tServerInfo.IncreasedUser(o);

							CCommand kCommand = new CCommand();
							kCommand.SetOrder((UINT)PROTOCOL.INFO_CHANNEL);
							kCommand.SetExtra((UINT)EXTRA.IN);
							kCommand.SetOption(o);

							SInfoChannelInGsToCl tData = new SInfoChannelInGsToCl(true);
							tData.aid = kPlayer_.GetAid();
							tData.key = kPlayer_.GetKey();
							tData.SetName(kPlayer_.GetName());

							INT iSize = Marshal.SizeOf(tData);
							kCommand.SetData(tData, iSize);

							BroadcastChannel(o, kCommand, iSize, kPlayer_);

							TRACE("CHANNEL: IN: aid: " + kPlayer_.GetAid() + ", key: " + kPlayer_.GetKey() + ", channel index: " + kPlayer_.GetChannelIndex() + ", user index: " + kPlayer_.GetUserIndex() + ", max user: " + GetChannelInfo(o).GetMaxUser() + ", user: " + GetChannelInfo(o).GetUser());

							FILELOG(LOG_EVENT_TYPE.LE_CLIENT_IN, LOG_FIELD_TYPE.LF_AID, kPlayer_.GetAid(), LOG_FIELD_TYPE.LF_LOGIN_ID, ConvertToString(kPlayer_.GetLoginId()));

							return true;
						} else {
							TRACE("CHANNEL: FULL: aid: " + kPlayer_.GetAid() + ", key: " + kPlayer_.GetKey() + ", max user: " + GetChannelInfo(o).GetMaxUser() + ", user: " + GetChannelInfo(o).GetUser());
						}
					} else {
						OUTPUT("critical error: wrong channel: [" + o + " : " + GetMaxChannel() + "]");
					}
				}
				return false;
			}

			public void
			OutUser(UINT o, CPlayer kPlayer_) {
				if(o < GetMaxChannel()) {
					if(0 < GetChannelInfo(o).GetUser()) {
						INT iIndex = kPlayer_.GetUserIndex();
						if((0 <= iIndex) && (iIndex < (INT)GetChannelInfo(o).GetMaxUser())) {
							m_kUserList[o, iIndex] = null;
							m_tServerInfo.DecreasedUser(o);

							kPlayer_.SetChannelIndex(-1);
							kPlayer_.SetUserIndex(-1);

							if(iIndex == m_iUserTopIndex[o] - 1) {
								--m_iUserTopIndex[o];
							} else {
								m_iEmptyUserList[o, m_iEmptyUserCount[o]] = iIndex;
								++m_iEmptyUserCount[o];
							}

							CCommand kCommand = new CCommand();
							kCommand.SetOrder((UINT)PROTOCOL.INFO_CHANNEL);
							kCommand.SetExtra((UINT)EXTRA.OUT);
							kCommand.SetOption(o);

							SInfoChannelOutGsToCl tData = new SInfoChannelOutGsToCl(true);
							tData.key = kPlayer_.GetKey();

							INT iSize = Marshal.SizeOf(tData);
							kCommand.SetData(tData, iSize);

							BroadcastChannel(o, kCommand, iSize, kPlayer_);

							TRACE("CHANNEL: OUT: aid: " + kPlayer_.GetAid() + ", key: " + kPlayer_.GetKey() + ", channel index: " + o + ", user index: " + iIndex + ", max user: " + GetChannelInfo(o).GetMaxUser() + ", user: " + GetChannelInfo(o).GetUser());

							FILELOG(LOG_EVENT_TYPE.LE_CLIENT_OUT, LOG_FIELD_TYPE.LF_AID, kPlayer_.GetAid(), LOG_FIELD_TYPE.LF_LOGIN_ID, ConvertToString(kPlayer_.GetLoginId()));
						} else {
							OUTPUT("critical error: index is wrong: " + kPlayer_.GetKey() + " : " + kPlayer_.GetChannelIndex());
						}
					} else {
						OUTPUT("critical error: user count error: " + kPlayer_.GetKey() + " : " + kPlayer_.GetChannelIndex());
					}
				}
			}

			public void
			GetRoomList(CPlayer kPlayer_) {
				if((0 <= kPlayer_.GetChannelIndex()) && ((UINT)kPlayer_.GetChannelIndex() < GetMaxChannel())) {
					CCommand kCommand = new CCommand();
					kCommand.SetOrder((UINT)PROTOCOL.ROOM_LIST);
					kCommand.SetMission(0);
					SRoomListGsToCl tSData = new SRoomListGsToCl(true);

					kCommand.SetExtra((UINT)EXTRA.NEW);
					INT iCount = 0;
					for(INT i = 0; i < m_iRoomTopIndex[kPlayer_.GetChannelIndex()]; ++i) {
						if(m_kRoomList[kPlayer_.GetChannelIndex(), i].IsAvailable()) {
							if(iCount >= iMAX_PACKET_ROOM_LIST) {
								TRACE("iMAX_PACKET_ROOM_LIST: count: " + iMAX_PACKET_ROOM_LIST + ", overflow: " + iCount);
								kCommand.SetOption((UINT)iMAX_PACKET_ROOM_LIST);

								INT iSize = Marshal.SizeOf(tSData);
								kCommand.SetData(tSData, iSize);

								kPlayer_.Launcher(kCommand, iSize);
								iCount = 0;
								kCommand.SetExtra((UINT)EXTRA.CHANGE);
							}

							tSData.list[iCount].id = m_kRoomList[kPlayer_.GetChannelIndex(), i].GetId();
							tSData.list[iCount].stage_id = m_kRoomList[kPlayer_.GetChannelIndex(), i].GetStageId();
							tSData.list[iCount].max = m_kRoomList[kPlayer_.GetChannelIndex(), i].GetMaxUser();

							tSData.list[iCount].SetName(m_kRoomList[kPlayer_.GetChannelIndex(), i].GetName());

							if(m_kRoomList[kPlayer_.GetChannelIndex(), i].IsDoing()) {
								tSData.list[iCount].offset = 1;
							} else {
								tSData.list[iCount].offset = 0;
							}

							++iCount;
						}
					}

					if(0 < iCount) {
						kCommand.SetOption((UINT)iCount);

						INT iSize = Marshal.SizeOf(typeof(SRoomInfo)) * iCount;
						kCommand.SetData(tSData, iSize);

						kPlayer_.Launcher(kCommand, iSize);
						TRACE("NEW or CHANGE: ");

						kCommand.SetExtra((UINT)EXTRA.DONE);
						kCommand.SetOption(0);
						kPlayer_.Launcher(kCommand);
					} else {
						if(EXTRA.NEW == (EXTRA)kCommand.GetExtra()) {
							kCommand.SetExtra((UINT)EXTRA.EMPTY);
							kCommand.SetOption(0);
							kPlayer_.Launcher(kCommand);
						}
					}
				}
			}

			public bool
			GetRoomMemberList(CPlayer kPlayer_) {
				if((0 <= kPlayer_.GetChannelIndex()) && ((UINT)kPlayer_.GetChannelIndex() < GetMaxChannel())) {
					CRoomHandler kRoomHandler = kPlayer_.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						CRoom kRoom = kRoomHandler.GetRoom();
						if(isptr(kRoom)) {
							INT iCount = 0;
							CPlayer kPlayer = null;
							CConnector kConnector = null;

							CCommand kCommand = new CCommand();
							kCommand.SetOrder((UINT)PROTOCOL.ROOM_LIST);
							kCommand.SetExtra((UINT)EXTRA.NEW);
							kCommand.SetMission(1);	// 1번은 맴버 목록.

							SRoomMemberListGsToCl tSData = new SRoomMemberListGsToCl(true);

							for(UINT i = 0; i < kRoom.GetTopCount(); ++i) {
								kPlayer = kRoom.GetMemeber(i);
								if(isptr(kPlayer)) {
									if(kRoom != kPlayer.GetRoomHandler().GetRoom()) {
										OUTPUT("critical error: unit is not in this room: " + kRoom.GetTopCount());
										return false;
									}

									kConnector = kPlayer.GetNetIO().GetConnector();
									if(isptr(kConnector)) {
										if(iCount >= iMAX_PACKET_ROOM_MEMBER_LIST) {
											TRACE("iMAX_PACKET_ROOM_MEMBER_LIST: count: " + iMAX_PACKET_ROOM_MEMBER_LIST + ", overflow: " + iCount);
											kCommand.SetOption((UINT)iMAX_PACKET_ROOM_MEMBER_LIST);

											INT iSize = Marshal.SizeOf(typeof(SRoomMember)) * iCount;
											kCommand.SetData(tSData, iSize);

											kPlayer_.Launcher(kCommand, iSize);
											iCount = 0;
											kCommand.SetExtra((UINT)EXTRA.CHANGE);
										}

										tSData.list[iCount].actor = kPlayer.GetKey();
										tSData.list[iCount].SetName(kPlayer.GetName());
										tSData.list[iCount].status = (UINT)kPlayer.GetStatus();

										tSData.list[iCount].public_ip = kConnector.GetPublicSinAddress();
										tSData.list[iCount].public_port = (UINT16)kConnector.GetPublicPort();
										tSData.list[iCount].local_ip = kConnector.GetLocalSinAddress();
										tSData.list[iCount].local_port = (UINT16)kConnector.GetLocalPort();

										TRACE("[" + ConvertToString(kPlayer.GetName()) + ":" + kPlayer.GetKey() + ":" + kPlayer.GetStatus() + "] : public addr: " + ConvertToString(kConnector.GetPublicAddress()) + ":" + kConnector.GetPublicPort() + ", local addr: " + ConvertToString(kConnector.GetLocalAddress()) + ", " + kConnector.GetLocalPort());

										++iCount;
									}
								} else {
									OUTPUT("[" + i + "] room's member is null: " + kRoom.GetTopCount());
									break;
								}
							}

							if(0 < iCount) {
								kCommand.SetOption((UINT)iCount);

								INT iSize = Marshal.SizeOf(typeof(SRoomMember)) * iCount;
								kCommand.SetData(tSData, iSize);

								kPlayer_.Launcher(kCommand, iSize);
								TRACE("NEW or CHANGE: ");

								kCommand.SetExtra((UINT)EXTRA.DONE);
								kCommand.SetOption(0);
								SRoomMemberLeaderGsToCl tLeaderData = new SRoomMemberLeaderGsToCl(true);
								if(isptr(kRoom.GetLeader())) {
									tLeaderData.leader = kRoom.GetLeader().GetKey();
								}

								iSize = Marshal.SizeOf(typeof(SRoomMemberLeaderGsToCl));
								kCommand.SetData(tLeaderData, iSize);

								kPlayer_.Launcher(kCommand, iSize);
							} else {
								if(EXTRA.NEW == (EXTRA)kCommand.GetExtra()) {
									kCommand.SetExtra((UINT)EXTRA.EMPTY);
									kCommand.SetOption(0);
									kPlayer_.Launcher(kCommand);
								}
							}
						}
					}
				}
				return true;
			}

			public void
			GetUserList(CPlayer kPlayer_) {
				if((0 <= kPlayer_.GetChannelIndex()) && ((UINT)kPlayer_.GetChannelIndex() < GetMaxChannel())) {
					CCommand kCommand = new CCommand();
					kCommand.SetOrder((UINT)PROTOCOL.INFO_USER_LIST);
					kCommand.SetExtra((UINT)EXTRA.NEW);

					SInfoUserListGsToCl tSData = new SInfoUserListGsToCl(true);

					INT iCount = 0;
					for(INT i = 0; i < m_iUserTopIndex[kPlayer_.GetChannelIndex()]; ++i) {
						if(isptr(m_kUserList[kPlayer_.GetChannelIndex(), i])) {
							if(iCount >= iMAX_PACKET_INFO_USER_LIST) {
								TRACE("iMAX_PACKET_INFO_USER_LIST: count: " + iMAX_PACKET_INFO_USER_LIST + ", overflow: " + iCount);
								kCommand.SetOption((UINT)iMAX_PACKET_INFO_USER_LIST);

								INT iSize = Marshal.SizeOf(tSData);
								kCommand.SetData(tSData, iSize);

								kPlayer_.Launcher(kCommand, iSize);
								iCount = 0;
								kCommand.SetExtra((UINT)EXTRA.CHANGE);
							}

							tSData.list[iCount].aid = m_kUserList[kPlayer_.GetChannelIndex(), i].GetAid();
							tSData.list[iCount].key = m_kUserList[kPlayer_.GetChannelIndex(), i].GetKey();
							tSData.list[iCount].SetName(m_kUserList[kPlayer_.GetChannelIndex(), i].GetName());

							++iCount;
						}
					}

					if(0 < iCount) {
						kCommand.SetOption((UINT)iCount);

						INT iSize = Marshal.SizeOf(typeof(SInfoUserData)) * iCount;
						kCommand.SetData(tSData, iSize);

						kPlayer_.Launcher(kCommand, iSize);
						TRACE("NEW or CHANGE: ");

						kCommand.SetExtra((UINT)EXTRA.DONE);
						kCommand.SetOption(0);
						kPlayer_.Launcher(kCommand);
					} else {
						if(EXTRA.NEW == (EXTRA)kCommand.GetExtra()) {
							kCommand.SetExtra((UINT)EXTRA.EMPTY);
							kCommand.SetOption(0);
							kPlayer_.Launcher(kCommand);
						}
					}
				}
			}

			public SChannelInfo[]	GetChannelInfo()			{ return m_tServerInfo.GetChannelInfo(); }
			public SChannelInfo		GetChannelInfo(UINT o)		{ return m_tServerInfo.GetChannelInfo(o); }
			public SServerInfo		GetServerInfo()				{ return m_tServerInfo; }

			public void				SetMaxChannel(INT o)		{ m_tServerInfo.SetMaxChannel((UINT)o); }
			public UINT				GetMaxChannel()				{ return m_tServerInfo.GetMaxChannel(); }

			public INT				GetUsedRoomCount(INT o)		{ return m_iUsedRoomCount[o]; }
			public INT				GetRoomTopIndex(INT o)		{ return m_iRoomTopIndex[o]; }
			public INT				GetEmptyRoomCount(INT o)	{ return m_iEmptyRoomCount[o]; }

			public void
			SendNotify(UINT uiChannel_, CHAR[] szData_, UINT uiType_) {
				CCommand kCommand = new CCommand();

				//SInfoNofityGsToCl tSData = new SInfoNofityGsToCl(true);
				//tSData.SetContent(szData_);
				kCommand.SetOrder((UINT)PROTOCOL.INFO_NOTIFY);
				//kCommand.SetOption(szData_.Length);
				kCommand.SetExtra(uiType_);

				//INT iSize = Marshal.SizeOf(typeof(SInfoNofityGsToCl)) - iMAX_CHAT_LEN - 1 + kCommand.GetOption();
				//kCommand.SetData(tSData, iSize);

				if(2 < kCommand.GetOption()) {
					TRACE("notify message: " + ConvertToString(szData_) + ", size: " + kCommand.GetOption());
					if(uiChannel_ == (UINT)(iMAX_CHANNEL_LIST)) {
						for(UINT i = 0; i < GetMaxChannel(); ++i) {
							for(INT j = 0; j < m_iUserTopIndex[i]; ++j) {
								if(isptr(m_kUserList[i, j])) {
									if(0 < m_kUserList[i, j].GetKey()) {
										TRACE("send notify: " + m_kUserList[i, j].GetKey());
										//m_kUserList[i, j].Launcher(kCommand, iSize);
										TRACE("send notify: OK: " + m_kUserList[uiChannel_, j].GetKey());
									}
								}
							}
						}
					}
				} else {
					OUTPUT("critical error: message size is 0");
				}
			}

			public void
			SendNotify(CCommand kCommand_, INT iSize_ =0, CPlayer kPlayer_ =null) {
				for(UINT i = 0; i < GetMaxChannel(); ++i) {
					for(INT j = 0; j < m_iUserTopIndex[i]; ++j) {
						if(isptr(m_kUserList[i, j])) {
							if(0 < m_kUserList[i, j].GetKey()) {
								if(m_kUserList[i, j] != kPlayer_) {
									TRACE("send notify: command: " + m_kUserList[i, j].GetKey());
									m_kUserList[i, j].Launcher(kCommand_, iSize_);
									TRACE("send notify: command: OK: " + m_kUserList[i, j].GetKey());
								}
							}
						}
					}
				}
			}

			public void
			Broadcast(CPlayer kPlayer_, CCommand kCommand_, INT iSize_ =0) {
				if(isptr(kPlayer_)) {
					if(isptr(kPlayer_.GetRoomHandler())) {
						if(-1 == kPlayer_.GetRoomHandler().GetOffset()) {
							if(kPlayer_.GetChannelIndex() < (INT)GetMaxChannel()) {
								for(INT i = 0; i < m_iUserTopIndex[kPlayer_.GetChannelIndex()]; ++i) {
									if(isptr(m_kUserList[kPlayer_.GetChannelIndex(), i])) {
										if(0 < m_kUserList[kPlayer_.GetChannelIndex(), i].GetKey()) {
											if(-1 == m_kUserList[kPlayer_.GetChannelIndex(), i].GetRoomHandler().GetOffset()) {
												TRACE("send: aid: " + m_kUserList[kPlayer_.GetChannelIndex(), i].GetAid() + ", key: " + m_kUserList[kPlayer_.GetChannelIndex(), i].GetKey());
												m_kUserList[kPlayer_.GetChannelIndex(), i].Launcher(kCommand_, iSize_);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			public void
			BroadcastChannel(UINT uiChannelIndex_, CCommand kCommand_, INT iSize_ =0, CPlayer kPlayer_ =null) {
				if(uiChannelIndex_ < GetMaxChannel()) {
					for(INT i = 0; i < m_iUserTopIndex[uiChannelIndex_]; ++i) {
						if(isptr(m_kUserList[uiChannelIndex_, i])) {
							if(m_kUserList[uiChannelIndex_, i] != kPlayer_) {
								if(0 < m_kUserList[uiChannelIndex_, i].GetKey()) {
									TRACE("send: aid: " + m_kUserList[uiChannelIndex_, i].GetAid() + ", key: " + m_kUserList[uiChannelIndex_, i].GetKey());
									m_kUserList[uiChannelIndex_, i].Launcher(kCommand_, iSize_);
								}
							}
						}
					}
				}
			}

			public void
			BroadcastAll(CCommand kCommand_, INT iSize_ =0, CPlayer kPlayer_ =null) {
				for(UINT i = 0; i < GetMaxChannel(); ++i) {
					for(INT j = 0; j < m_iUserTopIndex[i]; ++j) {
						if(isptr(m_kUserList[i, j])) {
							if(m_kUserList[i, j] != kPlayer_) {
								if(0 < m_kUserList[i, j].GetKey()) {
									m_kUserList[i, j].Launcher(kCommand_, iSize_);
								}
							}
						}
					}
				}
			}

			public void
			Shutdown() {
				for(UINT i = 0; i < GetMaxChannel(); ++i) {
					for(INT j = 0; j < m_iUserTopIndex[i]; ++j) {
						if(isptr(m_kUserList[i, j])) {
							if(0 < m_kUserList[i, j].GetKey()) {
								if(0 < m_kUserList[i, j].GetAid()) {
									m_kUserList[i, j].Disconnect();
								}
							}
						}
					}
				}
			}

			public void				SetId(INT o)				{ m_tServerInfo.SetId((UINT)o); }
			public void				SetRelay(INT o)				{ m_tServerInfo.SetRelay((UINT)o); }
			public void				SetMaxUser(INT o)			{ m_tServerInfo.SetMaxUser((UINT)o); }
			public void				SetUser(INT o)				{ m_tServerInfo.SetUser((UINT)o); }
			public void				IncreasedUser()				{ m_tServerInfo.IncreasedUser(); }
			public void				DecreasedUser()				{ m_tServerInfo.DecreasedUser(); }

			public void 			SetId(UINT o, INT p)		{ m_tServerInfo.SetId(o, (UINT)p); }
			public void				SetMaxUser(UINT o, INT p)	{ m_tServerInfo.SetMaxUser(o, (UINT)p); }
			public void				SetUser(UINT o, INT p)		{ m_tServerInfo.SetUser(o, (UINT)p); }
			public void				IncreasedUser(UINT o)		{ m_tServerInfo.IncreasedUser(o); }
			public void				DecreasedUser(UINT o)		{ m_tServerInfo.DecreasedUser(o); }

			private CRoom[,]	m_kRoomList = new CRoom[iMAX_CHANNEL_LIST, iMAX_ROOMS];
			private INT[,]		m_iEmptyRoomList = new INT[iMAX_CHANNEL_LIST, iMAX_ROOMS];

			private INT[]		m_iUsedRoomCount = new INT[iMAX_CHANNEL_LIST];
			private INT[]		m_iRoomTopIndex = new INT[iMAX_CHANNEL_LIST];
			private INT[]		m_iEmptyRoomCount = new INT[iMAX_CHANNEL_LIST];

			private CPlayer[,]	m_kUserList = new CPlayer[iMAX_CHANNEL_LIST, iMAX_CHANNEL_USERS];
			private INT[,]		m_iEmptyUserList = new INT[iMAX_CHANNEL_LIST, iMAX_CHANNEL_USERS];

			private INT[]		m_iUserMaxCount = new INT[iMAX_CHANNEL_LIST];
			private INT[]		m_iUserTopIndex = new INT[iMAX_CHANNEL_LIST];
			private INT[]		m_iEmptyUserCount = new INT[iMAX_CHANNEL_LIST];

			private SServerInfo	m_tServerInfo = new SServerInfo(true);
		}
	}
}

/* EOF */
