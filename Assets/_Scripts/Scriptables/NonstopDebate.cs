using Gnosronpa.Common;
using Gnosronpa.Scriptables.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(NonstopDebate), menuName = "NonstopDebate/Debate")]
	public class NonstopDebate : GameplaySegmentData
	{
		public AudioClip segmentMusic;

		public int visibleBullets;

		public List<TruthBulletData> bullets;

		public List<DialogMessage> debateHints;

		public List<DebateSequenceData> debateSequence;
	}
}
