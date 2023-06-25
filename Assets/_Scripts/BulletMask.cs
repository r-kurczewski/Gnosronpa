using UnityEngine;

namespace Gnosronpa
{
	public class BulletMask : MonoBehaviour
    {
      [SerializeField]
      private Canvas canvas;

		private void Update()
		{
			var canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
			transform.localScale = canvasSize;
		}
	}
}
