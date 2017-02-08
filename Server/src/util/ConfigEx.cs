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

			public override bool
			Initialize(CHAR[] szConfigPath_, CHAR[] szPackageName_, INT iServerId_) {
				bool bCheck = false;

				if(0 == iServerId_) {
					// config 파일로 로딩하지 않을경우를 위해 서버 기본값을 설정합니다.
					string szServerAddr = "0.0.0.0";

					SetTcpDefaultAddress(ConvertToBytes(szServerAddr), 11000);
					SetTcpBackboneAddress(ConvertToBytes(szServerAddr), 11001);
					SetUdpReliableAddress(ConvertToBytes(szServerAddr), 11000);

					string szLogPath = "";
					string szInfoPath = "";

					OperatingSystem kInfo = Environment.OSVersion;
					switch(kInfo.Platform) {
					case PlatformID.Win32NT:
					case PlatformID.Win32Windows:
					case PlatformID.Win32S:
					case PlatformID.WinCE:
						{
							szLogPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "tmp";
							szInfoPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "tmp";
							if(false == Directory.Exists(szLogPath)) {
								Directory.CreateDirectory(szLogPath);
							}

							if(false == Directory.Exists(szInfoPath)) {
								Directory.CreateDirectory(szInfoPath);
							}
						}
						break;
					default:
						{
							szLogPath = Path.DirectorySeparatorChar + "tmp";
							szInfoPath = Path.DirectorySeparatorChar + "tmp";
						}
						break;
					}
					
					SetLogPath(ConvertToBytes(szLogPath));
					SetInfoPath(ConvertToBytes(szInfoPath));

					// 최대 연결가능 수는 /etc/security/limits.conf 에서 아래와 같이 조정이 가능합니다.
					//
					// *                soft    nofile          65535
					// *                hard    nofile          65535
					// root             soft    nofile          65535
					// root             hard    nofile          65535
					//
					// 이렇게 수정한뒤 원하는 최대 연결수를 unsigned short 범위안에서 입력하세요.
					SetMaxConnection(1000);

					SetHeaderCrypt(true);

					if(base.Initialize(szPackageName_, iServerId_)) {
						bCheck = true;
					}
				} else {
					if(base.Initialize(szConfigPath_, szPackageName_, iServerId_)) {
						bCheck = true;
					}

					TRACE("server id: " + GetServerId());
					TRACE("tcp default address: " + ConvertToString(GetTcpDefaultAddress()));
					TRACE("tcp default port: " + GetTcpDefaultPort());
					TRACE("udp reliable address: " + ConvertToString(GetUdpReliableAddress()));
					TRACE("udp reliable port: " + GetUdpReliablePort());
					TRACE("tcp backbone address: " + ConvertToString(GetTcpBackboneAddress()));
					TRACE("tcp backbone port: " + GetTcpBackbonePort());
					TRACE("max connection: " + GetMaxConnection());
					TRACE("header crypt: " + IsHeaderCrypt());
					TRACE("log path: " + ConvertToString(GetLogPath()));
					TRACE("info path: " + ConvertToString(GetInfoPath()));
				}

				return bCheck;
			}

			public override bool
			Release() {
				base.Release();
				return true;
			}

			public override bool
			Load(CHAR[] szPath_) {
				bool bLoadingCheck = true;

				string szFilePath = ConvertToString(szPath_);

				if(File.Exists(szFilePath)) {
					StreamReader kReader = new StreamReader(szFilePath);

					CHAR[] szKey = null;
					CHAR[] szValue = null;

					string[] szToken = null;
					bool bCheck = false;

					string szBuffer = "";
					while((szBuffer = kReader.ReadLine()) != null) {
						if((0 < szBuffer.Length) && (szBuffer.ToCharArray()[0] != '#') && (szBuffer.ToCharArray()[0] != ' ') && (szBuffer.ToCharArray()[0] != '\0') && (szBuffer.ToCharArray()[0] != '\n') && (szBuffer.ToCharArray()[0] != '\r') && (szBuffer.ToCharArray()[0] != '\t')) {
							if(false == bCheck) {
								if((CHAR)'[' == szBuffer.ToCharArray()[0]) {
									szToken = szBuffer.Split(':');
									if(isptr(szToken)) {
										if(0 < szToken.Length) {
											szKey = ConvertToBytes(szToken[0].Replace("[", "").Trim());
										}

										if(isptr(szKey)) {
											if(0 == String.Compare(ConvertToString(szKey), ConvertToString(m_szPackageName))) {
												if(1 < szToken.Length) {
													INT iServerId = Convert.ToInt32(szToken[1].Replace("]", "").Trim());
													if(GetServerId() == iServerId) {
														bCheck = true;
														TRACE("find: key: " + ConvertToString(szKey) + ", server id: " + iServerId);
													}
												}
											}
										}
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
										if(0 == String.Compare(ConvertToString(szKey), KEY_DEFAULT_INTERFACE)) {
											if(false == HostAddress(ref m_saTcpDefaultAddr_in, szValue)) {
												OUTPUT("[" + ConvertToString(szKey) + "] error, this is not a ethernet interface name or somthing unexpected occurred: [" + ConvertToString(szValue) + "]");
												bLoadingCheck = false;
											}
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_DEFAULT_PORT)) {
											m_saTcpDefaultAddr_in.Port = Convert.ToInt32(ConvertToString(szValue));
											if(0 == m_saTcpDefaultAddr_in.Port) {
												OUTPUT("[" + ConvertToString(szKey) + "] error, port number is wrong or something unexpected occurred: [ " + ConvertToString(szValue) + "]");
												bLoadingCheck = false;
											}
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_BACKBONE_INTERFACE)) {
											if(false == HostAddress(ref m_saTcpBackboneAddr_in, szValue)) {
												OUTPUT("[" + ConvertToString(szKey) + "] error, this is not a ethernet interface name or somthing unexpected occurred: [" + ConvertToString(szValue) + "]");
												bLoadingCheck = false;
											}
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_BACKBONE_PORT)) {
											m_saTcpBackboneAddr_in.Port = Convert.ToInt32(ConvertToString(szValue));
											if(0 == m_saTcpBackboneAddr_in.Port) {
												OUTPUT("[" + ConvertToString(szKey) + "] error, port number is wrong or something unexpected occurred: [ " + ConvertToString(szValue) + "]");
												bLoadingCheck = false;
											}
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_RELIABLE_INTERFACE)) {
											if(false == HostAddress(ref m_saUdpReliableAddr_in, szValue)) {
												OUTPUT("[" + ConvertToString(szKey) + "] error, this is not a ethernet interface name or somthing unexpected occurred: [" + ConvertToString(szValue) + "]");
												bLoadingCheck = false;
											}
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_RELIABLE_PORT)) {
											m_saUdpReliableAddr_in.Port = Convert.ToInt32(ConvertToString(szValue));
											if(0 == m_saUdpReliableAddr_in.Port) {
												OUTPUT("[" + ConvertToString(szKey) + "] error, port number is wrong or something unexpected occurred: [ " + ConvertToString(szValue) + "]");
												bLoadingCheck = false;
											}
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_MAX_CONNECTION)) {
											SetMaxConnection(Convert.ToInt32(ConvertToString(szValue)));
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_HEADER_CRYPT)) {
											if((0 == String.Compare(ConvertToString(szValue), "true")) || (0 == String.Compare(ConvertToString(szValue), "True")) || (0 == String.Compare(ConvertToString(szValue), "TRUE"))) {
												SetHeaderCrypt(true);
											} else {
												SetHeaderCrypt(false);
											}
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_LOG_PATH)) {
											SetLogPath(szValue);
										} else if(0 == String.Compare(ConvertToString(szKey), KEY_INFO_PATH)) {
											SetInfoPath(szValue);
										} else {
											OUTPUT("[" + ConvertToString(szKey) + "] error, option is not valid");
											bLoadingCheck = false;
										}
									}
								}
							}
						}
					}
					kReader.Close();
				} else {
					OUTPUT("[" + szFilePath + "] was not found");
					return false;
				}

				return bLoadingCheck;
			}
		}
	}
}

/* EOF */
