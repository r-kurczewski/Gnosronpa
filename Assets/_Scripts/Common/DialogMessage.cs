using Gnosronpa.Scriptables;
using System;
using UnityEngine;

namespace Gnosronpa.Common
{
	[Serializable]
	public class DialogMessage
	{
		public DialogSource dialogSource;

		[TextArea]
		public string messageText;

		public bool skipAnimation;

		public Animation3DData cameraAnimation;

		public string FormatedMessage => dialogSource.FormatText(messageText); 

	}
}
