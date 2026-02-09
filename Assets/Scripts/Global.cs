using DG.Tweening;
using GameFramework.Hot;
using Takeover;
using UnityEngine.SceneManagement;

public static class Global
{
    public static TableCache TableCache
    { get; private set; }
    public static SkillTreeData SkillTree { get; private set; }
    public static LevelData LevelData { get; private set; }

    public static bool InLevel { get; set; }

    public static void Main()
    {
        // 初始化DOTween
        DOTween.Init(true, true, LogBehaviour.Default);

        TableCache = new TableCache();
        TableCache.Init();
        SkillTree = new SkillTreeData();
        SkillTree.Init();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        GFGlobal.UI.OpenPanel<MainMenuControl>("MainMenu");
        // InLevel = true;
        // SceneManager.LoadScene("LevelGreen1", LoadSceneMode.Additive);
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