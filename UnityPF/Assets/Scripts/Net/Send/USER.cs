/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Runtime.InteropServices;

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
        public static bool
        SEND_USER_STATUS_READY()
        {
            CCommand kCommand = new CCommand();
            kCommand.SetOrder((UINT)PROTOCOL.USER_STATUS);
            kCommand.SetExtra((UINT)EXTRA.NONE);

            kCommand.SetOption((UINT)STATUS_TYPE.STATUS_READY);

            if (0 > g_kNetMgr.Send(kCommand))
            {
                MESSAGE("SEND_USER_STATUS_READY: sending failed: ");
                return false;
            }
            MESSAGE("SEND_USER_STATUS_READY: NONE");

            return true;
        }

        public static bool
        SEND_USER_STATUS_WAITING()
        {
            CCommand kCommand = new CCommand();
            kCommand.SetOrder((UINT)PROTOCOL.USER_STATUS);
            kCommand.SetExtra((UINT)EXTRA.NONE);

            kCommand.SetOption((UINT)STATUS_TYPE.STATUS_WAITING);

            if (0 > g_kNetMgr.Send(kCommand))
            {
                MESSAGE("SEND_USER_STATUS_WAITING: sending failed: ");
                return false;
            }
            MESSAGE("SEND_USER_STATUS_WAITING: NONE");

            return true;
        }

        public static bool
        SEND_USER_CHAT(CHAT_TYPE eType_, string szConetnt_)
        {
            CCommand kCommand = new CCommand();
            kCommand.SetOrder((UINT)PROTOCOL.USER_CHAT);

            if (CHAT_TYPE.CHAT_CHEAT == eType_)
            {
                kCommand.SetExtra((UINT)EXTRA.NEW);
            }
            else if (CHAT_TYPE.CHAT_SYSTEM == eType_)
            {
                kCommand.SetExtra((UINT)EXTRA.CHECK);
            }
            else
            {
                kCommand.SetExtra((UINT)EXTRA.NONE);
            }

            SUserChatClToGs tSData = new SUserChatClToGs(true);
            tSData.SetContent(ConvertToBytes(szConetnt_));
            kCommand.SetOption((UINT)ConvertToBytes(szConetnt_).Length);

            INT iSize = Marshal.SizeOf(tSData);
            kCommand.SetData(tSData, iSize);

            // Size 변경을 SetData 전에 할경우 위험.
            // 실제 메세지 길이만큼만 전송.
            // 패킷 트레픽을 많이 줄일수 있음.
            iSize = iSize - (iMAX_CHAT_LEN + 1) + (INT)kCommand.GetOption();

            if (0 > g_kNetMgr.Send(kCommand, iSize, PACKET_TYPE.PACKET_THROW, CRYPT_TYPE.CRYPT_RC6_SERIAL))
            {
                MESSAGE("SEND_USER_CHAT: sending failed: ");
                return false;
            }

            MESSAGE(ConvertToString(g_kUnitMgr.GetMainPlayer().GetName()) + ": " + ConvertToString(tSData.GetContent()) + ", bytes: " + (iTCP_PACKET_HEAD_SIZE + iCOMMAND_HEAD_SIZE + (Marshal.SizeOf(tSData) - (iMAX_CHAT_LEN + 1)) + (INT)kCommand.GetOption()));

            return true;
        }

        public static bool
        SEND_USER_MOVE(UINT PlayerKey)
        {
            GamePlayer kPlayer = g_kUnitMgr.GetMainPlayer();
            if(isptr(kPlayer))
            {
                CCommand kCommand = new CCommand();
                kCommand.SetOrder((UINT)PROTOCOL.USER_MOVE);
                kCommand.SetExtra((UINT)EXTRA.OK);

                SUserMoveClToGs tSData = new SUserMoveClToGs(true);
                tSData.SetKet(kPlayer.GetKey());
               // tSData.SetPosition(kPlayer.GetGameObject().transform.position.x, kPlayer.GetGameObject().transform.position.y, kPlayer.GetGameObject().transform.position.z);
                tSData.SetPosition(kPlayer.inner.GetGameObject().transform.position.x, kPlayer.inner.GetGameObject().transform.position.y, kPlayer.inner.GetGameObject().transform.position.z);

                CRoomHandler kRoomHandler = kPlayer.GetRoomHandler();
                if(isptr(kRoomHandler))
                {
                    CRoom kRoom = kRoomHandler.GetRoom();
                    if(isptr(kRoom))
                    {
                        INT iSize = Marshal.SizeOf(tSData);
                        kCommand.SetData(tSData, iSize);

                        if (0 > g_kNetMgr.Send(kCommand, iSize))
                        {
                            MESSAGE("SEND_USER_MOVE: sending failde: ");
                            return false;
                        }
                    }
                }
            }
            else
            {
                MESSAGE("SEND_USER_MOVE: null is Player: PlayerKey =" + PlayerKey);
            }
            return true;
        }
    }
}

/* EOF */
