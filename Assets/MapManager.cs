using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [System.Serializable]
    public class TiledMap
    {
        public int width;
        public int height;
        public TiledLayer[] layers;

        public static TiledMap CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<TiledMap>(json);
        }
    }

    [System.Serializable]
    public class TiledLayer
    {
        public List<int> data;
        public int id;
        public string name;
    }

    public TiledMap tiledMap;
    public TextAsset tiledMapJson;

    private void Awake()
    {
        Init(tiledMapJson);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(TextAsset jsonAsset)
    {
        if (jsonAsset == null) return;
        tiledMap = TiledMap.CreateFromJSON(jsonAsset.text);
    }
    
    public void Init(string jsonText)
    {
        tiledMap = TiledMap.CreateFromJSON(jsonText);
    }
}
