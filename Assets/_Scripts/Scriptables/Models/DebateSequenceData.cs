using Gnosronpa.Common;
using System;
using UnityEngine;

namespace Gnosronpa.Scriptables.Models
{
	[Serializable]
	public class DebateSequenceData
	{
		public float delay;

		public DebateStatementData statement;

		public Animation2DData statementAnimation;

		public TransitionData statementTransition;

		public CameraAnimationData cameraAnimation;

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