using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Controllers;
using System;
using UnityEngine;

namespace Gnosronpa
{
	[SelectionBase]
	public class CounterAnimation : MonoBehaviour
	{
		[SerializeField]
		private Transform shakeParent;

		[SerializeField]
		private Transform setsuAvatar;

		[SerializeField]
		private Transform counterText;

		[SerializeField]
		private Transform counterShadow;

		[SerializeField]
		private AudioClip dialogue;

		[SerializeField]
		private AudioClip sound;

		public async UniTask PlayAnimation()
		{
			var animationEndPos = Vector2.zero;
			var setsuEndPos = setsuAvatar.transform.localPosition;
			var setsuStartPos = new Vector2(355, -36);
			var cg = GetComponent<CanvasGroup>();

			var seq = DOTween.Sequence()
				.SetUpdate(true)

				.AppendCallback(() =>
				{
					AudioController.instance.PlaySound(sound);
					AudioController.instance.PlaySound(dialogue);
				})

				.Append(transform.DOLocalMove(animationEndPos, 0.15f).SetEase(Ease.OutFlash))
				.Join(DOTween.Shake(() => shakeParent.localPosition, (vector) =>
				{
					vector.x = shakeParent.localPosition.x;
					shakeParent.localPosition = vector;
				},
				duration: 2f, strength: 10, vibrato: 20, randomness: 0, ignoreZAxis: true, fadeOut: false, ShakeRandomnessMode.Full))

				.Append(transform.DOScale(1.3f, 0.2f))
				.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, 0.2f));
			
			await seq.AwaitForComplete();
		}
	}
}
