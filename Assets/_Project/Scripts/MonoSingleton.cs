using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Mono Singleton for Mono Behaviours
 * David Leon Molero - 2013
 */
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static string FileName
    {
        get { return typeof(T).Name; }
    }

    private static string ResourcePath
    {
        get { return FileName; }
    }

    [Header("MonoSingleton Settings")] public bool destroyOnLoad = true;
    private static object _lock = new object();
    protected static T m_Instance = null;

    public static bool isApplicationQuit = false;

    public static T Instance
    {
        get
        {
            if (isApplicationQuit)
            {
                Debug.LogWarning($"Calling singleton while quitting app. Ignoring call.");
                return null;
            }

            if (!m_Instance)
            {
                lock (_lock)
                {
                    // Instance requiered for the first time, we look for it
                    m_Instance = FindObjectOfType(typeof(T)) as T;

                    if (m_Instance == null)
                    {
                        GameObject prefab = Resources.Load(ResourcePath) as GameObject;
                        if (prefab != null)
                        {
                            GameObject runtimeGO = null;
#if UNITY_EDITOR
                            runtimeGO = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                            Debug.Log($"Instantiating MonoSingleton of type {typeof(T)} : {m_Instance}", runtimeGO);
#else
                                runtimeGO = GameObject.Instantiate(prefab);
#endif
                            if (runtimeGO != null)
                            {
                                runtimeGO.name = FileName;
                                m_Instance = runtimeGO.GetComponent<T>();
                            }
                        }
                        //else
                        //{
                        //    Debug.LogError("Tried to load Singleton prefab at path " + ResourcePath + " and did not find it, include " + typeof(T).ToString() + " in resources folder");
                        //}

                        // Object not found, we create a temporary one
                        if (m_Instance == null)
                        {
                            //Debug.Log("No instance of " + typeof(T).ToString() + ", a temporary one is created.");
                            m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();

                            // Problem during the creation, this should not happen
                            if (m_Instance == null)
                            {
                                Debug.LogError("MonoSingleton: Problem during the creation of " + typeof(T).ToString());
                            }
                        }
                    }
                }
            }

            return m_Instance;
        }
    }

    protected virtual void OnSingletonAwake()
    {
    }

    protected virtual void OnSingletonStart()
    {
    }

    protected virtual void OnSingletonDestroy(bool isCurrentInstance)
    {
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected virtual void OnSceneUnloaded(Scene scene)
    {
    }

    // If no other monobehaviour request the instance in an awake function
    // executing before this one, no need to search the object.
    protected void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
            if (destroyOnLoad == false && Application.isPlaying) DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != m_Instance)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        // Call a custom Awake
        OnSingletonAwake();
    }

    private void Start()
    {
        // Custom Start
        OnSingletonStart();
    }

    /// <summary>
    /// Returns if there is an instance of this Singleton without instantiating it
    /// </summary>
    public static bool Exists()
    {
        return m_Instance && isApplicationQuit == false;
    }

    // Make sure the instance isn't referenced anymore when the user quit, just in case.
    protected virtual void OnApplicationQuit()
    {
        m_Instance = null;
        isApplicationQuit = true;
    }

    private void OnDestroy()
    {
        if (m_Instance == this)
        {
            this.OnSingletonDestroy(true);
            m_Instance = null;
        }
        else
        {
            this.OnSingletonDestroy(false);
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}