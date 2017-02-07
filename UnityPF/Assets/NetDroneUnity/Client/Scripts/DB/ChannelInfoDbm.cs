/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Text;
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
		public static CChannelInfoDbm	g_kChannelInfoDB		{ get { return CChannelInfoDbm.GetInstance(); } }

		public class CChannelInfoDbm : CSingleton<CChannelInfoDbm> {
			public CChannelInfoDbm() {}
			~CChannelInfoDbm() {}

			public virtual bool
			Load() {
				TextAsset kTextAsset = (TextAsset)(Resources.Load("Binary/channelinfo"));

				using(MemoryStream kStream = new MemoryStream(kTextAsset.bytes)) {
					// csv는 확장자를 txt로 resources 경로 아래에 저장해야 합니다.
					// csv는 LoadCSV로 읽고 bytes는 LoadBytes로 읽음.
					if(false == m_kDbm.LoadBytes(kStream)) {
						TRACE("channelinfo file was not found: ");
						return false;
					}
				}

				bool bCheck = true;

				g_kChannelInfoList.Clear();

				INT iIdOffset = m_kDbm.GetField("ci_id").GetOffset();
				INT iIndexOffset = m_kDbm.GetField("ci_index").GetOffset();
				INT iNameOffset = m_kDbm.GetField("ci_name").GetOffset();

				INT iIdSize = m_kDbm.GetField("ci_id").GetSize();
				INT iIndexSize = m_kDbm.GetField("ci_index").GetSize();
				INT iNameSize = m_kDbm.GetField("ci_name").GetSize();

				for(INT i = 0; i < m_kDbm.GetDataCount(); ++i) {
					// CSV의 경우 필드 타입을 구할수 없어 모두 string 처리를 하고있지만,
					// DB에서 직접 바이너리로 저장할 경우 필드 타입의 Size 만큼만 Byte로 저장할수 있습니다.
					// 이렇게 저장할 경우 해당 바이트들은 BitConverter로 읽어와야 합니다.
					// phpMyAdmin 용 Export(Binary) Plugin을 제작해 둔것이 있어 예제가 필요할경우 요청하세요.

					//UINT uiId = BitConverter.ToUInt32(m_kDbm.GetData(i, iIdOffset, iIdSize), 0);
					UINT uiId = Convert.ToUInt32(ConvertToString(m_kDbm.GetData(i, iIdOffset, iIdSize)));

					//UINT uiIndex = BitConverter.ToUInt32(m_kDbm.GetData(i, iIndexOffset, iIndexSize), 0);
					UINT uiIndex = Convert.ToUInt32(ConvertToString(m_kDbm.GetData(i, iIndexOffset, iIndexSize)));

					string szName = ConvertToString(m_kDbm.GetData(i, iNameOffset, iNameSize));

					TRACE(
						"id: " + uiId +
						", index: " + uiIndex +
						", name: " + szName
					);

					SChannelInfo tInfo = new SChannelInfo(true);
					tInfo.id = uiId;
					tInfo.index = uiIndex;
					tInfo.SetName(szName);

					g_kChannelInfoList.Insert(tInfo);
				}

				m_kDbm.Clear();

				return bCheck;
			}

			private CDBM	m_kDbm = new CDBM();
		}
	}
}

/* EOF */
