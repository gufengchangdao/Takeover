using System.Collections.Generic;
using GameFramework.AOT;
using TMPro;
using UnityEngine;

namespace GameFramework.Hot
{
    public class GFFontManager
    {
        public const string DefaultFontMaterial = "uifont Atlas Material";

        public static GFFontManager s_Instance;
        public static GFFontManager Instance
        {
            get
            {
                s_Instance ??= new GFFontManager();
                return s_Instance;
            }
        }

        private Dictionary<string, (TMP_FontAsset, Material)> materials = new();

        public void Init()
        {
            materials.Clear();

            string languageCode = AOTGameConfig.Language.GFGetCode();
            string location = $"Assets/Content/Font/{languageCode}/TMP_Font/uifont.asset";
            var assets = GFGlobal.Resource.LoadAllAssetsSyncNoCache(location);

            List<TMP_FontAsset> fonts = new();
            foreach (var asset in assets)
                if (asset is TMP_FontAsset font)
                    fonts.Add(font);

            foreach (var asset in assets)
            {
                TMP_FontAsset font = null;
                Material material = null;
                if (asset is TMP_FontAsset)
                {
                    font = asset as TMP_FontAsset;
                    material = font.material;
                }
                else if (asset is Material m)
                {
                    foreach (var f in fonts) //查找材质所属字体
                    {
                        if (f.atlasTexture == m.GetTexture(ShaderUtilities.ID_MainTex))
                        {
                            font = f;
                            break;
                        }
                    }
                    if (font == null)
                        Log.Error($"字体材质{asset.name}找不到对应的字体");
                    material = m;
                }

                if (font != null)
                {
                    materials[material.name] = (font, material);
                }
            }

            Log.Info($"[Font] 字体加载完成，共加载{fonts.Count}个字体材质");
        }

        public void Destroy()
        {
            materials.Clear();
        }

        public void GetFontMaterial(string materialName, out TMP_FontAsset font, out Material material)
        {
            if (!materials.TryGetValue(materialName, out var pair))
                pair = materials[DefaultFontMaterial];
            font = pair.Item1;
            material = pair.Item2;
        }
    }
}