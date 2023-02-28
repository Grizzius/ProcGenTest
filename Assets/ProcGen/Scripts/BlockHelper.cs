using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public static class BlockHelper
{
    private static Direction[] directions =
    {
        Direction.forward,
        Direction.right,
        Direction.up,
        Direction.down,
        Direction.left,
        Direction.backward
    };

    public static MeshData getMeshDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        GetFaceVertices(direction, x, y, z, meshData, blockType);
        meshData.AddQuadTriangles(BlockDataManager.blockTextureDataDictionnary[blockType].generateCollider);

        return meshData;
    }

    public static MeshData GetMeshData(ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        if(blockType == BlockType.air || blockType == BlockType.none)
        {
            return meshData;
        }

        foreach(Direction direction in directions)
        {
            var neightbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neightbourBlockType = Chunk.GetBlockFromChunkCoordinate(chunk, neightbourBlockCoordinates);
            
            if (neightbourBlockType != BlockType.none && BlockDataManager.blockTextureDataDictionnary[neightbourBlockType].isSolid == false)
            {
                if (blockType == BlockType.water)
                {
                    if (neightbourBlockType == BlockType.air)
                    {
                        meshData.waterMesh = getMeshDataIn(direction, chunk, x, y, z, meshData.waterMesh, blockType);
                    }
                }
                else
                {
                    meshData.waterMesh = getMeshDataIn(direction, chunk, x, y, z, meshData, blockType);
                }
            }
        }

        return meshData;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        var generateCollider = BlockDataManager.blockTextureDataDictionnary[blockType].generateCollider;

        switch (direction)
        {
            case Direction.forward:
                meshData.AddVertex(new Vector3(x + .5f, y - .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y + .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y + .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y - .5f, z + .5f), generateCollider);
                break;
            case Direction.backward:
                meshData.AddVertex(new Vector3(x - .5f, y - .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y + .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y + .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y - .5f, z - .5f), generateCollider);
                break;
            case Direction.right:
                meshData.AddVertex(new Vector3(x + .5f, y - .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y + .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y + .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y - .5f, z + .5f), generateCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - .5f, y - .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y + .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y + .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y - .5f, z - .5f), generateCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - .5f, y + .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y + .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y + .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y + .5f, z - .5f), generateCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - .5f, y - .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y - .5f, z - .5f), generateCollider);
                meshData.AddVertex(new Vector3(x + .5f, y - .5f, z + .5f), generateCollider);
                meshData.AddVertex(new Vector3(x - .5f, y - .5f, z + .5f), generateCollider);
                break;
            default: break;
        }
    }
}
