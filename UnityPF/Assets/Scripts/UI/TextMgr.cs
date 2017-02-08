/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
using System.Text.RegularExpressions;

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
		public static CTextMgr	g_kTextMgr	{ get { return CTextMgr.GetInstance(); } }

		public class CTextMgr : CSingleton<CTextMgr> {
			public CTextMgr() {}
			~CTextMgr() {}

			public void
			Clear() {
				m_kChannelTransform = null;
				m_kUserListTransform = null;
				m_kRoomListTransform = null;
				m_kMemberTransform = null;
				m_kMessageTransform = null;
			}

			public Transform	GetChannelTransform()				{ return m_kChannelTransform; }
			public void			SetChannelTransform(Transform o)	{ m_kChannelTransform = o; }

			public Transform	GetUserListTransform()				{ return m_kUserListTransform; }
			public void			SetUserListTransform(Transform o)	{ m_kUserListTransform = o; }

			public Transform	GetRoomListTransform()				{ return m_kRoomListTransform; }
			public void			SetRoomListTransform(Transform o)	{ m_kRoomListTransform = o; }

			public Transform	GetMemberTransform()				{ return m_kMemberTransform; }
			public void			SetMemberTransform(Transform o)		{ m_kMemberTransform = o; }

			public Transform	GetMessageTransform()				{ return m_kMessageTransform; }
			public void			SetMessageTransform(Transform o)	{ m_kMessageTransform = o; }

			private Transform	m_kChannelTransform = null;
			private Transform	m_kUserListTransform = null;
			private Transform	m_kRoomListTransform = null;
			private Transform	m_kMemberTransform = null;
			private Transform	m_kMessageTransform = null;
		}
	}
}

/* EOF */
