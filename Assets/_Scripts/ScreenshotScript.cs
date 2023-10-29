using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class ScreenshotScript : MonoBehaviour
	{
		private const string ShaderTextureField = "MainTex";

		[SerializeField]
		private RawImage screenshotImage;

		private Camera renderCamera;

		public RawImage ScreenshotImage => screenshotImage;

		private void OnDestroy()
		{
			if(renderCamera) Destroy(renderCamera.gameObject);
		}

		//private void OnPostRender()
		//{
		//	renderCamera.Render();
		//}

		public void TakeRender()
		{
			var camera = GetRenderCamera();

			var renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
			renderTexture.enableRandomWrite = true;
			renderTexture.name = "Render";
			renderTexture.Create();

			camera.targetTexture = renderTexture;
			screenshotImage.texture = renderTexture;
			screenshotImage.material.SetTexture(ShaderTextureField, renderTexture);
			screenshotImage.enabled = true;
		}

		public void TakeScreenshot()
		{
			StartCoroutine(ITakeScreenshot());
		}

		public void HideImage()
		{
			screenshotImage.enabled = false;
		}

		public IEnumerator ITakeScreenshot()
		{
			yield return new WaitForEndOfFrame();

			var texture = GetScreenshotTexture();
			screenshotImage.texture = texture;
			screenshotImage.material.SetTexture(ShaderTextureField, texture);
			screenshotImage.enabled = true;
		}

		private Camera GetRenderCamera()
		{
			if (!renderCamera)
			{
				renderCamera = new GameObject("RenderCamera", typeof(Camera)).GetComponent<Camera>();
				renderCamera.transform.parent = Camera.main.transform;
				renderCamera.CopyFrom(Camera.main);
			}

			return renderCamera;
		}

		private RenderTexture GetStaticRenderTexture()
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
