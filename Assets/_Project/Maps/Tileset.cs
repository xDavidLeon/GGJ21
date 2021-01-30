using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSet", menuName = "GGJ21/Create TileSet")]
public class Tileset : ScriptableObject
{
    [System.Serializable]
    public class TileGroup
    {
        public GameObject[] tiles;
    }
    
    [System.Serializable]
    public class TilesetLayerData : SerializableDictionaryBase<int, TileGroup> { }

    public TilesetLayerData data;
}
