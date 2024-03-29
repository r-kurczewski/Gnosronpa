﻿using Gnosronpa.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = "Statement", menuName = "NonstopDebate/Statement")]
	public class DebateStatementData : ScriptableObject
	{
		public CharacterData speakingCharacter;

		public StatementType statementType;

		[TextArea(1,3)]
		public string textTemplate;

		public string weakSpotText;

		public TruthBulletData correctBullet;

		public ColliderData textCollider;

		public ColliderData weakSpotCollider;

		public List<DialogMessage> hitDialogs;

		public enum StatementType
		{
			Normal,
			WeakSpot,
			Noise,
		}

		[Serializable]
		public class ColliderData
		{
			public Vector2 center;
			public Vector2 size;
		}

	}
}
