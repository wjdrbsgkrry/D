using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class UI_Popup : UI_Base
{
    public virtual void Init()
    {
        GameManager.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        GameManager.UI.ClosePopupUI(this);
    }
}
