using PacketGenerator;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _gameManager;
    public static GameManager Manager { get { Init(); return _gameManager; } }

    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static ResourceManager ResourceManager { get { return Manager._resource; } }
    public static SoundManager Sound { get { return Manager._sound; } }
    public static UIManager UI { get { return Manager._ui; } }

    static void Init()
    {
        GameObject go = GameObject.Find("@GameManager");
        if (go == null)
        {
            go = new GameObject { name = "@GameManager" };
            go.AddComponent<GameManager>();
        }

        DontDestroyOnLoad(go);
        _gameManager = go.GetComponent<GameManager>();

        _gameManager._sound.Init();
    }

    void Start()
    {
        Init();
        Program.Init(new string[] { "Assets\\Scripts\\PacketGenerator\\PDL.xml" });
        Program.batFileStart();

        UI.ShowPopupUI<UI_Button>();
        UI.ShowPopupUI<UI_Button>();
    }

    static void Clear()
    {
        Sound.Clear();
        UI.Clear();
    }

    int aaa = 1;

}
