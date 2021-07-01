using UnityEngine;
using System.Collections;

public static class Debugger
{
    public static void Log(object message)
    {
#if UNITY_EDITOR_WIN
        Debug.Log(message);
#endif
    }

    public static void Log(object message, Object context)
    {
#if UNITY_EDITOR_WIN
        Debug.Log(message, context);
#endif
    }
    public static void LogWarning(object message)
    {
#if UNITY_EDITOR_WIN
        Debug.LogWarning(message);
#endif
    }

    public static void LogWarning(object message, Object context)
    {
#if UNITY_EDITOR_WIN
        Debug.LogWarning(message, context);
#endif
    }

    public static void LogError(object message)
    {
#if UNITY_EDITOR_WIN
        Debug.LogError(message);
#endif
    }

    public static void LogError(object message, Object context)
    {
#if UNITY_EDITOR_WIN
        Debug.LogError(message, context);
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
#if UNITY_EDITOR_WIN
        Debug.DrawLine(start, end);
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
#if UNITY_EDITOR_WIN
        Debug.DrawLine(start, end, color);
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
#if UNITY_EDITOR_WIN
        Debug.DrawLine(start, end, color, duration);
#endif
    }

    public static void Assert(bool condition)
    {
#if UNITY_EDITOR_WIN
        Debug.Assert(condition);
#endif
    }

    public static void Assert(bool condition, string message)
    {
#if UNITY_EDITOR_WIN
        Debug.Assert(condition, message);
#endif
    }
}
