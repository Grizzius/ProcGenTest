using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public int chunkSize = 16;
    public int waterThreshold = 50;
    public List<noiseLayer> noiseLayers = new List<noiseLayer>();
    public float caveNoiseScale = 0.3f;
    public float noiseThreshold = 0.35f;
    public GameObject chunkPrefab;
    public int noiseOffset;
    public int worldHeight;
    public int groundLevel;
    public int seed;

    public int viewDistanceInChunk = 6;

    public Transform player;

    Direction[] directions =
    {
        Direction.up,
        Direction.down,
        Direction.left,
        Direction.right,
        Direction.forward,
        Direction.backward
    };
    

    Dictionary<Vector3Int, ChunkData> chunkDataDictionnary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionnary = new Dictionary<Vector3Int, ChunkRenderer>();

    public void Start()
    {
        seed = Random.Range(100,999);
    }

    public void Update()
    {
        Vector3Int location = new Vector3Int(Mathf.RoundToInt(player.position.x / chunkSize), Mathf.RoundToInt(player.position.y / chunkSize), Mathf.RoundToInt(player.position.z / chunkSize));
        StartCoroutine(GenerateNewChunk(new Vector3Int(location.x - viewDistanceInChunk / 2, location.y - viewDistanceInChunk / 2, location.z - viewDistanceInChunk / 2), viewDistanceInChunk));
    }

    IEnumerator GenerateNewChunk(Vector3Int centerPoint, int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Vector3Int startPoint = new Vector3Int(centerPoint.x + x, centerPoint.y + y, centerPoint.z + z) * chunkSize;

                    if (!chunkDataDictionnary.ContainsKey(startPoint))
                    {
                        ChunkData data = new ChunkData(chunkSize, this, startPoint);

                        //GenerateVoxel(data);
                        chunkDataDictionnary.Add(data.worldPosition, data);
                    }
                }
            }
        }

        Dictionary<Vector3Int, ChunkData> ChunksToBuild = new Dictionary<Vector3Int, ChunkData>();
        Dictionary<Vector3Int, float> chunkDistancesFromPlayer = new Dictionary<Vector3Int, float>();

        bool foundChunkToBuild = false;

        float closestChunkDistance = 9999999f;
        Vector3Int chunkToBuildKey = new Vector3Int();
        

        foreach (ChunkData data in chunkDataDictionnary.Values)
        {
            if (!chunkDictionnary.ContainsKey(data.worldPosition))
            {
                ChunksToBuild.Add(data.worldPosition, data);

                float distance = Vector3.Distance(player.position, data.worldPosition);

                if(distance < closestChunkDistance)
                {
                    closestChunkDistance = distance;
                    chunkToBuildKey = data.worldPosition;
                    foundChunkToBuild = true;
                }
            }
        }

        if (foundChunkToBuild)
        {
            GenerateVoxel(ChunksToBuild[chunkToBuildKey]);
            yield return new WaitForFixedUpdate();
            BuildChunk(ChunksToBuild[chunkToBuildKey]);
        }
        
    }

    void BuildChunk(ChunkData data)
    {
        MeshData meshData = Chunk.GetChunkMeshData(data);
        GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
        ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
        if (!chunkDictionnary.ContainsKey(data.worldPosition))
        {
            chunkDictionnary.Add(data.worldPosition, chunkRenderer);
        }

        chunkRenderer.InitializeChunk(data);
        chunkRenderer.UpdateChunk(meshData);

        foreach(Direction direction in directions)
        {
            var neightbouringChunkCoordinate = data.worldPosition + direction.GetVector() * chunkSize;
            if (chunkDictionnary.ContainsKey(neightbouringChunkCoordinate))
            {
                var neightbourChunkRenderer = chunkDictionnary[neightbouringChunkCoordinate];
                neightbourChunkRenderer.UpdateChunk();
            }
            
        }
    }

    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkdata, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
        ChunkData containerChunk = null;

        chunkDataDictionnary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
        {
            return BlockType.none;
        }

        Vector3Int blockInChunkCoordinate = Chunk.GetBlockInChunkCoordinate(containerChunk, new Vector3Int(x, y, z));

        return Chunk.GetBlockFromChunkCoordinate(containerChunk, blockInChunkCoordinate);
    }

    private void GenerateVoxel(ChunkData chunkData)
    {
        for (int x = 0; x < chunkData.chunkSize; x++)
        {
            for (int z = 0; z < chunkData.chunkSize; z++)
            {
                float noiseValue = 0;
                foreach (noiseLayer noiseLayer in noiseLayers)
                {
                    float noiseScale = noiseLayer.noiseScale;
                    float noiseIntensity = noiseLayer.noiseIntensity;
                    float heightOffset = noiseLayer.heightOffset;
                    float newNoise = Mathf.PerlinNoise((chunkData.worldPosition.x + x + seed) * noiseScale, (chunkData.worldPosition.z + z + seed) * noiseScale);

                    newNoise = Mathf.Clamp(newNoise - heightOffset, 0,1);

                    noiseValue += newNoise * noiseIntensity;
                }

                int groundPosition = Mathf.RoundToInt(noiseValue + groundLevel);
                
                for (int y = 0; y < chunkData.chunkSize; y++)
                {
                    BlockType voxelType = BlockType.dirt;
                    
                    if (groundPosition <= chunkData.worldPosition.y + y)
                    {
                        voxelType = BlockType.dirt;
                    }
                    else
                    {
                        voxelType = BlockType.air;
                    }
                    
                    //Add Caves
                    if (chunkData.worldPosition.y +  y < groundLevel - 1)
                    {
                        float caveNoiseValue = PerlinNoise3D.Perlin3D((chunkData.worldPosition.x + x) * caveNoiseScale, (chunkData.worldPosition.y + y) * caveNoiseScale, (chunkData.worldPosition.z + z) * caveNoiseScale, new Vector3(1, 1, 1) * noiseOffset);

                        int currentHeight = chunkData.worldPosition.y + y;

                        int caveBeginHeight = groundLevel - 2;

                        int caveFullHeight = groundLevel - 20;

                        caveNoiseValue = Mathf.Lerp(1, caveNoiseValue, Mathf.InverseLerp(caveBeginHeight, caveFullHeight, currentHeight));

                        if (caveNoiseValue < noiseThreshold)
                        {
                            voxelType = BlockType.dirt;
                        }
                        else
                        {
                            //voxelType = BlockType.air;
                        }
                    }
                    
                    Chunk.SetBlock(chunkData, new Vector3Int(x,y,z), voxelType);
                }
            }
        }
    }
}

[Serializable]
public struct noiseLayer
{
    public float noiseScale;
    public float noiseIntensity;
    public float heightOffset;
}