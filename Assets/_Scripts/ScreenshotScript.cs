using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class ScreenshotScript : MonoBehaviour
	{
		[SerializeField]
		private RawImage screenshotImage;

		public void TakeRender()
		{
			screenshotImage.texture = GetRenderTexture();
			screenshotImage.gameObject.SetActive(true);
		}

		public void TakeScreenshot()
		{
			StartCoroutine(ITakeScreenshot());
		}

		public IEnumerator ITakeScreenshot() 
		{
			yield return new WaitForEndOfFrame();
			screenshotImage.texture = GetScreenshotTexture();
			screenshotImage.gameObject.SetActive(true);
		}

		private RenderTexture GetRenderTexture()
		{
			var renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
			renderTexture.name = "Render";

			Camera.main.targetTexture = renderTexture;
			Camera.main.Render();
			Camera.main.targetTexture = null;

			return renderTexture;
		}

		private Texture2D GetScreenshotTexture()
		{
			var texture = ScreenCapture.CaptureScreenshotAsTexture();
			texture.name = "Screenshot";
			return texture;
		}
	}
}
