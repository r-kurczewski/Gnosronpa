using DG.Tweening;
using Gnosronpa.Assets._Scripts.Common;
using Gnosronpa.Controllers;
using Gnosronpa.ScriptableObjects;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static Gnosronpa.ScriptableObjects.DebateStatementData;

namespace Gnosronpa
{
	public class DebateStatement : MonoBehaviour
	{
		public event Action<TruthBullet, DebateStatement> OnCorrectBulletHit;

		public event Action<TruthBullet, DebateStatement> OnIncorrectBulletHit;

		public event Action<TruthBullet, DebateStatement> OnIncorrectBulletHitAnimationEnded;

		private const float statementColliderThickness = 5;
		private const float statementColliderDepth = 5;

		private const float weakSpotColliderThickness = 5;
		private const float weakSpotColliderDepth = -5;

		[SerializeField]
		private DebateStatementData data;

		[SerializeField]
		private StatementCollider statementCollider;

		[SerializeField]
		private WeakSpotCollider weakSpotCollider;

		[SerializeField]
		private AudioClip incorrectHitSound;

		[SerializeField]
		private AudioClip statementHitSound;

		[SerializeField]
		private AudioClip correctHitSound;

		[SerializeField]
		private TMP_Text text;

		public DebateStatementData Data => data;

		private string Gradient(string text) => $"<gradient=\"Weak spot\">{text}</gradient>";

		public bool IsCorrectBullet(TruthBulletData data) => this.data.correctBullet == data;

		private void OnDestroy()
		{
			weakSpotCollider.OnWeakSpotHit -= OnWeakSpotHit;
			statementCollider.OnStatementHit -= OnStatementHit;
			DOTween.Kill(transform);
		}

		public void Init(DebateSequenceData sequenceData)
		{
			weakSpotCollider.OnWeakSpotHit += OnWeakSpotHit;
			statementCollider.OnStatementHit += OnStatementHit;

			data = sequenceData.statement;
			var animation = sequenceData.statementAnimation;
			var transition = sequenceData.statementTransition;

			name = data.name;
			text.text = string.Format(data.textTemplate, Gradient(data.weakSpotText));

			statementCollider.SetColliderSize(
				new Vector3(data.textCollider.center.x, data.textCollider.center.y, statementColliderDepth),
				new Vector3(data.textCollider.size.x, data.textCollider.size.y, statementColliderThickness));

			if (data.statementType is StatementType.WeakSpot)
			{
				weakSpotCollider.SetColliderSize(
					new Vector3(data.weakSpotCollider.center.x, data.weakSpotCollider.center.y, weakSpotColliderDepth),
					new Vector3(data.weakSpotCollider.size.x, data.weakSpotCollider.size.y, weakSpotColliderThickness));
			}
			else weakSpotCollider.gameObject.SetActive(false);

			transform.SetLocalPositionAndRotation(animation.startPosition, Quaternion.Euler(0, 0, animation.startRotation));
			transform.localScale = new Vector3(animation.startScale.x, animation.startScale.y, 1);
			text.color = new Color(text.color.r, text.color.g, text.color.b, a: 0);

			var seq = DOTween.Sequence(transform)
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

		private void OnStatementHit(TruthBullet bullet)
		{
			if (bullet.HitObject) return;
			Debug.Log("StatementHit");

			AudioController.instance.PlaySound(statementHitSound);
			transform.BlendableShake(Vector3.one * 8, 1f, 5);

			bullet.HitObject = true;
		}

		private void OnWeakSpotHit(TruthBullet bullet, DebateStatement statement)
		{
			if (bullet.HitObject) return;

			var isCorrect = IsCorrectBullet(bullet.Data);

			if (isCorrect)
			{
				OnCorrectHit(bullet, statement);
			}
			else
			{
				OnIncorrectHit(bullet, statement);
			}
			bullet.HitObject = true;
		}

		private void OnCorrectHit(TruthBullet bullet, DebateStatement statement)
		{
			OnCorrectBulletHit?.Invoke(bullet, statement);
		}

		private void OnIncorrectHit(TruthBullet bullet, DebateStatement statement)
		{
			StartCoroutine(IOnIncorrectHit(bullet, statement));

			IEnumerator IOnIncorrectHit(TruthBullet bullet, DebateStatement statement)
			{
				AudioController.instance.PlaySound(incorrectHitSound);
				OnIncorrectBulletHit?.Invoke(bullet, statement);

				yield return new WaitForSecondsRealtime(incorrectHitSound.length);

				OnIncorrectBulletHitAnimationEnded?.Invoke(bullet, statement);
			}
		}
	}
}
