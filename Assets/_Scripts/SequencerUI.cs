using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class SequencerUI : MonoBehaviour {

    public Leap.Unity.PinchDetector pd_a;
    public Leap.Unity.PinchDetector pd_b;

    public GameObject grid8x8;

    private bool isPinched;
    private Cuboctahedron currentObj;

    private List<GameObject> objs;
    private List<Grid8x8> grids;

    private NotesManeger manager;
    private int[] xyz;

    // Use this for initialization
    void Start () {

        GameObject o = GameObject.Find("/DataManager");
        manager = o.GetComponent<NotesManeger>();

        isPinched = false;
        float s = Cuboctahedron.outerSize;
        int dn = 16;

        objs = new List<GameObject>();
        grids = new List<Grid8x8>();

        for (int d= 0; d < dn; d++) {

            GameObject g = Instantiate(grid8x8, new Vector3(0,0,(d-dn/2+0.5f)*s), Quaternion.identity);
            g.transform.parent = transform;
            objs.Add(g);

            Grid8x8 grid = g.GetComponent<Grid8x8>();
            grids.Add(grid);

        }

	}
	
	// Update is called once per frame
	void Update () {

        if (pd_a.IsActive && pd_b.IsActive) {

            if (!isPinched) {

                // Pinch Start
                isPinched = true;

                Vector3 m = (pd_a.Position + pd_b.Position) * 0.5f;

                float s = Cuboctahedron.outerSize;
                float h = Grid8x8.height;
                bool isOutArea = m.x < -s*4f || m.x > s*4f || m.y < h || m.y > h + s*8f
                    || m.z < -s * 8f || m.z > s * 8f;

                
                if (!isOutArea) {

                    xyz = CalcCoordToIndex(m);
                    currentObj = GetObj(xyz[0], xyz[1], xyz[2]);

                }
                
            } else {

                // Pinching
                if (currentObj != null) {

                    float dist = Vector3.Distance(pd_a.Position, pd_b.Position);
                    float d = dist / ( Cuboctahedron.innerSize * Mathf.Sqrt(2f) );

                    currentObj.UpdateVelocity(d);
                }

            }

        } else {

            // Pinch End
            if (currentObj != null) {

                Note n = manager.GetNote(xyz[0], xyz[1], xyz[2]);

                if (currentObj.isOn) {

                    if (n != null) {
                        // update
                        n.SetVeloctiy(currentObj.vel);
                        //n.Bang();
                    } else {
                        // add
                        manager.AddNote(xyz[0], xyz[1], xyz[2], currentObj.vel, currentObj);
                        currentObj.SetData(manager.GetNote(xyz[0], xyz[1], xyz[2]));
                                                    
                    }

                } else {

                    // off
                    if (n != null) {
                        manager.DeleteNote(xyz[0], xyz[1], xyz[2]);
                    }
                }

            } 

            // reset
            isPinched = false;
            currentObj = null;
            xyz = null;

        }


	}

    int[] CalcCoordToIndex(Vector3 pos) {

        // x: ch, y: note, z: beat
        int x, y, z;
        float s = Cuboctahedron.outerSize;
        float h = Grid8x8.height;
        
        x = (int)Mathf.Floor(pos.x / s) + 4;
        y = (int)Mathf.Floor((pos.y - h)/ s);
        z = (int)Mathf.Floor(pos.z / s) + 8;

        return new int[]{ x, y, z };

    }

    Cuboctahedron GetObj(int x, int y, int z) {

        return grids[z].GetNote(x, y);

    }
}
