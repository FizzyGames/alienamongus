using System;
using UnityEngine;

public static class Utility
{
    public const string s_dataUrlHeader = "data:image/png;base64,";
    public static string ConvertTexture2DToString(Texture2D texture)
    {
        Debug.Assert(texture);
        return s_dataUrlHeader + Convert.ToBase64String(texture.EncodeToPNG());
    }
    public static Texture2D ConvertStringToTexture2D(string input)
    {
        byte[] binaryData = Convert.FromBase64String(input.Substring(s_dataUrlHeader.Length));
        Texture2D tex = new Texture2D(4, 4);
        Debug.Assert(tex.LoadImage(binaryData));
        return tex;
    }
}
