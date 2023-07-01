using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class DebateStatement : MonoBehaviour
	{
		[SerializeField]
		private DebateStatementData data;

		private TMP_Text text;

		private BoxCollider boxCollider;

		private string Gradient(string text) => $"<gradient=\"Weak spot\">{text}</gradient>";

		private void Awake()
		{
			text = GetComponent<TMP_Text>();
			boxCollider = GetComponent<BoxCollider>();
		}
		private void OnTriggerEnter(Collider other)
		{
			var bullet = other.GetComponent<TruthBullet>();
			Debug.Log($"[{name}] hit with [{bullet.name}], correct: [{bullet.data == data.correctBullet}]");
		}

		public void Init(StatementConfigurationData configuration)
		{
			data = configuration.statement;
			var animation = configuration.statementAnimation;
			var transition = configuration.statementTransition;

			name = data.name;
			text.text = string.Format(data.textTemplate, Gradient(data.weakSpotText));

			boxCollider.center = data.collider.center;
			boxCollider.size = data.collider.extents; // correct

			transform.localPosition = animation.startPosition;
			transform.localRotation = Quaternion.Euler(0, 0, animation.startRotation);
			transform.localScale = new Vector3(animation.startScale.x, animation.startScale.y, 1);

			var seq = DOTween.Sequence()
				.Append(DOTween.ToAlpha(() => text.color, (color) => text.color = color, 1, transition.appearTime));

			if (animation.moveDuration > 0)
			{
				seq.Join(transform.DOLocalMove(animation.endPosition, animation.moveDuration));
			}

			if (animation.rotationDuration > 0)
			{
				seq.Join(transform.DOLocalRotate(Vector3.forward * animation.endRotation, animation.rotationDuration));
			}

			if (animation.scaleDuration > 0)
			{
				seq.Join(transform.DOScale(new Vector3(animation.endScale.x, animation.endScale.y, 1), animation.scaleDuration));
			}

			seq.AppendInterval(transition.waitingTime)
			.Append(DOTween.ToAlpha(() => text.color, (color) => text.color = color, 0, transition.disappearTime))
			.onComplete = () =>
			{
				DOTween.Kill(transform);
				Destroy(gameObject);
			};
		}
	}
}
