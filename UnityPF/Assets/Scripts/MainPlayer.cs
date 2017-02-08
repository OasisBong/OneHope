using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : GameFramework
{
    public void Awake()
    {
        g_kFramework.Initialize();
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if(this.tag == "MainPlayer")
        {
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.Translate(Vector3.forward);
                SEND_USER_MOVE(g_kUnitMgr.GetMainPlayer().GetKey());
            }
        }
    }
}