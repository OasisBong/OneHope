/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;

namespace UnityEngine
{
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

    public partial class GameFramework
    {
        public class CStateGame : CState
        {
            public CStateGame() { }
            ~CStateGame() { }

            public override bool
            Initialize()
            {
                if (false == m_bInitialized)
                {
                    //TRACE("CStateGame: Initialize()");
                    m_eStateType = STATE_TYPE.STATE_GAME;

                    if (base.Initialize())
                    {
                        m_bInitialized = true;
                        return true;
                    }
                }
                return false;
            }

            public override bool
            Release()
            {
                if (m_bInitialized)
                {
                    //TRACE("CStateGame: Release()");
                    m_eStateType = STATE_TYPE.STATE_NONE;

                    if (base.Release())
                    {
                        m_bInitialized = false;
                        return true;
                    }
                }
                return false;
            }

            public override bool
            Update()
            {
                if (base.Update())
                {
                    if (g_kNetMgr.IsConnected())
                    {
                        if (g_kTick.GetTick() > g_kUnitMgr.GetMainPlayer().GetNetIO().GetSendPingServerTick())
                        {
                            //TRACE("update: check: tick: " + g_kTick.GetTick() + ", current: " + g_kUnitMgr.GetMainPlayer().GetNetIO().GetSendPingServerTick());
                            SEND_ID_PING();
                            g_kUnitMgr.GetMainPlayer().GetNetIO().SetSendPingServerTick(g_kTick.GetTick() + iMAX_PING_LATENCY_TICK);
                        }
                        else
                        {
                            //TRACE("update: check: tick: " + g_kTick.GetTick() + ", next: " + g_kUnitMgr.GetMainPlayer().GetNetIO().GetSendPingServerTick());
                        }
                    }
                    else
                    {
                        if (g_kCfgMgr.IsAutoConnected())
                        {
                            // 연결이 끊긴 상태이며 재접속 시도.
                            if (0 == m_tkSendReconnectTick)
                            {
                                m_tkSendReconnectTick = g_kTick.GetTick() + iMAX_RECONNECT_LATENCY_TICK;
                            }
                            else if (g_kTick.GetTick() > m_tkSendReconnectTick)
                            {
                                g_kNetMgr.Reconnect();
                                m_tkSendReconnectTick = g_kTick.GetTick() + iMAX_RECONNECT_LATENCY_TICK;
                            }
                        }
                    }
                    return true;
                }
                return false;
            }

            public override bool
            PreProcess(STATE_TYPE o)
            {
                TRACE("preprocess: STATE_ROOM: " + o + " -> " + GetStateType());

                //TRACE("preprocess: STATE_ROOM: begin");

                m_tkSendReconnectTick = 0;

                if (false == g_kNetMgr.IsConnected())
                {
                    g_kNetMgr.Reconnect();
                }

                //SEND_ROOM_LIST();

                //TRACE("preprocess: STATE_ROOM: end");
                return true;
            }

            public override bool
            PostProcess(STATE_TYPE o)
            {
                TRACE("postprocess: STATE_ROOM: " + GetStateType() + " -> " + o);

                //TRACE("postprocess: STATE_ROOM: begin");

                //TRACE("postprocess: STATE_ROOM: end");
                return true;
            }

            private tick_t m_tkSendReconnectTick = 0;
        }
    }
}

/* EOF */
