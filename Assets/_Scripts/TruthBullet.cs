using UnityEngine;

public class TruthBullet : MonoBehaviour
{
	public float speed;

	private Rigidbody rb;

	public float hitPlane;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (transform.localPosition.z > 0)
		{
			Destroy(gameObject);
		}
	}

	public void Shoot(Vector3 srcWorldPos, Vector3 dstWorldPos)
	{
		transform.position = srcWorldPos;

		var direction = (dstWorldPos - srcWorldPos).normalized;
		transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0,90,0);
		rb.AddForce(direction * speed * Time.deltaTime);
	}
}

