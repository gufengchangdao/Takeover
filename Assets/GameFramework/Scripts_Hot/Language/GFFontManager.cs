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

        private readonly Dictionary<string, (TMP_FontAsset, Material)> materials = new();
        private readonly Dictionary<string, TMP_FontAsset> fonts = new();
        private string languageCode;

        public void Init()
        {
            materials.Clear();
            fonts.Clear();

            languageCode = AOTGameConfig.Language.GFGetCode();
            string location = $"Assets/Content/Font/{languageCode}/TMP_Font/uifont.asset";
            var assets = GFGlobal.Resource.LoadAllAssetsSyncNoCache(location);

            List<TMP_FontAsset> fontAssets = new();
            foreach (var asset in assets)
            {
                if (asset is not TMP_FontAsset font)
                    continue;

                fontAssets.Add(font);
                fonts[font.name] = font;
                RegisterMaterial(font, font.material);
            }

            foreach (var asset in assets)
            {
                if (asset is not Material material)
                    continue;

                var font = FindFontForMaterial(material, fontAssets);
                if (font == null)
                {
                    Log.Warning($"字体材质{asset.name}找不到对应的字体，已跳过");
                    continue;
                }

                RegisterMaterial(font, material);
            }

            Log.Info($"[Font] 字体加载完成，共加载{fontAssets.Count}个字体资源");
        }

        public void Destroy()
        {
            materials.Clear();
            fonts.Clear();
        }

        public void GetFontMaterial(string materialName, out TMP_FontAsset font, out Material material)
        {
            if (!materials.TryGetValue(materialName, out var pair))
            {
                if (!TryLoadMaterialVariant(materialName, out pair))
                    pair = materials[DefaultFontMaterial];
            }

            font = pair.Item1;
            material = pair.Item2;
        }

        private void RegisterMaterial(TMP_FontAsset font, Material material)
        {
            if (font == null || material == null)
                return;

            materials[material.name] = (font, material);
        }

        private TMP_FontAsset FindFontForMaterial(Material material, List<TMP_FontAsset> fontAssets)
        {
            var mainTex = material.GetTexture(ShaderUtilities.ID_MainTex);
            if (mainTex != null)
            {
                foreach (var font in fontAssets)
                {
                    if (font.atlasTexture == mainTex)
                        return font;
                }

                foreach (var font in fontAssets)
                {
                    if (font.atlasTexture != null && font.atlasTexture.name == mainTex.name)
                        return font;
                }
            }

            TMP_FontAsset prefixMatch = null;
            foreach (var font in fontAssets)
            {
                if (!material.name.StartsWith(font.name))
                    continue;

                if (prefixMatch == null || font.name.Length > prefixMatch.name.Length)
                    prefixMatch = font;
            }

            return prefixMatch;
        }

        private bool TryLoadMaterialVariant(string materialName, out (TMP_FontAsset, Material) pair)
        {
            pair = default;
            if (string.IsNullOrEmpty(materialName))
                return false;

            string location = $"Assets/Content/Font/{languageCode}/TMP_Font/Material/{materialName}.mat";
            var material = GFGlobal.Resource.LoadAssetSync<Material>(location, false);
            if (material == null)
                return false;

            var font = FindFontForMaterial(material, new List<TMP_FontAsset>(fonts.Values));
            if (font == null)
            {
                Log.Warning($"字体材质{materialName}加载成功，但找不到对应的字体");
                return false;
            }

            pair = (font, material);
            materials[material.name] = pair;
            return true;
        }
    }
}
