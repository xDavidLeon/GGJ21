using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "GGJ21/Create Map")]
public class Map : ScriptableObject
{
    [System.Serializable]
    public class TiledMap
    {
        public int width;
        public int height;
        public TiledLayer[] layers;
    }
    
    [System.Serializable]
    public class TiledLayer
    {
        public List<int> data;
        public int id;
        public string name;
    }

    // Text file
    public TextAsset txtJson;
    public TiledMap tiledMap;
    
    [ContextMenu("Load")]
    public void Load()
    {
        tiledMap = JsonUtility.FromJson<TiledMap>(txtJson.text);
    }
}
