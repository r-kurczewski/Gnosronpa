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

		/// <summary>
		/// Relative to speaking character in the statement
		/// </summary>
		[FormerlySerializedAs("characterRelativeCameraAnimation")]
		public Animation3DData cameraAnimation;

		public float SequenceDuration
		{
			get
			{
				var ca = cameraAnimation;
				var cameraAnimationDuration = Mathf.Max(ca.moveDuration, ca.rotationDuration);

				var sa = statementAnimation;
				var statementAnimationDuration = Mathf.Max(sa.moveDuration, sa.rotationDuration, sa.scaleDuration);

				var st = statementTransition;
				var statementTransitionDuration = st.waitingTime + st.disappearTime;

				return Mathf.Max(cameraAnimationDuration, statementAnimationDuration + statementTransitionDuration);
			}
		}
	}
}
