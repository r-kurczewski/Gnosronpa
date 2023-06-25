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
		public TruthBulletData correctBullet;

		public Animation2DData animationData;

		[Header("Transition")]
		public float appearTime;
		public float durationTime;
		public float disappearTime;

		public Bounds collider;
	}
}
