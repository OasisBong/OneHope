/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Collections.Generic;
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
		public class CConfigEx : CConfig {
			public CConfigEx() {}
			~CConfigEx() {}

			public virtual bool
			Initialize(MemoryStream kStream_) {
				SetMaxConnection(iMAX_ROOM_MEMBERS);

				if(isptr(kStream_)) {
					SetPackageName(ConvertToBytes("example"));

					if(Load(kStream_)) {
						m_bInitialized = true;
					} else {
						m_bInitialized = false;
					}
				} else {
					m_bInitialized = false;
				}

				return m_bInitialized;
			}

			public override bool
			Release() {
				SetMaxConnection(0);

				m_bInitialized = false;

				return true;
			}

			public virtual bool
			Load(MemoryStream kStream_) {
				bool bLoadingCheck = true;

				StreamReader kReader = new StreamReader(kStream_);

				CHAR[] szKey = null;
				CHAR[] szValue = null;

				string[] szToken = null;
				bool bCheck = false;

				string szBuffer = "";
				while((szBuffer = kReader.ReadLine()) != null) {
					if((0 < szBuffer.Length) && (szBuffer.ToCharArray()[0] != '#') && (szBuffer.ToCharArray()[0] != ' ') && (szBuffer.ToCharArray()[0] != '\0') && (szBuffer.ToCharArray()[0] != '\n') && (szBuffer.ToCharArray()[0] != '\r') && (szBuffer.ToCharArray()[0] != '\t')) {
						if(false == bCheck) {
							if((CHAR)'[' == szBuffer.ToCharArray()[0]) {
								szKey = ConvertToBytes(szBuffer.Replace("[", "").Replace("]", "").Trim());
								if(0 == String.Compare(ConvertToString(szKey), ConvertToString(m_szPackageName))) {
									bCheck = true;
									TRACE("find: key: " + ConvertToString(szKey));
								}
							}
						} else {
							if((CHAR)'[' == szBuffer.ToCharArray()[0]) {
								return true;
							}
							bool bRowCheck = false;
							szToken = szBuffer.Split(new char[] {'=', '\t'});
							if(isptr(szKey)) {
								Array.Clear(szKey, 0, szKey.Length);
							}
							if(isptr(szValue)) {
								Array.Clear(szValue, 0, szValue.Length);
							}
							if(isptr(szToken)) {
								if(0 < szToken.Length) {
									string szClear = szToken[0];
									//szClear = szClear.Replace('\"', "");
									szClear = szClear.Replace("\n", "");
									szClear = szClear.Replace("\r", "");
									szClear = szClear.Trim();
									if(0 < szClear.Length) {
										szKey = ConvertToBytes(szClear);
										bRowCheck = true;
									}
								}

								if(bRowCheck) {
									bRowCheck = false;
									for(INT i = 1; i < szToken.Length; ++i) {
										string szClear = szToken[i];
										//szClear = szClear.Replace("\"", "");
										szClear = szClear.Replace("\n", "");
										szClear = szClear.Replace("\r", "");
										szClear = szClear.Trim();
										if(0 < szClear.Length) {
											szValue = ConvertToBytes(szClear);
											bRowCheck = true;
											break;
										}
									}
								}

								//TRACE("key: " + ConvertToString(szKey) + ", value: " + ConvertToString(szValue));

								if(bRowCheck) {
									if(0 == String.Compare(ConvertToString(szKey), KEY_HEADER_CRYPT)) {
										if((0 == String.Compare(ConvertToString(szValue), "true")) || (0 == String.Compare(ConvertToString(szValue), "True")) || (0 == String.Compare(ConvertToString(szValue), "TRUE"))) {
											SetHeaderCrypt(true);
										} else {
											SetHeaderCrypt(false);
										}
									} else {
										MESSAGE("[" + ConvertToString(szKey) + "] error, option is not valid");
										bLoadingCheck = false;
									}
								}
							}
						}
					}
				}
				kReader.Close();

				return bLoadingCheck;
			}
		}
	}
}

/* EOF */
