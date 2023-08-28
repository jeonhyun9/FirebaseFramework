using System.Linq;
using System.Text;
using System;

public static class ExtensionUtils
{
    public static bool IsValidArray<T>(this T[] array)
    {
        return array != null && array.Length > 0;
    }

    public static bool IntegrityCheck(this byte[] bytes, byte[] compareBytes)
    {
        if (!bytes.IsValidArray() || !compareBytes.IsValidArray())
            return false;

        using (System.Security.Cryptography.MD5 md5Hash1 = System.Security.Cryptography.MD5.Create())
        using (System.Security.Cryptography.MD5 md5Hash2 = System.Security.Cryptography.MD5.Create())
        {
            return md5Hash1.ComputeHash(bytes).SequenceEqual(md5Hash2.ComputeHash(compareBytes));
        }
    }

    public static string GetStringUTF8(this byte[] bytes)
    {
        try
        {
            if (!bytes.IsValidArray())
                return null;

            return Encoding.UTF8.GetString(bytes);
        }
        catch (Exception e)
        {
            Logger.Exception("Failed to UTF8 Encoding", e);
            return null;
        }
    }

    public static void SafeSetText(this TMPro.TextMeshProUGUI text, string value)
    {
        if (text != null)
            text.text = value;
    }

    public static void SafeSetActive(this UnityEngine.GameObject gameObject, bool active)
    {
        if (gameObject == null || gameObject.activeSelf == active)
            return;

        gameObject.SetActive(active);
    }
}
