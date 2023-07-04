﻿using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using UnityEngine;

namespace Gnosronpa
{
	public class CameraFade : MonoBehaviour
	{
		[SerializeField]
		private float alpha = 0f;

		[SerializeField]
		private Color fadeColor = Color.black;

		private Texture2D texture;

		private void Start()
		{
			texture = new Texture2D(1, 1);
		}

		public void OnGUI()
		{
			if (alpha > 0f) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
			texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
			texture.Apply();
		}

		public TweenerCore<float, float, FloatOptions> DOFade(float endFade, float duration)
		{
			return DOTween.To(() => alpha, (a) => alpha = a, endFade, duration);
		}
	}
}