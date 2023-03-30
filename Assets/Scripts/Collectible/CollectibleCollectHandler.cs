using System;
using UnityEngine;

public class CollectibleCollectHandler : MonoBehaviour
{
    public CollectibleManager collectibleManager;
    public string collectibleId;

    private bool isCollected = false;

    private void Awake()
    {
        collectibleManager = FindFirstObjectByType<CollectibleManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("MainCamera"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                collectibleManager.UnlockCollectible(collectibleId);
                isCollected = true;
                Destroy(gameObject);
            }
        }
    }
}
