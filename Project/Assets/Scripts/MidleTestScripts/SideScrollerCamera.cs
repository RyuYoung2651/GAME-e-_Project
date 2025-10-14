using UnityEngine;

public class SideScrollerCamera : MonoBehaviour
{
	[Header("Target")]
	public Transform target;

	[Header("Follow Settings")]
	public bool followX = true;
	public bool followY = true;
	public float smoothTime = 0.15f;

	[Header("Bounds (optional)")]
	public bool clampX = false;
	public float minX = -Mathf.Infinity;
	public float maxX = Mathf.Infinity;
	public bool clampY = false;
	public float minY = -Mathf.Infinity;
	public float maxY = Mathf.Infinity;

	[Header("View Lock")]
	public float fixedZ = -10f;                 // keep camera at this Z
	public Vector3 fixedRotationEuler = new Vector3(0f, 0f, 0f); // face one direction

	[Header("Offset")]
	public Vector2 offset = new Vector2(2f, 3.5f); // raise camera a bit higher

	private Vector3 currentVelocity;

	void LateUpdate()
	{
		if (target == null) return;

		Vector3 desired = transform.position;
		if (followX)
		{
			desired.x = target.position.x + offset.x;
		}
		if (followY)
		{
			desired.y = target.position.y + offset.y;
		}

		if (clampX)
		{
			desired.x = Mathf.Clamp(desired.x, minX, maxX);
		}
		if (clampY)
		{
			desired.y = Mathf.Clamp(desired.y, minY, maxY);
		}

		desired.z = fixedZ;

		transform.position = Vector3.SmoothDamp(transform.position, desired, ref currentVelocity, smoothTime);
		transform.rotation = Quaternion.Euler(fixedRotationEuler);
	}
}
