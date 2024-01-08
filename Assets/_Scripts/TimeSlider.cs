using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class TimeSlider : MonoBehaviour
	{
		[SerializeField]
		private Slider slider;

		[SerializeField]
		private float transitionDuration;

		[SerializeField]
		private Ease transitionEase;

		public void SetPosition(float sliderPosition, bool instant = false)
		{
			if (instant)
			{
				slider.value = sliderPosition;
				return;
			}

			DOTween.To(() => slider.value, (v) => slider.value = v, sliderPosition, transitionDuration)
				.SetEase(transitionEase);
		}
	}
}
