using Cysharp.Threading.Tasks;
using Gnosronpa.Common;
using System;
using UnityEngine;

namespace Gnosronpa.StateMachines.Common
{
	[Serializable]
	public abstract class State<T>
	{
		[SerializeField]
		private string _stateName;

		protected State()
		{
			StateName = "FINAL";
		}

		public State(string stateName)
		{
			StateName = stateName.ToSnakeCase().ToUpper();
		}

		public string StateName { get => _stateName; protected set => _stateName = value; }

		public virtual Func<UniTask> OnEnter { get; set; }

		public virtual Func<UniTask<State<T>>> OnExecute { get; set; }

		public virtual Func<UniTask> OnExit { get; set; }

		public override string ToString()
		{
			return StateName;
		}
	}
}
