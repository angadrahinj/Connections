using UnityEngine;
using System;

public static class ColorExtensions
{
    /// <summary>
    /// Sets the alpha component of the color.
    /// </summary>
    /// <param name="color">The original color.</param>
    /// <param name="alpha">The new alpha value.</param>
    /// <returns>A new color with the specified alpha value.</returns>
    public static Color SetAlpha(this Color color, float alpha)
    {
        return new(color.r, color.g, color.b, alpha);
    }

    /// <summary>
    /// Adds the RGBA components of two colors and clamps the result between 0 and 1.
    /// </summary>
    /// <param name="thisColor">The first color.</param>
    /// <param name="otherColor">The second color.</param>
    /// <returns>A new color that is the sum of the two colors, clamped between 0 and 1.</returns>
    public static Color Add(this Color thisColor, Color otherColor)
    {
        return (thisColor + otherColor).Clamp01();
    }

    /// <summary>
    /// Subtracts the RGBA components of one color from another and clamps the result between 0 and 1.
    /// </summary>
    /// <param name="thisColor">The first color.</param>
    /// <param name="otherColor">The second color.</param>
    /// <returns>A new color that is the difference of the two colors, clamped between 0 and 1.</returns>
    public static Color Subtract(this Color thisColor, Color otherColor)
    {
        return (thisColor - otherColor).Clamp01();   
    }

    /// <summary>
    /// Clamps the RGBA components of the color between 0 and 1.
    /// </summary>
    /// <param name="color">The original color.</param>
    /// <returns>A new color with each component clamped between 0 and 1.</returns>
    static Color Clamp01(this Color color) 
    {
        return new Color {
            r = Mathf.Clamp01(color.r),
            g = Mathf.Clamp01(color.g),
            b = Mathf.Clamp01(color.b),
            a = Mathf.Clamp01(color.a)
        };
    }

    /// <summary>
    /// Blends two colors with a specified ratio.
    /// </summary>
    /// <param name="color1">The first color.</param>
    /// <param name="color2">The second color.</param>
    /// <param name="ratio">The blend ratio (0 to 1).</param>
    /// <returns>The blended color.</returns>
    public static Color Blend(this Color color1, Color color2, float ratio) 
    {
        ratio = Mathf.Clamp01(ratio);
        return new Color(
            color1.r * (1 - ratio) + color2.r * ratio,
            color1.g * (1 - ratio) + color2.g * ratio,
            color1.b * (1 - ratio) + color2.b * ratio,
            color1.a * (1 - ratio) + color2.a * ratio
        );
    }

    /// <summary>
    /// Averages two colors. Color.Lerp(colorA, colorB, 0.5f).
    /// </summary>
    /// <param name="color1">The first color.</param>
    /// <param name="color2">The second color.</param>
    /// <returns>The average color.</returns>
    public static Color Average(Color color1, Color color2)
    {
        return Color.Lerp(color1, color2, 0.5f);
    }

    /// <summary>
    /// Averages a list of colors.
    /// </summary>
    /// <returns>The average color.</returns>
    public static Color Average(params Color[] colors)
    {
        if (colors == null || colors.Length == 0)
            return Color.clear;

        float r = 0, g = 0, b = 0, a = 0;

        foreach (var c in colors)
        {
            r += c.r;
            g += c.g;
            b += c.b;
            a += c.a;
        }

        float inv = 1f / colors.Length;
        return new Color(r * inv, g * inv, b * inv, a * inv);
    }

    /// <summary>
    /// Inverts the color.
    /// </summary>
    /// <param name="color">The color to invert.</param>
    /// <returns>The inverted color.</returns>
    public static Color Invert(this Color color)
    {
        return new(1 - color.r, 1 - color.g, 1 - color.b, color.a);
    }

    /// <summary>
    /// Lightens the color by moving it toward white.
    /// </summary>
    /// <param name="factor">0 = no change, 1 = white</param>
    public static Color LightenColor(this Color color, float factor = 0.2f)
    {
        factor = Mathf.Clamp01(factor);

        Color result = Color.Lerp(color, Color.white, factor);
        result.a = color.a;
        return result;
    }

    /// <summary>
    /// Darkens the color by moving it toward black.
    /// </summary>
    /// <param name="factor">0 = no change, 1 = black</param>
    public static Color DarkenColor(this Color color, float factor = 0.2f)
    {
        factor = Mathf.Clamp01(factor);

        Color result = Color.Lerp(color, Color.black, factor);
        result.a = color.a;
        return result;
    }

    #region Hex
    /// <summary>
    /// Converts a Color to a hexadecimal string.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>A hexadecimal string representation of the color.</returns>
    public static string ToHex(this Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGBA(color)}";
    }

    /// <summary>
    /// Converts a hexadecimal string to a Color.
    /// </summary>
    /// <param name="hex">The hexadecimal string to convert.</param>
    /// <returns>The Color represented by the hexadecimal string.</returns>
    public static Color FromHex(this string hex) 
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color)) {
            return color;
        }

        throw new ArgumentException("Invalid hex string", nameof(hex));
    }
    #endregion
}
