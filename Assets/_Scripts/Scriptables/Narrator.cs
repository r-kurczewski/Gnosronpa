﻿using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Narrator), menuName = "Dialog/Narrator")]
	public class Narrator : DialogSource
	{
		public override string SourceName => "";

		public override bool ShowTitle => true;

		public override string FormatText(string text)
		{
			return $"<color=#093>{text}</color>";
		}
	}
}