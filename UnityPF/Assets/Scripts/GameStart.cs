using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : GameFramework {

    public GameObject MainPlayerObj;
    public GameObject PlayerObj;

    void Start()
    {
        if(g_kStateMgr.GetTransitionType() != STATE_TYPE.STATE_GAME)
        {
            g_kStateMgr.SetTransition(STATE_TYPE.STATE_GAME);
        }

        MainPlayerObj.tag = "MainPlayer";
        g_kUnitMgr.GetMainPlayer().inner.SetGameObject(MainPlayerObj);

        uint count = g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetTopCount();

        for(uint i = 0; i < count; i++)
        {
            if(g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetMember(i).GetKey() == g_kUnitMgr.GetMainPlayer().GetKey())
            {
                continue;
            }
            else
            {
                PlayerObj.tag = "Player";
                g_kUnitMgr.GetPlayer(g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetMember(i).GetKey()).inner.SetGameObject(Instantiate(PlayerObj));
            }
        }
    }
	
	void Update () {
        g_kStateMgr.Update();
	}
}
