using UnityEngine;

namespace Gnosronpa.ScriptableObjects
{
	[CreateAssetMenu(fileName = "TruthBullet", menuName = "NonstopDebate/TruthBullet")]
	public class TruthBulletData : ScriptableObject
	{
		public string bulletName;

		public Sprite bulletIcon;

		[TextArea(4,4)]
		public string description;
	}
}
