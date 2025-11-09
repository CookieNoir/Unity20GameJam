using System.Collections.Generic;
using UnityEngine;

public class WaterFlowDrawer : MonoBehaviour
{
    [SerializeField] private WaterFlow _waterFlow;
    [SerializeField] private Renderer _renderer;
    [SerializeField, Min(0)] private int _materialIndex = 0;
    [SerializeField] private string _waterMaskPropertyName = string.Empty;
    [SerializeField] private string _activityMaskPropertyName = string.Empty;
    [SerializeField] private FilterMode _texturesFilterMode = FilterMode.Bilinear;
    [SerializeField] private TextureWrapMode _texturesWrapMode = TextureWrapMode.Clamp;
    private Texture2D _waterTexture;
    private Texture2D _activityTexture;
    private MaterialPropertyBlock _materialPropertyBlock;

    private void OnEnable()
    {
        if (_waterFlow != null)
        {
            _waterFlow.OnPointsChanged?.AddListener(UpdateTextures);
            UpdateTextures();
        }
    }

    private void OnDisable()
    {
        if (_waterFlow != null)
        {
            _waterFlow.OnPointsChanged?.RemoveListener(UpdateTextures);
        }
    }

    private void UpdateTextures()
    {
        if (_waterFlow == null)
        {
            return;
        }
        var pointsActivity = _waterFlow.PointsActivity;
        var pointsWater = _waterFlow.PointsWater;
        int expectedWidth = pointsActivity != null ? pointsActivity.Count : 0;
        if (expectedWidth == 0)
        {
            SetTextures(Texture2D.blackTexture, Texture2D.blackTexture);
            return;
        }
        if (_waterTexture == null ||
            _waterTexture.width != pointsActivity.Count)
        {
            DestroyOldTextures();
            _waterTexture = CreateTexture(expectedWidth);
            _activityTexture = CreateTexture(expectedWidth);
        }
        FillTexture(_waterTexture, pointsWater);
        FillTexture(_activityTexture, pointsActivity);
        SetTextures(_waterTexture, _activityTexture);
    }

    private Texture2D CreateTexture(int width)
    {
        var texture = new Texture2D(width, 1, TextureFormat.R8, false);
        texture.filterMode = _texturesFilterMode;
        texture.wrapMode = _texturesWrapMode;
        return texture;
    }

    private void FillTexture(Texture2D texture, IReadOnlyList<bool> values)
    {
        var pixelData = texture.GetPixelData<byte>(0);
        int count = values.Count;
        for (int i = 0; i < count; ++i)
        {
            pixelData[i] = values[i] ? byte.MaxValue : byte.MinValue;
        }
        texture.Apply();
    }

    private void SetTextures(Texture2D waterTexture, Texture2D activityTexture)
    {
        if (_renderer == null ||
            !_renderer.HasMaterial(_materialIndex))
        {
            return;
        }
        if (_materialPropertyBlock == null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetTexture(_waterMaskPropertyName, waterTexture);
        _materialPropertyBlock.SetTexture(_activityMaskPropertyName, activityTexture);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void DestroyOldTextures()
    {
        if (_waterTexture != null)
        {
            Destroy(_waterTexture);
        }
        if (_activityTexture != null)
        {
            Destroy(_activityTexture);
        }
    }

    private void OnDestroy()
    {
        DestroyOldTextures();
    }
}
