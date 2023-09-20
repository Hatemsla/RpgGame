namespace Utils
{
    public static class Utils
    {
        public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return newMin + (newMax - newMin) * ((value - oldMin) / (oldMax - oldMin));
        }
    }
}