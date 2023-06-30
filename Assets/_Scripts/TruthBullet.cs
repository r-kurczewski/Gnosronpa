using Gnosronpa.ScriptableObjects;
using TMPro;
using UnityEngine;

public class TruthBullet : MonoBehaviour
{
	public TruthBulletData data;
	private Rigidbody rb;
	private TMP_Text text;
	private RectTransform rt;

	[SerializeField]
	private Vector3 localMoveDirection;
	private float speed;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		text = GetComponent<TMP_Text>();
		rt = GetComponent<RectTransform>();
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		name = data.name;
		text.text = data.bulletName;
	}

	private void FixedUpdate()
	{
		rb.velocity = transform.parent.TransformDirection(localMoveDirection) * speed;
	}

	private void Update()
	{
		transform.localRotation = Quaternion.LookRotation(localMoveDirection) * Quaternion.Euler(0, 90, 0);

		if (rt.localPosition.z - rt.rect.width > 0)
		{
			Destroy(gameObject);
		}
	}

	public void Shoot(Vector3 srcWorldPos, Vector3 dstWorldPos, float speed)
	{
		this.speed = speed;
		transform.position = srcWorldPos;

		var direction = (dstWorldPos - srcWorldPos).normalized;
		localMoveDirection = transform.parent.InverseTransformDirection(direction);
	}
}

