using PacketGenerator;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _gameManager;
    static GameManager gameManager { get { return _gameManager; } }
    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();
    public static ResourceManager ResourceManager { get { return gameManager._resource; } }
    public static UIManager UI { get { return gameManager._ui; } }
   
    static void Init()
    {
        GameObject go = GameObject.Find("GameManager");
        if (go == null)
        {
            go = new GameObject("GameManager");
            DontDestroyOnLoad(go);
        }
        if (_gameManager == null)
        {
            _gameManager = new GameManager();
        }
    }

    void Start()
    {
        Init();
        Program.Init(new string[] { "Assets\\Scripts\\PacketGenerator\\PDL.xml" });
        Program.batFileStart();

        UI.ShowPopupUI<UI_Button>();
        UI.ShowPopupUI<UI_Button>();
    }

}
