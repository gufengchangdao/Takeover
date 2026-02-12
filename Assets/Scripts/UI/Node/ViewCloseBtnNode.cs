using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GFButton))]
public class ViewCloseBtnNode : MonoBehaviour
{
    void Start()
    {
        var btn = GetComponent<GFButton>();
        btn.onClick.AddEventListener(ClosePanel);
    }

    private void ClosePanel(BaseEventData e)
    {
        var view = GetComponentInParent<AbstractBaseView>();
        if (view != null)
            view.Close();
        else
            Log.Warning("[UI] 父对象中没有View组件，无法关闭界面。");
    }
}
