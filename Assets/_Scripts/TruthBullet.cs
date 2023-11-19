using Gnosronpa.Scriptables;
using TMPro;
using UnityEngine;

public class TruthBullet : MonoBehaviour
{
	[SerializeField]
	private TruthBulletData data;

	private Rigidbody rb;
	private RectTransform rt;
	private TMP_Text text;

	private Vector3 localMoveDirection;
	private float speed;

	public TruthBulletData Data => data;

	public bool HitObject { get; set; }

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		text = GetComponent<TMP_Text>();
		rt = GetComponent<RectTransform>();
	}

	private void FixedUpdate()
	{
		rb.velocity = transform.parent.TransformDirection(localMoveDirection) * speed;
	}

	//private void OnTriggerEnter(Collider other)
	//{
	//	Debug.Log($"Bullet hit {other.name}");
	//}

	private void Update()
	{
		transform.localRotation = Quaternion.LookRotation(localMoveDirection) * Quaternion.Euler(0, 90, 0);

		if (rt.localPosition.z - rt.rect.width > 0)
		{
			Destroy(gameObject);
		}
	}

	public void Init(TruthBulletData data)
	{
		this.data = data;
		name = data.name;
		text.text = data.bulletName;
		HitObject = false;
	}

	public void Shoot(Vector3 srcWorldPos, Vector3 dstWorldPos, float speed)
	{
		this.speed = speed;
		transform.position = srcWorldPos;

		var direction = (dstWorldPos - srcWorldPos).normalized;
		localMoveDirection = transform.parent.InverseTransformDirection(direction);
	}
}

