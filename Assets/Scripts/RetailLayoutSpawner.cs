using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class RetailLayoutSpawner : MonoBehaviour
{
    [Header("Retail Prefabs")]
    public GameObject shelfPrefab;
    public GameObject wallPanelPrefab;
    public GameObject checkoutPrefab;

    void Start()
    {
        if (MRUK.Instance.IsInitialized)
        {
            OnSceneLoaded(); // already loaded, call immediately
        }
        else
        {
            MRUK.Instance.RegisterSceneLoadedCallback(OnSceneLoaded);
        }
    }

    void OnSceneLoaded()
    {
        Debug.Log("OnSceneLoaded fired");
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        Debug.Log(room == null ? "Room is NULL" : $"Room loaded: {room.name}");
        if (room == null)
        {
            Debug.LogError("No room loaded from MRUK.");
            return;
        }

        foreach (MRUKAnchor anchor in room.Anchors)
        {
            SpawnRetailContent(anchor);
        }
    }

    void SpawnRetailContent(MRUKAnchor anchor)
    {
        if (anchor.Label == MRUKAnchor.SceneLabels.TABLE)
        {
            SpawnOnSurface(shelfPrefab, anchor);
        }
        else if (anchor.Label == MRUKAnchor.SceneLabels.WALL_FACE)
        {
            SpawnOnWall(wallPanelPrefab, anchor);
        }
    }

    void SpawnOnSurface(GameObject prefab, MRUKAnchor anchor)
    {
        if (prefab == null) return;

        Vector3 surfaceTop;

        if (anchor.VolumeBounds.HasValue)
        {
            // VolumeBounds is in local space — half the Y gives you the top face
            float halfHeight = anchor.VolumeBounds.Value.size.y * 0.5f;
            surfaceTop = anchor.transform.position + anchor.transform.up * halfHeight;
        }
        else
        {
            surfaceTop = anchor.transform.position + anchor.transform.up * 0.4f;
        }

        //Quaternion spawnRot = anchor.transform.rotation * Quaternion.Euler(90, 0, 0);

        GameObject obj = Instantiate(prefab, surfaceTop, Quaternion.Euler(0, 0, 0));
        obj.transform.SetPositionAndRotation(anchor.transform.position, Quaternion.Euler(0, 90, 0));

        // Scale shelf to match the actual desk width
        //if (anchor.VolumeBounds.HasValue)
        //{
        //    GetWorldTableDimensions(anchor, out float deskWidth, out float deskDepth);
        //    obj.transform.localScale = new Vector3(deskWidth * (1f / obj.transform.localScale.z), obj.transform.localScale.y, deskDepth * (1f / obj.transform.localScale.z));
        //}
    }

    void SpawnOnWall(GameObject prefab, MRUKAnchor anchor)
    {
        if (prefab == null) return;

        Vector3 spawnPos = anchor.transform.position + anchor.transform.forward * 0.01f;
        Quaternion spawnRot = anchor.transform.rotation * Quaternion.Euler(0, 180, 0);

        GameObject obj = Instantiate(prefab, spawnPos, spawnRot);
        obj.transform.SetParent(anchor.transform);

        if (anchor.PlaneRect.HasValue)
        {
            float wallWidth = anchor.PlaneRect.Value.width;
            float wallHeight = anchor.PlaneRect.Value.height;
            obj.transform.localScale = new Vector3(wallWidth * 0.4f, wallHeight * 0.2f, 1f);
        }
    }

    void GetWorldTableDimensions(MRUKAnchor anchor, out float worldWidth, out float worldDepth)
    {
        worldWidth = 1.5f; 
        worldDepth = 0.6f;

        if (!anchor.VolumeBounds.HasValue) return;

        Bounds b = anchor.VolumeBounds.Value;

        // Transform all 4 top-face corners into world space
        Vector3 c0 = anchor.transform.TransformPoint(new Vector3(b.min.x, b.max.y, b.min.z));
        Vector3 c1 = anchor.transform.TransformPoint(new Vector3(b.max.x, b.max.y, b.min.z));
        Vector3 c2 = anchor.transform.TransformPoint(new Vector3(b.min.x, b.max.y, b.max.z));
        Vector3 c3 = anchor.transform.TransformPoint(new Vector3(b.max.x, b.max.y, b.max.z));

        // Measure world X and Z extents from those corners
        float minX = Mathf.Min(c0.x, c1.x, c2.x, c3.x);
        float maxX = Mathf.Max(c0.x, c1.x, c2.x, c3.x);
        float minZ = Mathf.Min(c0.z, c1.z, c2.z, c3.z);
        float maxZ = Mathf.Max(c0.z, c1.z, c2.z, c3.z);

        worldWidth = maxX - minX;
        worldDepth = maxZ - minZ;
    }
}