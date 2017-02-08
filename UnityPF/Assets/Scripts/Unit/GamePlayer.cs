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

	public class GamePlayer : GameFramework {
		public GamePlayer() {}
		~GamePlayer() {}

		public bool
		Initialize() {
			inner = (CPlayerEx)g_kPlayerProvider.Create();
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

		public bool
		Update() {
			if(isptr(inner)) {
				return inner.Update();
			}
			return false;
		}

		public void
		Clear() {
			if(isptr(inner)) {
				inner.Clear();
			}
		}

		public bool
		Create() {
			if(isptr(inner)) {
				// 캐릭터 생성에 필요한 초기화.
				return inner.Create();
			}
			return false;
		}

		public void
		Disconnect() {
			if(isptr(inner)) {
				inner.Disconnect();
			}
		}

		public INT
		Launcher(CCommand o, INT p =0, PACKET_TYPE q =PACKET_TYPE.PACKET_THROW, CRYPT_TYPE r =CRYPT_TYPE.CRYPT_NONE) {
			if(isptr(inner)) {
				return inner.Launcher(o, p, q, r);
			}
			return -1;
		}

		public bool
		Broadcast(CCommand kCommand_, INT iSize_ =0, GamePlayer kFrom_ =null, GamePlayer kTo_ =null) {
			if(isptr(inner)) {
				return inner.Broadcast(kCommand_, iSize_, (CUnit)kFrom_.inner, (CUnit)kTo_.inner);
			}
			return false;
		}

		public bool
		ReCalculateData() {
			if(isptr(inner)) {
				return inner.ReCalculateData();
			}
			return false;
		}

		public CNetIO
		GetNetIO() {
			if(isptr(inner)) {
				return inner.GetNetIO();
			}
			return null;
		}

		public CRoomHandler
		GetRoomHandler() {
			if(isptr(inner)) {
				return inner.GetRoomHandler();
			}
			return null;
		}

		public INT
		GetChannelIndex() {
			if(isptr(inner)) {
				return inner.GetChannelIndex();
			}
			return -1;
		}

		public void
		SetChannelIndex(INT o) {
			if(isptr(inner)) {
				inner.SetChannelIndex(o);
			}
		}

		public INT
		GetUserIndex() {
			if(isptr(inner)) {
				return inner.GetUserIndex();
			}
			return -1;
		}

		public void
		SetUserIndex(INT o) {
			if(isptr(inner)) {
				inner.SetUserIndex(o);
			}
		}

		public UINT
		GetAid() {
			if(isptr(inner)) {
				return inner.GetAid();
			}
			return 0;
		}

		public void
		SetAid(UINT o) {
			if(isptr(inner)) {
				inner.SetAid(o);
			}
		}

		public UINT
		GetKey() {
			if(isptr(inner)) {
				return inner.GetKey();
			}
			return 0;
		}

		public void
		SetKey(UINT o) {
			if(isptr(inner)) {
				inner.SetKey(o);

				if(this == g_kUnitMgr.GetMainPlayer()) {
					transform.gameObject.name = "Main Player (" + o + ")";
				} else {
					transform.gameObject.name = "Other Player (" + o + ")";
				}
			}
		}

		public STATUS_TYPE
		GetStatus() {
			if(isptr(inner)) {
				return inner.GetStatus();
			}
			return STATUS_TYPE.STATUS_EXIT;
		}

		public void
		SetStatus(STATUS_TYPE o) {
			if(isptr(inner)) {
				inner.SetStatus(o);
			}
		}

		public CHAR[]
		GetLoginId() {
			if(isptr(inner)) {
				return inner.GetLoginId();
			}
			return null;
		}

		public void
		SetLoginId(CHAR[] o) {
			if(isptr(inner)) {
				inner.SetLoginId(o);
			}
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

        public GameObject
        GetGameObject()
        {
            if(isptr(inner))
            {
                return inner.ActorObj;
            }

            return null;
        }

        public void
        SetGameObject(GameObject o)
        {
            if(isptr(inner))
            {
                inner.ActorObj = o;
            }
        }

		public CPlayerEx	inner = null;

	}
}

/* EOF */
