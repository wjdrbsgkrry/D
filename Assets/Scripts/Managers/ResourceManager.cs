using System.Collections;
using System.Collections.Generic;
using PacketGenerator;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject go = Load<GameObject>($"Prefabs/{path}");
        if (go == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Object.Instantiate(go);
    }

    public void Destroy(GameObject go)
    {
        if (go == null) return;

        Object.Destroy(go);
    }

}
