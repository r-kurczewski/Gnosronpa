using Gnosronpa.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gnosronpa.Editors
{
   [CustomEditor(typeof(CharacterPositioning))]
    public class CharacterPositioningEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
      {
         base.OnInspectorGUI();
         var script = target as CharacterPositioning;

         if (GUILayout.Button("Set Positions"))
         {
            script.SetCharacterPositions();
         }
      }
    }
}
