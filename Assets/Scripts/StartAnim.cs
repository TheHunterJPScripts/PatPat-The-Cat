using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnim : MonoBehaviour {

    public float time;
    public float realtime;
    public bool t;
	// Update is called once per frame
	void Update () {

        realtime = Time.time;
        float i = Time.time / time;


        if(!t && (Time.time > time))
        {
            GetComponent<Animator>().SetInteger("change", Mathf.FloorToInt(Random.Range(0, 2f)));
            t = true;
        }
	}
}
