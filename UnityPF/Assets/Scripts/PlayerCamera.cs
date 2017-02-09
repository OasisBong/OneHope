using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : GameFramework
{
    public GameObject MainPlayer;
	void Start () {
		
	}
	
	void Update () {

        if (MainPlayer.tag != "MainPlayer") { return; }

        MainPlayer.transform.position.Set(
            MainPlayer.transform.position.x,
            MainPlayer.transform.position.y - 10f,
            MainPlayer.transform.position.z);

        this.transform.position = MainPlayer.transform.position;
        this.transform.rotation = MainPlayer.transform.rotation;
	}
}
