﻿using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Gnosronpa.StateMachines.Common
{
	public abstract class StateMachine<T, S> : MonoBehaviour
		where S : State<T>, new()
	{
		protected const string FinalStateName = "Final";

		[SerializeField]
		private S _currentState;

		private S _finalState;

		public S CurrentState
		{
			get
			{
				return _currentState;
			}
			protected set
			{
				OnStateChange(_currentState, value);
				_currentState = value;
			}
		}

		protected abstract S StartingState { get; }

		public S FinalState { get => _finalState; private set => _finalState = value; }

		protected S RequestedState { get; private set; }

		protected async UniTask InitStateMachine()
		{
			DefineMachineStates();
			FinalState = new() { StateName = FinalStateName, OnEnter = DefineFinalStateBehaviour() };

			try
			{
				await StateMachineExecute();
			}
			catch (Exception ex)
			{
				Debug.LogError($"State machine error: {ex.Message}", this);
			}
		}

		protected abstract void DefineMachineStates();

		protected virtual Func<UniTask> DefineFinalStateBehaviour()
		{
			return () => UniTask.CompletedTask;
		}

		private async UniTask StateMachineExecute()
		{
			if (StartingState is null)
			{
				Debug.LogError($"Starting state is null");
				return;
			}

			CurrentState = StartingState;
			if (CurrentState.OnEnter is not null) await CurrentState.OnEnter();

			S nextState = null;
			while (nextState != FinalState)
			{
				if (CurrentState.OnExecute is null)
				{
					Debug.LogError($"{CurrentState} have no {nameof(CurrentState.OnExecute)} implementation");
					return;
				}

				try
				{
					nextState = await CurrentState.OnExecute() as S;
				}
				catch (StateChangeRequestedException)
				{
					nextState = RequestedState;
					RequestedState = null;
				}

				if (nextState is null)
				{
					Debug.LogError($"{CurrentState} returns next state: null");
					return;
				}

				if (nextState != CurrentState)
				{
					if (CurrentState.OnExit is not null) await CurrentState.OnExit();
					CurrentState = nextState;
					if (CurrentState.OnEnter is not null) await CurrentState.OnEnter();
				}
			}
		}

		protected async UniTask RequestStateChange(State<T> requestedState)
		{
			RequestedState = requestedState as S;
			await UniTask.WaitUntil(() => CurrentState == requestedState);
		}

		protected void CheckStateChangeRequest()
		{
			if (RequestedState is not null)
			{
				throw new StateChangeRequestedException();
			}
		}

		protected virtual void OnStateChange(State<T> oldState, State<T> newState) { }
	}
}
