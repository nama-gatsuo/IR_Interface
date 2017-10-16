using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid8x8 : MonoBehaviour {

    public GameObject cuboctahedron;

    private List<List<GameObject>> objs;
    private List<List<Cuboctahedron>> notes;
    private uint wn;
    private uint hn;

    public static float height = 0.5f;

    void Start () {

        wn = 8; // channel
        hn = 8; // midi note

        objs = new List<List<GameObject>>();
        notes = new List<List<Cuboctahedron>>();

        for (int w = 0; w < wn; w++) {

            List<GameObject> olist = new List<GameObject>();
            List<Cuboctahedron> nlist = new List<Cuboctahedron>();

            for (int h = 0; h < hn; h++) {

                float s = Cuboctahedron.outerSize;
                Vector3 pos = new Vector3((w - wn/2 + 0.5f) * s, (h+0.5f) * s + height, 0);
                pos = pos + transform.position;

                GameObject g = Instantiate(cuboctahedron, pos, Quaternion.identity);
                g.transform.parent = transform;
                olist.Add(g);

                Cuboctahedron co = g.GetComponent<Cuboctahedron>();
                nlist.Add(co);

            }
            objs.Add(olist);
            notes.Add(nlist);
        }

	}
	
	void Update () {
		
	}

    public Cuboctahedron GetNote(int x, int y) {

        return notes[x][y];

    }
}
