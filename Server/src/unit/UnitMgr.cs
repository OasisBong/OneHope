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
		public static CUnitMgr	g_kUnitMgr { get { return CUnitMgr.GetInstance(); } }

		public class CUnitMgr : CSingleton< CUnitMgr > {
			public CUnitMgr() {}
			~CUnitMgr() {}

			public bool
			Release() {
				for(UINT i = 0; i < (UINT)(iMAX_TRUNK_CAPACITY); ++i) {
					for(UINT j = 0; j < m_auiTrunkCapacity[i]; ++j) {
						if(isptr(m_akUnit[i, j])) {
							m_akUnit[i, j].Clear();
							m_akUnit[i, j].Release();
							SAFE_DELETE(ref m_akUnit[i, j]);
						}
						m_akUnit[i, j] = null;
					}
					m_auiTrunkCapacity[i] = 0;
				}
				return true;
			}

			public INT
			NewTrunk(UINT uiTrunk_, UINT uiSize_) {
				if(uiTrunk_ < (UINT)(iMAX_TRUNK_CAPACITY)) {
					if(0 == m_auiTrunkCapacity[uiTrunk_]) {
						UINT uiSize = Min(uiSize_, (UINT)iMAX_UNIT_CAPACITY);

						m_auiTrunkCapacity[uiTrunk_] = uiSize;
						CUnit kUnit = null;

						for(UINT i = 0; i < uiSize; ++i) {
							kUnit = (CUnit)g_kPlayerProvider.Create((INT)UNIT_TYPE.UNIT_PLAYER);
							if(isptr(kUnit)) {
								m_akUnit[uiTrunk_, i] = kUnit;
								kUnit.Initialize();
								kUnit.Clear();
							} else {
								for(UINT j = 0; j < i; ++j) {
									if(isptr(m_akUnit[uiTrunk_, j])) {
										m_akUnit[uiTrunk_, j].Release();
										SAFE_DELETE(ref m_akUnit[uiTrunk_, j]);
									}
									m_akUnit[uiTrunk_, j] = null;
								}
								return 0;
							}
						}	///< for

						return (INT)uiSize;
					}
				}
				return -1;
			}

			public CUnit
			GetUnit(UINT uiTrunk_, UINT uiKey_) {
				if(InTrunkCapacity(uiTrunk_, uiKey_)) {
					return m_akUnit[uiTrunk_, uiKey_];
				}
				return null;
			}

			public CPlayerEx
			GetPlayer(UINT uiTrunk_, UINT uiKey_) {
				if(InTrunkCapacity(uiTrunk_, uiKey_)) {
					return (CPlayerEx)(m_akUnit[uiTrunk_, uiKey_].GetTypeAs(UNIT_TYPE.UNIT_PLAYER));
				}
				return null;
			}

			public CUnit
			GetUnit(SBogoKey o) {
				return GetUnit(o.trunk, o.key);
			}

			public CPlayerEx
			GetPlayer(SBogoKey o) {
				return GetPlayer(o.trunk, o.key);
			}

			public CUnit
			GetUnit(UINT o) {
				return GetUnit(SplitBogoKey(o));
			}

			public CPlayerEx
			GetPlayer(UINT o) {
				return GetPlayer(SplitBogoKey(o));
			}

			public CPlayerEx
			FindPlayer(UINT uiTrunk_, CHAR[] szName_) {
				UINT uiTrunkCapacity = (UINT)GetTrunkCapacity(uiTrunk_);
				if(0 < uiTrunkCapacity) {
					for(UINT i = 0; i < uiTrunkCapacity; ++i) {
						CPlayerEx kPlayer = GetPlayer(uiTrunk_, i);
						if(isptr(kPlayer)) {
							if(0 == String.Compare(ConvertToString(kPlayer.GetName()), ConvertToString(szName_))) {
								return kPlayer;
							}
						}
					}	///< for
				}
				return null;
			}

			public CPlayerEx
			FindPlayer(UINT uiTrunk_, UINT uiAid_) {
				UINT uiTrunkCapacity = (UINT)GetTrunkCapacity(uiTrunk_);
				if(0 < uiTrunkCapacity) {
					for(UINT i = 0; i < uiTrunkCapacity; ++i) {
						CPlayerEx kPlayer = GetPlayer(uiTrunk_, i);
						if(isptr(kPlayer)) {
							if(uiAid_ == kPlayer.GetAid()) {
								return kPlayer;
							}
						}
					}
				}
				return null;
			}

			public INT
			GetTrunkCapacity(UINT o) {
				if(o < (UINT)(iMAX_TRUNK_CAPACITY)) {
					return (INT)m_auiTrunkCapacity[o];
				} else {
					TRACE("Capacity is overflow: " + o);
				}
				return -1;
			}

			private bool
			InTrunkCapacity(UINT o, UINT p) {
				if(o < (UINT)(iMAX_TRUNK_CAPACITY)) {
					if(p < m_auiTrunkCapacity[o]) {
						return true;
					} else {
						TRACE("TRUNK is " + o + ", Capacity is overflow: " + p + "(" + m_auiTrunkCapacity[o] + ")");
					}
				} else {
					TRACE("TRUNK is error: maybe, this is not PC: " + o + ":" + p);
				}
				return false;
			}

			private CUnit[,]	m_akUnit = new CUnit[iMAX_TRUNK_CAPACITY, iMAX_UNIT_CAPACITY];
			private UINT[]		m_auiTrunkCapacity = new UINT[iMAX_TRUNK_CAPACITY];
		}
	}
}

/* EOF */
