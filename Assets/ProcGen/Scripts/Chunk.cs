using System;
using UnityEngine;

public static class Chunk
{
    public static void LoopTroughTheBlocks(ChunkData chunkData, Action<int, int, int> actionToPerfom)
    {
        for (int i = 0; i < chunkData.blocks.Length; i++)
        {
            var position = GetPositionFromIndex(chunkData, i);
            actionToPerfom(position.x, position.y, position.z);
        } 
    }

    private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int index)
    {
        int x = index % chunkData.chunkSize;
        int y = (index / chunkData.chunkSize) % chunkData.chunkSize;
        int z = index / (chunkData.chunkSize * chunkData.chunkSize);
        return new Vector3Int(x, y, z);
    }

    private static bool InRange(ChunkData chunkData, int index)
    {
        if (index < 0 || index >= chunkData.chunkSize)
        {
            return false;
        }
        return true;
    }

    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block)
    {
        if (InRange(chunkData, localPosition.x) && InRange(chunkData, localPosition.y) && InRange(chunkData, localPosition.z))
        {
            int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.blocks[index] = block;

        }
        else
        {
            throw new Exception("need to ask world for appropriate chunk");
        }
    }

    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkSize * z;
    }

    public static Vector3Int GetBlockInChunkCoordinate(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPosition.x,
            y = pos.y - chunkData.worldPosition.y,
            z = pos.z - chunkData.worldPosition.z,
        };
    }
    public static BlockType GetBlockFromChunkCoordinate(ChunkData chunkdata, Vector3Int chunkCoordinate)
    {
        return GetBlockFromChunkCoordinate(chunkdata, chunkCoordinate.x, chunkCoordinate.y, chunkCoordinate.z);
    }
    public static BlockType GetBlockFromChunkCoordinate(ChunkData chunkdata, int x, int y, int z)
    {
        if (InRange(chunkdata, x) && InRange(chunkdata, y) && InRange(chunkdata, z))
        {
            int index = GetIndexFromPosition(chunkdata, x, y, z);
            return chunkdata.blocks[index];
        }

        return chunkdata.worldReference.GetBlockFromChunkCoordinates(chunkdata, chunkdata.worldPosition.x + x, chunkdata.worldPosition.y + y, chunkdata.worldPosition.z + z);
    }

    public static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData(true);

        LoopTroughTheBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.blocks[GetIndexFromPosition(chunkData, x, y, z)]));

        return meshData;
    }

    internal static Vector3Int ChunkPositionFromBlockCoords(World world, int x, int y, int z)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkSize) * world.chunkSize,
            z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize,
        };
        return pos;
    }
}