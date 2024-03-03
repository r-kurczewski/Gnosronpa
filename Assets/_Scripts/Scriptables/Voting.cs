using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gnosronpa.Scriptables
{
	[CreateAssetMenu(fileName = nameof(Voting), menuName = "NonstopDebate/" + nameof(Voting))]
	public class Voting : GameplaySegmentData
	{
		public List<CharacterData> possibleVotes;

		public List<VotingResult> voteResults;

		public bool voteSuccess;

		public int randomSeed;

		[Serializable]
		public class VotingResult
		{
			public CharacterData primaryResult;

			public CharacterData secondaryHalfResult;
		}

		private void OnValidate()
		{
			voteResults.ForEach(result =>
			{
				if (result.primaryResult == result.secondaryHalfResult)
				{
					Debug.LogWarning("Provided incorrect secondary result.");
					result.secondaryHalfResult = null;
				}

				if (!possibleVotes.Contains(result.primaryResult))
				{
					Debug.LogError("Result not in possible votes.");
				}

				if (result.secondaryHalfResult != null && !possibleVotes.Contains(result.secondaryHalfResult))
				{
					Debug.LogWarning("Secondary result not in possible votes.");
					result.secondaryHalfResult = null;
				}
			});
		}
	}
}
