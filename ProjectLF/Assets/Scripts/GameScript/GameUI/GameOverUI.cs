using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Image gameOverBackground;
    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private float fadeDuration = 1.5f;
    [SerializeField]
    private float targetAlpha = 0.75f;

    private void Awake()
    {
        gameOverBackground.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        Debug.Log("[GameOverUI] ShowGameOver 호출됨");
        Debug.Log($"[GameOverUI] this activeSelf: {gameObject.activeSelf}");
        Debug.Log($"[GameOverUI] this activeInHierarchy: {gameObject.activeInHierarchy}");
        Debug.Log($"[GameOverUI] background null?: {gameOverBackground == null}");
        Debug.Log($"[GameOverUI] panel null?: {gameOverPanel == null}");

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("[GameOverUI] GameOverUI 오브젝트가 비활성화 상태라 코루틴 실행 불가");
            return;
        }
        StartCoroutine(GameOverRoutine());
    }

    public IEnumerator GameOverRoutine()
    {
        Debug.Log("[GameOverUI] 코루틴 시작");

        if (gameOverBackground == null)
        {
            Debug.LogError("[GameOverUI] gameOverBackground가 연결 안 됨");
            yield break;
        }

        if (gameOverPanel == null)
        {
            Debug.LogError("[GameOverUI] gameOverPanel이 연결 안 됨");
            yield break;
        }

        Debug.Log($"[GameOverUI] Background activeSelf 전: {gameOverBackground.gameObject.activeSelf}");
        Debug.Log($"[GameOverUI] Panel activeSelf 전: {gameOverPanel.activeSelf}");


        gameOverBackground.gameObject.SetActive(true);

        Debug.Log($"[GameOverUI] Background activeSelf 후: {gameOverBackground.gameObject.activeSelf}");
        Debug.Log($"[GameOverUI] Background activeInHierarchy 후: {gameOverBackground.gameObject.activeInHierarchy}");

        float timer = 0f;
        Color color = gameOverBackground.color;
        color.a = 0f;
        gameOverBackground.color = color;

        Debug.Log($"[GameOverUI] Fade 시작 / fadeDuration: {fadeDuration}, targetAlpha: {targetAlpha}");

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(0f, targetAlpha, timer / fadeDuration);
            gameOverBackground.color = new Color(color.r, color.g, color.b, alpha);

            Debug.Log($"[GameOverUI] Fading... timer: {timer:F2}, alpha: {alpha:F2}");


            yield return null;
        }
        gameOverPanel.SetActive(true);
        Debug.Log($"[GameOverUI] Panel SetActive(true) 실행");
        Debug.Log($"[GameOverUI] Panel activeSelf 후: {gameOverPanel.activeSelf}");
        Debug.Log($"[GameOverUI] Panel activeInHierarchy 후: {gameOverPanel.activeInHierarchy}");

        Time.timeScale = 0f;

        Debug.Log("[GameOverUI] Time.timeScale = 0 실행");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

}
