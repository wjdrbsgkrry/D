using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager
{
    int _order = 10;
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas cvs = Util.GetOrAddComponent<Canvas>(go);
        cvs.renderMode = RenderMode.ScreenSpaceOverlay;
        cvs.overrideSorting = true;

        if (sort)
        {
            cvs.sortingOrder = _order;
            _order++;
        }
        else
        {
            cvs.sortingOrder = 0;
        }
    }

    // name == 프리팹 이름, T == 타입
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.ResourceManager.Instantiate($"UI/Popup/{name}");
        T popupUI = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popupUI);

        go.transform.SetParent(Root.transform);
        return popupUI;
    }

    
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.ResourceManager.Instantiate($"UI/Popup/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);
        return sceneUI;
    }

    //만약 다른 팝업이라면 먼저 지워야되는 팝업을 띄운다(커졌다 작아지는 효과 추가).
    public void ClosePopupUI(UI_Popup popupUI)
    {
        if (_popupStack.Count == 0)
            return;

        GameManager.ResourceManager.Destroy(popupUI.gameObject);
        popupUI = null;

        _order--;
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        GameManager.ResourceManager.Destroy(popup.gameObject);
        popup = null;

        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }
}
