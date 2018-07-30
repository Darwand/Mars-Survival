using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Generator
{
    #region permanent variables
    volatile float[,] heightMap;
    volatile bool done;
    #endregion

    #region Runtime vars
    float[,] heightMapBuffer;
    float[,] slopes;

    Crater[] craters;

    #endregion

    Mutex m;
    Thread generationThread;

    #region Generation Vars

    bool generateSeed = true;
    int seed;

    int sizeX;
    int sizeY;

    float heightScale = 120f;

    float perlinScale = 70;

    float perlinSeedX;
    float perlinSeedY;

    float secondPerlinScale = 35f;
    float secondperlinSeedX;
    float secondperlinSeedY;

    float erodeHeightDiff = .5f;
    float erodeAmnt = .5f;

    //craters per 1000 size
    int craterDensity = 1;

    float craterMinWidth = 80f;
    float craterMaxWidth = 130f;

    #endregion

    #region Life cycle
    static Generator generator;

    public static Generator GetGenerator()
    {
        return generator;
    }

    public static void CreateGenerator( int sizeX, int sizeY )
    {
        generator = new Generator(sizeX, sizeY);
    }

    public static void EndGenerator()
    {
        generator = null;
    }

    Generator( int sizeX, int sizeY )
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        m = new Mutex();

        //generationThread = new Thread(new ThreadStart(CalculateHeight));
        CalculateHeight();
    }

    #endregion

    public float GetHeight( int x, int y )
    {
        float ret = 0;

        if (x < sizeX && y < sizeY)
        {
            ret = heightMap[x, y];
        }
        else
        {
            Debug.Log("");
        }

        return ret;
    }

    void CalculateHeight()
    {
        done = false;
        m.WaitOne();

        heightMapBuffer = new float[sizeX, sizeY];
        slopes = new float[sizeX, sizeY];

        //add base elevation
        InitHeightMap();
        SinPerlin();
        MultiplyHeight(heightScale);
        Erode(100);
        MultiplyPerlin();

        heightMap = heightMapBuffer;

        ////create craters
        //heightMapBuffer = new float[sizeX, sizeY];
        //GenerateCraters();
        //ApplyCraters();
        //Erode(12);

        //MultiplyHeight(5f);

        //Debug.Log(craters.Length);


        //heightMap = heightMapBuffer;
        heightMapBuffer = null;
        done = true;
        m.ReleaseMutex();
    }

    void InitHeightMap()
    {
        for(int x = 0; x < heightMapBuffer.GetLength(0); x++)
        {
            for (int y = 0; y < heightMapBuffer.GetLength(1); y++)
            {
                heightMapBuffer[x, y] = 1;
            }
        }
    }

    void GenerateSeeds()
    {
        if(generateSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
        
        Random.InitState(seed);
        Debug.Log("Seed: " + seed);

        perlinSeedX = 0;/*Random.Range(float.MinValue / 2, float.MaxValue / 2);*/
        perlinSeedY = 0;/*Random.Range(float.MinValue / 2, float.MaxValue / 2);*/
        secondperlinSeedX = 0;
        secondperlinSeedY = 0;
    }

    void AddHeightModifier( float[,] heightMap )
    {
        if (heightMap.GetLength(0) != heightMapBuffer.GetLength(0) || heightMap.GetLength(1) != heightMapBuffer.GetLength(1))
        {
            throw new System.ArgumentException("Size of input height map does not match the buffer");
        }
        

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for(int y = 0; y < heightMap.GetLength(1); y++)
            {
                float val = heightMapBuffer[x, y] * heightMap[x, y];
            }
        }
    }

    void MultiplyHeight(float height)
    {
        for (int x = 0; x < heightMapBuffer.GetLength(0); x++)
        {
            for (int y = 0; y < heightMapBuffer.GetLength(1); y++)
            {
                heightMapBuffer[x, y] *= height;
            }
        }
    }

    void SinPerlin()
    {
        float coordX;
        float coordY;
        
        for(int x = 0; x < sizeX; x++)
        {
            for(int y = 0; y < sizeY; y++)
            {
                coordX = (float)x;/* + perlinSeedX;*/
                coordY = (float)y;/* + perlinSeedY;*/

                coordX /= perlinScale;
                coordY /= perlinScale;

                heightMapBuffer[x, y] *= (1 - Mathf.Sin(Mathf.PerlinNoise(coordX, coordY) * 3));
            }
        }
    }

    void MultiplyPerlin()
    {
        float coordX;
        float coordY;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                coordX = (float)x;/* + perlinSeedX;*/
                coordY = (float)y;/* + perlinSeedY;*/

                coordX /= secondPerlinScale;
                coordY /= secondPerlinScale;

                heightMapBuffer[x, y] *= Mathf.PerlinNoise(coordX, coordY);
            }
        }
    }

    void Erode(int itr)
    {
        for (int i = 0; i < itr; ++i)
        {
            CalculateSlopes();
            ErodeItr();
        }
    }

    void ErodeItr()
    {
        for(int x = 0; x < slopes.GetLength(0); x++)
        {
            for (int y = 0; y < slopes.GetLength(1); y++)
            {
                float ang = slopes[x, y];

                if(ang > erodeHeightDiff)
                {
                    int gx = -1;
                    int gy = -1;

                    GetLowestAdj(x, y, out gx, out gy);

                    float amnt = erodeAmnt * ((heightMapBuffer[x, y] - heightMapBuffer[gx, gy]) - erodeHeightDiff);
                    
                    heightMapBuffer[x, y] -= amnt;
                    //heightMapBuffer[gx, gy] += amnt;
                }
            }
        }
    }

    void GetLowestAdj(int x, int y, out int lx, out int ly)
    {
        float lowest = 0;
        int ix = -1;
        int iy = -1;

        if (x > 0)
        {
            if(ix == -1 || heightMapBuffer[x - 1, y] < lowest)
            {
                ix = x - 1;
                iy = y;

                lowest = heightMapBuffer[ix, iy];
            }
        }

        if (x < heightMapBuffer.GetLength(0) - 1)
        {
            if (ix == -1 || heightMapBuffer[x + 1, y] < lowest)
            {
                ix = x + 1;
                iy = y;

                lowest = heightMapBuffer[ix, iy];
            }
        }

        if (y > 0)
        {
            if (ix == -1 || heightMapBuffer[x, y - 1] < lowest)
            {
                ix = x;
                iy = y - 1;

                lowest = heightMapBuffer[ix, iy];
            }
        }

        if (y < heightMapBuffer.GetLength(1) - 1)
        {
            if (ix == -1 || heightMapBuffer[x, y + 1] < lowest)
            {
                ix = x;
                iy = y + 1;

                lowest = heightMapBuffer[ix, iy];
            }
        }

        lx = ix;
        ly = iy;

    }

    void CalculateSlopes()
    {
        for(int x = 0; x < heightMapBuffer.GetLength(0); x++)
        {
            for (int y = 0; y < heightMapBuffer.GetLength(1); y++)
            {

                float slope = 0f;
                float h = heightMapBuffer[x, y];
                float check;

                if(x > 0)
                {
                    check = heightMapBuffer[x - 1, y];
                    slope = CompareSlope(slope, h, check);
                }

                if(x < heightMapBuffer.GetLength(0) - 1)
                {
                    check = heightMapBuffer[x + 1, y];
                    slope = CompareSlope(slope, h, check);
                }

                if (y > 0)
                {
                    check = heightMapBuffer[x, y - 1];
                    slope = CompareSlope(slope, h, check);
                }

                if (y < heightMapBuffer.GetLength(1) - 1)
                {
                    check = heightMapBuffer[x, y + 1];
                    slope = CompareSlope(slope, h, check);
                }

                slopes[x, y] = slope;
            }
        }
    }

    float CompareSlope(float current, float h, float c)
    {
        float s = current;

        if (SlopeValue(h, c) > s)
        {
            s = SlopeValue(h, c);
        }

        return s;
    }

    float SlopeValue(float h, float c)
    {
        return Mathf.Abs(h - c);
    }

    void GenerateCraters()
    {
        int count = (sizeX * sizeY) / 30000;
        count *= craterDensity;

        count = 10;

        craters = new Crater[count];

        float range = craterMaxWidth - craterMinWidth;

        for (int i = 0; i < craters.Length; i++)
        {
            int x = Random.Range(0, sizeX);
            int y = Random.Range(0, sizeY);

            float size = craterMinWidth + Random.Range(0, range);

            craters[i] = new Crater(x, y, size);
        }
    }

    void ApplyCraters()
    {
        for (int x = 0; x < heightMapBuffer.GetLength(0); x++)
        {
            for(int y = 0; y < heightMapBuffer.GetLength(1); y++)
            {
                foreach(Crater c in craters)
                {
                    float h = c.GetModifierForPosition(x, y);
                    if (heightMapBuffer[x, y] == 0f || (h != 0 && h < heightMapBuffer[x, y]))
                    {
                        heightMapBuffer[x, y] = h;
                    }
                }
            }
        }
    }
}
