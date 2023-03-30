using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct PrefabSettings {
    public GameObject prefab;
    
    public int density;
    public float minHeight;
    public float maxHeight;
    public int maxPrefabs;
}

public class PlacementGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs;
    public PrefabSettings[] settingsList;

    [Header("Raycast Settings")]
    [SerializeField] int density;

    [Space]

    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;

    [Header("Prefab Variation Settings")]
    [SerializeField, Range(0f, 1f)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;
    [SerializeField] Vector3 minScale;
    [SerializeField] Vector3 maxScale;

    private void Start()
    {
        settingsList = new PrefabSettings[prefabs.Count];
        for (int i = 0; i < prefabs.Count; ++i)
        {
            settingsList[i] = new PrefabSettings();
            settingsList[i].prefab = prefabs[i];
        }
    }

#if UNITY_EDITOR
    public void Generate()
    {
        Clear();
        for (int prefab_i = 0; prefab_i < prefabs.Count; prefab_i++)
        {
            GameObject prefab = prefabs[prefab_i];
            PrefabSettings currentSettingList = settingsList[prefab_i];
            for (int i = 0; i < currentSettingList.density; ++i)
            {
                float minHeight = currentSettingList.minHeight;
                float maxHeight = currentSettingList.maxHeight;

                float sampleX = Random.Range(xRange.x, xRange.y);
                float sampleY = Random.Range(zRange.x, zRange.y);
                Vector3 rayStart = new(sampleX, maxHeight, sampleY);

                if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                    continue;

                if (hit.point.y < minHeight)
                    continue;

                GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);

                instantiatedPrefab.transform.SetParent(transform);
                instantiatedPrefab.transform.position = hit.point;
                instantiatedPrefab.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);
                instantiatedPrefab.transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.FromToRotation(instantiatedPrefab.transform.up, hit.normal), rotateTowardsNormal);
                instantiatedPrefab.transform.localScale = new Vector3(
                    Random.Range(minScale.x, maxScale.x),
                    Random.Range(minScale.y, maxScale.y),
                    Random.Range(minScale.z, maxScale.z)
                );

                currentSettingList.maxPrefabs++;
            }
        }
    }

    public void Clear()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

#endif
}

