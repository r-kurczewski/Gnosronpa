﻿using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(BulletObtained), menuName = "Dialog/" + nameof(BulletObtained))]
	public class BulletObtained : GameplaySegmentData
	{
		public const string messageTemplate = "<style=\"emphasis\">{0}</style> information has been added to your Silver Key knowledge section.";

		public List<TruthBulletData> bullets;

		public string GetMessage(TruthBulletData bullet)
		{
			return string.Format(messageTemplate, bullet.bulletName);
		}
	}
}
