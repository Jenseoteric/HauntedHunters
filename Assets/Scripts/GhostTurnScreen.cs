using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class GhostTurnScreen : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text titleText;
    [SerializeField] private Button startButton;
    [SerializeField] private string nextSceneName = "GhostTurnActive";
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
