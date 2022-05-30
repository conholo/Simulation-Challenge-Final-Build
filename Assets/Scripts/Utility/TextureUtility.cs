using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class TextureUtility
{
    public static Texture2D Texture2DFromRenderTexture(this RenderTexture rt)
    {
        var oldRenderTexture = RenderTexture.active;
        var texture = new Texture2D(rt.width, rt.height, rt.graphicsFormat, TextureCreationFlags.None);
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();
        RenderTexture.active = oldRenderTexture;

        return texture;
    }
}