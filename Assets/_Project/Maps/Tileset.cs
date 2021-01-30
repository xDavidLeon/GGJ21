using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;
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

    [SerializeField]
    public TilesetLayerData data;

    public Vector3 offset = Vector3.zero;
    public bool isActive = true;
    
    [ContextMenu("Save")]
    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
}
