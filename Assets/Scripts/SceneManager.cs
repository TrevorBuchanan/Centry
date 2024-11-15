using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameplayScene = "Gameplay";

    [Header("Networking")]
    [SerializeField] private GameMode gameMode = GameMode.Host;

    private NetworkRunner networkRunner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist this object across scenes
    }

    /// <summary>
    /// Load a Unity scene by name without networking, with fade transition.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneTransitionManager.Instance.LoadScene(sceneName, isNetworked: false);
    }

    /// <summary>
    /// Start a Fusion networking session and transition to the gameplay scene.
    /// </summary>
    public async void StartGame()
    {
        if (networkRunner == null)
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
        }

        // Set up the Fusion session
        var startGameArgs = new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = "GameRoom",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        Debug.Log("Starting networked game...");

        // Start the Fusion session
        var result = await networkRunner.StartGame(startGameArgs);

        if (result.Ok)
        {
            // Transition to the gameplay scene with networking
            SceneTransitionManager.Instance.LoadScene(gameplayScene, isNetworked: true);
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }
    }

    /// <summary>
    /// Return to the main menu, clean up Fusion networking, and fade transition.
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneTransitionManager.Instance.LoadScene(mainMenuScene, isNetworked: true);
    }

    /// <summary>
    /// Quit the game application.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
