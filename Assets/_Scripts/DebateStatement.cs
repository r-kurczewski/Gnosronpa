using DG.Tweening;
using DG.Tweening.Core;
using Gnosronpa.Assets._Scripts.Common;
using Gnosronpa.Controllers;
using Gnosronpa.ScriptableObjects;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Gnosronpa
{
	public class DebateStatement : MonoBehaviour
	{
		public delegate void TruthBulletHitBehaviour(TruthBullet bullet);

		public event TruthBulletHitBehaviour OnCorrectBulletHit;

		[SerializeField]
		private DebateStatementData data;

		[SerializeField]
		private AudioClip incorrectHitSound;

		[SerializeField]
		private AudioClip correctHitSound;

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
				OnCorrectHit(bullet);
			}
			else
			{
				OnIncorrectHit(bullet);
			}

			Debug.Log($"[{name}] hit with [{bullet.name}], correct: [{isCorrect}]");
		}

		private void OnCorrectHit(TruthBullet bullet)
		{
			StartCoroutine(ICorrectHit(bullet));

			IEnumerator ICorrectHit(TruthBullet bullet)
			{
				//AudioController.instance.PlaySound(correctHitSound);
				//yield return new WaitForSecondsRealtime(correctHitSound.length);
				OnCorrectBulletHit?.Invoke(bullet);
				yield return null;
			}
		}

		private void OnIncorrectHit(TruthBullet bullet)
		{
			AudioController.instance.PlaySound(incorrectHitSound);
			transform.BlendableShake(Vector3.one * 8, 1f, 5);
		}


		public void Init(DebateSequenceData data)
		{
			this.data = data.statement;
			var animation = data.statementAnimation;
			var transition = data.statementTransition;

			name = this.data.name;

			text.text = string.Format(this.data.textTemplate, Gradient(this.data.weakSpotText));
			boxCollider.center = this.data.collider.center;
			boxCollider.size = this.data.collider.extents; // correct behaviour

			transform.localPosition = animation.startPosition;
			transform.localRotation = Quaternion.Euler(0, 0, animation.startRotation);
			transform.localScale = new Vector3(animation.startScale.x, animation.startScale.y, 1);

			var seq = DOTween.Sequence()
				.Append(DOTween.ToAlpha(() => text.color, (color) => text.color = color, 1, transition.appearTime));

			if (animation.moveDuration > 0)
			{
				seq.Join(transform.DOBlendableLocalMoveBy(animation.move, animation.moveDuration));
			}

			if (animation.rotationDuration > 0)
			{
				seq.Join(transform.DOBlendableLocalRotateBy(Vector3.forward * animation.rotation, animation.rotationDuration));
			}

			if (animation.scaleDuration > 0)
			{
				seq.Join(transform.DOBlendableScaleBy(new Vector3(animation.scale.x, animation.scale.y, 1), animation.scaleDuration));
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
