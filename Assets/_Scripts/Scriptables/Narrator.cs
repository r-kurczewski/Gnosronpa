using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Narrator), menuName = "Dialog/Narrator")]
	public class Narrator : DialogSource
	{
		public override string SourceName => "";

		public override bool ShowTitle => false;

		public override string FormatText(string text)
		{
			return $"<style=\"narrator\">{text}</style>";
		}
	}
}
