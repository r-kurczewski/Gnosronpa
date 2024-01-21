using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Common;
using Gnosronpa.Controllers;
using Gnosronpa.Scriptables;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static Gnosronpa.Scriptables.DebateStatementData;

namespace Gnosronpa
{
	public class DebateStatement : MonoBehaviour
	{
		public event Func<TruthBullet, DebateStatement, UniTask> OnCorrectBulletHit;

		public event Action<TruthBullet, DebateStatement> OnIncorrectBulletHit;

		public event Func<TruthBullet, DebateStatement, UniTask> OnIncorrectBulletHitAnimationEnded;

		private const float statementColliderThickness = 5;
		private const float statementColliderDepth = 5;

		private const float weakSpotColliderThickness = 5;
		private const float weakSpotColliderDepth = -5;

		[SerializeField]
		private DebateSequenceData data;

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

		public DebateSequenceData Data => data;

		private string Gradient(string text) => $"<gradient=\"Weak spot\">{text}</gradient>";

		public bool IsCorrectBullet(TruthBulletData data) => this.data.statement.correctBullet == data;

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

			data = sequenceData;
			var animation = sequenceData.statementAnimation;
			var transition = sequenceData.statementTransition;

			name = data.statement.name;
			text.text = string.Format(data.statement.textTemplate, Gradient(data.statement.weakSpotText));

			var colliderData = data.statement.textCollider;
			statementCollider.SetColliderSize(
				new Vector3(colliderData.center.x, colliderData.center.y, statementColliderDepth),
				new Vector3(colliderData.size.x, colliderData.size.y, statementColliderThickness));
			statementCollider.gameObject.SetActive(false);

			if (data.statement.statementType is StatementType.WeakSpot)
			{
				colliderData = data.statement.weakSpotCollider;
				weakSpotCollider.SetColliderSize(
					new Vector3(colliderData.center.x, colliderData.center.y, weakSpotColliderDepth),
					new Vector3(colliderData.size.x, colliderData.size.y, weakSpotColliderThickness));
			}
			weakSpotCollider.gameObject.SetActive(false);

			transform.SetLocalPositionAndRotation(animation.startPosition, Quaternion.Euler(0, 0, animation.startRotation));
			transform.localScale = new Vector3(animation.startScale.x, animation.startScale.y, 1);
			text.color = new Color(text.color.r, text.color.g, text.color.b, a: 0);

			PlayStatementAnimation();
		}

		private void PlayStatementAnimation()
		{
			var animation = data.statementAnimation;
			var transition = data.statementTransition;

			var seq = DOTween.Sequence(transform);

			seq.Join(DOTween.ToAlpha(() => text.color, (color) => text.color = color, 1, transition.appearTime)
				.OnComplete(() =>
				{
					statementCollider.gameObject.SetActive(true);
					weakSpotCollider.gameObject.SetActive(data.statement.statementType is StatementType.WeakSpot);
				}));

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
				.AppendCallback(() =>
				{
					statementCollider.gameObject.SetActive(false);
					weakSpotCollider.gameObject.SetActive(false);
				})
			.Join(DOTween.ToAlpha(() => text.color, (color) => text.color = color, 0, transition.disappearTime))
			.onComplete = () =>
			{
				DOTween.Kill(transform);
				Destroy(gameObject);
			};
		}

		private void OnStatementHit(TruthBullet bullet)
		{
			if (bullet.HitObject) return;

			_ = AudioController.Instance.PlaySound(statementHitSound);
			transform.BlendableShake(Vector3.one * 8, 1f, 5, true);

			bullet.HitObject = true;
		}

		private void OnWeakSpotHit(TruthBullet bullet, DebateStatement statement)
		{
			if (bullet.HitObject) return;

			var isCorrect = IsCorrectBullet(bullet.Data);

			if (isCorrect)
			{
				_ = OnCorrectWeakSpotHit(bullet, statement);
			}
			else
			{
				OnIncorrectWeakspotHit(bullet, statement);
			}
			bullet.HitObject = true;
		}

		private async UniTask OnCorrectWeakSpotHit(TruthBullet bullet, DebateStatement statement)
		{
			if (OnCorrectBulletHit is not null) await OnCorrectBulletHit(bullet, statement);
		}

		private void OnIncorrectWeakspotHit(TruthBullet bullet, DebateStatement statement)
		{
			StartCoroutine(IOnIncorrectHit(bullet, statement));

			IEnumerator IOnIncorrectHit(TruthBullet bullet, DebateStatement statement)
			{
				_ = AudioController.Instance.PlaySound(incorrectHitSound);
				OnIncorrectBulletHit?.Invoke(bullet, statement);

				yield return new WaitForSecondsRealtime(incorrectHitSound.length);

				OnIncorrectBulletHitAnimationEnded?.Invoke(bullet, statement);
			}
		}
	}
}
