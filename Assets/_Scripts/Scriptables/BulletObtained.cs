using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(BulletObtained), menuName = "Dialog/BulletObtained")]
	public class BulletObtained : GameplaySegmentData
	{
		public const string messageTemplate = "<style=\"emphasis\">{0}</style> information has been added to your Silver Key knowledge section.";

		public TruthBulletData bullet;

		public string GetMessage(TruthBulletData bullet)
		{
			return string.Format(messageTemplate, bullet.bulletName);
		}
	}
}
