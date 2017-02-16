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
		public static CStageInfoDbm	g_kStageInfoDB		{ get { return CStageInfoDbm.GetInstance(); } }

		public class CStageInfoDbm : CSingleton<CStageInfoDbm> {
			public CStageInfoDbm() {}
			~CStageInfoDbm() {}

			public virtual bool
			Load() {
				TextAsset kTextAsset = (TextAsset)(Resources.Load("Text/stageinfo"));

				using(MemoryStream kStream = new MemoryStream(kTextAsset.bytes)) {
					// csv는 확장자를 txt로 resources 경로 아래에 저장해야 합니다.
					// csv는 LoadCSV로 읽고 bytes는 LoadBytes로 읽음.
					if(false == m_kDbm.LoadCSV(kStream)) {
						TRACE("stageinfo file was not found: ");
						return false;
					}
				}

				bool bCheck = true;

				g_kStageInfoList.Clear();

				INT iIdOffset = m_kDbm.GetField("si_id").GetOffset();
				INT iNameOffset = m_kDbm.GetField("si_name").GetOffset();
				INT iTypeOffset = m_kDbm.GetField("si_type").GetOffset();
				INT iDescOffset = m_kDbm.GetField("si_desc").GetOffset();

				INT iIdSize = m_kDbm.GetField("si_id").GetSize();
				INT iNameSize = m_kDbm.GetField("si_name").GetSize();
				INT iTypeSize = m_kDbm.GetField("si_type").GetSize();
				INT iDescSize = m_kDbm.GetField("si_desc").GetSize();

				for(INT i = 0; i < m_kDbm.GetDataCount(); ++i) {
					// CSV의 경우 필드 타입을 구할수 없어 모두 string 처리를 하고있지만,
					// DB에서 직접 바이너리로 저장할 경우 필드 타입의 Size 만큼만 Byte로 저장할수 있습니다.
					// 이렇게 저장할 경우 해당 바이트들은 BitConverter로 읽어와야 합니다.
					// phpMyAdmin 용 Export(Binary) Plugin을 제작해 둔것이 있어 예제가 필요할경우 요청하세요.

					//UINT uiId = BitConverter.ToUInt32(m_kDbm.GetData(i, iIdOffset, iIdSize), 0);
					UINT uiId = Convert.ToUInt32(ConvertToString(m_kDbm.GetData(i, iIdOffset, iIdSize)));

					string szName = ConvertToString(m_kDbm.GetData(i, iNameOffset, iNameSize));

					//STAGE_TYPE eType = (STAGE_TYPE)(BitConverter.ToUInt32(m_kDbm.GetData(i, iTypeOffset, iTypeSize), 0));
					STAGE_TYPE eType = STAGE_TYPE.STAGE_NONE;
					string szType = ConvertToString(m_kDbm.GetData (i, iTypeOffset, iTypeSize));
					switch(szType) {
					case "A":
						eType = STAGE_TYPE.STAGE_A;
						break;
					case "B":
						eType = STAGE_TYPE.STAGE_B;
						break;
					case "C":
						eType = STAGE_TYPE.STAGE_C;
						break;
					case "D":
						eType = STAGE_TYPE.STAGE_D;
						break;
					case "E":
						eType = STAGE_TYPE.STAGE_E;
						break;
					}

					string szDesc = ConvertToString(m_kDbm.GetData(i, iDescOffset, iDescSize));

					TRACE(
						"id: " + uiId +
						", name: " + szName +
						", type: " + eType +
						", desc: " + szDesc
					);

					SStageInfo tInfo = new SStageInfo(true);
					tInfo.id = uiId;
					tInfo.SetName(szName);
					tInfo.stage_type = eType;
					tInfo.SetDesc(szDesc);

					g_kStageInfoList.Insert(tInfo);
				}

				m_kDbm.Clear();

				return bCheck;
			}

			private CDBM	m_kDbm = new CDBM();
		}
	}
}

/* EOF */
