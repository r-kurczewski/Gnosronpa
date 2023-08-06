using DG.Tweening;
using Gnosronpa.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class LieAnimation : MonoBehaviour
	{
		private const string AnimationProgressField = "_AnimationProgress";

		[SerializeField]
		private RawImage image;

		[SerializeField]
		private AudioClip lieSound;

		private void OnDestroy()
		{
			ResetAnimationProgress();
		}

		public void PlayAnimation()
		{
			AudioController.instance.PlaySound(lieSound);
			ResetAnimationProgress();

			var tween = DOTween.To(
				() => image.material.GetFloat(AnimationProgressField),
				(v) => image.material.SetFloat(AnimationProgressField, v),
				endValue: 0.5f, duration: 2f)
				.SetEase(Ease.Linear);
			tween.SetUpdate(true);
		}

		private void ResetAnimationProgress()
		{
			image.material.SetFloat(AnimationProgressField, 0f);
		}
	}
}
