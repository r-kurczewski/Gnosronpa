using UnityEditor;
using UnityEngine;

namespace Fangan
{
   [CustomEditor(typeof(CharacterRotation))]
    public class CharacterRotationEditor : Editor
    {
        public override void OnInspectorGUI()
      {
         base.OnInspectorGUI();
         var script = target as CharacterRotation;

         if (GUILayout.Button("Set Rotation"))
         {
           script.SetRotation();
         }
      }
    }
}
