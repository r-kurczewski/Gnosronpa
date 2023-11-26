﻿using Gnosronpa.Common;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Discussion), menuName = "Dialog/Discussion")]
	public class Discussion : GameplaySegmentData
	{
		public List<DialogMessage> messages;
	}
}
