/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Text.RegularExpressions;

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

	public partial class GameFramework : AutoFramework {
		public const string KEY_HEADER_CRYPT = "HeaderCrypt";

		public static UINT	Max(UINT x, UINT y)					{ return (x > y) ? x : y; }
		public static UINT	Min(UINT x, UINT y)					{ return (x < y) ? x : y; }

//		public static void	SAFE_DELETE(ref SPlayerData o)		{ o.Clear(); }
//		public static void	SAFE_DELETE(ref SNpcData o)			{ o.Clear(); }
		public static void	SAFE_DELETE(ref CRoomHandler o)		{ o = null; }
		public static void	SAFE_DELETE(ref CNetIO o)			{ o = null; }

		public static void	SAFE_DELETE_RELEASE(ref CState o)	{ if(isptr(o)) { o.Release(); } o = null; }

		public static void
		MESSAGE(string o) {
			if(isptr(o)) {
				if(0 < o.Length) {
					if(isptr(g_kTextMgr.GetMessageTransform())) {
						Text kText = g_kTextMgr.GetMessageTransform().gameObject.GetComponentInChildren<Text>();
						if(isptr(kText)) {
							StringBuilder kBuilder = new StringBuilder();

							INT iCount = Regex.Matches(kText.text, Environment.NewLine).Count;
							if(iCount >= 100) {
								INT iOffset = kText.text.IndexOf(Environment.NewLine)+1;
								kBuilder.Append(kText.text.Remove(0, iOffset));
							} else {
								kBuilder.Append(kText.text);
							}
							kBuilder.AppendLine(o);
							kText.text = kBuilder.ToString();

							ScrollRect kScollRect = g_kTextMgr.GetMessageTransform().gameObject.GetComponentInChildren<ScrollRect>();
							if(isptr(kScollRect)) {
								kScollRect.verticalNormalizedPosition = 0f;
							}
							return;
						}
					}
					UnityEngine.Debug.Log(o);
				}
			}
		}

		public static void
		REFRESH_CHANNEL_LIST() {
			if(0 < g_kChannelInfoList.Size()) {
				StringBuilder kBuilder = new StringBuilder();

				for(UINT i = 0; i < g_kChannelInfoList.Size(); ++i) {
					SChannelInfo tChannelInfo = g_kChannelInfoList.Seek(i);
					kBuilder.AppendLine(tChannelInfo.GetName() + ", id: " + tChannelInfo.GetId());
				}

				if(0 < kBuilder.Length) {
					if(isptr(g_kTextMgr.GetChannelTransform())) {
						Text kText = g_kTextMgr.GetChannelTransform().gameObject.GetComponentInChildren<Text>();
						if(isptr(kText)) {
							kText.text = kBuilder.ToString();
							return;
						}
					}
					UnityEngine.Debug.Log(kBuilder.ToString());
				}
			}
		}

		public static void
		REFRESH_USER_LIST() {
			if(0 < g_kChannelMgr.SizeUserList()) {
				StringBuilder kBuilder = new StringBuilder();

				for(UINT i = 0; i < g_kChannelMgr.SizeUserList(); ++i) {
					SUserInfo tUserInfo = g_kChannelMgr.SeekUserList(i);
					kBuilder.AppendLine(tUserInfo.GetName() + ", key: " + tUserInfo.GetKey() + ", aid: " + tUserInfo.GetAid());
				}

				if(0 < kBuilder.Length) {
					if(isptr(g_kTextMgr.GetUserListTransform())) {
						Text kText = g_kTextMgr.GetUserListTransform().gameObject.GetComponentInChildren<Text>();
						if(isptr(kText)) {
							kText.text = kBuilder.ToString();
							return;
						}
					}
					UnityEngine.Debug.Log(kBuilder.ToString());
				}
			} else {
				if(isptr(g_kTextMgr.GetUserListTransform())) {
					Text kText = g_kTextMgr.GetUserListTransform().gameObject.GetComponentInChildren<Text>();
					if(isptr(kText)) {
						kText.text = "";
					}
				}
			}
		}

		public static void
		REFRESH_ROOM_LIST() {
			if(0 < g_kChannelMgr.SizeRoomList()) {
				StringBuilder kBuilder = new StringBuilder();

				for(UINT i = 0; i < g_kChannelMgr.SizeRoomList(); ++i) {
					SRoomInfo tRoomInfo = g_kChannelMgr.SeekRoomList(i);
					if(0 < tRoomInfo.id) {
						SStageInfo tStageInfo = g_kStageInfoList.Find(tRoomInfo.stage_id);
						if(0 < tStageInfo.GetId()) {
							kBuilder.Append(ConvertToString(tRoomInfo.GetName()));
							kBuilder.Append(", ROOM: id: " + tRoomInfo.id);
							kBuilder.Append(", offset: " + tRoomInfo.offset);
							kBuilder.Append(", max user: " + tRoomInfo.max);
							kBuilder.Append(", STAGE: name: " + tStageInfo.GetName());
							kBuilder.Append(", id: " + tStageInfo.GetId());
							kBuilder.Append(", type: " + tStageInfo.stage_type);
							kBuilder.Append(", desc: " + tStageInfo.GetDesc());
							kBuilder.AppendLine();
						}
					}
				}

				if(0 < kBuilder.Length) {
					if(isptr(g_kTextMgr.GetRoomListTransform())) {
						Text kText = g_kTextMgr.GetRoomListTransform().gameObject.GetComponentInChildren<Text>();
						if(isptr(kText)) {
							kText.text = kBuilder.ToString();
							return;
						}
					}
					UnityEngine.Debug.Log(kBuilder.ToString());
				}
			} else {
				if(isptr(g_kTextMgr.GetRoomListTransform())) {
					Text kText = g_kTextMgr.GetRoomListTransform().gameObject.GetComponentInChildren<Text>();
					if(isptr(kText)) {
						kText.text = "";
					}
				}
			}
		}

		public static void
		REFRESH_MEMBER_LIST() {
			if(isptr(g_kChannelMgr.GetMainRoom())) {
				if(0 < g_kChannelMgr.GetMainRoom().GetTopCount()) {
					StringBuilder kBuilder = new StringBuilder();

					for(UINT i = 0; i < g_kChannelMgr.GetMainRoom().GetTopCount(); ++i) {
						GamePlayer kPlayer = g_kChannelMgr.GetMainRoom().GetMember(i);
						if(isptr(kPlayer)) {
							kBuilder.AppendLine("[" + ConvertToString(kPlayer.GetName()) + "," + kPlayer.GetKey() + "," + kPlayer.GetAid() + "]: " + kPlayer.GetStatus().ToString().Substring((kPlayer.GetStatus().ToString().IndexOf("_") + 1)) + ", addr: " + ConvertToString(kPlayer.GetNetIO().GetConnector().GetPublicAddress()) + "(" + ConvertToString(kPlayer.GetNetIO().GetConnector().GetLocalAddress()) + "):" + kPlayer.GetNetIO().GetConnector().GetPublicPort());
						}
					}

					if(0 < kBuilder.Length) {
						if(isptr(g_kTextMgr.GetMemberTransform())) {
							Text kText = g_kTextMgr.GetMemberTransform().gameObject.GetComponentInChildren<Text>();
							if(isptr(kText)) {
								kText.text = kBuilder.ToString();
								return;
							}
						}
						UnityEngine.Debug.Log(kBuilder.ToString());
					}
				} else {
					if(isptr(g_kTextMgr.GetMemberTransform())) {
						Text kText = g_kTextMgr.GetMemberTransform().gameObject.GetComponentInChildren<Text>();
						if(isptr(kText)) {
							kText.text = "";
						}
					}
				}
			}
		}

	}
}

/* EOF */
