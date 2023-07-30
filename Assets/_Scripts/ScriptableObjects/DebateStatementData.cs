using UnityEngine;

namespace Gnosronpa.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Statement", menuName = "NonstopDebate/Statement")]
	public class DebateStatementData : ScriptableObject
	{
		public CharacterData speakingCharacter;

		public StatementType statementType;

		public string textTemplate;

		public string weakSpotText;

		public TruthBulletData correctBullet;

		public Bounds collider;

		public enum StatementType
		{
			Normal,
			WeakSpot,
			Noise,
		}
	}
}
