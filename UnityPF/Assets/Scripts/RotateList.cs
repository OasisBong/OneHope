using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateList : MonoBehaviour {

    enum MovingState
    {
        Stand,
        Left,
        Right,

    }
    enum BuildListIndex
    {
        CampFire,
    }

    MovingState m_state;
    float m_Radius = 3f;
    float tempff = 0f;
    int m_index = 0;
    public GameObject[] objList;
    public float m_onceRad { get; set; }

	// Use this for initialization
	void Start () {

        m_state = MovingState.Stand;

        m_onceRad = 360f / objList.Length;

        for(int i = 0; i < objList.Length;i++)
        {
            objList[i].transform.position += -this.gameObject.transform.forward * m_Radius;
            objList[i].transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, m_onceRad * i);
        }
        
    }
	
    
	// Update is called once per frame
	void Update () {

        ObjRotate();
        ObjSelect();

        if (Input.GetKeyDown(KeyCode.A) && m_state== MovingState.Stand)
        {
            m_state = MovingState.Left;
            m_index -= 1;
            if (m_index < 0) m_index = objList.Length - 1;
        }
        else if(Input.GetKeyDown(KeyCode.D) && m_state == MovingState.Stand)
        {
            m_state = MovingState.Right;
            m_index += 1;
            if (m_index >= objList.Length) m_index = 0;
        }
        
        if(m_state != MovingState.Stand)
        {
            tempff += m_onceRad / 10f;

            if(m_state == MovingState.Left)
                this.gameObject.transform.Rotate(Vector3.up, -m_onceRad / 10f);
            else
                this.gameObject.transform.Rotate(Vector3.up, m_onceRad / 10f);

            if (tempff >= m_onceRad)
            {
                tempff = 0f;
                m_state = MovingState.Stand;
            }
        }
	}

    void ObjRotate()
    {
        for (int i = 0; i < objList.Length; i++)
        {
            objList[i].transform.Rotate(Vector3.up, 1.0f);
        }
    }

    void ObjSelect()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            BuildListIndex index;
            index = (BuildListIndex)m_index ;

            switch (index)
            {
                case BuildListIndex.CampFire:

                    CreateManager.Instance.CreateCampfire();

                    break;
                default:
                    break;
            }

        }
    }
}
