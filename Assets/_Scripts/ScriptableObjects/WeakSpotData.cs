using UnityEngine;

namespace Fangan.ScriptableObjects
{
	[CreateAssetMenu(fileName = "WeakSpot", menuName = "NonstopDebate/WeakSpot")]
	public class WeakSpotData : ScriptableObject
	{
		public string textTemplate;
		public string weakSpotText;
		public TruthBulletData correctBullet;

		[Header("Move")]
		public Vector2 startPosition;
		public Vector2 endPosition;
		public float moveDuration;

		[Header("Rotation")]
		public float startRotation;
		public float endRotation;
		public float rotationDuration;

		[Header("Scale")]
		public Vector2 startScale;
		public Vector2 endScale;
		public float scaleDuration;

		[Header("Transition")]
		public float durationTime;
		public float disappearTime;

		[Header("Collider")]
		public Bounds collider = new Bounds(Vector3.zero, new Vector3(50, 50, 3));

	}
}
