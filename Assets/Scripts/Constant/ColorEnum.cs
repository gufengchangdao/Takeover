using TableStructure;
using UnityEngine;

namespace Takeover
{
    public static class ColorEnum
    {
        public static Color ParseColor(string s, Color fallback = default)
        {
            return ColorUtility.TryParseHtmlString(s, out var c) ? c : fallback;
        }

        public static readonly Color CampGreen = ParseColor("#61AA30");
        public static readonly Color CampBlue = ParseColor("#17D0FF");
        public static readonly Color CampRed = ParseColor("#FF4545");
        public static readonly Color CampPurple = ParseColor("#9C6AFF");

        public static Color GetCampColor(ECamp camp)
        {
            return camp switch
            {
                ECamp.Green => CampGreen,
                ECamp.Blue => CampBlue,
                ECamp.Red => CampRed,
                ECamp.Purple => CampPurple,
                _ => Color.gray
            };
        }
    }
}