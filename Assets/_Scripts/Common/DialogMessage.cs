using Gnosronpa.ScriptableObjects;
using System;
using UnityEngine;

namespace Gnosronpa.Common
{
	[Serializable]
	public class DialogMessage
	{
		public CharacterData speakingCharacter;

		[TextArea]
		public string messageText;

		public Animation3DData cameraAnimation;

	}
}
