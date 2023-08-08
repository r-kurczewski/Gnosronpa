using DG.Tweening;
using Gnosronpa.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class LieAnimation : MonoBehaviour
	{
		private const string AnimationProgressField = "_AnimationProgress";

		public event TweenCallback OnAnimationEnd;

		[SerializeField]
		private AudioClip lieSound;

		[SerializeField]
		private GraphicType graphicType;

		private Material LieMaterial
		{
			get
			{
				switch (graphicType)
				{
					case GraphicType.SpriteRenderer: return GetComponent<SpriteRenderer>().material;
					case GraphicType.Image: return GetComponent<Graphic>().material;
					default: return null;
				}
			}
		}

		private void OnDestroy()
		{
			ResetAnimationProgress();
		}

		public void PlayAnimation()
		{
			AudioController.instance.PlaySound(lieSound);
			ResetAnimationProgress();

			var tween = DOTween.To(
				() => LieMaterial.GetFloat(AnimationProgressField),
				(v) => LieMaterial.SetFloat(AnimationProgressField, v),
				endValue: 1f, duration: 2f)
				.SetEase(Ease.InOutSine)
				.SetUpdate(true)
				.onComplete += OnAnimationEnd;
		}

		private void ResetAnimationProgress()
		{
			LieMaterial.SetFloat(AnimationProgressField, 0f);
		}

		private enum GraphicType { SpriteRenderer, Image }
	}
}
