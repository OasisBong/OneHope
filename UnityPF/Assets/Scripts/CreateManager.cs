using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateManager : GameFramework {

    public CreatePrefabs player;

    public GameObject CampFire;
    public Material CampfireMtl;

    private static CreateManager sInstance = null;
    public static CreateManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                /*
                GameObject inc = new GameObject("_GameManager");
                sInstance = inc.AddComponent<GameManager>();
                */
                sInstance = new CreateManager();
            }
            return sInstance;
        }
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateCampfire()
    {
        Debug.Log("크리에이트 캠프파이어");
        player.OnObjDecide(CampFire, CampfireMtl);
        
        player.GetComponent<MainPlayerController>().CloseCreateInterface();
    }
}
