using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnosronpa.ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gnosronpa
{
	public class BulletDescriptionPanel : MonoBehaviour
	{
		private const float fadeDuration = 0.1f;

		[SerializeField]
		private CanvasGroup canvasGroup;

		[SerializeField]
		private InputActionReference changeBulletDescription;

		[SerializeField]
		private GameObject bulletDescriptionListElementPrefab;

		[SerializeField]
		private ScrollRect scroll;

		[SerializeField]
		private TMP_Text bulletDescription;

		[SerializeField]
		private Image bulletImage;

		[SerializeField]
		private int selectedIndex;

		[SerializeField]
		private List<BulletDescriptionListElement> listElements = new List<BulletDescriptionListElement>();

		public DebateData debateData;

		public BulletDescriptionListElement SelectedBullet
		{
			get
			{
				if (selectedIndex >= listElements.Count)
				{
					Debug.LogWarning("Invalid bullet description index", this);
					return null;
				}

				return listElements[selectedIndex];
			}
		}

		private void Start()
		{
			Init(debateData.bullets);
		}

		private void OnEnable()
		{
			changeBulletDescription.action.performed += OnBulletDescriptionChange;
			UpdateView();

			//Time.timeScale = 1 - Time.timeScale;
		}

		private void OnDisable()
		{
			changeBulletDescription.action.performed -= OnBulletDescriptionChange;
		}

		public void Init(IEnumerable<TruthBulletData> bullets)
		{
			listElements.Clear();
			foreach (var bullet in bullets)
			{
				var bulletDescription = Instantiate(bulletDescriptionListElementPrefab, scroll.content)
					.GetComponent<BulletDescriptionListElement>();

				bulletDescription.Init(bullet);
				listElements.Add(bulletDescription);
			}

			selectedIndex = 0;
			UpdateView();
		}

		public async UniTask ToggleVisibility()
		{
			var toggle = !gameObject.activeSelf;
			await SetVisibility(toggle);
		}

		public async UniTask SetVisibility(bool visible)
		{
			if(visible) gameObject.SetActive(true);
			await DOTween.To(() => canvasGroup.alpha, (v) => canvasGroup.alpha = v, visible ? 1 : 0, fadeDuration).SetUpdate(true);
			if(!visible) gameObject.SetActive(false);
			
		}

		private void OnBulletDescriptionChange(InputAction.CallbackContext context = default)
		{
			var direction = context.ReadValue<float>();
			if (direction > 0)
			{
				selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, listElements.Count - 1);
			}
			else if (direction < 0)
			{
				selectedIndex = Mathf.Clamp(selectedIndex + 1, 0, listElements.Count - 1);
			}

			UpdateView();
		}

		private void UpdateView()
		{
			for (int i = 0; i < listElements.Count; i++)
			{
				listElements[i].SetSelected(selectedIndex == i);
			}
			SetScrollValue();
			bulletDescription.text = SelectedBullet?.BulletData.description ?? default;
			bulletImage.sprite = SelectedBullet?.BulletData.bulletIcon ?? default;
		}



		private void SetScrollValue()
		{
			var firstElement = scroll.content.childCount > 0 ? scroll.content.GetChild(0).GetComponent<RectTransform>() : null;
			if (firstElement)
			{
				var elementHeight = firstElement.rect.height;
				var contentHeight = scroll.content.rect.height;
				var viewHeight = scroll.viewport.rect.height;
				var scrollableHeight = contentHeight - viewHeight;

				scroll.verticalNormalizedPosition = Mathf.Clamp01((scrollableHeight - selectedIndex * elementHeight) / scrollableHeight);
			}
			else scroll.verticalNormalizedPosition = 1;
		}
	}
}
