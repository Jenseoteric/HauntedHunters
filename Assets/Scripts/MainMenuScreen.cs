using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button beginButton;
    [SerializeField] private Button chooseMapButton;

    [Header("Scene Navigation")]
    [SerializeField] private string gameSceneName = "GhostStart";
    [SerializeField] private string mapSelectSceneName = "MapSelect";

    void Start()
    {
        beginButton.onClick.AddListener(OnBegin);
        chooseMapButton.onClick.AddListener(OnChooseMap);
    }

    void OnBegin()
    {
        //start the game with the currently selected map (defaults to 0)
        SceneManager.LoadScene(gameSceneName);
    }

    void OnChooseMap()
    {
        SceneManager.LoadScene(mapSelectSceneName);
    }

    void OnDestroy()
    {
        beginButton.onClick.RemoveAllListeners();
        chooseMapButton.onClick.RemoveAllListeners();
    }
}
