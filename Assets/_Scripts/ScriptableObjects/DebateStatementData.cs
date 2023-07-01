using Gnosronpa.Common;
using UnityEngine;

namespace Gnosronpa.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Statement", menuName = "NonstopDebate/Statement")]
	public class DebateStatementData : ScriptableObject
	{
		public CharacterData speakingCharacter;

		public string textTemplate;

		public string weakSpotText;

		public TruthBulletData correctBullet;

		public Bounds collider;
	}
}
