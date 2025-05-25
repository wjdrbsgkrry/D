using PacketGenerator;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager gameManager { get { return instance; } }

    static void Init()
    {
        int test = 32;
        GameObject go = GameObject.Find("GameManager");
        if (go == null)
        {
            go = new GameObject("GameManager");
            DontDestroyOnLoad(go);
        }
        if (instance == null)
        {
            instance = new GameManager();
        }
    }

    void Start()
    {
        Init();
        Program.Init(new string[] { "Assets\\Scripts\\PacketGenerator\\PDL.xml" });
        Program.batFileStart();
    }

}
