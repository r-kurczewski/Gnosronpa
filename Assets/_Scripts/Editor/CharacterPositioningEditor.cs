using UnityEditor;
using UnityEngine;

namespace Fangan
{
   [CustomEditor(typeof(CharacterPositioning))]
    public class CharacterPositioningEditor : Editor
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
