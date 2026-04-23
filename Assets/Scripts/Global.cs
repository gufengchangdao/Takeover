using DG.Tweening;
using GameFramework.AOT;
using GameFramework.Hot;
using Takeover;
using UnityEngine;
using UnityEngine.U2D;
using YooAsset;

public static class Global
{
    /// <summary>
    /// 设置数据
    /// </summary>
    public static SettingData SettingData { get; private set; }
    /// <summary>
    /// table数据整理
    /// </summary>
    public static TableCache TableCache { get; private set; }
    /// <summary>
    /// 技能树数据
    /// </summary>
    public static SkillTreeData SkillTree { get; private set; }
    /// <summary>
    /// 游戏进度数据
    /// </summary>
    public static LevelData LevelData { get; private set; }

    public static LevelLogic LevelLogic { get; set; }

    /// <summary>
    /// 关卡内玩家自己的数据
    /// </summary>
    public static CombotantData CombotantData
    {
        get
        {
            return LevelLogic.Combotants[LevelData.Camp];
        }
    }

    public static void Main()
    {
        // 初始化DOTween
        DOTween.Init(true, true, LogBehaviour.Default);

        SettingData = new SettingData();
        SettingData.OnLoad();
        TableCache = new TableCache();
        TableCache.Init();
        SkillTree = new SkillTreeData();
        SkillTree.OnLoad();
        LevelData = new LevelData();
        // LevelData.OnLoad();

        Application.quitting += OnQuit;

        // 初始化流程
        GFGlobal.Procedure.Init(
            new ProcedureStart(),
            new ProcedureLevelSelect(),
            new ProcedureLevel()
        );

        GFGlobal.UI.OpenPanel<HudSettingControl>();
        GFGlobal.Sound.PlayMusic("musicMenuClass.mp3");

        GFGlobal.Procedure.ChangeState<ProcedureStart>();
    }

    // 退出游戏
    public static void Quit()
    {
        Application.Quit();
    }

    private static void OnQuit()
    {
        Log.Info("游戏退出");
    }
}