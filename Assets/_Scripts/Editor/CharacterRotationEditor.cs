using UnityEditor;
using UnityEngine;

namespace Gnosronpa
{
   [CustomEditor(typeof(Character))]
    public class CharacterRotationEditor : Editor
    {
        public override void OnInspectorGUI()
      {
         base.OnInspectorGUI();
         var script = target as Character;

         if (GUILayout.Button("Set Rotation"))
         {
           script.UpdateRotation();
         }
      }
    }
}
