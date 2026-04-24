namespace GameFramework.Hot
{
    public static class UIUtility
    {
        public static string GetColorText(string text, string color)
        {
            if (!color.StartsWith("#"))
                color = "#" + color;
            return $"<color={color}>{text}</color>";
        }
    }
}