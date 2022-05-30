using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIFluidContainerDisplay : MonoBehaviour
{
    private static readonly int ColorMask = Shader.PropertyToID("_MaskTexture");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int ThreadsPerGroup = 32;
    private const int MaskResolution = 2048;

    [SerializeField] private ComputeShader _fluidSpriteMaskTextureWrite;
    [SerializeField] private Shader _displayShader;
    private readonly List<UIFluidContentsDisplay> _contentDisplays = new List<UIFluidContentsDisplay>();
    
    private ComputeBuffer _colorBuffer;
    [SerializeField] private RenderTexture _colorMaskTexture;
    private Material _displayMaterial;


    private float SumThresholdPrior(Dictionary<FluidIngredientTemplate, float> contents, List<FluidIngredientTemplate> sortedDescending, int i)
    {
        var sum = 0.0f;

        while (i >= 0)
        {
            sum += contents[sortedDescending[i]];
            i--;
        }

        return sum;
    }
    
    private Vector3[] GetColorMap(Dictionary<FluidIngredientTemplate, float> contents, float totalFillPercent)
    {
        var colorArray = new Vector3[MaskResolution];
        var keysSortedByValueMin = contents
            .OrderByDescending(t => t.Value)
            .Select(t => t.Key)
            .ToList();

        var thresholds = new float[contents.Count];
        for (var i = 0; i < keysSortedByValueMin.Count; i++)
            thresholds[i] = SumThresholdPrior(contents, keysSortedByValueMin, i);

        for (var i = 0; i < MaskResolution; i++)
        {
            var slice = i / (float)MaskResolution;
            var color = colorArray[i];

            if (slice > totalFillPercent) break;
            
            for (var j = 0; j < thresholds.Length; j++)
            {
                var t = thresholds[j];

                if (t < slice) continue;

                var sideColor = keysSortedByValueMin[j].SideColor;
                color = new Vector3(sideColor.r, sideColor.g, sideColor.b);
                break;
            }

            colorArray[i] = color;
        }

        return colorArray;
    }

    private void UpdateGraphic(Dictionary<FluidIngredientTemplate, float> contents, float totalFillPercent, Image imageSprite)
    {
        if (_displayMaterial == null)
            _displayMaterial = new Material(_displayShader);
        
        _colorBuffer?.Release();
        if(_colorMaskTexture != null)
            _colorMaskTexture.Release();

        var resolution = new Vector2Int(imageSprite.sprite.texture.width, imageSprite.sprite.texture.height);
        _colorMaskTexture = new RenderTexture(resolution.x, resolution.y, 0)
        {
            dimension = TextureDimension.Tex2D, enableRandomWrite = true
        };

        _colorBuffer = new ComputeBuffer(MaskResolution, sizeof(float) * 3);
        _colorBuffer.SetData(GetColorMap(contents, totalFillPercent));
        
        var groupCount = new Vector2Int(
            Mathf.CeilToInt(resolution.x / (float) ThreadsPerGroup),
            Mathf.CeilToInt(resolution.y / (float) ThreadsPerGroup)
        );


        var textureMaskGeneratorKernel = _fluidSpriteMaskTextureWrite.FindKernel("TextureMaskGenerator");  
        _fluidSpriteMaskTextureWrite.SetInt("_Width", resolution.x);
        _fluidSpriteMaskTextureWrite.SetInt("_Height", resolution.y);
        _fluidSpriteMaskTextureWrite.SetInt("_MaskResolution", MaskResolution);
        _fluidSpriteMaskTextureWrite.SetBuffer(textureMaskGeneratorKernel, "_HeightColorData", _colorBuffer);
        _fluidSpriteMaskTextureWrite.SetTexture(textureMaskGeneratorKernel, "_ResultTexture", _colorMaskTexture);
        _fluidSpriteMaskTextureWrite.Dispatch(textureMaskGeneratorKernel, groupCount.x, groupCount.y, 1);
        
        _displayMaterial.SetTexture(ColorMask, _colorMaskTexture);
        _displayMaterial.SetTexture(MainTex, imageSprite.sprite.texture);

        imageSprite.material = _displayMaterial;
    }
    
    public void SetFluidContents(Dictionary<FluidIngredientTemplate, float> contents, float totalFillPercent, Image sprite)
    {
        UpdateGraphic(contents, totalFillPercent, sprite);
    }

    public void Clear()
    {
        _contentDisplays.ForEach(t => Destroy(t.gameObject));
        _contentDisplays.Clear();
    }
}