using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCreateobj : GameFramework
{
    Dictionary<string, GameObject> m_CreateObj = new Dictionary<string, GameObject>();

    public GameObject CampFire;

    void Awake()
    {
        byte[] temp = System.Text.Encoding.UTF8.GetBytes(CampFire.name);
        string temp2 = System.Text.Encoding.UTF8.GetString(temp);
        m_CreateObj.Add(CampFire.name, CampFire);
    }

    void Update()
    {

    }

    public void Createobj(string name, Vector3 Pos)
    {
        GameObject temps;

        byte[] tempbyte = System.Text.Encoding.UTF8.GetBytes(name.Trim('\0'));
        if(m_CreateObj.TryGetValue(System.Text.Encoding.Default.GetString(tempbyte), out temps))
        {
            GameObject temp = m_CreateObj[name];
            temp.transform.position = Pos;
            Instantiate(temp);
        }
        else
        {
            Debug.Log("키값에 의한 오브젝트를 참조하지 못하였습니다. Key: " + name);
        }
    }
}
