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
		public class CState {
			public CState() {}
			~CState() { Release(); }

			public virtual bool
			Initialize() {
				if(false == m_bInitialized) {
					return true;
				}
				return false;
			}

			public virtual bool
			Release() {
				if(m_bInitialized) {
					return true;
				}
				return false;
			}

			public virtual bool
			Update() {
				//TRACE("update: state: " + m_eStateType);
				g_kFramework.Update();
				return true;
			}

			public virtual bool	PreProcess(STATE_TYPE o)	{ return true; }
			public virtual bool PostProcess(STATE_TYPE o)	{ return true; }

			public virtual STATE_TYPE
			GetStateType() {
				return m_eStateType;
			}

			public CState
			GetTypeAs(STATE_TYPE o) {
				if(m_eStateType == o) {
					return this;
				}
				return null;
			}

			protected STATE_TYPE	m_eStateType = STATE_TYPE.STATE_NONE;
			protected CStateMgr		m_kManager = null;

			protected bool			m_bInitialized = false;
		}
	}
}

/* EOF */
