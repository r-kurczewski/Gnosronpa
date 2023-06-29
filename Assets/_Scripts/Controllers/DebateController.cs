using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gnosronpa.Controllers
{
	public class DebateController : MonoBehaviour
	{
		[SerializeField]
		private DebateData data;

		[SerializeField]
		private Transform debateParent;

		[SerializeField]
		private GameObject statementPrefab;

		[SerializeField]
		private CameraController cameraController;

		[SerializeField]
		private CharacterInfo characterInfo;

		private Queue<StatementConfiguration> statementsQueue;

		private float time;

		private void Awake()
		{
			statementsQueue = new Queue<StatementConfiguration>();
			LoadDebate(data);
		}

		private void Update()
		{
			if (statementsQueue.Count == 0) return;

			var nextStatement = statementsQueue.Peek();
			if (nextStatement.delay < time)
			{
				cameraController.ApplyCameraAnimation(nextStatement.cameraAnimation);
				if (nextStatement.statement)
				{
					LoadStatement(nextStatement);
					characterInfo.SetCharacter(nextStatement.statement.speakingCharacter);

				}
				statementsQueue.Dequeue();
				time = 0;
			}

			time += Time.deltaTime;
		}

		private void LoadStatement(StatementConfiguration statementData)
		{
			var statement = Instantiate(statementPrefab, debateParent).GetComponent<DebateStatement>();
			statement.Init(statementData);

		}

		public void LoadDebate(DebateData debate)
		{
			data = debate;
			debate.data.ForEach(statement => statementsQueue.Enqueue(statement));
		}

		public void Stop()
		{
			Time.timeScale = 0;
		}

		public void SetSpeed(float speed)
		{
			Time.timeScale = speed;
		}

		public void Restart()
		{
			time = 0;
		}
	}
}
