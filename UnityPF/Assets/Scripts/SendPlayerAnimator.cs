using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPlayerAnimator : GameFramework
{
    Animator m_Ani;

    void Start()
    {
        m_Ani = GetComponent<Animator>();
    }

    void Update()
    {

    }

    public void Animator(ANIME_NUM index)
    {
        if(index == ANIME_NUM.STOP)
        {
            m_Ani.SetFloat("IsWalk", 0f);
        }
        else if(index == ANIME_NUM.WALK_F)
        {
            m_Ani.SetFloat("IsWalk", 1f);
        }
        else if(index == ANIME_NUM.WALK_B)
        {
            m_Ani.SetFloat("IsWalk", -1f);
        }
    }
}
