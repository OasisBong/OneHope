using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateManager : GameFramework {

    public CreatePrefabs player;

    public GameObject CampFire;
    public Material CampfireMtl;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateCampfire()
    {
        Debug.Log("크리에이트 캠프파이어");
        player.OnObjDecide(CampFire, CampfireMtl);
    }
}
