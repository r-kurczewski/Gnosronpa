using Gnosronpa.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gnosronpa.ScriptableObjects
{
   [CreateAssetMenu(fileName ="Debate", menuName ="NonstopDebate/Debate")]
   public class DebateData : ScriptableObject
    {
      public List<StatementConfigurationData> data;
    }

   [Serializable]
   public class StatementConfigurationData
   {
		public float delay;

		public DebateStatementData statement;

		public Animation2DData statementAnimation;

		public TransitionData statementTransition;

		public Animation3DData cameraOffset;
	}
}
