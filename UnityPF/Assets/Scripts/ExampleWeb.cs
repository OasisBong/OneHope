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

	public class ExampleWeb : GameFramework {
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
			if(0 < ConvertToString(g_kUnitMgr.GetMainPlayer().GetLoginId()).Length) {
				m_kLoginIdInputField.text = ConvertToString(g_kUnitMgr.GetMainPlayer().GetLoginId());
			} else {
				m_kLoginIdInputField.text = DefaultLoginId;
			}

			if(0 < g_kCfgMgr.GetRequestUrl().Length) {
				m_kRequestUrlInputField.text = g_kCfgMgr.GetRequestUrl();
			} else {
				m_kRequestUrlInputField.text = DefaultRequestUrl;
			}

			g_kTextMgr.Clear();
			g_kTextMgr.SetMessageTransform(m_kMessagePanel);

			g_kCfgMgr.SetSendType(SEND_TYPE.SEND_MASTER_MAIN);

			CNetIO kNetIO = g_kUnitMgr.GetMainPlayer().GetNetIO();
			if(isptr(kNetIO)) {
				kNetIO.SetConnector(g_kNetMgr.GetCurrentConnector());
			}

			Application.runInBackground = true;

			if(STATE_TYPE.STATE_EMPTY == g_kStateMgr.GetTransitionType()) {
				g_kStateMgr.SetTransition(STATE_TYPE.STATE_LOGIN);
			}
		}

		public void
		Update() {
			g_kStateMgr.Update();
		}

		public void
		OnGUI() {
			//
		}

		public void
		OnAuthorize() {
			CConnector kConnector = g_kNetMgr.GetCurrentConnector();
			if(isptr(kConnector)) {
				GamePlayer kMainPlayer = g_kUnitMgr.GetMainPlayer();
				if(isptr(kMainPlayer)) {
					if(0 < m_kLoginIdInputField.text.Length) {
						if(0 < m_kRequestUrlInputField.text.Length) {
							g_kCfgMgr.SetRequestUrl(m_kRequestUrlInputField.text);

							MESSAGE("connecting: url: " + g_kCfgMgr.GetRequestUrl());

							kMainPlayer.SetLoginId(ConvertToBytes(m_kLoginIdInputField.text));

							if(0 < g_kChannelMgr.GetMainRoom().GetId()) {
								g_kChannelMgr.GetMainRoom().Clear();
							}

							SEND_WEB_AUTHORIZE();
							return;
						} else {
							MESSAGE("error: request url is empty.");
						}
					} else {
						MESSAGE("error: login Id is empty.");
					}
				} else {
					MESSAGE("critical error: main player is null.");
				}
			} else {
				MESSAGE("critical error: main connector is null.");
			}
		}

		public void
		OnVerify() {
			if(0 < m_kRequestUrlInputField.text.Length) {
				g_kCfgMgr.SetRequestUrl(m_kRequestUrlInputField.text);

				SEND_WEB_CHECK();
			} else {
				MESSAGE("error: request url is empty.");
			}
		}

		public void
		OnMenu() {
			Application.LoadLevel((INT)SCENE_TYPE.SCENE_MENU);
		}

		//
		// Unity Editor Variables
		//
		public string		DefaultLoginId;
		public string		DefaultRequestUrl;

		//
		// Member Variables
		//
		public Button		m_kAuthorizeButton;
		public Button		m_kVerifyButton;

		public Button		m_kMenuButton;

		public InputField	m_kLoginIdInputField;
		public InputField	m_kRequestUrlInputField;

		public Transform	m_kIdentityPanel;
		public Transform	m_kMessagePanel;

	}
}

/* EOF */
