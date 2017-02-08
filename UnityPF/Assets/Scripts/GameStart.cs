using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : GameFramework {

    public GameObject Playerobj;

    void Start()
    {
        uint count = g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetTopCount();

        for(uint i = 0; i < count; i++)
        {
            if(g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetMember(i).GetKey() == g_kUnitMgr.GetMainPlayer().GetKey())
            {
                Playerobj.tag = "MainPlayer";
                g_kUnitMgr.GetMainPlayer().SetGameObject(Instantiate(Playerobj));
            }
            else
            {
                Playerobj.tag = "Player";
                g_kUnitMgr.GetPlayer(g_kUnitMgr.GetMainPlayer().GetRoomHandler().GetRoom().GetMember(i).GetKey()).SetGameObject(Instantiate(Playerobj));
            }
        }
    }
	
	void Update () {
		
	}
}
