using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// GhostTurnScreen.cs
/// Attach to a Canvas GameObject in your Ghost Turn scene.
///
/// INSPECTOR SETUP:
/// - titleText       → TMP_Text component on your "Ghost Begin your turn!" label
/// - startButton     → Button component on the Start button
/// - nextSceneName   → Name of the scene to load when Start is pressed
/// - flickerInterval → Seconds between title flicker effects (default 6)
/// </summary>
public class GhostTurnScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMPro.TMP_Text titleText;
    [SerializeField] private Button startButton;

    [Header("Scene Navigation")]
    [SerializeField] private string nextSceneName = "GhostTurnActive";

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

    // ─────────────────────────────────────────────
    // Ghostly flicker effect on the title text
    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flickerInterval);

            // Quick dip 1
            SetTitleAlpha(flickerMinAlpha);
            yield return new WaitForSeconds(flickerDuration);
            SetTitleAlpha(1f);
            yield return new WaitForSeconds(0.05f);

            // Quick dip 2 (slightly deeper)
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

    // ─────────────────────────────────────────────
    void OnDestroy()
    {
        StopAllCoroutines();
        startButton.onClick.RemoveAllListeners();
    }
}
