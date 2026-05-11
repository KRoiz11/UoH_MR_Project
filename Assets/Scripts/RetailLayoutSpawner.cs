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

        Quaternion spawnRot = anchor.transform.rotation * Quaternion.Euler(90, 0, 0);

        GameObject obj = Instantiate(prefab, surfaceTop, spawnRot);
        obj.transform.SetParent(anchor.transform);

        // Scale shelf to match the actual desk width
        if (anchor.VolumeBounds.HasValue)
        {
            float deskWidth = anchor.VolumeBounds.Value.size.x;
            float deskDepth = anchor.VolumeBounds.Value.size.z;
            //obj.transform.localScale = new Vector3(deskWidth, obj.transform.localScale.y, deskDepth);
        }
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
}