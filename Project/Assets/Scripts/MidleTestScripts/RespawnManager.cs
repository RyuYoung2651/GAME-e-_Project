using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
	public static RespawnManager Instance { get; private set; }

	[Header("Respawn Settings")]
	public GameObject playerPrefab;
	public Transform respawnPoint;
	public float defaultDelay = 1.0f;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		// Optionally persist across scenes
		// DontDestroyOnLoad(gameObject);
	}

	public void RequestRespawn(float delay)
	{
		StartCoroutine(RespawnCoroutine(delay > 0f ? delay : defaultDelay));
	}

	private IEnumerator RespawnCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (playerPrefab == null)
		{
			Debug.LogWarning("RespawnManager: playerPrefab not set; cannot respawn.");
			yield break;
		}
		Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : Vector3.zero;
		GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

		// Rebind camera target if side scroller camera is present
		if (Camera.main != null)
		{
			var follow = Camera.main.GetComponent<SideScrollerCamera>();
			if (follow != null)
			{
				follow.target = newPlayer.transform;
			}
		}
	}
}
