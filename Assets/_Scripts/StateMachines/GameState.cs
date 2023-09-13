using Gnosronpa.Assets._Scripts.Controllers;
using Gnosronpa.Controllers;
using Gnosronpa.StateMachines.Common;
using System;

namespace Gnosronpa.StateMachines
{
	[Serializable]
	public class GameState : State<GameController>
	{
		public GameState() : base()
		{

		}

		public GameState(string stateName) : base(stateName)
		{

		}
	}
}
