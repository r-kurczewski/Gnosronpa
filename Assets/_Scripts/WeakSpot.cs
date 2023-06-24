using DG.Tweening;
using Fangan.ScriptableObjects;
using TMPro;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
	public WeakSpotData data;

	private TMP_Text text;

	private BoxCollider boxCollider;

	private Time time;

	private string Gradient(string text) => $"<gradient=\"Weak spot\">{text}</gradient>";

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
		boxCollider = GetComponent<BoxCollider>();
	}

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		name = data.name;
		text.text = string.Format(data.textTemplate, Gradient(data.weakSpotText));

		boxCollider.center = data.collider.center;
		boxCollider.size = data.collider.extents; // correct

		transform.localPosition = data.startPosition;
		transform.rotation = Quaternion.Euler(0, 0, data.startRotation);

		transform.DOLocalMove(data.endPosition, data.moveDuration);
		transform.DOLocalRotate(Vector3.forward * data.endRotation, data.rotationDuration);
		transform.DOScale(new Vector3(data.endScale.x, data.endScale.y, 1), data.scaleDuration);


		var seq = DOTween.Sequence();
		seq.target = transform;
		seq.AppendInterval(data.durationTime)
		.Append(DOTween.ToAlpha(() => text.color, (color) => text.color = color, 0, data.disappearTime))
		.onComplete = () =>
		{
			DOTween.Kill(transform);
			Destroy(gameObject);
		};
		;
	}

	private void OnTriggerEnter(Collider other)
	{
		var bullet = other.GetComponent<TruthBullet>();
		Debug.Log($"[{name}] hit with [{bullet.name}], correct: [{bullet.data == data.correctBullet}]");
	}
}
