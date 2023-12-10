using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Gnosronpa.Common.AnimationConsts;

namespace Gnosronpa
{
	public class CameraFade : SingletonBase<CameraFade>
	{
		[SerializeField]
		private Image image;

		public TweenerCore<float, float, FloatOptions> DOFade(float endFade, float duration = defaultFadeTime)
		{
			return DOTween.To(
				() => image.color.a, (a) =>
				{
					var c = image.color;
					c.a = a;
					image.color = c;
				},
				endFade,
				duration);
		}
	}
}
