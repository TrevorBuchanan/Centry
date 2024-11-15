using UnityEngine;
using Fusion;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private NetworkRunner networkRunner;

    private void Start()
    {
        // Attempt to get the NetworkRunner in the scene
        networkRunner = FindObjectOfType<NetworkRunner>();

        if (networkRunner == null)
        {
            Debug.LogError("NetworkRunner not found in the scene.");
            return;
        }

        // Check if the current runner has State Authority (Server role)
        if (networkRunner.IsServer)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition = Vector3.zero; // Set spawn position
        Quaternion spawnRotation = Quaternion.identity;

        // Spawn the player object
        networkRunner.Spawn(playerPrefab, spawnPosition, spawnRotation, networkRunner.LocalPlayer);
    }
}
