using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        return MD5.Create().ComputeHash(bytes).SequenceEqual(MD5.Create().ComputeHash(compareBytes));
    }

    public static string GetStringUTF8(this byte[] bytes)
    {
        try
        {
            if (!bytes.IsValidArray())
                return null;

            return Encoding.UTF8.GetString(bytes);
        }
        catch (System.Exception e)
        {
            Logger.Exception("Failed to UTF8 Encoding", e);
            return null;
        }
    }
}
