using System;
using UnityEngine;

namespace Gnosronpa.Common
{
	[Serializable]
	public class Animation3DData
	{
		[Header("Move")]
		public Vector3 startPosition;
		public Vector3 endPosition;
		public float moveDuration;

		[Header("Rotation")]
		public Vector3 startRotation;
		public Vector3 endRotation;
		public float rotationDuration;
	}
}
