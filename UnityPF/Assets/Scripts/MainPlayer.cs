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
        if (this.tag != "MainPlayer") return;

        Move();
    }

    public void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //Player_Forward();
            SEND_USER_MOVE(g_kUnitMgr.GetMainPlayer().GetKey());
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //Player_Back();
            SEND_USER_MOVE(g_kUnitMgr.GetMainPlayer().GetKey());
        }

        if (Input.GetKey(KeyCode.A))
        {
            //Player_Right();
            SEND_USER_MOVE(g_kUnitMgr.GetMainPlayer().GetKey());
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //Player_Left();
            SEND_USER_MOVE(g_kUnitMgr.GetMainPlayer().GetKey());
        }
        
    }

    public void Player_Forward()
    {
        this.transform.Translate(Vector3.forward);
    }

    public void Player_Back()
    {
        this.transform.Translate(Vector3.back);
    }

    public void Player_Right()
    {
        this.transform.Translate(Vector3.left);
    }

    public void Player_Left()
    {
        this.transform.Translate(Vector3.right);
    }
}