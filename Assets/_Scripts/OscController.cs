using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscController : MonoBehaviour {

	// Use this for initialization
	void Start () {

        OSCHandler.Instance.Init();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SendOscToMac(int x, int y, int note, float velocity) {
        string address = x + "/" + y + "/" + note;

        OSCHandler.Instance.SendMessageToClient("Mac", address, velocity);
        Debug.Log("ch: " + x + ", note: " + note + ", vel: " + velocity);
    }

}
