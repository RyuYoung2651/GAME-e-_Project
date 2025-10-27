using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalZone : MonoBehaviour
{
	[Header("Next Scene")]
	public string nextSceneName = "Test 2";

	[Header("Settings")]
	public bool useTrigger = true;

	private void OnTriggerEnter(Collider other)
	{
		if (!useTrigger) return;
		var mario = other.GetComponentInParent<MarioController>();
		if (mario != null)
		{
			LoadNextScene();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		var mario = collision.collider.GetComponentInParent<MarioController>();
		if (mario != null)
		{
			LoadNextScene();
		}
	}

	public void LoadNextScene()
	{
		if (string.IsNullOrEmpty(nextSceneName))
		{
			Debug.LogWarning("GoalZone: nextSceneName is not set.");
			return;
		}
		Debug.Log($"Loading scene: {nextSceneName}");
		SceneManager.LoadScene(nextSceneName);
	}
}

