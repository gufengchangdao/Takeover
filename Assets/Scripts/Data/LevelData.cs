using System.Collections.Generic;
using GameFramework.Hot;
using SimpleJSON;
using TableStructure;

namespace Takeover
{
    public class LevelData : IDataSaveLoad
    {
        private const string SaveKeyPrefix = "LevelData_";
        private const string CurrentLevelKey = "currentLevel";
        private const string LevelScoreKey = "levelScore";

        public ECamp Camp { get; private set; }

        /// <summary>
        /// 关卡总数
        /// </summary>
        public int LevelCount { get; private set; }

        public int CurrentLevel { get; private set; }

        /// <summary>
        /// 已通关关卡评分
        /// </summary>
        public List<int> LevelScore { get; private set; }

        public void OnLoad()
        {
        }

        public void OnSave()
        {
        }

        public void SetLevelData(LevelData data)
        {
            Camp = data.Camp;
            LevelCount = data.LevelCount;
            CurrentLevel = data.CurrentLevel;
            LevelScore = data.LevelScore;
        }

        public static LevelData LoadData(ECamp camp)
        {
            string jsonData = GFGlobal.Save.GetString(GetSaveKey(camp));
            LevelData levelData = new();
            levelData.Camp = camp;
            levelData.LevelScore = new();

            // 统计关卡总数
            foreach (var campaignData in GFGlobal.Tables.TbCampaignData.DataList)
                if (campaignData.Camp == camp)
                    levelData.LevelCount++;

            if (!string.IsNullOrEmpty(jsonData))
            {
                // 有保存的数据
                JSONNode jsonNode = JSON.Parse(jsonData);
                levelData.CurrentLevel = jsonNode[CurrentLevelKey].AsInt;
                JSONNode levelScoreNode = jsonNode[LevelScoreKey];
                foreach (JSONNode scoreNode in levelScoreNode.Children)
                    levelData.LevelScore.Add(scoreNode.AsInt);
            }
            else
            {
                levelData.CurrentLevel = 0;
            }
            return levelData;
        }

        public static void SaveData(LevelData levelData)
        {
            JSONObject jsonObject = new JSONObject
            {
                [CurrentLevelKey] = levelData.CurrentLevel,
            };
            JSONArray levelScoreArray = new();
            foreach (int score in levelData.LevelScore)
                levelScoreArray.Add(score);
            jsonObject[LevelScoreKey] = levelScoreArray;
            GFGlobal.Save.SetString(GetSaveKey(levelData.Camp), jsonObject.ToString());
        }

        private static string GetSaveKey(ECamp camp)
        {
            return SaveKeyPrefix + camp;
        }
    }
}
