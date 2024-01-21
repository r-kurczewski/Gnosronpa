using Gnosronpa.Common;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Discussion), menuName = "Dialog/" + nameof(Discussion))]
	public class Discussion : GameplaySegmentData
	{
		public AudioClip segmentMusic;

		public CutsceneData cutscene;

		public bool playFirstMessageSound = false;

		public List<DialogMessage> messages;
	}
}
