using DG.Tweening;
using GameFramework.AOT;
using GameFramework.Hot;
using Takeover;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;

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
    /// 当前关卡数据
    /// </summary>
    public static LevelInData LevelInData { get; private set; }
    /// <summary>
    /// 游戏进度数据
    /// </summary>
    public static LevelData LevelData { get; private set; }

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
        LevelData.OnLoad();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.quitting += OnQuit;

        GFGlobal.UI.OpenPanel<HudSettingControl>();
        GFGlobal.UI.OpenPanel<MainMenuControl>();
        GFGlobal.Sound.PlayMusic("musicMenuClass.mp3");
        // SceneManager.LoadScene("LevelGreen1", LoadSceneMode.Additive);
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

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
}