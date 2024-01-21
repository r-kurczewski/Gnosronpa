using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(InnerVoice), menuName = "Dialog/" + nameof(InnerVoice))]
	public class InnerVoice : DialogSource
	{
		public CharacterData mainCharacter;

		public override string SourceName => mainCharacter.characterName;

		public override GameObject CameraTarget => mainCharacter.CameraTarget;

		public override bool ShowTitle => true;

		public override string FormatText(string text)
		{
			return $"<style=\"innervoice\">{text}</style>";
		}
	}
}
