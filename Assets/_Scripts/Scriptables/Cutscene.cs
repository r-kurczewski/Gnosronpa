using System.Linq;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Cutscene), menuName = "Dialog/" + nameof(Cutscene))]
	public class Cutscene : Discussion
	{
		public Sprite cutsceneSprite;

		private void OnValidate()
		{
			if (!cutsceneSprite)
			{
				Debug.LogError($"No cutscene sprite attached");
			}
			else
			{
				var messagesWithAnimations = messages.Where(x => !x.skipAnimation);
				foreach (var message in messagesWithAnimations)
				{
					Debug.LogWarning("Messages animations should be disabled during cutscene");
					message.skipAnimation = true;
				}

			}
		}
	}
}
