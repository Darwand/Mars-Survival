using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int size = 1;

    public static int totalY;

	void Start ()
	{
        Generator.CreateGenerator(size * WorldMeshBuilder.size + 1, size * WorldMeshBuilder.size + 1);

        totalY = size * WorldMeshBuilder.size;

        for(int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                GameObject gameobject = new GameObject(x + " : " + z);
                gameobject.AddComponent<WorldMeshBuilder>().Load(x, z);
                gameobject.transform.position = new Vector3(x * WorldMeshBuilder.size, 0, z * WorldMeshBuilder.size);
            }
        }

        Generator.EndGenerator();
	}
}
