using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : GameFramework {

    public GameObject MainPlayerObj;
    public GameObject PlayerObj;

    float deltaTime = 0.0f;
    GUIStyle style;
    Rect rect;
    float msec;
    float fps;
    float worstFps = 100f;
    string text;

    void Awake()
    {
        int w = Screen.width, h = Screen.height;
        rect = new Rect(0, 0, w, h * 4 / 100);
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = Color.cyan;
        StartCoroutine("worstReset");
    }

    IEnumerator worstReset()
    {
        while(true)
        {
            yield return new WaitForSeconds(15f);
            worstFps = 100f;
        }
    }

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

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

    void OnGUI()
    {
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        if(fps < worstFps)
        {
            worstFps = fps;
        }

        text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + ") //worst : " + worstFps.ToString("F1");
        GUI.Label(rect, text, style);
    }
}
