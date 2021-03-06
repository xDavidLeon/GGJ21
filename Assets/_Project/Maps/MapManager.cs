using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class MapManager : MonoSingleton<MapManager>
{
    public Map map;
    public TilesetDatabase tilesetDatabase;
    public Transform mapBase;
    public Transform tiles;

    public float mapWidth = 128.0f;
    public float mapHeight = 128.0f;
    public float mapScale = 1.0f;

    public bool generateOnStart = false;
    public bool updateBase = false;
    public Vector2 posOffset;
    public List<Tent> tents;

    // Start is called before the first frame update
    protected override void OnSingletonStart()
    {
        base.OnSingletonStart();
        if (generateOnStart) Init();
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (tiles == null) tiles = new GameObject("Tiles").transform;
        if (tiles.childCount > 0)
            foreach (Transform child in tiles)
            {
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
    }

    // Update is called once per frame
    void Update()
    {
    }

    [ContextMenu("Init")]
    public void Init()
    {
        Clear();

        if (updateBase)
        {
            mapBase.localScale = new Vector3(mapWidth * mapScale, mapHeight * mapScale, 1);
            mapBase.localPosition = new Vector3(mapBase.localScale.x / 2.0f, 0.0f, mapBase.localScale.y / 2.0f);
        }

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
        if (tileset.isActive == false) return;
        var tilesetLayer = tileset.data;
        if (tilesetLayer.Count == 0) return;
        var layerContainer = new GameObject(tileset.name);
        layerContainer.transform.SetParent(tiles);
        int w = map.tiledMap.width;
        int h = map.tiledMap.height;
        for (int i = 0; i < w; i++)
        for (int j = 0; j < h; j++)
        {
            // Tile ID?
            var id = tileLayer.data[map.tiledMap.width * j + i];

            Tileset.TileGroup tilegroup;
            if (tilesetLayer.TryGetValue(id, out tilegroup) == false) continue; // get 3d tiles for this Tiled ID

            var tile3D = tilegroup.tiles[UnityEngine.Random.Range(0, tilegroup.tiles.Length)];
            if (tile3D == null) continue;
            int posx = i;
            int posz = h - j - 1;
            var position = new Vector3(posx * mapScale + posOffset.x * mapScale, 0,
                posz * mapScale + posOffset.y * mapScale);
            position += tileset.offset;
            position.x += UnityEngine.Random.Range(-tileset.positionVariance, tileset.positionVariance) * mapScale;
            position.z += UnityEngine.Random.Range(-tileset.positionVariance, tileset.positionVariance) * mapScale;

            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = new Vector3(0,
                UnityEngine.Random.Range(-tileset.rotationVariance, tileset.rotationVariance), 0);

#if UNITY_EDITOR
            GameObject gTile3D = PrefabUtility.InstantiatePrefab(tile3D) as GameObject;
            gTile3D.transform.SetPositionAndRotation(position, rot);
            gTile3D.transform.SetParent(layerContainer.transform);
#else
            var gTile3D = GameObject.Instantiate(tile3D, position, rot, layerContainer.transform);
#endif

            var localScale = Vector3.one * mapScale;
            localScale *= 1 + UnityEngine.Random.Range(-tileset.scaleVariance, tileset.scaleVariance);
            gTile3D.transform.localScale = localScale;
            gTile3D.name = $"{tile3D.name}_x:{i}_y:{j}";
        }
    }
}