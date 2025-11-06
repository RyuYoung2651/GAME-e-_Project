using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap : MonoBehaviour
{
    public GameObject grassPrefab;
    public GameObject dirtPrefab;
    public GameObject waterPrefab;

    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16; // Y
    public int waterHeight = 5;
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

                if (h <= 0) continue;

                for (int y = 0; y < h; y++)
                {
                    Place(x, y, z, h);
                    WaterPlace(x, y, z, h);
                }
            }
        }
    }

    private void Place(int x, int y, int z, int h)
    {


        //최대높이에서 잔디생성
        if(y == h -1)
        {
            var go = Instantiate(grassPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"B_{x}_{y}_{z}";
        }
        else
        {
            var go = Instantiate(dirtPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"B_{x}_{y}_{z}";
        }


        
    }
    private void WaterPlace(int x, int y, int z, int h)
    {
        //일정높이에서 물이 생성
        if (waterHeight < 5)
        {
            var go = Instantiate(waterPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"B_{x}_{y}_{z}";
        }
    }
}