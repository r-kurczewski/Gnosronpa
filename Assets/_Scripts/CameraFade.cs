using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using static Gnosronpa.Common.AnimationConsts;

namespace Gnosronpa
{
	public class CameraFade : SingletonBase<CameraFade>
	{
		[SerializeField]
		private Image image;

		public bool IsFadeIn => image.color.a == 0;

		public bool IsFadeOut => image.color.a == 1;

		public async UniTask FadeOut()
		{
			await FadeOut(defaultFadeTime, ignoreTimeScale: false);
		}

		public async UniTask FadeIn()
		{
			await FadeIn(defaultFadeTime, ignoreTimeScale: false);
		}

		public async UniTask FadeOut(float duration)
		{
			await FadeOut(duration, ignoreTimeScale: false);
		}

		public async UniTask FadeIn(float duration)
		{
			await FadeIn(duration, ignoreTimeScale: false);
		}

		public async UniTask FadeOut(bool ignoreTimeScale)
		{
			await FadeOut(defaultFadeTime, ignoreTimeScale);
		}

		public async UniTask FadeIn(bool ignoreTimeScale)
		{
			await FadeIn(defaultFadeTime, ignoreTimeScale);
		}

		public async UniTask FadeOut(float duration, bool ignoreTimeScale)
		{
			if (IsFadeOut) return;
			await Instance.DOFade(1, duration).SetEase(Ease.InOutFlash).SetUpdate(ignoreTimeScale);
		}

		public async UniTask FadeIn(float duration, bool ignoreTimeScale)
		{
			if (IsFadeIn) return;
			await Instance.DOFade(0, duration).SetEase(Ease.InOutFlash).SetUpdate(ignoreTimeScale);
		}

		private TweenerCore<float, float, FloatOptions> DOFade(float toValue, float duration)
		{
			return DOTween.To(
				() => image.color.a, (a) =>
				{
					var c = image.color;
					c.a = a;
					image.color = c;
				},
				toValue,
				duration);
		}
	}
}
