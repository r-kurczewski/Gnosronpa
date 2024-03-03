using Gnosronpa.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Discussion), menuName = "Dialog/" + nameof(Discussion))]
	public class Discussion : GameplaySegmentData
	{
		public AudioClip segmentMusic;

		public bool playFirstMessageSound = false;

		public List<DialogMessage> messages;
	}
}
