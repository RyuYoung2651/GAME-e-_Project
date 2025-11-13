using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap : MonoBehaviour
{
    public GameObject blockPrefabDirt;
    public GameObject blockPrefabGrass;
    public GameObject blockPrefabWater;

    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16; // y
    public int waterLevel = 4;

    [SerializeField] float noiseScale = 20f;

    void Start()
    {
        float offsetX = Random.Range(-9999f, 9999f);
        float offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;
                float noise = Mathf.PerlinNoise(nx, nz);
                int h = Mathf.FloorToInt(noise * maxHeight);
                if (h < 0) h = 0;

                for (int y = 0; y <= h; y++)
                {
                    if (y == h)
                        PlaceGrass(x, y, z);
                    else
                        PlaceDirt(x, y, z);
                }

                for (int y = h + 1; y <= waterLevel; y++)
                    PlaceWater(x, y, z);
            }
        }
    }
    private void PlaceWater(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabWater, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Water_{x}_{y}_{z}";

        var D = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        D.type = BlockType.Water;
        D.maxHP = 3;
        D.dropCount = 1;
        D.mineable = true;
    }

    private void PlaceDirt(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabDirt, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Dirt_{x}_{y}_{z}";

        var D = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        D.type = BlockType.Dirt;
        D.maxHP = 3;
        D.dropCount = 1;
        D.mineable = true;
    }

    private void PlaceGrass(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabGrass, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Grass_{x}_{y}_{z}";

        var D = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        D.type = BlockType.Grass;
        D.maxHP = 3;
        D.dropCount = 1;
        D.mineable = true;
    }

}
