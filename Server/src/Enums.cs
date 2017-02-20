/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;

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
		public enum PROTOCOL {
			PROTOCOL_NULL = 0,

			PROTOCOL_COMMON_BEGIN = 100,
			ID_AUTHORIZE,
			ID_PING,
			ID_PONG,
			ID_QUIT,

			INFO_USER_LIST,
			INFO_NOTIFY,
			INFO_SERVER,
			INFO_CHANNEL,
			INFO_OTHER,

			ROOM_CREATE,
			ROOM_JOIN,
			ROOM_LEAVE,
			ROOM_START,
			ROOM_STOP,
			ROOM_INFO,
			ROOM_LIST,

			ROOM_JOIN_OTHER,
			ROOM_LEAVE_OTHER,
			ROOM_START_OTHER,
			ROOM_STOP_OTHER,

			USER_STATUS,
			USER_CHAT,
            USER_MOVE,
            USER_CREATE_OBJ,

            OTHER_STATUS,
			OTHER_CHAT,
			PROTOCOL_COMMON_END,

			PROTOCOL_CLIENT_BEGIN = 200,
			PROTOCOL_CLIENT_END,

			PROTOCOL_MAX
		}

		public enum EXTRA {
			// Default
			NONE = 0,
			OK,
			FAIL,

			// Fail
			BUSY,
			EMPTY,
			FULL,
			BLOCK,
			TIMEOUT,
			DATA_ERROR,
			CANT_DO,
			CANT_DO_ANYTHING,
			DONT_EXIST,
			NOT_FOUND,
			NOT_ENOUGH,
			NO_PERMISSION,
			OUT_OF_BOUND,
			OUT_OF_CONDITION,

			// Pair
			OPEN,
			CLOSE,

			REQUEST,
			RESPONSE,

			AGREE,
			DISAGREE,

			ACCEPT,
			CANCEL,

			ALLOW,
			DENY,

			START,
			STOP,

			BEGIN,
			END,

			LOCK,
			UNLOCK,

			IN,
			OUT,

			ADD,
			DEL,

			// Etc
			MODIFY,
			DONE,
			CHECK,
			CHANGE,
			NEW,
			APPEND,
			ATTACH,

			MAX_EXTRA
		}

		public enum STATUS_TYPE {
			STATUS_EXIT = 0,
			STATUS_WAITING,
			STATUS_READY,
			STATUS_NORMAL,
			STATUS_DYING,
		}

		public enum CHAT_TYPE {
			CHAT_NORMAL = 0,
			CHAT_SYSTEM,
			CHAT_CHEAT,
			CHAT_MAX
		}

		public enum NOTICE_TYPE {
			NOTICE_NONE = 0,
			NOTICE_SERVER
		}

		public enum UNIT_TYPE {
			UNIT_SELF = 0,
			UNIT_PLAYER,
			UNIT_NPC,
			UNIT_MAX
		}

		public enum LOG_EVENT_TYPE {
			// 서버 시작
			LE_SERVER_START = 0,

			// 서버 정지
			LE_SERVER_STOP = 1,

			// 핵
			// LF_AID,[account id],LF_DEBUG,[line number],LF_STRING,[message]
			LE_HACK = 2,

			// 서버 버그
			// LF_AID,[account id],LF_DEBUG,[line number],LF_STRING,[message]
			LE_BUG = 3,

			// 서버 접속 성공
			// LF_AID,[account id],LF_LOGIN_ID,[login id]
			LE_CLIENT_IN = 6,

			// 서버 접속 종료
			// LF_AID,[account id],LF_LOGIN_ID,[login id]
			LE_CLIENT_OUT = 7,

			// 채팅
			// LF_AID,[account id],LF_LOGIN_ID,[login id],LF_CHANNEL_ID,[channel id],LF_STRING,[message]
			LE_CHAT_CHANNEL = 10,

			// 방 생성
			// LF_AID,[account id],LF_ROOM_ID,[room id],LF_STAGE_ID,[stage id],LF_STRING,[message]
			LE_ROOM_CREATE = 26,

			// 방 참여
			// LF_AID,[account id],LF_ROOM_ID,[room id],LF_STAGE_ID,[stage id],LF_STRING,[message]
			LE_ROOM_JOIN = 27,
		}

		public enum LOG_FIELD_TYPE {
			LF_NONE = 0,				// NONE
			LF_DEBUG = 1,				// Line number
			LF_SUCCESS = 3,				// NONE
			LF_FAIL = 4,				// NONE
			LF_TIME = 5,				// UTC(sec)
			LF_STRING = 6,				// Strings
			LF_AID = 7,					// Account id (Actor)
			LF_AID_TARGET = 8,			// Account id (Target)
			LF_NAME = 9,				// Account name
			LF_CHANNEL_ID = 10,			// Channel id
			LF_LOGIN_ID = 13,			// Login id
			LF_COUNT = 16,				// Count
			LF_ROOM_ID = 22,			// Room number
			LF_STAGE_ID = 23,			// Stage number
		}
	}
}

/* EOF */
