using Gnosronpa.Controllers;
using Gnosronpa.StateMachines.Core;
using System;

namespace Gnosronpa.StateMachines
{
	[Serializable]
	public class DebateState : State<DebateController>
	{
		public DebateState() : base()
		{

		}

		public DebateState(string stateName) : base(stateName)
		{

		}
	}
}