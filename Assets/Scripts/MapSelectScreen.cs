using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MapSelectScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button map1Button;
    [SerializeField] private Button map2Button;

    [SerializeField] private Button map3Button;
    [SerializeField] private Button backButton;

    [Header("Scene Navigation")]
    [SerializeField] private string gameSceneName = "GhostStart";
    [SerializeField] private string menuSceneName = "MainMenu";

    void Start()
    {
        map1Button.onClick.AddListener(() => SelectMap(0));
        map2Button.onClick.AddListener(() => SelectMap(1));
        map3Button.onClick.AddListener(() => SelectMap(2));
        if (backButton != null)
            backButton.onClick.AddListener(OnBack);
    }

    //set preset
    void SelectMap(int mapIndex)
    {
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
        map3Button.onClick.RemoveAllListeners();

        if (backButton != null)
            backButton.onClick.RemoveAllListeners();
    }
}
