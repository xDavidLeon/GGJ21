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
    public float scaleVariance = 0.0f;
    public float rotationVariance = 0.0f;
    public float positionVariance = 0.0f;
    
    public bool isActive = true;
    
    #if UNITY_EDITOR
    [ContextMenu("Save")]
    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    #endif
    
}
