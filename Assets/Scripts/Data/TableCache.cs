using GameFramework.Hot;
using TableStructure;
using UnityEngine;
using UnityEngine.U2D;

namespace Takeover
{
    public class TableCache
    {
        public void Init()
        {

        }

        public TableStructure.LevelData GetLevelData(ECamp camp, int level)
        {
            var list = GFGlobal.Tables.TbLevelData.DataList;
            for (int i = 0; i < list.Count; i++)
            {
                var data = list[i];
                if (data.Camp == camp && level == data.LevelNum)
                {
                    return data;
                }
            }
            return null;
        }
    }
}