using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class TimeSlider : MonoBehaviour
    {
      [SerializeField]
      private Slider slider;

		[SerializeField]
		private float speed;

		public void SetPosition(float sliderPosition)
		{
			DOTween.To(() => slider.value, (v) => slider.value = v, sliderPosition, 0.5f);
			//slider.value = Mathf.Clamp01(sliderPosition);
		}

		//private void Update()
		//{
		//	slider.value += speed * Time.deltaTime;
		//	if (slider.value == 1) slider.value = 0;
		//}
	}
}
