using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using System;
using TMPro;
using UnityEngine;
using static Gnosronpa.ScriptableObjects.DebateStatementData;

namespace Gnosronpa
{
	public class DebateStatement : MonoBehaviour
	{
		public delegate void TruthBulletHitBehaviour(TruthBullet bullet);

		public event TruthBulletHitBehaviour OnCorrectBulletHit;

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
			var isCorrect = IsCorrectBullet(bullet.Data);

			if (isCorrect)
			{
				OnCorrectBulletHit?.Invoke(bullet);
			}

			Debug.Log($"[{name}] hit with [{bullet.name}], correct: [{isCorrect}]");
		}


		public void Init(DebateSequenceData data)
		{
			this.data = data.statement;
			var animation = data.statementAnimation;
			var transition = data.statementTransition;

			name = this.data.name;

			if (data.statement.statementType == StatementType.Normal)
			{
				text.text = this.data.textTemplate;
				boxCollider.enabled = false;
			}
			else
			{
				text.text = string.Format(this.data.textTemplate, Gradient(this.data.weakSpotText));
				boxCollider.center = this.data.collider.center;
				boxCollider.size = this.data.collider.extents; // correct behaviour
			}

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

		public bool IsCorrectBullet(TruthBulletData data)
		{
			return this.data.correctBullet == data;
		}
	}
}
