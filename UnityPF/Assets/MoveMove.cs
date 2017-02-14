using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMove : MonoBehaviour {

    bool Upward = false;
    // Use this for initialization
    void Start () {

        StartCoroutine(moveCamera());
	}

	// Update is called once per frame
	void Update () {

        Vector3 temp = this.transform.position;
        if (Upward)
        {
            temp.y += 0.2f;
        }
        else
        {
            temp.y -= 0.2f;
        }

        this.transform.position = temp;
    }
    
    IEnumerator moveCamera()
    {
        yield return new WaitForSeconds(0.3f);

        if (Upward) Upward = false;
        else Upward = true;
        StartCoroutine(moveCamera());
    }

}
