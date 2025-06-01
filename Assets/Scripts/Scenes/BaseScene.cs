using UnityEngine;

public class BaseScene : MonoBehaviour
{
    Define.Scene scene = Define.Scene.Null;

    public virtual void Init()
    {
        GameObject go = GameObject.Find("@EventSystem");
        if (go == null)
        {
            go = GameManager.ResourceManager.Instantiate("");
            go.name = "@EventSystem";
        }
    }
}
