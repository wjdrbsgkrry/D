using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    enum Buttons
    {
        StartButton
    }

    enum Texts
    {
        StartText
    }

    enum Images
    {
        PlayerIcon
    }

    enum GameObjects
    {
        TestObject
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
        //Test Code
        Get<Text>((int)Texts.StartText).text = "안녕";
        GameObject go = Get<Image>((int)Images.PlayerIcon).gameObject;
        AddUIEvent(go, ((PointerEventData data) => { go.transform.position = data.position; }), Define.UIEvent.Drag);
        // Debug.Log(Get<Text>((int)Texts.StartText).text);
    
    }

    void Start()
    {
        Init();
    }
}
