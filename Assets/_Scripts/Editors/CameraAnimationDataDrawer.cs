using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using Gnosronpa.Common;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Gnosronpa.Editors
{
	[CustomPropertyDrawer(typeof(CameraAnimationData))]
	public class CameraAnimationDataDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();
			var prop = new PropertyField(property);
			container.Add(prop);

			// TODO: hide button with PropertyField
			//if (property.isExpanded)

			var buttonFlex = new VisualElement();
			buttonFlex.style.flexBasis = 20;
			buttonFlex.style.display = DisplayStyle.Flex;
			buttonFlex.style.justifyContent = Justify.FlexEnd;
			buttonFlex.style.flexDirection = FlexDirection.Row;

			var setCameraStartPosition = new Button(() => SetCameraPositionFromPreviousButton(property));
			setCameraStartPosition.text = "Set start values from previous";
			buttonFlex.Add(setCameraStartPosition);

			container.Add(buttonFlex);

			var separator = new VisualElement();
			separator.style.height = 10;

			container.Add(separator);
			return container;
		}

		private void SetCameraPositionFromPreviousButton(SerializedProperty property)
		{
			var previous = GetPreviousCameraAnimationsData(property);

			if (previous == null)
			{
				Debug.LogError("No previous item was found. Operation canceled.");
				return;
			}

			if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to set animation values from a previous one?", "Continue", "Cancel"))
			{
				var current = property.boxedValue as CameraAnimationData;
				current.startPosition = previous.FinalPosition;
				current.startRotation = previous.FinalRotation;

				property.boxedValue = current;
				property.serializedObject.ApplyModifiedProperties();
			}
		}

		private CameraAnimationData GetPreviousCameraAnimationsData(SerializedProperty property)
		{
			var pathArray = Regex.Split(property.propertyPath, @"(?<=[\[\]])");

			var indexString = pathArray[pathArray.Length - 2];
			var index = int.Parse(indexString.Remove(indexString.Length - 1));
			int? previousIndex = index > 0 ? index - 1 : null;
			string previousIndexString = previousIndex.HasValue ? previousIndex.ToString() + indexString.Last() : null;

			CameraAnimationData previous = null;
			if (previousIndexString != null)
			{
				pathArray[pathArray.Length - 2] = previousIndexString;
				previous = property.serializedObject.FindProperty(string.Join("", pathArray)).boxedValue as CameraAnimationData;
			}

			return previous;
		}
	}
}
