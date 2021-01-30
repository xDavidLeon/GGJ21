using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSetDb", menuName = "GGJ21/Create TileSet Db")]
public class TilesetDatabase : ScriptableObject
{
    public List<Tileset> tilesets;

    public Tileset GetTileset(string tilesetName)
    {
        return tilesets.FirstOrDefault(tileset => tileset.name == tilesetName);
    }

}
