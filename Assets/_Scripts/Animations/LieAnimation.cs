using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa.Animations
{
	public class LieAnimation : MonoBehaviour
	{
		private const string AnimationProgressField = "_AnimationProgress";

		[SerializeField]
		private AudioClip lieSound;

		[SerializeField]
		private GraphicType graphicType;

		private Material LieMaterial
		{
			get
			{
				return graphicType switch
				{
					GraphicType.SpriteRenderer => GetComponent<SpriteRenderer>().material,
					GraphicType.Image => GetComponent<Graphic>().material,
					_ => null,
				};
			}
		}

		private void OnDestroy()
		{
			ResetAnimationProgress();
		}

		public async UniTask PlayAnimation()
		{
			ResetAnimationProgress();
			AudioController.instance.PlaySound(lieSound);

			await DOTween.To(
				() => LieMaterial.GetFloat(AnimationProgressField),
				(v) => LieMaterial.SetFloat(AnimationProgressField, v),
				endValue: 1f, duration: 2.5f)
				.SetEase(Ease.InOutCubic)
				.SetUpdate(true);
		}

		private void ResetAnimationProgress()
		{
			LieMaterial.SetFloat(AnimationProgressField, 0f);
		}

		private enum GraphicType { SpriteRenderer, Image }
	}
}
