using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class ChunkData
{
    public BlockType[] blocks;
    public int chunkSize = 16;
    public World worldReference;
    public Vector3Int worldPosition;

    public bool modifiedByPlayer = false;

    public ChunkData(int chunkSize, World world, Vector3Int worldPosition)
    {
        this.chunkSize = chunkSize;
        this.worldReference = world;
        this.worldPosition = worldPosition;
        blocks = new BlockType[chunkSize * chunkSize * chunkSize];
    }
}

