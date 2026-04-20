using GameFramework.AOT;
using GameFramework.Hot;
using Sirenix.OdinInspector;
using Takeover;
using UnityEngine;

public class LevelSelectCastle : MonoBehaviour
{
    [ReadOnly, ShowInInspector]
    private int level = -1;

    void Awake()
    {
        if (!int.TryParse(gameObject.name.Substring("level".Length), out level))
            Log.Error("关卡名解析失败，命名不规范：{0}", gameObject.name);
    }

    void Start()
    {
        if (level == -1 || level > Global.LevelData.CurrentLevel + 1)
            gameObject.SetActive(false);
    }

    void OnMouseUpAsButton()
    {
        GFGlobal.UI.OpenPanel<LevelIntroduceControl>(userData: level);
    }
}
