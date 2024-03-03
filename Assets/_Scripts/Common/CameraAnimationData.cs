using System;
using UnityEngine;

namespace Gnosronpa.Common
{
	/// <summary>
	/// Contains data used to animate camera. Position of camera is calculated relative to chosen target.
	/// </summary>
	[Serializable]
	public class CameraAnimationData
	{
		[Header("Move")]
		public Vector3 startPosition;
		public Vector3 endPosition;
		public float moveDuration;

		[Header("Rotation")]
		public Vector3 startRotation;
		public Vector3 endRotation;
		public float rotationDuration;

		public Vector3 FinalPosition => moveDuration > 0 ? endPosition : startPosition;

		public Vector3 FinalRotation => rotationDuration > 0 ? endRotation : startRotation;
	}
}
