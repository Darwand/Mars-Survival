

namespace World
{
    public class VoxelChunk
    {

        static int width = 1;
        static int height = 1;

        VoxelBlock[,,] voxChunks;

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

            voxChunks = new VoxelBlock[width, height, width];
            AddChunks();
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

    }
}
