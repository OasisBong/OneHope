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

			PROTOCOL_WEB_BEGIN = 300,
			WEB_CHECK,
			WEB_AUTHORIZE,
			WEB_QUIT,
			PROTOCOL_WEB_END,

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

		public enum SEND_TYPE {
			SEND_MASTER_MAIN = 0,
			SEND_MASTER_SUB,
			SEND_SLAVE_MAIN
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

		public enum STAGE_TYPE {
			STAGE_A = 0,
			STAGE_B,
			STAGE_C,
			STAGE_D,
			STAGE_E,
			STAGE_NONE,
			STAGE_MAX = STAGE_NONE
		}

		public enum STATE_TYPE {
			STATE_EMPTY = 0,
			STATE_LOGIN,
			STATE_CHANNEL,
			STATE_LOBBY,
			STATE_ROOM,
            STATE_GAME,
			STATE_NONE,
			STATE_MAX = STATE_NONE
		}

		public enum SCENE_TYPE {
			SCENE_MENU = 0,
			SCENE_AUTO_CONNECT,
			SCENE_CHAT,
			SCENE_ROOM,
			SCENE_WEB,
            SCENE_MAIN_GAME,
			SCENE_NONE,
			SCENE_MAX = SCENE_NONE
		}
	}
}

/* EOF */
