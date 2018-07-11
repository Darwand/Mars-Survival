using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO replace with voxel based shell meshes

public class WorldChunk : MonoBehaviour {

    MeshFilter filter;
    Mesh mesh;

    public static int size = 120;
    int internalSize;
    public static int seedX = 0;
    public static int seedY = 0;

    int depth = 50;
    int noiseDepth = 1;

    float scale = 1.5f;
    float noiseScale = 30f;

    int startX = 0;
    int startY = 0;

    public void Load( int startX, int startY )
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


        this.startX = (startX * size) - 1;
        this.startY = (startY * size) - 1;

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
                verts[(y * internalSize) + x] = (new Vector3(y, GetHeight(x, y), x));

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

        //apply to mesh

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    float GetHeight( int x, int y )
    {
        float xCoord = (float)(x + startX) / size * scale;
        xCoord += seedX;

        float yCoord = (float)(y + startY) / size * scale;
        yCoord += seedY;


        float height = Mathf.PerlinNoise(xCoord, yCoord);

        height = -Mathf.Sin(height * (1.5f * Mathf.PI));

        height *= depth;
        return height;
    }
}
