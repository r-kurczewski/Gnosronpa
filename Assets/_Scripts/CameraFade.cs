using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class CameraFade : MonoBehaviour
	{
		private const float fadeTime = 0.75f;

		[SerializeField]
		private Image image;

		public TweenerCore<float, float, FloatOptions> DOFade(float endFade, float duration = fadeTime)
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
