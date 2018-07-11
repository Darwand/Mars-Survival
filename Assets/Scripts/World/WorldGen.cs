using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour {

    WorldChunk[,] chunks;

    private void Start()
    {
        chunks = new WorldChunk[4,4];

        for(int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                chunks[x, y] = CreateWorldChunk(x, y);
            }
        }
    }

    WorldChunk CreateWorldChunk( int x, int y )
    {
        GameObject chunkObject = new GameObject("WorldChunk " + x + " : " + y);

        WorldChunk chunk = chunkObject.AddComponent<WorldChunk>();

        chunkObject.transform.parent = gameObject.transform;
        chunkObject.transform.localPosition = new Vector3(y * WorldChunk.size, 0, x * WorldChunk.size);

        chunk.Load(x, y);

        return chunk;
    }
}
