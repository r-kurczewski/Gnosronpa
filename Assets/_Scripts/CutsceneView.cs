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
		private CutsceneData _data;

		public CutsceneData Data => _data;

		public void SetCutscene(CutsceneData data)
		{
			_data = data;
			image.sprite = data?.sprite;
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
