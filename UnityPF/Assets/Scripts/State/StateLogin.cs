/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;

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
		public class CStateLogin : CState {
			public CStateLogin() {}
			~CStateLogin() {}

			public override bool
			Initialize() {
				if(false == m_bInitialized) {
					//TRACE("CStateLogin: Initialize()");
					m_eStateType = STATE_TYPE.STATE_LOGIN;

					if(base.Initialize()) {
						m_bInitialized = true;
						return true;
					}
				}
				return false;
			}

			public override bool
			Release() {
				if(m_bInitialized) {
					//TRACE("CStateLogin: Release()");
					m_eStateType = STATE_TYPE.STATE_NONE;

					if(base.Release()) {
						m_bInitialized = false;
						return true;
					}
				}
				return false;
			}

			public override bool
			Update() {
				if(base.Update()) {
					return true;
				}
				return false;
			}

			public override bool
			PreProcess(STATE_TYPE o) {
				TRACE("preprocess: STATE_LOGIN: " + o + " -> " + GetStateType());

				//TRACE("preprocess: STATE_LOGIN: begin");

				if(g_kNetMgr.IsConnected()) {
					g_kNetMgr.DisconnectAll();
				}

				g_kChannelMgr.Clear();
				g_kUnitMgr.Clear();

				REFRESH_CHANNEL_LIST();
				REFRESH_USER_LIST();
				REFRESH_ROOM_LIST();
				REFRESH_MEMBER_LIST();

				//TRACE("preprocess: STATE_LOGIN: end");
				return true;
			}

			public override bool
			PostProcess(STATE_TYPE o) {
				TRACE("postprocess: STATE_LOGIN: " + GetStateType() + " -> " + o);

				//TRACE("postprocess: STATE_LOGIN: begin");

				//TRACE("postprocess: STATE_LOGIN: end");
				return true;
			}
		}
	}
}

/* EOF */
