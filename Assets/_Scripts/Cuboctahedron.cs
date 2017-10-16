using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]

public class Cuboctahedron : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float vel;
    public bool isOn;

    public Material wire;
    public Material lambert;

    private List<List<Vector3>> octaVerts;
    private List<List<Vector3>> hexaVerts;
    private List<Vector3> verts;
    private List<List<Vector3>> cubeVerts;
    private Mesh mesh;
    public static float outerSize = 0.2f;
    public static float innerSize = 0.16f;

    private MeshCollider collid;
    private MeshRenderer rend;
    private Note data;
    bool isGlow;

    void Start () {

        isGlow = false;
        createMesh();

        collid = GetComponent<MeshCollider>();
        collid.sharedMesh = mesh;
        collid.enabled = false;

        rend = GetComponent<MeshRenderer>();
        rend.material = wire;

        MeshFilter filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;

    }
	
	void Update () {

        if (isGlow) {
            StartCoroutine(ResetMaterial());
            isGlow = false;
        }

    }

    public void SetData(Note _note) {
        data = _note;
    }

    public void UpdateVelocity(float pinchValue) {

        if (pinchValue > 1.0f) pinchValue = 1.0f;

        isOn = pinchValue >= 0.5f;

        if (isOn) {
            vel = (pinchValue - 0.5f) * 2f;
            UpdateMesh();
            rend.material = lambert;
            collid.enabled = true;

        } else {
            vel = 0;
            UpdateMesh();
            rend.material = wire;
            collid.enabled = false;
        }
    }

    public void Glow() {

        if (isOn) {
            float intensity = 1.5f;
            Color c = new Color(0.3f, 0.9f, 1.0f, 1.0f);
            
            rend.material.SetColor("_EmissionColor", c * intensity);
            isGlow = true;
        }

    }

    private IEnumerator ResetMaterial() {

        yield return new WaitForSeconds(0.2f);

        rend.material = lambert;
        
    }

    void createMesh() {

        float s = innerSize / 2f;

        mesh = new Mesh();
        verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<int> indeces = new List<int>();

        cubeVerts = new List<List<Vector3>> {

            new List<Vector3> { new Vector3(-1,-1,-1) * s, new Vector3( 1,-1,-1) * s, new Vector3( 1, 1,-1) * s, new Vector3(-1, 1,-1) * s }, // nz
            new List<Vector3> { new Vector3( 1,-1,-1) * s, new Vector3( 1,-1, 1) * s, new Vector3( 1, 1, 1) * s, new Vector3( 1, 1,-1) * s }, // px
            new List<Vector3> { new Vector3( 1,-1, 1) * s, new Vector3(-1,-1, 1) * s, new Vector3(-1, 1, 1) * s, new Vector3( 1, 1, 1) * s }, // pz
            new List<Vector3> { new Vector3(-1,-1, 1) * s, new Vector3(-1,-1,-1) * s, new Vector3(-1, 1,-1) * s, new Vector3(-1, 1, 1) * s }, // nx
            
            new List<Vector3> { new Vector3( 1, 1,-1) * s, new Vector3( 1, 1, 1) * s, new Vector3(-1, 1, 1) * s, new Vector3(-1, 1,-1) * s }, // py
            new List<Vector3> { new Vector3(-1,-1,-1) * s, new Vector3(-1,-1, 1) * s, new Vector3( 1,-1, 1) * s, new Vector3( 1,-1,-1) * s }  // ny
        };
        List<Vector3> cubeNorms = new List<Vector3> {

            new Vector3(0,0,-1),
            new Vector3(1,0,0),

            new Vector3(0,0,1),
            new Vector3(-1,0,0),

            new Vector3(0,1,0),
            new Vector3(0,-1,0)
        };

        // Octagon faces
        octaVerts = new List<List<Vector3>>();
        
        for (int i = 0; i < 6; i++) {
            List<Vector3> list = new List<Vector3>();
            

            for (int j = 0; j < 8; j++) {
                list.Add(new Vector3());

                norms.Add(cubeNorms[i]);

                if (j < 6) {
                    indeces.Add(i*8);
                    indeces.Add(i*8 + 2 + j);
                    indeces.Add(i*8 + 1 + j);
                }

            }

            octaVerts.Add(list);
            calcOctaVerts(octaVerts[i], cubeVerts[i]);

            for (int j = 0; j < octaVerts[i].Count; j++) {
                verts.Add(octaVerts[i][j]);
            }

        }

        // Hexagon faces
        hexaVerts = new List<List<Vector3>>();

        int vertsCount = verts.Count;
        for (int i = 0; i < 8; i++) {
            List<Vector3> list = new List<Vector3>();

            for (int j = 0; j < 6; j++) {
                list.Add(new Vector3());
                verts.Add(new Vector3());
                norms.Add(new Vector3());
            }

            hexaVerts.Add(list);

            for (int j = 0; j < 4; j++) {
                indeces.Add(48 + i*6);
                indeces.Add(48 + i*6 + j + 2);
                indeces.Add(48 + i*6 + j + 1);
            }
        }

        for (int i = 0; i < 4; i++) {

            calcHexaVerts(hexaVerts[i], i, true);
            calcHexaVerts(hexaVerts[4+i], i, false);

            Vector3 topNorm;
            Vector3 bottomNorm;

            if (i == 3) {
                topNorm = Vector3.Normalize(cubeNorms[4] + cubeNorms[i] + cubeNorms[0]);
                bottomNorm = Vector3.Normalize(cubeNorms[5] + cubeNorms[i] + cubeNorms[0]);
            } else {

                topNorm = Vector3.Normalize(cubeNorms[4] + cubeNorms[i] + cubeNorms[i+1]);
                bottomNorm = Vector3.Normalize(cubeNorms[5] + cubeNorms[i] + cubeNorms[i+1]);
            }

            for (int j = 0; j < 6; j++) {

                norms[48 + i*6 + j] = topNorm;
                verts[48 + i*6 + j] = hexaVerts[i][j];

                norms[72 + i*6 + j] = bottomNorm;
                verts[72 + i*6 + j] = hexaVerts[i+4][j];

            }
            
        }

        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.triangles = indeces.ToArray();
       

    }

    void UpdateMesh () {
        
        for (int i = 0; i < 6; i++) {

            calcOctaVerts(octaVerts[i], cubeVerts[i]);

            for (int j = 0; j < octaVerts[i].Count; j++) {
                verts[i*8 + j] = octaVerts[i][j];
            }
        }

        
        for (int i = 0; i < 4; i++) {
            
            calcHexaVerts(hexaVerts[i], i, true);
            calcHexaVerts(hexaVerts[4+i], i, false);

            for (int j = 0; j < hexaVerts[i].Count; j++) {
                verts[48 + i*6 + j] = hexaVerts[i][j];
                verts[48 + (i+4)*6 + j] = hexaVerts[(i+4)][j];
            }

        }

        mesh.vertices = verts.ToArray();
        collid.sharedMesh = mesh;

    }
    void calcOctaVerts(List<Vector3> ov, List<Vector3> cv) {

        float rate = 1.0f - vel;

        if (rate <= 0.5f){

            for (int i = 0; i < 4; i++) {

                if (i == 0) {

                    ov[i*2] = Vector3.Lerp(cv[i], cv[3], rate);
                    ov[i*2+1] = Vector3.Lerp(cv[i], cv[i+1], rate);

                } else if (i == 3) {

                    ov[i*2] = Vector3.Lerp(cv[i], cv[i-1], rate);
                    ov[i*2+1] = Vector3.Lerp(cv[i], cv[0], rate);

                } else {

                    ov[i*2] = Vector3.Lerp(cv[i], cv[i-1], rate);
                    ov[i*2+1] = Vector3.Lerp(cv[i], cv[i+1], rate);

                }

            }

        } else {

            Vector3 center = (cv[0] + cv[1] + cv[2] + cv[3]) / 4;

            float r = (rate - 0.5f) * 2f;

            for (int i = 0; i < 4; i++) {

                if (i == 0) {

                    ov[i*2] = Vector3.Lerp((cv[i] + cv[3]) * 0.5f, center, r);
                    ov[i*2+1] = Vector3.Lerp((cv[i] + cv[i+1]) * 0.5f, center, r);

                } else if (i == 3) {

                    ov[i*2] = Vector3.Lerp((cv[i] + cv[i-1]) * 0.5f, center, r);
                    ov[i*2+1] = Vector3.Lerp((cv[i] + cv[0]) * 0.5f, center, r);

                } else {

                    ov[i*2] = Vector3.Lerp((cv[i] + cv[i-1]) * 0.5f, center, r);
                    ov[i*2+1] = Vector3.Lerp((cv[i] + cv[i+1]) * 0.5f, center, r);

                }


            }

        }
    }

    void calcHexaVerts(List<Vector3> hv, int index, bool isTop) {

        if (isTop) {
            if (index == 3) {

                hv[0] = octaVerts[index][5];
                hv[1] = octaVerts[index][4];

                hv[2] = octaVerts[0][7];
                hv[3] = octaVerts[0][6];

                hv[4] = octaVerts[4][index*2+1];
                hv[5] = octaVerts[4][index*2];

            } else {

                hv[0] = octaVerts[index][5];
                hv[1] = octaVerts[index][4];

                hv[2] = octaVerts[index+1][7];
                hv[3] = octaVerts[index+1][6];

                hv[4] = octaVerts[4][index*2+1];
                hv[5] = octaVerts[4][index*2];

            }
        } else {

            if (index == 3) {

                hv[0] = octaVerts[index][3];
                hv[1] = octaVerts[index][2];

                hv[2] = octaVerts[5][(3-index)*2+1];
                hv[3] = octaVerts[5][(3-index)*2];

                hv[4] = octaVerts[0][1];
                hv[5] = octaVerts[0][0];

            } else {

                hv[0] = octaVerts[index][3];
                hv[1] = octaVerts[index][2];

                hv[2] = octaVerts[5][(3-index)*2+1];
                hv[3] = octaVerts[5][(3-index)*2];

                hv[4] = octaVerts[index+1][1];
                hv[5] = octaVerts[index+1][0];

            }

        }

    }

    void OnTriggerEnter(Collider c) {

        if (!isOn) return;

        if (data != null) {
            
            data.Bang();
            Glow();

        }
        
    }

}
