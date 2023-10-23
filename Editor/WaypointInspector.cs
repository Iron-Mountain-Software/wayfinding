using UnityEditor;
using UnityEngine;

namespace IronMountain.Wayfinding.Editor
{
    [CustomEditor(typeof(Waypoint), true)]
    public class WaypointInspector : UnityEditor.Editor
    {
        private Waypoint _waypoint;

        private void OnEnable()
        {
            _waypoint = (Waypoint) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("reference"));
            if (GUILayout.Button("Find", GUILayout.MaxWidth(50)))
            {
                _waypoint.Reference =
                    WaypointManager.WaypointReferences.Find(test => test && test.name == _waypoint.name);
                return;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("neighbors"));
            WaypointManager.ShouldDrawLines = EditorGUILayout.Toggle("Draw Lines", WaypointManager.ShouldDrawLines);
            WaypointManager.ShouldDrawDiscs = EditorGUILayout.Toggle("Draw Discs", WaypointManager.ShouldDrawDiscs);
            WaypointManager.ShouldDrawLabels = EditorGUILayout.Toggle("Draw Labels", WaypointManager.ShouldDrawLabels);
            serializedObject.ApplyModifiedProperties();
        }
    }
}