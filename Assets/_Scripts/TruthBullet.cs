using Fangan.ScriptableObjects;
using System;
using TMPro;
using UnityEngine;

public class TruthBullet : MonoBehaviour
{
	public TruthBulletData data;
	private Rigidbody rb;
	private TMP_Text text;
	private RectTransform rt;

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

	private void Update()
	{
		if (rt.localPosition.z - rt.rect.width > 0)
		{
			Destroy(gameObject);
		}
	}

	public void Shoot(Vector3 srcWorldPos, Vector3 dstWorldPos, float speed)
	{
		transform.position = srcWorldPos;

		var direction = (dstWorldPos - srcWorldPos).normalized;
		transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0,90,0);
		rb.AddForce(direction * speed);
	}
}

