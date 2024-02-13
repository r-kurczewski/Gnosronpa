//using Gnosronpa.Scriptables;
//using UnityEditor;
//using UnityEngine.UIElements;

//namespace Gnosronpa.Editors.Assets._Scripts.Editors
//{
//	[CustomEditor(typeof(Scenario))]
//	public class ScenarioEditor : Editor
//	{
//		public override VisualElement CreateInspectorGUI()
//		{
//			VisualElement myInspector = new VisualElement();
//			myInspector.viewDataKey = "strings";

//			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Scripts/Editors/Scenario_Inspector.uxml");
//			visualTree.CloneTree(myInspector);

//			return myInspector;
//		}
//	}
//}
