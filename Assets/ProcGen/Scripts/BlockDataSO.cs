using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Data/Block Data")]
public class BlockDataSO : ScriptableObject
{
    public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData
{
    public BlockType blockType;
    public Material material;
    public bool isSolid = true;
    public bool generateCollider = true;
}
