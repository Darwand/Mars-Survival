using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MeshFilter f = gameObject.AddComponent<MeshFilter>();
        Mesh m = new Mesh();

        f.mesh = m;

        List<Vector3> verts = new List<Vector3>();

        verts.Add(new Vector3(0.5f, 0, 0));
        verts.Add(new Vector3(1, 1, .5f));
        verts.Add(new Vector3(.5f, 1, 1));
        verts.Add(new Vector3(.5f, 0, 1));

        m.SetVertices(verts);

        List<int> tris = new List<int>();

        tris.Add(2);
        tris.Add(1);
        tris.Add(3);
        tris.Add(1);
        tris.Add(0);
        tris.Add(3);

        m.SetTriangles(tris.ToArray(), 0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
