using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.Scriptables;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class Slot : MonoBehaviour
	{
		private const float rotateOnceDuration = 0.2f;
		private const int loopElementsCount = 2;

		[SerializeField]
		private RectTransform scrollBox;

		[SerializeField]
		private GameObject slotImagesPrefab;

		[SerializeField]
		private bool stop;

		private Dictionary<Image, CharacterData> imageDictionary = new();

		public bool Stop { get => stop; set => stop = value; }

		public void SetImages(List<CharacterData> characters)
		{
			var randomizedCharacters = new List<CharacterData>(characters)
				.OrderBy(x => Random.value)
				.ToList();

			foreach (var image in imageDictionary.Keys)
			{
				Destroy(image.gameObject);
			}
			imageDictionary.Clear();

			foreach (var character in randomizedCharacters)
			{
				SpawnImageSlot(character);
			}

			// additional loop images
			for (int i = 0; i < loopElementsCount; i++)
			{
				SpawnImageSlot(randomizedCharacters[i]);
			}

			void SpawnImageSlot(CharacterData character)
			{
				var image = Instantiate(slotImagesPrefab, scrollBox).GetComponent<Image>();
				image.sprite = character.chibiAvatar;
				image.name = character.characterName;
				imageDictionary.Add(image, character);
			}
		}

		public async UniTask RotateTo(CharacterData character)
		{
			while (!stop)
			{
				await RotateSlotOnce();
			}
		}

		private async Task RotateSlotOnce()
		{
			var elementsCount = scrollBox.childCount;
			var elementHeight = scrollBox.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
			var loopLength = (elementsCount - loopElementsCount) * elementHeight;

			await scrollBox.DOBlendableLocalMoveBy(elementHeight * Vector2.down, rotateOnceDuration).SetEase(Ease.Linear);

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
