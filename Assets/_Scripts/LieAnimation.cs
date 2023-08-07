using DG.Tweening;
using Gnosronpa.Controllers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class LieAnimation : MonoBehaviour
	{
		private const string AnimationProgressField = "_AnimationProgress";

		public event TweenCallback OnAnimationEnd;

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
				endValue: 0.5f, duration: 1.5f)
				.SetEase(Ease.Linear)
				.SetUpdate(true)
				.onComplete += OnAnimationEnd;
		}

		private void ResetAnimationProgress()
		{
			image.material.SetFloat(AnimationProgressField, 0f);
		}
	}
}
