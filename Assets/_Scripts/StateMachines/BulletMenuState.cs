﻿using Gnosronpa.Controllers;
using Gnosronpa.StateMachines.Common;
using System;

namespace Gnosronpa.StateMachines
{
	[Serializable]
	public class BulletMenuState : State<BulletController>
	{
		public BulletMenuState() : base()
		{

		}

		public BulletMenuState(string stateName) : base(stateName)
		{

		}
	}
}
