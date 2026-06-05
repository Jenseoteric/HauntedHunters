using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// MapSelectScreen.cs
/// Lets the player pick which preset maze to play.
/// Selecting a map sets GameManager.selectedMap and starts the game.
///
/// SCENE NAME: "MapSelect"
///
/// INSPECTOR SETUP:
/// - map1Button     -> Button for the first preset (PresetBoard)
/// - map2Button     -> Button for the second preset (PresetBoard2)
/// - backButton     -> Button to return to the main menu (optional)
/// - gameSceneName  -> "GhostStart"
/// - menuSceneName  -> "MainMenu"
/// </summary>
public class MapSelectScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button map1Button;
    [SerializeField] private Button map2Button;
    [SerializeField] private Button backButton;

    [Header("Scene Navigation")]
    [SerializeField] private string gameSceneName = "GhostStart";
    [SerializeField] private string menuSceneName = "MainMenu";

    void Start()
    {
        map1Button.onClick.AddListener(() => SelectMap(0));
        map2Button.onClick.AddListener(() => SelectMap(1));

        //back button is optional, only wire it if assigned
        if (backButton != null)
            backButton.onClick.AddListener(OnBack);
    }

    //set which preset GameManager will load, then start the game
    void SelectMap(int mapIndex)
    {
        //selectedMap is static so it survives the scene load into GhostStart
        GameManager.selectedMap = mapIndex;
        SceneManager.LoadScene(gameSceneName);
    }

    void OnBack()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    void OnDestroy()
    {
        map1Button.onClick.RemoveAllListeners();
        map2Button.onClick.RemoveAllListeners();
        if (backButton != null)
            backButton.onClick.RemoveAllListeners();
    }
}
