using Cysharp.Threading.Tasks;
using Gnosronpa.Controllers;
using Gnosronpa.Scriptables;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gnosronpa.Common.AnimationConsts;

namespace Gnosronpa
{
	public class VotingAnimation : MonoBehaviour
	{
		private const int rotationDurationMilliseconds = 4000;

		[SerializeField]
		private List<VotingSlot> slots;

		[SerializeField]
		private AudioClip votingLoopSound;

		[SerializeField]
		private AudioClip slotStoppedSound;

		[SerializeField]
		private AudioClip slotStartedSound;

		[SerializeField]
		private AudioClip votingFinishedSuccessSound;

		[SerializeField]
		private AudioClip votingFinishedFailureSound;

		[SerializeField]
		private Voting votingResult;

		public async UniTask PlayAnimation(Voting data)
		{
			var rand = new System.Random(data.randomSeed);

			for (int i = 0; i < slots.Count; i++)
			{
				var randomizedCharacters = data.possibleVotes.OrderBy(x => rand.Next()).ToList();
				if (data.voteResults[i].secondaryHalfResult)
				{
					SetHalfSlotCharacter(randomizedCharacters, data.voteResults[i]);
				}
				slots[i].Init(randomizedCharacters, data.voteResults[i]);
			}

			await AudioController.Instance.PlaySound(slotStartedSound);
			await UniTask.Delay(500);

			slots.ForEach(x => x.StartRotating());
			AudioController.Instance.PlayAmbient(votingLoopSound, 0.5f);

			await CameraFade.Instance.FadeIn(2 * defaultFadeTime);

			await UniTask.Delay(rotationDurationMilliseconds);

			for (int i = 0; i < slots.Count; i++)
			{
				await slots[i].StopRotating();

				if (i == slots.Count - 1) // if last slot
				{
					AudioController.Instance.StopAmbient();
					await AudioController.Instance.PlaySound(slotStoppedSound);
				}
				else
				{
					_ = AudioController.Instance.PlaySound(slotStoppedSound);
				}
			}
			await AudioController.Instance.PlaySound(data.voteSuccess ? votingFinishedSuccessSound : votingFinishedFailureSound);
		}

		private void SetHalfSlotCharacter(List<CharacterData> charactersList, Voting.VotingResult votingResult)
		{
			var primaryResultIndex = charactersList.IndexOf(votingResult.primaryResult);
			var secondaryResultIndex = charactersList.IndexOf(votingResult.secondaryHalfResult);
			var newSecondaryResultIndex = (primaryResultIndex + 1) % charactersList.Count;

			var temp = charactersList[newSecondaryResultIndex];
			charactersList[newSecondaryResultIndex] = votingResult.secondaryHalfResult;
			charactersList[secondaryResultIndex] = temp;
		}
	}
}
