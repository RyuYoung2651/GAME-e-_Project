using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
	[Header("UI References")]
	public GameObject gameOverPanel;
	public Button restartButton;

	[Header("Settings")]
	public string restartSceneName = ""; // 비워두면 현재 씬으로 리로드

	private void Start()
	{
		if (gameOverPanel != null)
		{
			gameOverPanel.SetActive(false);
		}
		if (restartButton != null)
		{
			restartButton.onClick.AddListener(OnRestartClicked);
		}
	}

	public void ShowGameOver()
	{
		if (gameOverPanel != null)
		{
			gameOverPanel.SetActive(true);
		}
	}

	public void OnRestartClicked()
	{
		// 현재 씬 이름 가져오기 (restartSceneName이 비어있으면 현재 씬으로 리로드)
		string sceneToLoad = string.IsNullOrEmpty(restartSceneName) ? SceneManager.GetActiveScene().name : restartSceneName;
		Debug.Log($"Restarting scene: {sceneToLoad} (current: {SceneManager.GetActiveScene().name})");
		SceneManager.LoadScene(sceneToLoad);
	}
}

