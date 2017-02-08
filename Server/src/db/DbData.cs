/*
 * NetDrone Engine
 * Copyright © 2015-2016 Origin Studio Inc.
 *
 */

using System;
using System.Linq;

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
		public struct SChannelInfo {
			public SChannelInfo(bool o) : this() {
				if(o) {
					id = 0;
					max_user = 0;
					user = 0;
				}
			}

			public void
			Clear() {
				id = 0;
				max_user = 0;
				user = 0;
			}

			public UINT		GetId()				{ return id; }
			public void		SetId(UINT o)		{ id = o; }

			public UINT		GetMaxUser()		{ return max_user; }
			public void		SetMaxUser(UINT o)	{ max_user = o; }

			public UINT		GetUser()			{ return user; }
			public void		SetUser(UINT o)		{ user = o; }

			public void		IncreasedUser()		{ ++user; }
			public void		DecreasedUser()		{ if(0 < user) --user; else OUTPUT("critical error: user count: "); }

			public UINT		id;
			public UINT		max_user;
			public UINT		user;
		}

		public struct SServerInfo {
			public SServerInfo(bool o) : this() {
				if(o) {
					id = 0;
					relay = 0;
					max_user = 0;
					user = 0;
					max_channel = 0;

					channel_info =  (new SChannelInfo[iMAX_CHANNEL_LIST]).Select(x => new SChannelInfo(true)).ToArray();

					for(INT i = 0; i < channel_info.Length; ++i) {
						channel_info[i].Clear();
					}
				}
			}

			public void
			Clear() {
				id = 0;
				relay = 0;
				max_user = 0;
				user = 0;
				max_channel = 0;

				for(INT i = 0; i < channel_info.Length; ++i) {
					channel_info[i].Clear();
				}
			}

			public UINT				GetId()					{ return id; }
			public void				SetId(UINT o)			{ id = o; }

			public UINT				GetRelay()				{ return relay; }
			public void				SetRelay(UINT o)		{ relay = o; }

			public UINT				GetMaxUser()			{ return max_user; }
			public void				SetMaxUser(UINT o)		{ max_user = o; }

			public UINT				GetUser()				{ return user; }
			public void				SetUser(UINT o)			{ user = o; }

			public UINT				GetMaxChannel()			{ return max_channel; }
			public void				SetMaxChannel(UINT o)	{ max_channel = o; }

			public void				IncreasedUser()			{ ++user; }
			public void				DecreasedUser()			{ if(0 < user) --user; else OUTPUT("critical error: user count: "); }

			public SChannelInfo[]	GetChannelInfo()		{ return channel_info; }
			public SChannelInfo		GetChannelInfo(UINT o)	{ return channel_info[(INT)o]; }

			public void
			SetId(UINT o, UINT p) {
				if(isptr(channel_info)) {
					channel_info[o].SetId(p);
				}
			}

			public void
			SetMaxUser(UINT o, UINT p) {
				if(isptr(channel_info)) {
					channel_info[o].SetMaxUser(p);
				}
			}

			public void
			SetUser(UINT o, UINT p) {
				if(isptr(channel_info)) {
					channel_info[o].SetUser(p);
				}
			}

			public void
			IncreasedUser(UINT o) {
				if(isptr(channel_info)) {
					channel_info[o].IncreasedUser();
				}
			}

			public void
			DecreasedUser(UINT o) {
				if(isptr(channel_info)) {
					channel_info[o].DecreasedUser();
				}
			}

			public UINT id;
			public UINT	relay;
			public UINT	max_user;
			public UINT	user;
			public UINT	max_channel;

			public SChannelInfo[]	channel_info;
		}
	}
}

/* EOF */
