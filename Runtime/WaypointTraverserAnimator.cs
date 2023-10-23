using UnityEngine;

namespace IronMountain.Wayfinding
{
    [DisallowMultipleComponent]
    public class WaypointTraverserAnimator : MonoBehaviour
    {
        [SerializeField] private WaypointTraverser waypointTraverser;
        [SerializeField] private Animator animator;
        [SerializeField] private string movingBoolName = "Walking";
        
        public bool AnimatorBoolValue => animator && animator.GetBool(movingBoolName);

        private void Awake()
        {
            if (!waypointTraverser) waypointTraverser = GetComponentInParent<WaypointTraverser>();
            if (!animator) animator = GetComponentInChildren<Animator>();
        }

        private void OnValidate()
        {
            if (!waypointTraverser) waypointTraverser = GetComponentInParent<WaypointTraverser>();
            if (!animator) animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            if (waypointTraverser) waypointTraverser.OnMovingChanged += RefreshState;
            RefreshState();
        }

        private void OnDisable()
        {
            if (waypointTraverser) waypointTraverser.OnMovingChanged -= RefreshState;
        }

        private void RefreshState()
        {
            SetBool(waypointTraverser && waypointTraverser.Moving);
        }

        public void SetBool(bool value)
        {
            if (animator) animator.SetBool(movingBoolName, value);
        }
    }
}
