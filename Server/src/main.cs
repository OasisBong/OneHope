/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

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
        public static void
        CallbackSigint(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;

            g_kFramework.Release();

            TRACE("[" + Process.GetCurrentProcess().Id + "] SIGINT: ");
        }

        public static void
        Version()
        {
            PRINT(GetNetDroneVersion());
            PRINT(GetServiceVersion());
#if _THREAD
            PRINT("CLR thread");
#else
			PRINT("non-thread");
#endif
        }
    }

    public class main : GameFramework
    {
        public static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CallbackSigint);

            bool bServerId = false;
            bool bConfig = false;
            bool bDirectory = false;
            bool bDaemon = false;

            INT iServerId = 0;
            string szConfigPath = "";
            //string szMessagePath = "";

            for (INT i = 0; i < args.Length; ++i)
            {
                if (0 == String.Compare(args[i], "-v"))
                {
                    Version();
                    return;
                }

                if (0 == String.Compare(args[i], "-s"))
                {
                    bServerId = true;
                    continue;
                }

                if (0 == String.Compare(args[i], "-c"))
                {
                    bConfig = true;
                    continue;
                }

                if (0 == String.Compare(args[i], "-d"))
                {
                    bDirectory = true;
                    bDaemon = true;
                    continue;
                }

                if (bServerId)
                {
                    bServerId = false;
                    iServerId = Convert.ToInt32(args[i]);
                }

                if (bConfig)
                {
                    bConfig = false;
                    szConfigPath = args[i];
                }

                if (bDirectory)
                {
                    bDirectory = false;
                    // 아직 사용 안함.
                    //szMessagePath = "";
                }
            }

            //TRACE("process started: " + Process.GetCurrentProcess().Id);

            string PACKAGE_NAME = Assembly.GetExecutingAssembly().GetName().Name;

            if (false == bDaemon)
            {
                // remote script, local에서 개발 할 때 사용함.
                Action("process mode: frontend", true);

                //TRACE("option s: " + iServerId);
                //TRACE("option c: " + szConfigPath);
                //TRACE("option d: " + szMessagePath);

                try
                {
                    if (g_kFramework.Initialize(ConvertToBytes(szConfigPath), ConvertToBytes(PACKAGE_NAME), iServerId))
                    {
                        while (g_kFramework.IsDoing())
                        {
                            g_kFramework.Update();
                            Thread.Sleep(10);
                        }
                    }
                }
                catch (Exception e)
                {
                    Backtrace(e);
                }
                finally
                {
                    g_kFramework.Release();
                }
            }
            else
            {
                // Windows는 Service로 인스톨 할수 있게 작성하기 바랍니다.
                // http://www.codeproject.com/Articles/3990/Simple-Windows-Service-Sample
                //
                // Linux는 Mono.UNIX와 Shell Script를 활용 하시기 바랍니다.
                // http://stackoverflow.com/questions/1221110/windows-like-services-development-in-linux-using-mono
                //

                Action("process mode: daemon", true);

                try
                {
                    if (g_kFramework.Initialize(ConvertToBytes(szConfigPath), ConvertToBytes(PACKAGE_NAME), iServerId))
                    {
                        while (g_kFramework.IsDoing())
                        {
                            g_kFramework.Update();
                            Thread.Sleep(10);
                        }
                    }
                }
                catch (Exception e)
                {
                    Backtrace(e);
                }
                finally
                {
                    g_kFramework.Release();
                }

                TRACE("child: exit: " + Process.GetCurrentProcess().Id);
            }
            //Environment.Exit(0);
        }
    }
}

/* EOF */
