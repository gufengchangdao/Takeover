#if USE_LUBAN
using TableStructure;
#endif
using SimpleJSON;
using UnityEngine;
using GameFramework.AOT;

namespace GameFramework.Hot
{
    public class GFDataTable : GFBaseModule
    {
#if USE_LUBAN
        public Tables Tables { get; private set; }
#endif
        void Awake()
        {
#if USE_LUBAN
            Tables = new Tables(ReadTableFile);
            Log.Info("[DataTable] datatable files read complete!");
#endif
        }

        private JSONNode ReadTableFile(string fileName)
        {
            if (fileName == "tbmultilanguagetext")
                fileName += "_" + AOTGameConfig.Language.GFGetCode();

            string filePath = $"{GFGlobal.Config.datatableAssetPath}/{fileName}.json";
            Log.Info("[DataTable] Reading table file: {0}", filePath);
            var jsonData = GFGlobal.Resource.LoadAssetSync<TextAsset>(filePath, false).text;
            return JSON.Parse(jsonData);
        }
    }
}