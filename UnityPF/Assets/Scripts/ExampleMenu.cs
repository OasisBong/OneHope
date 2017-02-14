/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Net;
using System.Collections;

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

	public class ExampleMenu : GameFramework {
		public void
		Awake() {
#if UNITY_STANDALONE
			Screen.SetResolution(360, 640, false);
#else
			Screen.SetResolution(360, 640, true);
#endif

			g_kFramework.Initialize();
		}

		public void
		OnApplicationQuit() {
			g_kFramework.Release();
		}

		public void
		Start() {
			g_kTextMgr.Clear();
		}

		public void
		Update() {
			g_kStateMgr.Update();
		}

		public void
		OnAutoConnect() {
			Application.LoadLevel((INT)SCENE_TYPE.SCENE_AUTO_CONNECT);
		}

		public void
		OnChat() {
			Application.LoadLevel((INT)SCENE_TYPE.SCENE_CHAT);
		}

		public void
		OnRoom() {
			Application.LoadLevel((INT)SCENE_TYPE.SCENE_ROOM);
		}

		public void
		OnWeb() {
			Application.LoadLevel((INT)SCENE_TYPE.SCENE_WEB);
		}

		public void
		OnClose() {
			Application.Quit();
		}

		//
		// Member Variables
		//
		public Button		m_kAutoConnectButton;
		public Button		m_kChatButton;
		public Button		m_kRoomButton;
		public Button		m_kCloseButton;

	}
}

/* EOF */
