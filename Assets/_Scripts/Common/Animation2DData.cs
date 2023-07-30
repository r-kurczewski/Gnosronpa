using System;
using UnityEngine;

namespace Gnosronpa.Common
{
	[Serializable]
	public class Animation2DData
	{
		[Header("Move")]
		public Vector2 startPosition;
		public Vector2 endPosition;
		public float moveDuration;

		[Header("Rotation")]
		public float startRotation;
		public float endRotation;
		public float rotationDuration;

		[Header("Scale")]
		public Vector2 startScale = Vector2.one;
		public Vector2 endScale;
		public float scaleDuration;
	}
}
