using UnityEngine;

public static class RendererExtensions
{
    public static bool IsHavingMaterial(this Renderer renderer, int materialIndex)
    {
        if (renderer == null ||
            materialIndex < 0)
        {
            return false;
        }
        var materials = renderer.sharedMaterials;
        if (materialIndex >= (materials?.Length ?? 0))
        {
            return false;
        }
        return materials[materialIndex] != null;
    }
}
