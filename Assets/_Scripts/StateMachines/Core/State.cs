using Cysharp.Threading.Tasks;
using Gnosronpa.Common;
using System;
using UnityEngine;

namespace Gnosronpa.StateMachines.Core
{
	[Serializable]
	public abstract class State<T>
	{
		[SerializeField]
		private string _stateName;

		protected State() { }

		public State(string stateName)
		{
			StateName = stateName;
		}

		public string StateName { get => _stateName; init => _stateName = value.ToSnakeCase().ToUpper(); }

		public virtual Func<UniTask> OnEnter { get; init; }

		public virtual Func<UniTask<State<T>>> OnExecute { get; init; }

		public virtual Func<UniTask> OnExit { get; init; }

		public override string ToString()
		{
			return StateName;
		}
	}
}
