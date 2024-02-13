using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Scenario), menuName = nameof(Scenario))]
	public class Scenario : ScriptableObject
	{
		public string scenarioName;

		public GameplaySegmentData startSegment;

		public List<CharacterData> startCharacters;

		public float roomSizeScale;
	}
}
