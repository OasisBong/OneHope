/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
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

	public class GameRoom : GameFramework {
		public GameRoom() {}
		~GameRoom() {}

		public bool
		Initialize() {
			inner = new CRoomEx();
			inner.outer = this;

			return inner.Initialize();
		}

		public bool
		Release() {
			if(isptr(inner)) {
				if(inner.Release()) {
					inner.outer = null;
					inner = null;
					return true;
				}
				inner.outer = null;
				inner = null;
			}
			return false;
		}

		public void
		Clear() {
			if(isptr(inner)) {
				SetId(0);	// 게임오브젝트 이름 변경을 위해 실행.

				inner.Clear();
			}
		}

		public bool
		Update() {
			if(isptr(inner)) {
				return inner.Update();
			}
			return false;
		}

		public bool
		Create(GamePlayer kUnit_) {
			if(isptr(inner)) {
				return inner.Create((CUnit)kUnit_.inner);
			}
			return false;
		}

		public bool
		Join(GamePlayer kUnit_) {
			if(isptr(inner)) {
				return inner.Join((CUnit)kUnit_.inner);
			}
			return false;
		}

		public bool
		Leave(GamePlayer kUnit_) {
			if(isptr(inner)) {
				return inner.Leave((CUnit)kUnit_.inner);
			}
			return false;
		}

		public bool
		Broadcast(CCommand kCommand_, INT iSize_ =0, GamePlayer kActor_ =null, GamePlayer kTarget_ =null) {
			if(isptr(inner)) {
				return inner.Broadcast(kCommand_, iSize_, (CUnit)kActor_.inner, (CUnit)kTarget_.inner);
			}
			return false;
		}

		public GamePlayer
		GetMember(UINT o) {
			if(isptr(inner)) {
				CPlayerEx kPlayer = (CPlayerEx)inner.GetMember(o);
				if(isptr(kPlayer)) {
					return kPlayer.outer;
				}
			}
			return null;
		}

		public GamePlayer
		GetLeader() {
			if(isptr(inner)) {
				CPlayerEx kPlayer = (CPlayerEx)inner.GetLeader();
				if(isptr(kPlayer)) {
					return kPlayer.outer;
				}
			}
			return null;
		}

		public CHAR[]
		GetName() {
			if(isptr(inner)) {
				return inner.GetName();
			}
			return null;
		}

		public void
		SetName(CHAR[] o) {
			if(isptr(inner)) {
				inner.SetName(o);
			}
		}

		public void
		SetId(UINT o) {
			if(isptr(inner)) {
				inner.SetId(o);

				if(0 < inner.GetId()) {
					transform.gameObject.name = "Main Room (" + inner.GetId() + ")";
				} else {
					transform.gameObject.name = "Main Room";
				}
			}
		}

		public UINT			GetId()				{ if(isptr(inner)) { return inner.GetId(); } return 0; }

		public INT			GetIndex()			{ if(isptr(inner)) { return inner.GetIndex(); } return -1; }

		public INT			GetChannelIndex()	{ if(isptr(inner)) { return inner.GetChannelIndex(); } return -1; }

		public UINT			GetStageId()		{ if(isptr(inner)) { return inner.GetStageId(); } return 0; }
		public void			SetStageId(UINT o)	{ if(isptr(inner)) { inner.SetStageId(o); } }

		public void			SetDoing(bool o)	{ if(isptr(inner)) { inner.SetDoing(o); } }
		public bool			IsDoing()			{ if(isptr(inner)) { return inner.IsDoing(); } return false; }

		public bool			IsAvailable()		{ if(isptr(inner)) { return inner.IsAvailable(); } return false; }

		public void			SetMaxUser(UINT o)	{ if(isptr(inner)) { inner.SetMaxUser(o); } }
		public UINT			GetMaxUser()		{ if(isptr(inner)) { return inner.GetMaxUser(); } return 0; }

		public UINT			GetTopCount()		{ if(isptr(inner)) { return inner.GetTopCount(); } return 0; }

		public CRoomEx	inner = null;
	}
}

/* EOF */
