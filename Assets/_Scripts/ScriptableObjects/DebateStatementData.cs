using Gnosronpa.Common;
using UnityEngine;

namespace Gnosronpa.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Statement", menuName = "NonstopDebate/Statement")]
	public class DebateStatementData : ScriptableObject
	{
		public string textTemplate;

		public string weakSpotText;

		public CharacterData speakingCharacter;

		public DebateStatementData statement;

		public TruthBulletData correctBullet;

		public Bounds collider;
	}
}
