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
		public class CStateEmpty : CState {
			public CStateEmpty() {}
			~CStateEmpty() {}

			public override bool
			Initialize() {
				if(false == m_bInitialized) {
					//TRACE("CStateEmpty: Initialize()");
					m_eStateType = STATE_TYPE.STATE_EMPTY;

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
					//TRACE("CStateEmpty: Release()");
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
				TRACE("preprocess: STATE_EMPTY: " + o + " -> " + GetStateType());

				//TRACE("preprocess: STATE_EMPTY: begin");

				//TRACE("preprocess: STATE_EMPTY: end");
				return true;
			}

			public override bool
			PostProcess(STATE_TYPE o) {
				TRACE("postprocess: STATE_EMPTY: " + GetStateType() + " -> " + o);

				//TRACE("postprocess: STATE_EMPTY: begin");

				//TRACE("postprocess: STATE_EMPTY: end");
				return true;
			}
		}
	}
}

/* EOF */
