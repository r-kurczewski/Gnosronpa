using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Common;
using Gnosronpa.Controllers;
using UnityEngine;

namespace Gnosronpa.Animations
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
					_ = AudioController.instance.PlaySound(sound);
					_ = AudioController.instance.PlaySound(dialogue);
				})

				.Append(transform.DOLocalMove(animationEndPos, 0.15f).SetEase(Ease.OutFlash))
				.Join(shakeParent.BlendableShake(Vector2.up * 10, 2f, 20, false))
				.Append(transform.DOScale(1.3f, 0.2f))
				.Join(DOTween.To(() => cg.alpha, (a) => cg.alpha = a, 0, 0.2f));

			await seq.AwaitForComplete();
		}
	}
}
