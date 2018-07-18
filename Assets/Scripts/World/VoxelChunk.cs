

using UnityEngine;

namespace World
{
    public enum WorldMaterial
    {
        NONE, Dirt, MAX
    }

    public class VoxelChunk
    {

        static int width = 2;
        static int height = 1;

        VoxelBlock[,,] voxChunks;

        WorldMaterial[,,] voxelMap;

        #region generator_vars

        static float peakHeight = 5;
        static float valleyHeight = 1;

        static float dirtValue = .7f;
        static float baseScale = 5f;


        static float xzxSeed = 0;
        static float xzySeed = 0;

        static float xyxSeed = 0;
        static float xyySeed = 0;

        #endregion

        int startX;
        int startZ;

        public static int GetHeight()
        {
            return height;
        }

        public static int GetWidth()
        {
            return width;
        }

        public VoxelChunk(int x, int z)
        {
            startX = x * width;
            startZ = z * width;

            GenerateVoxelMap();

            voxChunks = new VoxelBlock[width, height, width];
            AddChunks();

            
        }

        public void Destroy(int x, int y, int z)
        {
            voxelMap[x, y, z] = WorldMaterial.NONE;
        }

        void GenerateVoxelMap()
        {
            voxelMap = new WorldMaterial[(width * VoxelBlock.size), (height * VoxelBlock.size), (width * VoxelBlock.size)];

            for (int x = 0; x < (width * VoxelBlock.size); x++)
            {
                for (int y = 0; y < (height * VoxelBlock.size); y++)
                {
                    for (int z = 0; z < (width * VoxelBlock.size); z++)
                    {
                        voxelMap[x, y, z] = CreateMaterialForVoxel(x, y, z);
                    }
                }
            }
        }

        public void DrawCollection()
        {

        }

        void AddChunks()
        {

            //voxChunks[0, 0, 0] = VoxelSubChunk.MakeVoxelChunk(0, 0, 0, this);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        voxChunks[x, y, z] = VoxelBlock.MakeVoxelChunk(x + startX, y, z + startZ, this);
                    }
                }
            }

        }

        public VoxelBlock GetChunkAt(int x, int y, int z)
        {
            if((x < 0 || x >= width) || (y < 0 || y >= height) || (z < 0 || z >= width))
            {
                return null;
            }
            return voxChunks[x, y, z];
        }

        WorldMaterial CreateMaterialForVoxel( int x, int y, int z )
        {
            float w = GetWeightAtPosition(x, y, z);

            if (w < dirtValue)
            {
                return WorldMaterial.NONE;
            }
            else if (w >= dirtValue)
            {
                return WorldMaterial.Dirt;
            }

            return WorldMaterial.Dirt;
        }

        float GetWeightAtPosition( int x, int y, int z )
        {
            float height = GetHeight(x, z);

            float weight = 0;

            if (y > height)
            {
                weight = 0;
            }
            else
            {
                weight = 1;
            }
            return weight;
        }

        float GetHeight( float x, float z )
        {

            float height = 0;
            float basePerlin = Mathf.PerlinNoise(x / baseScale, z / baseScale);
            float scale = peakHeight - valleyHeight;
            
            height = basePerlin * scale;
            height += valleyHeight;

            return height;
        }

        float GetDepthDenstity( int x, int y, int z )
        {
            return Mathf.PerlinNoise(xyxSeed + x, xyySeed + y);
        }

        public WorldMaterial GetMaterialForVoxel( int x, int y, int z )
        {
            if(x < 0 || x >= VoxelBlock.size * width || y < 0 || y >= VoxelBlock.size * height || z < 0 || z >= VoxelBlock.size * width)
            {
                return WorldMaterial.MAX;
            }
            return voxelMap[x, y, z];
        }

        public static bool MaterialIsHole(WorldMaterial mat)
        {
            return mat == WorldMaterial.NONE || mat == WorldMaterial.MAX;
        }
    }

    
}
