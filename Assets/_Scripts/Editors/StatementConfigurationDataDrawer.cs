using UnityEditor;
using Gnosronpa.ScriptableObjects;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using System.Linq;
using Gnosronpa.Controllers;

namespace Gnosronpa.Editors
{
	[CustomPropertyDrawer(typeof(DebateSequenceData))]
	public class StatementConfigurationDataDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();

			var type = typeof(DebateSequenceData);

			foreach (var field in type.GetFields())
			{
				container.Add(new PropertyField(property.FindPropertyRelative(field.Name)));
			}

			var buttonFlex = new VisualElement();
			buttonFlex.style.flexBasis = 20;
			buttonFlex.style.display = DisplayStyle.Flex;
			buttonFlex.style.justifyContent = Justify.FlexEnd;
			buttonFlex.style.flexDirection = FlexDirection.Row;

			var button = new Button(() =>
			{
				var debateController = Object.FindObjectOfType<DebateController>();
				var cameraController = Object.FindObjectOfType<CameraController>();

				var statements = GetStatementConfigurations(property);
				var prevSpeakingCharacter = GameObject.FindGameObjectsWithTag("Character")
				.Select(x => x.GetComponent<Character>())
				.FirstOrDefault(x => x.Data == statements.prev?.statement.speakingCharacter);

				cameraController.SetLastStateOfAnimation(statements.prev?.cameraAnimation, prevSpeakingCharacter?.gameObject);
				debateController.PlayAnimation(statements.current);
			});
			button.text = "Play animation";

			buttonFlex.Add(button);

			container.Add(buttonFlex);


			var separator = new VisualElement();
			separator.style.height = 10;

			container.Add(separator);

			return container;
		}

		public (DebateSequenceData current, DebateSequenceData prev) GetStatementConfigurations(SerializedProperty property)
		{
			var indexStr = property.propertyPath.Split('[').Last();
			indexStr = indexStr.Remove(indexStr.Length - 1);
			var index = int.Parse(indexStr);

			var target = property.serializedObject.targetObject as DebateData;
			return (target.debateSequence[index], index - 1 >= 0 ? target.debateSequence[index - 1] : null);
		}
	}
}
