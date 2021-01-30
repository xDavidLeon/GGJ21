using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapManager : MonoBehaviour
{
    public Map map;
    public TilesetDatabase tilesetDatabase;

    public Transform tiles;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
    }

    [ContextMenu("Init")]
    public void Init()
    {
        if (tiles == null) tiles = new GameObject("Tiles").transform;
        if (tiles.childCount > 0)
            foreach (Transform child in tiles)
                Destroy(child);

        // Loop through each layer in the TiledMap
        foreach (var tileLayer in map.tiledMap.layers)
        {
            Tileset tileSet = tilesetDatabase.GetTileset(tileLayer.name);
            if (tileSet == null) continue;
            InitLayer(tileLayer, tileSet);
        }
    }

    public void InitLayer(Map.TiledLayer tileLayer, Tileset tileset)
    {
        if (tileset == null) return;
        var tilesetLayer = tileset.data;
        if (tilesetLayer.Count == 0) return;
        var layerContainer = new GameObject(tileset.name);
        layerContainer.transform.SetParent(tiles);
        
        for (int i = 0; i < map.tiledMap.width; i++)
        for (int j = 0; j < map.tiledMap.height; j++)
        {
            // Tile ID?
            var id = tileLayer.data[map.tiledMap.width * j + i];

            Tileset.TileGroup tilegroup;
            if (tilesetLayer.TryGetValue(id, out tilegroup) == false) continue; // get 3d tiles for this Tiled ID

            var tile3D = tilegroup.tiles[UnityEngine.Random.Range(0, tilegroup.tiles.Length)];
            if (tile3D == null) continue;

            var gTile3D = GameObject.Instantiate(tile3D, new Vector3(i, 0, j), Quaternion.identity, layerContainer.transform);
            gTile3D.name = $"{tile3D.name}_x:{i}_y:{j}";
        }
    }
}