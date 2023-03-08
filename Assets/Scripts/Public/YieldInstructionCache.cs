using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class YieldInstructionCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new();

    private static readonly Dictionary<float, WaitForSeconds> timeInterval = new(new FloatComparer());
    private static readonly Dictionary<float, WaitForSecondsRealtime> timeInterval_Real = new(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!timeInterval.TryGetValue(seconds, out wfs))
            timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
        return wfs;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime wfs;
        if (!timeInterval_Real.TryGetValue(seconds, out wfs))
            timeInterval_Real.Add(seconds, wfs = new WaitForSecondsRealtime(seconds));
        else
            wfs.Reset();
        return wfs;
    }
}