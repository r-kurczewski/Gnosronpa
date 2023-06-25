using UnityEngine;

namespace Gnosronpa.ScriptableObjects
{
	[CreateAssetMenu(fileName = "TruthBullet", menuName = "NonstopDebate/TruthBullet")]
	public class TruthBulletData : ScriptableObject
	{
		public string bulletName;

		public Bounds collider = new Bounds(Vector3.zero, new Vector3(50, 50, 3));
	}
}
