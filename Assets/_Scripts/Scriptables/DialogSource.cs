using UnityEngine;

namespace Gnosronpa.Scriptables
{
	public abstract class DialogSource : ScriptableObject
	{
		public abstract string SourceName { get; }

		public virtual GameObject CameraTarget { get; }

		public abstract bool ShowTitle { get; }

		public abstract string FormatText(string text);
	}
}
