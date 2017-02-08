/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Net;
using System.IO;

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
			Initialize() {
				TextAsset kTextAsset = (TextAsset)(Resources.Load("Config/testclient-unity"));
				using(MemoryStream kStream = new MemoryStream(kTextAsset.bytes)) {
					return inner.Initialize(kStream);
				}
			}

			public bool
			Release() {
				m_bAutoConnected = false;
				m_eSendType = SEND_TYPE.SEND_MASTER_MAIN;
				m_szRequestUrl = "";

				return inner.Release();
			}

			public bool			Load(CHAR[] szPath_)		{ return inner.Load(szPath_); }

			public bool
			SetTcpDefaultAddress(CHAR[] szAddress_, UINT16 usPort_) {
				return inner.SetTcpDefaultAddress(szAddress_, usPort_);
			}

			public CHAR[]		GetTcpDefaultAddress()		{ return inner.GetTcpDefaultAddress(); }
			public ULONG		GetTcpDefaultSinAddress()	{ return inner.GetTcpDefaultSinAddress(); }
			public IPEndPoint	GetTcpDefaultAddressIn()	{ return inner.GetTcpDefaultAddressIn(); }
			public INT			GetTcpDefaultPort()			{ return inner.GetTcpDefaultPort(); }

			public bool
			SetUdpReliableAddress(CHAR[] szAddr_, UINT16 usPort_) {
				return inner.SetUdpReliableAddress(szAddr_, usPort_);
			}

			public CHAR[]		GetUdpReliableAddress()		{ return inner.GetUdpReliableAddress(); }
			public ULONG		GetUdpReliableSinAddress()	{ return inner.GetUdpReliableSinAddress(); }
			public IPEndPoint	GetUdpReliableAddressIn()	{ return inner.GetUdpReliableAddressIn(); }
			public INT			GetUdpReliablePort()		{ return inner.GetUdpReliablePort(); }

			public INT			GetMaxConnection()			{ return inner.GetMaxConnection(); }
			public void			SetMaxConnection(INT o)		{ inner.SetMaxConnection(o); }

			public void			SetAutoConnect(bool o)		{ m_bAutoConnected = o; }
			public bool			IsAutoConnected()			{ return m_bAutoConnected; }

			public void			SetHeaderCrypt(bool o)		{ inner.SetHeaderCrypt(o); }
			public bool			IsHeaderCrypt()				{ return inner.IsHeaderCrypt(); }

			public void			SetSendType(SEND_TYPE o)	{ m_eSendType = o; }
			public SEND_TYPE	GetSendType()				{ return m_eSendType; }

			public void			SetRequestUrl(string o)		{ m_szRequestUrl = o; }
			public string		GetRequestUrl()				{ return m_szRequestUrl; }

			private CConfigEx	inner = new CConfigEx();

			private bool		m_bAutoConnected = false;

			private SEND_TYPE	m_eSendType = SEND_TYPE.SEND_MASTER_MAIN;

			private string		m_szRequestUrl = "";
		}
	}
}

/* EOF */
