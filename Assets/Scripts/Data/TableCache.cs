using GameFramework.Hot;
using TableStructure;

namespace Takeover
{
    public class TableCache
    {
        public void Init()
        {

        }

        // 获取关卡的标题和文本
        public bool GetLevelInfo(ECamp camp, int level, out string title, out string content)
        {
            title = null;
            content = null;

            var list = GFGlobal.Tables.TbLevelData.DataList;
            for (int i = 0; i < list.Count; i++)
            {
                var data = list[i];
                if (data.Camp == camp && level == data.LevelNum)
                {
                    title = data.Title;
                    content = data.Content;
                    return true;
                }
            }
            return false;
        }
    }
}