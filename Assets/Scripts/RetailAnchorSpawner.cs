using Meta.XR.MRUtilityKit;
using UnityEngine;

public class RetailAnchorSpawner : AnchorPrefabSpawner
{
    // CustomPrefabScaling receives the scale that Stretch would apply
    // We only want to use X and Z — ignore Y to preserve shelf height
    public override Vector3 CustomPrefabScaling(Vector3 localScale)
    {
        return new Vector3(localScale.x, 1f, localScale.z);
    }

    // CustomPrefabAlignment returns a local offset from the anchor pivot
    // We want the shelf to sit on top of the desk surface
    public override Vector3 CustomPrefabAlignment(
        Bounds anchorVolumeBounds, Bounds? prefabBounds)
    {
        // float surfaceX = anchorVolumeBounds.max.x;
        // float surfaceY = anchorVolumeBounds.max.y;
        float surfaceZ = anchorVolumeBounds.max.z;
        return new Vector3(0f, 0f, surfaceZ);
    }

    public override Vector2 CustomPrefabScaling(Vector2 localScale)
    {
        float widthProportion = 0.4f;
        float heightProportion = 0.3f;

        return new Vector2(localScale.x * widthProportion, localScale.y * heightProportion);
    }
    public override Vector3 CustomPrefabAlignment(Rect anchorPlaneRect, Bounds? prefabBounds)
    {
        return new Vector3(0f, 0f, 0.001f);
    }
}