using UnityEngine;

public class ObjectRandomizer : MonoBehaviour
{
    [Header("Items to Randomize")]
    public GameObject[] spawnableObjects; 

    [Header("Spawn Chance")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f; 
    public void RandomizeObjects()
    {
        if (spawnableObjects == null || spawnableObjects.Length == 0) return;

        foreach (GameObject obj in spawnableObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        int randomIndex = Random.Range(0, spawnableObjects.Length);
        GameObject selectedObject = spawnableObjects[randomIndex];

        
        if (selectedObject != null)
        {
            // à»Ô´µÑÇÇÑµ¶Ø¾èÍµÑÇËÅÑ¡
            selectedObject.SetActive(true);

        
            Transform[] allChildren = selectedObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}