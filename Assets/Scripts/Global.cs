using DG.Tweening;
using GameFramework.Hot;
using Takeover;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Global
{
    public static SettingData SettingData { get; private set; }
    public static TableCache TableCache { get; private set; }
    public static SkillTreeData SkillTree { get; private set; }
    public static LevelData LevelData { get; private set; }

    public static bool InLevel { get; set; }

    public static void Main()
    {
        // 初始化DOTween
        DOTween.Init(true, true, LogBehaviour.Default);

        SettingData = new SettingData();
        SettingData.OnLoad();
        TableCache = new TableCache();
        TableCache.Init();
        SkillTree = new SkillTreeData();
        SkillTree.Init();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        GFGlobal.UI.OpenPanel<MainMenuControl>("MainMenu");
        GFGlobal.Sound.PlayMusic("musicMenuClass.mp3");
        // InLevel = true;
        // SceneManager.LoadScene("LevelGreen1", LoadSceneMode.Additive);
    }

    // 退出游戏
    public static void Quit()
    {
        Application.Quit();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (InLevel)
        {
            LevelData = new LevelData();
            LevelData.Init();
        }
        else
        {
            LevelData = null;
        }
    }
}