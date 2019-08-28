using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    public static T Instance { get { return _instance; } }

    
    public static bool Setup()
    {
        if (_instance) return true;

        _instance = FindObjectOfType<T>();
        if (_instance) return false;

        var name = typeof(T).ToString();
        var prefab = Resources.Load<T>("prefabs/managers/" + name);
        if (prefab)
        {
            _instance = Instantiate(prefab);
            _instance.name = prefab.name;
            return false;
        }

        var go = new GameObject(name);
        _instance = go.AddComponent<T>();
        return false;
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
            return;
        }


        _instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
