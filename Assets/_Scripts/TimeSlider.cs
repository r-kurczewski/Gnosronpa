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

		private void Update()
		{
			slider.value += speed * Time.deltaTime;
			if (slider.value == 1) slider.value = 0;
		}
	}
}
