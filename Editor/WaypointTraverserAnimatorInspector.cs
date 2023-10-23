using UnityEditor;
using UnityEngine;

namespace IronMountain.Wayfinding.Editor
{
    [CustomEditor(typeof(WaypointTraverserAnimator))]
    public class WaypointTraverserAnimatorInspector : UnityEditor.Editor
    {
        private WaypointTraverserAnimator _waypointTraverserAnimator;
        
        private void OnEnable()
        {
            _waypointTraverserAnimator = (WaypointTraverserAnimator) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            bool isWalking = _waypointTraverserAnimator 
                             && _waypointTraverserAnimator.AnimatorBoolValue;
        
            GUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            EditorGUILayout.LabelField("Preview", GUILayout.MaxWidth(80));
            
            EditorGUI.BeginDisabledGroup(isWalking);
            if (GUILayout.Button("Walk") && _waypointTraverserAnimator) _waypointTraverserAnimator.SetBool(true);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(!isWalking);
            if (GUILayout.Button("Idle") && _waypointTraverserAnimator) _waypointTraverserAnimator.SetBool(false);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.EndDisabledGroup();
            
            GUILayout.EndHorizontal();
        }
    }
}
