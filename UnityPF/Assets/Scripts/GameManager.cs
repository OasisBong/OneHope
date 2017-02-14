using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager sInstance = null;
    public static GameManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                /*
                GameObject inc = new GameObject("_GameManager");
                sInstance = inc.AddComponent<GameManager>();
                */
                sInstance = new GameManager();
            }
            return sInstance;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
}
