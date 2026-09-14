using System;
using System.Globalization;
using Firebot2.GameModel.Base;
using Firebot2.Utilities;
using Il2CppTMPro;
using UnityEngine;

namespace Firebot2.GameModel.Primitives;

public class GameText : GameElement
{
    public GameText(string path = null, GameElement parent = null, Transform transform = null)
        : base(path, parent, transform) { }

    public DateTime Time => TimeParser.ParseExpectedTime(GetParsedText());

    public DateTime TimeMultiplier(double multiplier = 1) =>
        TimeParser.ParseExpectedTime(GetParsedText(), multiplier: multiplier);

    public int GetParsedInt(int fallback = 0)
    {
        var parsedText = GetParsedText();
        return int.TryParse(parsedText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;
    }

    public double GetParsedDouble(double fallback = 0)
    {
        var parsedText = GetParsedText();

        if (double.TryParse(parsedText, NumberStyles.Float, CultureInfo.InvariantCulture, out var invariantValue))
            return invariantValue;

        return double.TryParse(parsedText, NumberStyles.Float, CultureInfo.CurrentCulture, out var currentCultureValue)
            ? currentCultureValue
            : fallback;
    }

    /// <summary>
    ///     Parses compact/abbreviated numbers (e.g. "86,27M", "10.916.942.093,4") into a double.
    ///     Tries invariant and current culture first, then falls back to European-style grouping
    ///     ('.' as thousands separator, ',' as decimal separator).
    /// </summary>
    public double GetParsedDoubleAbbreviated(double fallback = 0)
    {
        var text = GetParsedText().Trim();
        if (text.Length == 0) return fallback;

        var multiplier = 1d;
        var suffix = char.ToUpperInvariant(text[^1]);
        if (suffix is 'K' or 'M' or 'B' or 'T')
        {
            multiplier = suffix switch { 'K' => 1e3, 'M' => 1e6, 'B' => 1e9, _ => 1e12 };
            text = text[..^1].Trim();
        }

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var invariantValue))
            return invariantValue * multiplier;

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out var currentCultureValue))
            return currentCultureValue * multiplier;

        var europeanStyle = text.Replace(".", "").Replace(",", ".");
        return double.TryParse(europeanStyle, NumberStyles.Float, CultureInfo.InvariantCulture, out var europeanValue)
            ? europeanValue * multiplier
            : fallback;
    }

    public string GetParsedText()
    {
        if (!IsVisible()) return string.Empty;

        if (!TryGetComponent(out TMP_Text tmp)) return string.Empty;
        try
        {
            return tmp.text;
        }
        catch (Exception e)
        {
            Debug($"[FAILED] Exception while reading text: {e.Message}. Path: {Path}");
            return string.Empty;
        }
    }

    public void SetColor(Color newColor)
    {
        if (TryGetComponent(out TMP_Text tmp))
            tmp.color = newColor;
    }

    public void SetOutline(Color color, float thickness = 0.2f)
    {
        if (!TryGetComponent(out TMP_Text tmp) || tmp.fontSharedMaterial == null)
        {
            Debug($"[FAILED] FAILED to set outline: TMP_Text or Material missing. Path: {Path}");
            return;
        }

        tmp.fontSharedMaterial.EnableKeyword("OUTLINE_ON");
        tmp.outlineColor = color;
        tmp.outlineWidth = thickness;
        tmp.UpdateMeshPadding();
        tmp.SetAllDirty();
    }

    public void RemoveOutline()
    {
        if (TryGetComponent(out TMP_Text tmp) && tmp.fontSharedMaterial != null)
        {
            tmp.outlineWidth = 0f;
            tmp.fontSharedMaterial.DisableKeyword("OUTLINE_ON");
            tmp.UpdateMeshPadding();
            tmp.SetAllDirty();
        }
    }
}
