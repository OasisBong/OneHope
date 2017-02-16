/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
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

	public partial class GameFramework {
		public static CStageInfoList		g_kStageInfoList		{ get { return CStageInfoList.GetInstance(); } }
		public static CChannelInfoList		g_kChannelInfoList		{ get { return CChannelInfoList.GetInstance(); } }

		public class CStageInfoList : CSingleton<CStageInfoList> {
			public CStageInfoList() {}
			~CStageInfoList() {}

			public bool			Initialize()				{ return true; }
			public bool			Release()					{ Clear(); m_kBox = null; return true; }

			public SStageInfo	Find(UINT o)				{ if(false == Empty()) { if(0 < o) { if(m_kBox.ContainsKey(o)) { return m_kBox[o]; } } } return (new SStageInfo(true)); }
			public bool			Insert(SStageInfo o)		{ m_kBox.Add(o.GetId(), o); return true; }
			public void			Clear()						{ m_kBox.Clear(); }
			public bool			Empty()						{ return (0 >= m_kBox.Count); }

			private Dictionary<UINT, SStageInfo>	m_kBox = new Dictionary<UINT, SStageInfo>();
		}

		public class CChannelInfoList : CSingleton<CChannelInfoList> {
			public CChannelInfoList() {}
			~CChannelInfoList() {}

			public bool				Initialize()			{ return true; }
			public bool				Release()				{ Clear(); m_kBox = null; return true; }

			public SChannelInfo
			Find(UINT uiId_) {
				if(false == Empty()) {
					foreach(SChannelInfo tInfo in m_kBox) {
						if(tInfo.GetId() == uiId_) {
							return tInfo;
						}
					}
				}
				return (new SChannelInfo(true));
			}

			public SChannelInfo
			Seek(UINT uiIndex_) {
				if(false == Empty()) {
					if((INT)(uiIndex_) < m_kBox.Count) {
						return m_kBox[(INT)uiIndex_];
					}

//					UINT uiCount = 0;
//					foreach(SChannelInfo tInfo in m_kBox) {
//						if(uiCount == uiIndex_) {
//							return tInfo;
//						}
//						++uiCount;
//					}
				}
				return (new SChannelInfo(true));
			}

			public bool				Insert(SChannelInfo o)	{ m_kBox.Add(o); return true; }
			public void				Clear()					{ m_kBox.Clear(); }

			public size_t			Size()					{ return (size_t)(m_kBox.Count); }
			public bool				Empty()					{ return (0 >= m_kBox.Count); }

			private List<SChannelInfo>	m_kBox = new List<SChannelInfo>();
		}
	}
}

/* EOF */
