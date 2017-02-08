/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Collections.Generic;

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

	using UnityEngine.UI;

	public partial class GameFramework {
		public static CChannelMgr	g_kChannelMgr		{ get { return CChannelMgr.GetInstance(); } }

		public class CChannelMgr : CSingleton<CChannelMgr> {
			public CChannelMgr() {}
			~CChannelMgr() { /* Release(); */ }

			public bool
			Initialize() {
				if(false == m_bInitialized) {
					GameObject kChannelsObject = new GameObject("Channels");
					m_kChannelsText = kChannelsObject.AddComponent<Text>();
					DontDestroyOnLoad(kChannelsObject);

					GameObject kUserListObject = new GameObject("User List");
					m_kUserListText = kUserListObject.AddComponent<Text>();
					kUserListObject.transform.SetParent(kChannelsObject.transform);
					DontDestroyOnLoad(kUserListObject);

					GameObject kRoomListObject = new GameObject("Room List");
					m_kRoomListText = kRoomListObject.AddComponent<Text>();
					kRoomListObject.transform.SetParent(kChannelsObject.transform);
					DontDestroyOnLoad(kRoomListObject);

					GameObject kRoomObject = new GameObject("Main Room");
					m_kMainRoom = kRoomObject.AddComponent<GameRoom>();
					m_kMainRoom.Initialize();
					DontDestroyOnLoad(kRoomObject);

					m_bInitialized = true;
					return true;
				}
				return false;
			}

			public bool
			Release() {
				if(m_bInitialized) {
					m_kMainRoom.Release();

					if(isptr(m_kMainRoom)) {
						Destroy(m_kMainRoom.gameObject);
					}
					if(isptr(m_kRoomListText)) {
						Destroy(m_kRoomListText.gameObject);
					}
					if(isptr(m_kUserListText)) {
						Destroy(m_kUserListText.gameObject);
					}
					if(isptr(m_kChannelsText)) {
						Destroy(m_kChannelsText.gameObject);
					}

					m_kUserBox.Clear();
					m_kRoomBox.Clear();

					m_kMainRoom = null;
					m_kUserBox = null;
					m_kRoomBox = null;

					m_bInitialized = false;
					return true;
				}
				return false;
			}

			public void
			Clear() {
				m_kMainRoom.Clear();

				ClearUserList();
				ClearRoomList();
			}

			public void
			ClearUserList() {
				if(isptr(m_kUserListText)) {
					Transform[] kUserList = m_kUserListText.gameObject.GetComponentsInChildren<Transform>();
					foreach(Transform kUser in kUserList) {
						if(m_kUserListText.gameObject != kUser.gameObject) {
							Destroy (kUser.gameObject);
						}
					}
				}
				m_kUserBox.Clear();
			}

			public void
			ClearRoomList() {
				if(isptr(m_kRoomListText)) {
					Transform[] kRoomList = m_kRoomListText.gameObject.GetComponentsInChildren<Transform>();
					foreach(Transform kRoom in kRoomList) {
						if(m_kRoomListText.gameObject != kRoom.gameObject) {
							Destroy(kRoom.gameObject);
						}
					}
				}
				m_kRoomBox.Clear();
			}

			//public bool
			//Update() {
			//	if(isptr(m_kMainRoom)) {
			//		return m_kMainRoom.Update();
			//	}
			//	return false;
			//}

			public SUserInfo
			FindUserList(UINT uiKey_) {
				if(false == EmptyUserList()) {
					foreach(SUserInfo tInfo in m_kUserBox) {
						if(tInfo.GetKey() == uiKey_) {
							return tInfo;
						}
					}
				}
				return (new SUserInfo(true));
			}

			public SRoomInfo
			FindRoomList(UINT uiId_) {
				if(false == EmptyRoomList()) {
					foreach(SRoomInfo tInfo in m_kRoomBox) {
						if(tInfo.id == uiId_) {
							return tInfo;
						}
					}
				}
				return (new SRoomInfo(true));
			}

			public SUserInfo
			SeekUserList(UINT uiIndex_) {
				if(false == EmptyUserList()) {
					if((INT)(uiIndex_) < m_kUserBox.Count) {
						return m_kUserBox[(INT)uiIndex_];
					}

//					UINT uiCount = 0;
//					foreach(SUserInfo tInfo in m_kUserBox) {
//						if(uiCount == uiIndex_) {
//							return tInfo;
//						}
//						++uiCount;
//					}
				}
				return (new SUserInfo(true));
			}

			public SRoomInfo
			SeekRoomList(UINT uiIndex_) {
				if(false == EmptyRoomList()) {
					if((INT)(uiIndex_) < m_kRoomBox.Count) {
						return m_kRoomBox[(INT)uiIndex_];
					}

//					UINT uiCount = 0;
//					foreach(SRoomInfo tInfo in m_kRoomBox) {
//						if(uiCount == uiIndex_) {
//							return tInfo;
//						}
//						++uiCount;
//					}
				}
				return (new SRoomInfo(true));
			}

			public bool
			RemoveUserList(UINT uiKey_) {
				if(false == EmptyUserList()) {
					INT iCount = 0;
					foreach(SUserInfo tInfo in m_kUserBox) {
						if(tInfo.GetKey() == uiKey_) {
							Transform[] kUserList = m_kUserListText.gameObject.GetComponentsInChildren<Transform>();
							foreach(Transform kUser in kUserList) {
								if(kUser.name.Contains(uiKey_.ToString())) {
									Destroy(kUser.gameObject);
									break;
								}
							}

							m_kUserBox.RemoveAt(iCount);
							return true;
						}
						++iCount;
					}
				}
				return false;
			}

			public bool
			RemoveRoomList(UINT uiId_) {
				if(false == EmptyRoomList()) {
					INT iCount = 0;
					foreach(SRoomInfo tInfo in m_kRoomBox) {
						if(tInfo.id == uiId_) {
							Transform[] kRoomList = m_kRoomListText.gameObject.GetComponentsInChildren<Transform>();
							foreach(Transform kRoom in kRoomList) {
								if(kRoom.name.Contains(uiId_.ToString())) {
									Destroy(kRoom.gameObject);
									break;
								}
							}

							m_kRoomBox.RemoveAt(iCount);
							return true;
						}
						++iCount;
					}
				}
				return false;
			}

			public bool
			AddUserList(SUserInfo o) {
				m_kUserBox.Add(o);

				GameObject kUserObject = new GameObject(o.GetName() + " (" + o.GetKey() + ")");
				if(isptr(m_kUserListText)) {
					kUserObject.transform.SetParent(m_kUserListText.gameObject.transform);
				}
				DontDestroyOnLoad(kUserObject);

				return true;
			}

			public bool
			AddRoomList(SRoomInfo o) {
				m_kRoomBox.Add(o);

				GameObject kRoomObject = new GameObject(ConvertToString(o.GetName()) + " (" + o.id + ")");
				Text kRoomText = kRoomObject.AddComponent<Text>();

				SStageInfo tStageInfo = g_kStageInfoList.Find(o.stage_id);
				if(0 < tStageInfo.GetId()) {
					kRoomText.text = "offset: " + o.offset + ", max user: " + o.max + ", stage name: " + tStageInfo.GetName() + ", stage id: " + tStageInfo.GetId() + ", stage type: " + tStageInfo.GetStageType() + ", desc: " + tStageInfo.GetDesc();
				}

				if(isptr(m_kRoomListText)) {
					kRoomObject.transform.SetParent(m_kRoomListText.gameObject.transform);
				}
				DontDestroyOnLoad(kRoomObject);

				return true;
			}

			public size_t			SizeUserList()				{ return (size_t)(m_kUserBox.Count); }
			public size_t			SizeRoomList()				{ return (size_t)(m_kRoomBox.Count); }

			public bool				EmptyUserList()				{ return (0 >= m_kUserBox.Count); }
			public bool				EmptyRoomList()				{ return (0 >= m_kRoomBox.Count); }

			public GameRoom			GetMainRoom()				{ return m_kMainRoom; }

			private GameRoom		m_kMainRoom = null;

			private Text			m_kChannelsText = null;
			private Text			m_kUserListText = null;
			private Text			m_kRoomListText = null;

			private bool			m_bInitialized = false;

			private List<SUserInfo>	m_kUserBox = new List<SUserInfo>();
			private List<SRoomInfo>	m_kRoomBox = new List<SRoomInfo>();
		}
	}
}

/* EOF */
