using UnityEditor;
using Gnosronpa.Scriptables;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using System.Linq;
using Gnosronpa.Controllers;
using Gnosronpa.Scriptables.Models;

namespace Gnosronpa.Editors
{
	[CustomPropertyDrawer(typeof(DebateSequenceData))]
	public class StatementConfigurationDataDrawer : PropertyDrawer
	{
		public bool isFold = true;
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();

			container.Add(new PropertyField(property));

			var buttonFlex = new VisualElement();
			buttonFlex.style.flexBasis = 30;
			buttonFlex.style.display = DisplayStyle.Flex;
			buttonFlex.style.justifyContent = Justify.FlexEnd;
			buttonFlex.style.flexDirection = FlexDirection.Row;

			var playAnimationButton = new Button(() => PlayAnimation(property));
			playAnimationButton.text = "Play animation";
			buttonFlex.Add(playAnimationButton);

			container.Add(buttonFlex);

			var separator = new VisualElement();
			separator.style.height = 10;

			container.Add(separator);

			return container;
		}

		private void PlayAnimation(SerializedProperty property)
		{
			if (Application.isPlaying)
			{
				var debateController = Object.FindObjectOfType<DebateController>();
				var cameraController = Object.FindObjectOfType<CameraController>();

				var statements = GetDebateSequenceData(property);
				var prevSpeakingCharacter = GameObject.FindGameObjectsWithTag("Character")
				.Select(x => x.GetComponent<Character>())
				.FirstOrDefault(x => x.Data == statements.prev?.statement.speakingCharacter);

				cameraController.SetLastStateOfAnimation(statements.prev?.cameraAnimation, prevSpeakingCharacter?.gameObject);
				debateController.PlayAnimation(statements.current);
			}
			else Debug.LogWarning("Animation will play only in play mode!");
		}

		private (DebateSequenceData current, DebateSequenceData prev) GetDebateSequenceData(SerializedProperty property)
		{
			var indexStr = property.propertyPath.Split('[').Last();
			indexStr = indexStr.Remove(indexStr.Length - 1);
			var index = int.Parse(indexStr);

			var target = property.serializedObject.targetObject as NonstopDebate;
			var result = (target.debateSequence[index], index - 1 >= 0 ? target.debateSequence[index - 1] : null);
			return result;
		}
	}
}
