using UnityEngine;

namespace Gnosronpa.Assets._Scripts.Scriptables
{
	public abstract class GameplaySegmentData : ScriptableObject
	{
		public GameplaySegmentData nextGameplaySegment;

		public AudioClip segmentMusic;
	}
}
