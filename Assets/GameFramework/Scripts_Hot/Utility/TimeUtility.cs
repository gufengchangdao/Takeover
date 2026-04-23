namespace GameFramework.Hot
{
    public static class TimeUtility
    {
        public static string FormatElapsedTime(int seconds)
        {
            if (seconds < 0)
                seconds = 0;

            int minutes = seconds / 60;
            int remainSeconds = seconds % 60;
            return $"{minutes}:{remainSeconds:00}";
        }
    }
}
