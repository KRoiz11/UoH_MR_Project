using UnityEngine;

public class ShelfProductSpawner : MonoBehaviour
{
    public GameObject[] productPrefabs;

    void Start()
    {
        int index = 0;

        foreach (Transform child in transform)
        {
            if (!child.name.StartsWith("Slot")) continue;

            GameObject prefab = productPrefabs[index % productPrefabs.Length];
            GameObject product = Instantiate(prefab, child.position, child.rotation);

            // Parent to the anchor, not the shelf — avoids inheriting shelf scale
            product.transform.SetParent(transform.parent);
            index++;
        }
    }
}