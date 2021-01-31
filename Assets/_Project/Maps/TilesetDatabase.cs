using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSetDb", menuName = "GGJ21/Create TileSet Db")]
public class TilesetDatabase : ScriptableObject
{
    [SerializeField] public List<Tileset> tilesets;

    public Tileset GetTileset(string tilesetName)
    {
        return tilesets.FirstOrDefault(tileset => tileset.name == tilesetName);
    }

#if UNITY_EDITOR

    [ContextMenu("Save")]
    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}