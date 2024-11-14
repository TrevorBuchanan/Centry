using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    private NetworkRunner _runner;

    private async void Start()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestRoom",
            Scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("Connected to Photon Fusion.");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }
    }
}
