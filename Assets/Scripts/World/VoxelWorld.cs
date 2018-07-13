using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class VoxelWorld : MonoBehaviour
    {
        int worldSize = 32;

        VoxelChunk[,] collections;

        private void Start()
        {
            collections = new VoxelChunk[worldSize, worldSize];

            collections[0, 0] = new VoxelChunk(0,0);
        }
    }
}


