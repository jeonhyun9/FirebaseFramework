using UnityEngine;

public static class Logger
{
    private static readonly string separator = new('-', 50);
    public static void Log(string message, bool usePrefix = false)
    {
        Debug.Log($"{(usePrefix ? "[Log] " : string.Empty)}{message}");
    }

    public static void Success(string message)
    {
        Debug.Log($"<color=cyan>[Success] {message}</color>");
    }

    public static void Warning(string message)
    {
        Debug.Log($"<color=yellow>[Warning] {message}</color>");
    }

    public static void Exception(string message, System.Exception e)
    {
        Error(message);
        Debug.LogError($"[Exception] {e}");
    }

    public static void Null(object nullObject)
    {
        Debug.LogError($"[Null] {nullObject} is null");
    }

    public static void Error(string message)
    {
        Debug.LogError($"[Error] {message}");
    }

    public static void Separator()
    {
        Debug.Log(separator);
    }
}