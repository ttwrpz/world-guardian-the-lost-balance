using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Gradient gradient;

    Texture2D texture;
    [Range(1, 256)]
    public int textureResolution = 50;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        if (texture == null || texture.width != textureResolution)
        {
            texture = new Texture2D(textureResolution, 1, TextureFormat.RGBA32, false);
        }

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
        UpdateColors(material);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        material.SetFloat("_MinHeight", minHeight);
        material.SetFloat("_MaxHeight", maxHeight);
    }

    public void UpdateColors(Material material)
    {
        Color[] colors = new Color[textureResolution];
        
        for (int i = 0; i < textureResolution; i++)
        {
            colors[i] = gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colors);
        texture.Apply();
        material.SetTexture("_WorldTexture", texture);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (autoUpdate)
        {
            NotifyOfUpdateValues();
        }
    }
#endif

}
