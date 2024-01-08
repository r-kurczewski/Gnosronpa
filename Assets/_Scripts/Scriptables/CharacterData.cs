using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(CharacterData), menuName = "Dialog/Character")]
	public class CharacterData : DialogSource
	{
		public string characterName;

		public Sprite avatar;

		public Sprite chibiAvatar;

		public Sprite VoteSprite => chibiAvatar;

        public override string SourceName => characterName;

		public override GameObject CameraTarget => Character.TryGet(this).gameObject;

		public override bool ShowTitle => true;

		public override string FormatText(string text)
		{
			return text;
		}
	}
}
