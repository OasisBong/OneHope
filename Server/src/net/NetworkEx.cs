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
		public class CNetworkEx : CNetwork {
			public CNetworkEx() {}
			~CNetworkEx() {}

			public override bool
			ParseCommand(CExtendCommand kCommand_) {
				CCommand kCommand = kCommand_.GetCommand();
				CConnector kConnector = kCommand_.GetConnector();

				if(((UINT)PROTOCOL.PROTOCOL_NULL < kCommand.GetOrder()) && (kCommand.GetOrder() < (UINT)PROTOCOL.PROTOCOL_MAX)) {
					NativeLauncher kNativeLauncher = g_bfNativeLauncher[kCommand.GetOrder()];
					if(isptr(kNativeLauncher)) {
						if(isptr(kConnector)) {
							CUnit kActor = g_kUnitMgr.GetUnit((UINT)UNIT_TYPE.UNIT_PLAYER, (UINT)kCommand_.GetKey());
							if(isptr(kActor)) {
								if(0 < kActor.GetAid()) {
									return kNativeLauncher(kActor, kCommand);
								}
							} else {
								TRACE("[" + kCommand.GetOrder() + "] actor is null: (" + kCommand_.GetRelay() + ":" + kCommand_.GetKey() + ")");
							}
						}
					} else {
						ExtendLauncher kExtendLauncher = g_bfExtendLauncher[kCommand.GetOrder()];
						if(isptr(kExtendLauncher)) {
							return kExtendLauncher(kCommand_);
						} else {
							if(isptr(kConnector)) {
								CUnit kActor = g_kUnitMgr.GetUnit((UINT)UNIT_TYPE.UNIT_PLAYER, (UINT)kCommand_.GetKey());
								if(isptr(kActor)) {
									if(0 < kActor.GetAid()) {
										// Hack
										OUTPUT("[" + g_kTick.GetTime() + "] error: order range over [" + kCommand_.GetOrder() + "]: server attack by protocol hack: " + ConvertToString(kConnector.GetRemoteAddress()));
										kActor.Disconnect();
										return false;
									}
								}
							}
						}
						OUTPUT("[" + g_kTick.GetTime() + "] error: order is none: " + kCommand.GetOrder());
#if DEBUG
						CUnit kUnit = g_kUnitMgr.GetUnit((UINT)UNIT_TYPE.UNIT_PLAYER, (UINT)kCommand_.GetKey());
						if(isptr(kUnit)) {
							TRACE("[" + g_kTick.GetTime() + "] error: [" + kCommand.GetOrder() + "] order is none: (" + kUnit.GetAid() + "): " + ConvertToString(kUnit.GetName()));
						}
#endif
					}
				} else {
					if(isptr(kConnector)) {
						CUnit kActor = g_kUnitMgr.GetUnit((UINT)UNIT_TYPE.UNIT_PLAYER, (UINT)kCommand_.GetKey());
						if(isptr(kActor)) {
							if(0 < kActor.GetAid()) {
								// Hack
								OUTPUT("[" + g_kTick.GetTime() + "] error: order range over [" + kCommand.GetOrder() + "]: server attack by protocol hack: " + ConvertToString(kConnector.GetRemoteAddress()));
								kActor.Disconnect();
								return false;
							}
						}
					}
					OUTPUT("[" + g_kTick.GetTime() + "] error: order range over: " + kCommand.GetOrder());
				}
				return false;
			}
		}
	}
}

/* EOF */
