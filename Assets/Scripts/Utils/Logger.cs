using UnityEngine;

public static class Logger {
    public static void Log(object message) {
        #if UNITY_EDITOR
        Debug.Log(message);
        #endif
    }
    public static void Log(object message, Object context) {
        #if UNITY_EDITOR
        Debug.Log(message, context);
        #endif
    }
}