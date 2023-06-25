using Gnosronpa.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.ScriptableObjects
{
   [CreateAssetMenu(fileName ="Debate", menuName ="NonstopDebate/Debate")]
   public class DebateData : ScriptableObject
    {
      public List<StatementConfiguration> data;
    }

   [Serializable]
   public class StatementConfiguration
   {
      public DebateStatementData statement;

      public float delay;

		public Animation3DData cameraAnimation;
	}
}
