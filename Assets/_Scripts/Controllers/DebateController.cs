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

		private float time;

		private List<StatementConfiguration> statementsQueue;

		private void Awake()
		{
			LoadDebate(data);
		}

		private void Update()
		{
			var statementsToLoad = statementsQueue.Where(x => x.delay <= time).ToList();
			LoadStatements(statementsToLoad);

			var lastStatement = statementsToLoad.LastOrDefault();
			if (lastStatement != null)
			{
				var seq = cameraController.ApplyCameraAnimation(lastStatement.cameraAnimation);
				time = 0;
			}

			statementsToLoad.ForEach(x => statementsQueue.Remove(x));
			time += Time.deltaTime;
		}

		private void LoadStatements(IEnumerable<StatementConfiguration> statementsToLoad)
		{
			foreach (var statementToLoad in statementsToLoad)
			{
				var statement = Instantiate(statementPrefab, debateParent);
			}
		}

		public void LoadDebate(DebateData debate)
		{
			data = debate;
			statementsQueue = debate.data.ToList();
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
