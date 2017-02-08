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
		public class CRoom {
			public CRoom() {}
			~CRoom() {}

			public virtual bool
			Initialize() {
				Clear();
				return true;
			}

			public virtual bool
			Release() {
				for(UINT i = 0; i < GetTopCount(); ++i) {
					if(isptr(m_kMembers[i])) {
						m_kMembers[i] = null;
					} else {
						TRACE("critical error: unit[" + i + "] is null: " + GetTopCount());
					}
				}

				return true;
			}

			public virtual void
			Clear() {
				//TRACE("Clear: ROOM_LEAVE: ");
				CCommand kCommand = new CCommand();
				kCommand.SetOrder((UINT)PROTOCOL.ROOM_LEAVE);
				kCommand.SetExtra((UINT)EXTRA.CLOSE);

				for(UINT i = 0; i < GetTopCount(); ++i) {
					CPlayer kPlayer = m_kMembers[i];
					if(isptr(kPlayer)) {
						if(isptr(kPlayer.GetRoomHandler())) {
							kPlayer.GetRoomHandler().Clear();
						}
						kPlayer.Launcher(kCommand);
					}
				}

				for(INT i = 0; i < iMAX_ROOM_MEMBERS; ++i) {
					m_kMembers[i] = null;
				}

				m_uiTopCount = 0;

				m_uiStageId = 0;
				m_bOffset = false;
				m_uiMaxUser = 0;
			}

			public virtual bool
			Update() {
				return true;
			}

			public virtual bool
			Create(CUnit kUnit_) {
				CRoomHandler kRoomHandler = kUnit_.GetRoomHandler();
				if(isptr(kRoomHandler)) {
					if(null == kRoomHandler.GetRoom()) {
						m_kMembers[0] = (CPlayer)kUnit_;

						kRoomHandler.SetRoom(this);
						kRoomHandler.SetOffset(0);
						kUnit_.SetStatus(STATUS_TYPE.STATUS_READY);

						IncreasedTopCount();

						CCommand kCommand = new CCommand();
						kCommand.SetOrder((UINT)PROTOCOL.ROOM_INFO);
						kCommand.SetExtra((UINT)EXTRA.IN);

						SRoomInfo tOtherData = new SRoomInfo(true);
						tOtherData.id = GetId();
						tOtherData.max = GetMaxUser();
						tOtherData.stage_id = GetStageId();
						if(IsDoing()) {
							tOtherData.offset = 1;
						} else {
							tOtherData.offset = 0;
						}
						tOtherData.SetName(GetName());

						INT iSize = Marshal.SizeOf(tOtherData);
						kCommand.SetData(tOtherData, iSize);

						SRoomKey tKey = new SRoomKey((UINT16)GetId());
						g_kChannelMgr.BroadcastChannel(tKey.channel, kCommand, iSize, (CPlayer)kUnit_);

						return true;
					} else {
						TRACE("critical error: in room");
					}
				} else {
					OUTPUT("critical error: handler is null: ");
				}
				return false;
			}

			public virtual bool
			Join(CUnit kUnit_) {
				CCommand kCommand = new CCommand();
				kCommand.SetOrder((UINT)PROTOCOL.ROOM_JOIN);

				if(0 < GetTopCount() && (GetTopCount() < GetMaxUser())) {
					CRoomHandler kRoomHandler = kUnit_.GetRoomHandler();
					if(isptr(kRoomHandler)) {
						if(null == kRoomHandler.GetRoom()) {
							SRoomKey tKey = new SRoomKey((UINT16)GetId());
							CPlayer kPlayer = (CPlayer)kUnit_;
							if(isptr(kPlayer)) {
								if(tKey.channel != kPlayer.GetChannelIndex()) {
									g_kChannelMgr.InUser(tKey.channel, kPlayer);
								}
							}

							kRoomHandler.SetRoom(this);
							kRoomHandler.SetOffset((INT)GetTopCount());
							// Join 한 캐릭터 정보를 클라이언트에 보냅니다.
							if(kRoomHandler.Join()) {
								m_kMembers[GetTopCount()] = (CPlayer)kUnit_;
								IncreasedTopCount();

								TRACE("Join OK: [" + kUnit_.GetAid() + " (" + kUnit_.GetKey() + ")] : " + GetTopCount() + " < " + iMAX_ROOM_MEMBERS);

								kCommand.SetExtra((UINT)EXTRA.OK);
								SRoomJoinGsToCl tSData = new SRoomJoinGsToCl(true);
								tSData.id = GetId();

								tSData.max = GetMaxUser();
								tSData.stage_id = GetStageId();
								if(IsDoing()) {
									tSData.offset = 1;
								} else {
									tSData.offset = 0;
								}
								tSData.SetName(GetName());

								INT iSize = Marshal.SizeOf(tSData);
								kCommand.SetData(tSData, iSize);

								kUnit_.Launcher(kCommand, iSize);

								return true;
							} else {
								kRoomHandler.Clear();
								kCommand.SetExtra((UINT)EXTRA.DENY);
								TRACE("critical error: in room");
							}
						} else {
							kCommand.SetExtra((UINT)EXTRA.DENY);
							TRACE("critical error: in room");
						}
					} else {
						TRACE("critical error: RoomHandler is null: ");
						kCommand.SetExtra((UINT)EXTRA.FAIL);
					}
				} else {
					TRACE("error: room is full. or empty: " + GetTopCount() + "[" +  iMAX_ROOM_MEMBERS + "] " + kUnit_.GetKey());
					kCommand.SetExtra((UINT)EXTRA.FULL);
				}
				kUnit_.Launcher(kCommand);

				return false;
			}

			public virtual bool
			Leave(CUnit kUnit_) {
				CRoomHandler kRoomHandler = kUnit_.GetRoomHandler();
				if(isptr(kRoomHandler)) {
					UINT uiOutIndex = (UINT)kRoomHandler.GetOffset();
					if(InRange(uiOutIndex)) {
						if(kUnit_ == (CUnit)m_kMembers[uiOutIndex]) {
							DecreasedTopCount();

							if(0 >= GetTopCount()) {
								TRACE("room clear: ");
								g_kChannelMgr.DeleteRoom((UINT16)GetId());
								kRoomHandler.Clear();

								CCommand kCommand = new CCommand();
								kCommand.SetOrder((UINT)PROTOCOL.ROOM_INFO);
								kCommand.SetExtra((UINT)EXTRA.OUT);

								SRoomInfo tOtherData = new SRoomInfo(true);
								tOtherData.id = GetId();

								INT iSize = Marshal.SizeOf(typeof(UINT32));
								kCommand.SetData(tOtherData, iSize);

								SRoomKey tKey = new SRoomKey((UINT16)GetId());
								g_kChannelMgr.BroadcastChannel(tKey.channel, kCommand, iSize, (CPlayer)kUnit_);
							} else {
								if(uiOutIndex == GetTopCount()) {
									m_kMembers[GetTopCount()] = null;
									kRoomHandler.Leave();
									TRACE("out unit index: " + GetTopCount());
								} else {
									CRoomHandler kNewRoomHandler = null;
									if(kUnit_ == (CUnit)m_kMembers[0]) {
										// 방장이 나갈 경우.
										// 응답 지연시간 비교로 가장빠른 맴버를 방장으로 정하는 코드를 넣으려면 여기서합니다.
										// 아래는 무조건 다음 맴버로 고정하거나 종료처리됩니다.
										UINT uiIndex = 1;

										if(3 > GetTopCount()) {
											if(2 != GetTopCount()) {
												uiIndex = 0;
											}
										}

										if((0 < uiIndex) && (uiIndex <= GetTopCount())) {
											m_kMembers[0] = m_kMembers[uiIndex];
											m_kMembers[uiIndex] = null;
											kNewRoomHandler = m_kMembers[0].GetRoomHandler();
											if(isptr(kNewRoomHandler)) {
												kNewRoomHandler.SetOffset(0);
											}

											if(uiIndex < GetTopCount()) {
												m_kMembers[uiIndex] = m_kMembers[GetTopCount()];
												m_kMembers[GetTopCount()] = null;
												kNewRoomHandler = m_kMembers[uiIndex].GetRoomHandler();
												if(isptr(kNewRoomHandler)) {
													kNewRoomHandler.SetOffset((INT)uiIndex);
												}
											}

											TRACE("Leave OK: ");
											TRACE("leader: out unit: name: " + ConvertToString(kUnit_.GetName()));
											TRACE("leader: in unit: name: " + ConvertToString(m_kMembers[0].GetName()) + " (" + uiIndex + " -> 0)");
											TRACE("leader: move unit: name: " + ConvertToString(m_kMembers[uiIndex].GetName()) + " (" + GetTopCount() + " -> " + uiIndex + ")");

											// 빠져나간 유닛과 새로운 방장을 함께 전달할경우 각 클라이언트들은 동일한 갱신이 가능합니다.
											kRoomHandler.Leave(m_kMembers[0]);
										} else {
											// 방안에 아무도 없음.

											m_kMembers[0] = m_kMembers[GetTopCount()];
											m_kMembers[GetTopCount()] = null;

											kNewRoomHandler = m_kMembers[0].GetRoomHandler();
											if(isptr(kNewRoomHandler)) {
												kNewRoomHandler.SetOffset(0);
											}

											TRACE("Leave OK: ");
											kRoomHandler.Leave(m_kMembers[0]);
										}
										return true;
									}

									TRACE("switch a member position in room: " + uiOutIndex + " <-> " + GetTopCount());
									m_kMembers[uiOutIndex] = m_kMembers[GetTopCount()];
									m_kMembers[GetTopCount()] = null;
									kNewRoomHandler = m_kMembers[uiOutIndex].GetRoomHandler();
									if(isptr(kNewRoomHandler)) {
										kNewRoomHandler.SetOffset((INT)uiOutIndex);
									}
									kRoomHandler.Leave();
								}
							}
							return true;
						}
					} else {
						OUTPUT("critical error: offset is error: ");
					}
				}
				return false;
			}

			public virtual bool
			Broadcast(CCommand kCommand_, INT iSize_ =0, CUnit kActor_ =null, CUnit kTarget_ =null) {
				if(0 < GetTopCount()) {
					CUnit kUnit = null;
					for(UINT i = 0; i < GetTopCount(); ++i) {
						kUnit = m_kMembers[i];
						if(isptr(kUnit)) {
							if((kUnit != kActor_) && (kUnit != kTarget_)) {
								kUnit.Launcher(kCommand_, iSize_);
							}
						} else {
							OUTPUT("critical error: unit[" + i + "] is null: " + GetTopCount());
						}
					}
					return true;
				} else {
					OUTPUT("room is empty");
				}
				return false;
			}

			public CPlayer
			GetMemeber(UINT o)	{
				if(o < (UINT)(iMAX_ROOM_MEMBERS)) {
					return m_kMembers[o];
				}
				return null;
			}

			public CPlayer
			GetLeader() {
				return m_kMembers[0];
			}

			public CHAR[]		GetName()			{ return m_szName; }

			public void
			SetName(CHAR[] o) {
				INT iLength = o.Length;
				if(iROOM_NAME_LEN < iLength) {
					iLength = iROOM_NAME_LEN;
				}
				Array.Copy(o, 0, m_szName, 0, iLength);
				m_szName[iROOM_NAME_LEN] = (CHAR)('\0');
			}

			public void			SetId(UINT o)		{ m_usId = (UINT16)o; }
			public UINT			GetId()				{ return m_usId; }

			public INT
			GetIndex() {
				return (INT)((UINT16)(m_usId) & 0x03FF);
			}

			public INT
			GetChannelIndex() {
				return (INT)((UINT32)(m_usId >> 10) - 1);
			}

			public UINT			GetStageId()		{ return m_uiStageId; }
			public void			SetStageId(UINT o)	{ m_uiStageId = o; }

			public void			SetDoing(bool o)	{ m_bOffset = o; }
			public bool			IsDoing()			{ return m_bOffset; }

			public bool			IsAvailable()		{ return (0 < m_uiTopCount); }

			public void
			SetMaxUser(UINT o)	{
				if((UINT)(iMAX_ROOM_MEMBERS) > o) {
					m_uiMaxUser = o;
				} else {
					m_uiMaxUser = iMAX_ROOM_MEMBERS;
				}
			}

			public UINT			GetMaxUser()		{ return m_uiMaxUser; }

			public UINT			GetTopCount()		{ return m_uiTopCount; }

			protected void		IncreasedTopCount()	{ ++m_uiTopCount; }
			protected void		DecreasedTopCount()	{ --m_uiTopCount; }

			protected void		SetTopCount(UINT o)	{ m_uiTopCount = o; }
			protected bool		InRange(UINT o)		{ return (o < m_uiTopCount); }

			protected CPlayer[]	m_kMembers = new CPlayer[iMAX_ROOM_MEMBERS];
			protected UINT		m_uiTopCount = 0;

			private UINT16		m_usId = 0;
			private UINT		m_uiStageId = 0;
			private bool		m_bOffset = false;
			private UINT		m_uiMaxUser = 0;

			private CHAR[]		m_szName = new CHAR[iROOM_NAME_LEN + 1];
		}
	}
}

/* EOF */
