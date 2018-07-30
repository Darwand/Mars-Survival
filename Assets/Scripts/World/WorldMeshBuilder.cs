using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMeshBuilder : MonoBehaviour
{

    MeshFilter filter;
    Mesh mesh;

    public static readonly int size = 120;
    int internalSize;

    int startX = 0;
    int startY = 0;

    public void Load(int startX, int startY)
    {
        internalSize = size + 1;

        filter = GetComponent<MeshFilter>();

        if (!filter)
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }

        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        MeshCollider col = gameObject.AddComponent<MeshCollider>();



        mesh = new Mesh();
        mesh.name = "Landscape " + startX + " - " + startY;
        filter.mesh = mesh;



        mr.material = Resources.Load<Material>("Sand 03/Sand pattern 03");


        this.startX = startX;
        this.startY = startY;

        CreateMesh();

        col.sharedMesh = mesh;
    }

    void CreateMesh()
    {

        Vector3[] verts = new Vector3[internalSize * internalSize];
        Vector2[] uvs = new Vector2[internalSize * internalSize];

        mesh.Clear();
        //add all verts
        for (int y = 0; y < internalSize; ++y)
        {
            for (int x = 0; x < internalSize; ++x)
            {
                verts[(y * internalSize) + x] = (new Vector3(y, 0, x));

                float u = (float)x / 10;

                float v = (float)y / 10;

                uvs[(y * internalSize) + x] = new Vector2(u, v);
            }
        }

        //create triangles;

        int[] tris = new int[2 * (internalSize * internalSize) * 3];

        for (int sq = 0; sq < (internalSize * internalSize) - internalSize; sq++)
        {

            //quad array index
            int t = sq * 6;

            int vertical = sq + internalSize;

            if ((sq + 1) % internalSize != 0)
            {

                //first tri of quad
                tris[t] = sq;
                tris[t + 1] = sq + 1;
                tris[t + 2] = vertical;

                //second tri of quad
                tris[t + 3] = sq + 1;
                tris[t + 4] = vertical + 1;
                tris[t + 5] = vertical;

            }
        }

        for (int x = 0; x < internalSize; x++)
        {
            for(int y = 0; y < internalSize; y++)
            {
                verts[(x * internalSize) + y].y = Generator.GetGenerator().GetHeight(x + (startX * size), y + (startY * size));

            }
        }

        //apply to mesh

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
