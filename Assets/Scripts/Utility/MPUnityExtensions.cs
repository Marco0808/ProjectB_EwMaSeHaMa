using System.Collections.Generic;
using UnityEngine;

public static class MPUnityExtensions
{
    /// <summary>Adds a color tag with the specified color to the string.</summary>
    public static string Color(this string str, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
    }

    /// <summary>Adds a color tag with the specified color to the string.</summary>
    public static string Color(this string str, string color)
    {
        return $"<color={color}>{str}</color>";
    }
}
