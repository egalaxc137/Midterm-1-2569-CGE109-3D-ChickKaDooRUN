using UnityEngine;
using System.Collections.Generic;


public class TileSpawner : MonoBehaviour
{
    public List<Transform> tiles;
    public float tileLength = 30f;
    public Transform playerTransform;

    private void Update()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null && !player.isGameStarted) return;


        if (playerTransform.position.z - tileLength > tiles[0].position.z)
        {
            Transform movedTile = tiles[0];
            tiles.RemoveAt(0);

            float nextSpawnZ = tiles[tiles.Count - 1].position.z + tileLength;

            movedTile.position = new Vector3(movedTile.position.x,movedTile.position.y,nextSpawnZ);

            tiles.Add(movedTile);

            ObjectRandomizer randomizer = movedTile.GetComponent<ObjectRandomizer>();
            if (randomizer != null)
            {
                randomizer.RandomizeObjects();
            }
        }

    }
}
