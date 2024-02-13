using Gnosronpa.Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class CutsceneView : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup canvasGroup;

		[SerializeField]
		private Image image;

		[SerializeField]
		private Cutscene _data;

		public Cutscene Data => _data;

		public void SetCutscene(Cutscene cutscene)
		{
			_data = cutscene;
			image.sprite = cutscene?.cutsceneSprite;
		}

		public void Show()
		{
			canvasGroup.alpha = 1;
		}

		public void Hide()
		{
			canvasGroup.alpha = 0;
		}
	}
}
