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

		private Queue<StatementConfigurationData> statementsQueue;

		private float time;

		private void Awake()
		{
			statementsQueue = new Queue<StatementConfigurationData>();
			if(data) LoadDebate(data);
		}

		private void Update()
		{
			if (statementsQueue.Count == 0) return;

			var statement = statementsQueue.Peek();
			if (statement.delay < time)
			{
				LoadAnimation(statement);
				statementsQueue.Dequeue();
				time = 0;
			}

			time += Time.deltaTime;
		}

		public void LoadAnimation(StatementConfigurationData nextStatement)
		{
			var speakingCharacter = GameObject.FindGameObjectsWithTag("Character")
								.Select(x => x.GetComponent<Character>())
								.FirstOrDefault(x => x.Data == nextStatement.statement.speakingCharacter);

			cameraController.ApplyCameraAnimation(nextStatement.cameraOffset, speakingCharacter?.gameObject);
			if (nextStatement.statement)
			{
				LoadStatement(nextStatement);
				characterInfo.SetCharacter(nextStatement.statement.speakingCharacter);

			}
		}

		private void LoadStatement(StatementConfigurationData statementData)
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
