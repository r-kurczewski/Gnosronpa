using Gnosronpa.Scriptables;
using UnityEngine;
using static UnityEngine.SceneManagement.SceneManager;

namespace Gnosronpa.Controllers
{
	public class SceneController : SingletonBase<SceneController>
	{
		private const int DebateSceneId = 0;
		private const int MenuSceneId = 1;

		public void LoadScenario(Scenario scenario)
		{
			GameController.startingSegment = scenario.startSegment;
			LoadScene(DebateSceneId);
		}

		public void LoadMenu()
		{
			LoadScene(MenuSceneId);
		}
	}
}
