using UnityEngine;

namespace Fangan.ScriptableObjects
{
	[CreateAssetMenu(fileName = "WeakSpot", menuName = "NonstopDebate/WeakSpot")]
	public class WeakSpotData : ScriptableObject
	{
		public string textTemplate;
		public string weakSpotText;
		public TruthBulletData correctBullet;

		[Header("Position")]
		public float startRotation;

		public Vector2 movement;
		public float rotation;

		public float movementSpeed;
		public float rotationSpeed;

		public Bounds collider = new Bounds(Vector3.zero, new Vector3(50, 50, 3));
	}
}
