using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerBar : MonoBehaviour {

    private SequenceManager manager;

	void Start () {

        Vector3 pos = new Vector3(0, 0.5f + Cuboctahedron.outerSize * 4f, Cuboctahedron.outerSize * (-8f+ 0.5f));
        transform.localPosition = pos;

        Vector3 sca = new Vector3(Cuboctahedron.outerSize * 8f, Cuboctahedron.outerSize * 8f, Cuboctahedron.outerSize);
        transform.localScale = sca;

	}
	
	void Update () {
	    
	}

    public void UpdatePos(int beat) {

        Vector3 pos = new Vector3(0, 0.5f + Cuboctahedron.outerSize * 4f, Cuboctahedron.outerSize * ((float)beat-8f+0.5f));
        transform.localPosition = pos;

    }
}
