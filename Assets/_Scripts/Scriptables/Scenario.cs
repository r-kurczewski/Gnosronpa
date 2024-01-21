using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Scenario), menuName = nameof(Scenario))]
	public class Scenario : ScriptableObject
	{
		public string scenarioName;
		public GameplaySegmentData startSegment;
	}
}
