using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    public Collectible collectible;

    private CollectibleManager collectibleManager;

    private void Start()
    {
        collectibleManager = FindFirstObjectByType<CollectibleManager>();
        if (collectibleManager == null)
        {
            Debug.LogError("AchievementManager not found in the scene.");
            return;
        }
    }

    private void OnMouseDown()
    {
        if (!collectible.isCollected)
        {
            collectibleManager.UnlockCollectible(collectible.id);
            collectible.isCollected = true;
            collectible.collectedDate = System.DateTime.Now;

            if (collectible.type == Collectible.CollectibleType.Items || collectible.type == Collectible.CollectibleType.Lores)
            {
                gameObject.SetActive(false);
            }
        }
    }
}