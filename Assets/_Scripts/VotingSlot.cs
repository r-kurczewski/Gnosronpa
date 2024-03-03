using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Scriptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Gnosronpa.Scriptables.Voting;

namespace Gnosronpa
{
	public class VotingSlot : MonoBehaviour
	{
		private const float rotateSpeed = 1250;
		private const int loopElementsCount = 2;

		[SerializeField]
		private RectTransform scrollBox;

		[SerializeField]
		private GameObject slotImagesPrefab;

		[SerializeField]
		private List<Image> slotImages = new();

		[SerializeField]
		private int _visibleIndex = 0;

		[SerializeField]
		private VotingResult stopValue;

		[SerializeField]
		private bool rotate;

		private UniTask rotateTask;

		public int VisibleIndex { get => _visibleIndex; set => _visibleIndex = value % (slotImages.Count - loopElementsCount); }
		private float ElementHeight => scrollBox.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;

		public void Init(List<CharacterData> characters, VotingResult stopValue)
		{
			this.stopValue = stopValue;
			foreach (var image in slotImages)
			{
				Destroy(image.gameObject);
			}
			slotImages.Clear();

			foreach (var character in characters)
			{
				SpawnImageSlot(character);
			}

			// additional loop images
			for (int i = 0; i < loopElementsCount; i++)
			{
				SpawnImageSlot(characters[i]);
			}

			void SpawnImageSlot(CharacterData character)
			{
				var image = Instantiate(slotImagesPrefab, scrollBox).GetComponent<Image>();
				image.sprite = character.VoteSprite;
				image.name = character.characterName;
				slotImages.Add(image);

				var slotSize = GetComponentInParent<GridLayoutGroup>().cellSize;
				image.GetComponent<RectTransform>().sizeDelta = slotSize;
			}
		}

		public void StartRotating()
		{
			rotate = true;
			rotateTask = Rotate();
		}

		public async UniTask StopRotating()
		{
			rotate = false;
			await rotateTask;
		}

		private async UniTask Rotate()
		{
			while (rotate || slotImages[VisibleIndex].sprite != stopValue?.primaryResult?.VoteSprite)
			{
				await RotateSlotOnce();
			}
			if (stopValue.secondaryHalfResult != null)
			{
				await RotateSlotByHalf(incrementIndex: false);
			}
		}

		private async UniTask RotateSlotOnce()
		{
			await RotateSlotByLength(ElementHeight);
			VisibleIndex++;
		}

		private async UniTask RotateSlotByHalf(bool incrementIndex)
		{
			await RotateSlotByLength(ElementHeight/2);
			if (incrementIndex) VisibleIndex++;
		}

		private async UniTask RotateSlotByLength(float length)
		{
			var elementsCount = scrollBox.childCount;
			var loopLength = (elementsCount - loopElementsCount) * ElementHeight;
			var duration = length / rotateSpeed;

			await scrollBox.DOBlendableLocalMoveBy(length * Vector2.down, duration).SetEase(Ease.Linear);

			// move loop tape to start if necessary
			if (Mathf.Abs(scrollBox.anchoredPosition.y) >= loopLength)
			{
				var newPosition = scrollBox.anchoredPosition;
				newPosition.y += loopLength;
				scrollBox.anchoredPosition = newPosition;
			}
		}
	}
}
