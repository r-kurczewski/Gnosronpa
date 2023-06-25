using DG.Tweening;
using Gnosronpa.Common;
using Gnosronpa.ScriptableObjects;
using System;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class DebateStatement : MonoBehaviour
	{
		public DebateStatementData data;

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

			var animation = data.animationData;
			transform.localPosition = animation.startPosition;
			transform.localRotation = Quaternion.Euler(0, 0, animation.startRotation);

			var animationDuration = CalculateAnimationDuration(data.animationData);

			var seq = DOTween.Sequence();

			seq.Join(transform.DOLocalMove(animation.endPosition, animation.moveDuration))
				.Join(transform.DOLocalRotate(Vector3.forward * animation.endRotation, animation.rotationDuration))
				.Join(transform.DOScale(new Vector3(animation.endScale.x, animation.endScale.y, 1), animation.scaleDuration))
				//.AppendInterval(data.durationTime - animationDuration)
				.Append(DOTween.ToAlpha(() => text.color, (color) => text.color = color, 0, data.disappearTime))
				.onComplete = () =>
				{
					DOTween.Kill(transform);
					Destroy(gameObject);
				};
		}

		private float CalculateAnimationDuration(Animation2DData animationData)
		{
			return default;
			//throw new NotImplementedException();
		}

		private void OnTriggerEnter(Collider other)
		{
			var bullet = other.GetComponent<TruthBullet>();
			Debug.Log($"[{name}] hit with [{bullet.name}], correct: [{bullet.data == data.correctBullet}]");
		}
	}
}
