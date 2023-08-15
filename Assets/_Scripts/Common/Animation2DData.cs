using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gnosronpa.Common
{
	[Serializable]
	public class Animation2DData
	{
		[Header("Move")]
		public Vector2 startPosition;
		[FormerlySerializedAs("endPosition")]
		public Vector2 move;
		public float moveDuration;

		[Header("Rotation")]
		public float startRotation;
		[FormerlySerializedAs("endRotation")]
		public float rotation;
		public float rotationDuration;

		[Header("Scale")]
		public Vector2 startScale = Vector2.one;
		[FormerlySerializedAs("endScale")]
		public Vector2 scale;
		public float scaleDuration;
	}
}
