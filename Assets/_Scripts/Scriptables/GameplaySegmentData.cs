using UnityEngine;

namespace Gnosronpa.Scriptables
{
	public abstract class GameplaySegmentData : ScriptableObject
	{
		public GameplaySegmentData nextGameplaySegment;

		public AudioClip segmentMusic;
	}
}
