/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Net;

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
		public static CCfgMgr	g_kCfgMgr { get { return CCfgMgr.GetInstance(); } }

		public class CCfgMgr : CSingleton< CCfgMgr > {
			public CCfgMgr() {}
			~CCfgMgr() { Release(); }

			public bool
			Initialize(CHAR[] szConfigPath_, CHAR[] szPackageName_, INT iServerId_) {
				return inner.Initialize(szConfigPath_, szPackageName_, iServerId_);
			}

			public bool
			Release() {
				return inner.Release();
			}

			public bool			Load(CHAR[] szPath_)		{ return inner.Load(szPath_); }

			public bool			Create()					{ return inner.Create(); }
			public bool			Delete()					{ return inner.Delete(); }

			public bool
			SetTcpDefaultAddress(CHAR[] szAddress_, UINT16 usPort_) {
				return inner.SetTcpDefaultAddress(szAddress_, usPort_);
			}

			public CHAR[]		GetTcpDefaultAddress()		{ return inner.GetTcpDefaultAddress(); }
			public ULONG		GetTcpDefaultSinAddress()	{ return inner.GetTcpDefaultSinAddress(); }
			public IPEndPoint	GetTcpDefaultAddressIn()	{ return inner.GetTcpDefaultAddressIn(); }
			public INT			GetTcpDefaultPort()			{ return inner.GetTcpDefaultPort(); }

			public bool
			SetTcpBackboneAddress(CHAR[] szAddress_, UINT16 usPort_) {
				return inner.SetTcpBackboneAddress(szAddress_, usPort_);
			}

			public CHAR[]		GetTcpBackboneAddress()		{ return inner.GetTcpBackboneAddress(); }
			public ULONG		GetTcpBackboneSinAddress()	{ return inner.GetTcpBackboneSinAddress(); }
			public IPEndPoint	GetTcpBackboneAddressIn()	{ return inner.GetTcpBackboneAddressIn(); }
			public INT			GetTcpBackbonePort()		{ return inner.GetTcpBackbonePort(); }

			public bool
			SetUdpReliableAddress(CHAR[] szAddr_, UINT16 usPort_) {
				return inner.SetUdpReliableAddress(szAddr_, usPort_);
			}

			public CHAR[]		GetUdpReliableAddress()		{ return inner.GetUdpReliableAddress(); }
			public ULONG		GetUdpReliableSinAddress()	{ return inner.GetUdpReliableSinAddress(); }
			public IPEndPoint	GetUdpReliableAddressIn()	{ return inner.GetUdpReliableAddressIn(); }
			public INT			GetUdpReliablePort()		{ return inner.GetUdpReliablePort(); }

			public INT			GetServerId()				{ return inner.GetServerId(); }
			public void			SetServerId(INT o)			{ inner.SetServerId(o); }

			public INT			GetMaxConnection()			{ return inner.GetMaxConnection(); }
			public void			SetMaxConnection(INT o)		{ inner.SetMaxConnection(o); }

			public bool			IsHeaderCrypt()				{ return inner.IsHeaderCrypt(); }
			public void			SetHeaderCrypt(bool o)		{ inner.SetHeaderCrypt(o); }

			public CHAR[]		GetPackageName()			{ return inner.GetPackageName(); }
			public void			SetPackageName(CHAR[] o)	{ inner.SetPackageName(o); }

			public CHAR[]		GetLogPath()				{ return inner.GetLogPath(); }
			public void			SetLogPath(CHAR[] o)		{ inner.SetLogPath(o); }

			public CHAR[]		GetInfoPath()				{ return inner.GetInfoPath(); }
			public void			SetInfoPath(CHAR[] o)		{ inner.SetInfoPath(o); }

			public INT			GetGPid()					{ return inner.GetGPid(); }
			public INT			GetPPid()					{ return inner.GetPPid(); }
			public INT			GetPid()					{ return inner.GetPid(); }

			private CConfigEx	inner = new CConfigEx();
		}
	}
}

/* EOF */
