using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : GameFramework {

    public GameObject Playerobj;

    void Start()
    {
        if(g_kStateMgr.GetTransitionType() != STATE_TYPE.STATE_ROOM)
        {
            g_kStateMgr.SetTransition(STATE_TYPE.STATE_ROOM);
        }
        uint count = g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetTopCount();

        for(uint i = 0; i < count; i++)
        {
            if(g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetMember(i).GetKey() == g_kUnitMgr.GetMainPlayer().GetKey())
            {
                Playerobj.tag = "MainPlayer";
                g_kUnitMgr.GetMainPlayer().inner.SetGameObject(Instantiate(Playerobj));
            }
            else
            {
                Playerobj.tag = "Player";
                g_kUnitMgr.GetPlayer(g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetMember(i).GetKey()).inner.SetGameObject(Instantiate(Playerobj));
            }
        }
    }
	
	void Update () {
        g_kStateMgr.Update();
	}
}
