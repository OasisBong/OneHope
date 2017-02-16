using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveObjInfo : GameFramework {

    public string m_strName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnActive()
    {
        if (m_strName == "CampFire") CampFireActive();
    }


    void CampFireActive()
    {
        this.GetComponentInChildren<FireLightScript>().LightOn();
    }

    public bool CheckBuildPos()
    {
        return true;
    }
}
