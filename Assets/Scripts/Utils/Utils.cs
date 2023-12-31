using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return newMin + (newMax - newMin) * ((value - oldMin) / (oldMax - oldMin));
        }
        
        public static void DebugLog(object message)
        {
            Debug.Log($"{Time.time} {message}");
        }
        
        public static void DebugLogError(object message)
        {
            Debug.LogError($"{Time.time} {message}");
        }
    }
}