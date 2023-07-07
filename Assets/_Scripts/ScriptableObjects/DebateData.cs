using Gnosronpa.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gnosronpa.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Debate", menuName = "NonstopDebate/Debate")]
	public class DebateData : ScriptableObject
	{
		public List<TruthBulletData> bullets;

		[FormerlySerializedAs("data")]
		public List<DebateSequenceData> debateSequence;
	}

	[Serializable]
	public class DebateSequenceData
	{
		public float delay;

		public DebateStatementData statement;

		public Animation2DData statementAnimation;

		public TransitionData statementTransition;

		[FormerlySerializedAs("cameraOffset")]
		public Animation3DData characterRelativeCameraAnimation;
	}
}
