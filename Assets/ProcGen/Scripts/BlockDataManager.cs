using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{

    public static float textureOffset = 0.001f;
    public static Dictionary<BlockType, TextureData> blockTextureDataDictionnary = new Dictionary<BlockType, TextureData>();
    public BlockDataSO textureData;

    private void Awake()
    {
        foreach(var item in textureData.textureDataList)
        {
            if(blockTextureDataDictionnary.ContainsKey(item.blockType) == false)
            {
                blockTextureDataDictionnary.Add(item.blockType, item);
            };
        }
    }
}
