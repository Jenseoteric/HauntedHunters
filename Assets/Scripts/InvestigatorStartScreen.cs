using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// InvestigatorStartScreen.cs
/// The "Investigator — Begin your turn!" prompt screen.
/// Mirrors the structure of GhostTurnScreen.
///
/// SCENE NAME: "InvestigatorStart"
///
/// INSPECTOR SETUP:
/// - titleText      → TMP_Text for the heading
/// - startButton    → Button to proceed to the board
/// - nextSceneName  → "InvestigatorBoard"
/// </summary>
public class InvestigatorStartScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMPro.TMP_Text titleText;
    [SerializeField] private Button startButton;

    [Header("Scene Navigation")]
    [SerializeField] private string nextSceneName = "InvestigatorBoard";

    [Header("Flicker Settings")]
    [SerializeField] private float flickerInterval = 6f;
    [SerializeField] private float flickerDuration = 0.08f;
    [SerializeField] private float flickerMinAlpha = 0.7f;

    void Start()
    {
        startButton.onClick.AddListener(OnStartPressed);
        StartCoroutine(FlickerRoutine());
    }

    void OnStartPressed()
    {
        startButton.interactable = false;
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flickerInterval);

            SetTitleAlpha(flickerMinAlpha);
            yield return new WaitForSeconds(flickerDuration);
            SetTitleAlpha(1f);
            yield return new WaitForSeconds(0.05f);

            SetTitleAlpha(flickerMinAlpha - 0.15f);
            yield return new WaitForSeconds(flickerDuration * 0.6f);
            SetTitleAlpha(1f);
        }
    }

    void SetTitleAlpha(float alpha)
    {
        if (titleText == null) return;
        Color c = titleText.color;
        c.a = Mathf.Clamp01(alpha);
        titleText.color = c;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        startButton.onClick.RemoveAllListeners();
    }
}
