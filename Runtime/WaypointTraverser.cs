using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IronMountain.Wayfinding
{
    public class WaypointTraverser : MonoBehaviour
    {
        public event Action OnMovingChanged;
        public event Action OnCurrentWaypointChanged;

        [SerializeField] private Waypoint currentWaypoint;
        [SerializeField] private Waypoint destinationWaypoint;
        [Space]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float multiplier = 1f;
        [SerializeField] private Vector3 offset;

        [Header("Cache")]
        private bool _moving;
        private readonly List<Waypoint> _path = new ();

        public float Speed
        {
            get => speed;
            set => speed = value;
        }
        
        public float Multiplier
        {
            get => multiplier;
            set => multiplier = value;
        }

        public bool Moving
        {
            get => _moving;
            private set
            {
                if (_moving == value) return;
                _moving = value;
                OnMovingChanged?.Invoke();
            }
        }

        public Waypoint CurrentWaypoint
        {
            get => currentWaypoint;
            set
            {
                if (!value || currentWaypoint == value) return;
                currentWaypoint = value;
                SnapToCurrent();
                OnCurrentWaypointChanged?.Invoke();
                RefreshPath();
            }
        }

        public Waypoint DestinationWaypoint
        {
            get => destinationWaypoint;
            set
            {
                if (destinationWaypoint == value) return;
                destinationWaypoint = value;
                RefreshPath();
            }
        }

        public void Initialize(Waypoint startWaypoint, Waypoint destinationWaypoint = null)
        {
            currentWaypoint = startWaypoint;
            SnapToCurrent();
            OnCurrentWaypointChanged?.Invoke();
            this.destinationWaypoint = destinationWaypoint;
            RefreshPath();
        }
        
        public Vector3 GetLookDirection()
        {
            if (_path.Count > 0)
            {
                Vector3 targetPosition = _path[0].transform.position + offset;
                return (targetPosition - transform.position).normalized;
            }
            return CurrentWaypoint 
                ? CurrentWaypoint.transform.forward 
                : transform.forward;
        }
        
        private void OnEnable() => Moving = false;

        private void OnDisable() => Moving = false;

        private void SnapToCurrent()
        {
            Transform currentWaypointTransform = currentWaypoint ? currentWaypoint.transform : null;
            if (!currentWaypointTransform) return;
            Transform myTransform = transform;
            myTransform.position = currentWaypointTransform.position + offset;
            myTransform.rotation = currentWaypointTransform.rotation;
        }

        private void RefreshPath()
        {
            _path.Clear();
            if (currentWaypoint && DestinationWaypoint && currentWaypoint != DestinationWaypoint)
            {
                _path.AddRange(WaypointManager.GetShortestPath(currentWaypoint, DestinationWaypoint));
                if (_path.Count > 0 && _path[0] == currentWaypoint) _path.RemoveAt(0);
            }
        }

        private void MoveToNextNodeInPath()
        {
            if (_path.Count == 0) return;
            do
            {
                Waypoint nextWaypoint = _path[0];
                _path.RemoveAt(0);
                if (nextWaypoint)
                {
                    CurrentWaypoint = nextWaypoint;
                    return;
                }
            } while (_path.Count > 0);
        }

        private void Update()
        {
            if (currentWaypoint
                && destinationWaypoint
                && currentWaypoint != destinationWaypoint
                && _path is null || _path is {Count: 0} || _path[0] != currentWaypoint)
            {
                RefreshPath();
            }

            if (_path is {Count: 0})
            {
                Moving = false;
                return;
            }
            
            if (_path != null && !_path[0])
            {
                Moving = false;
                MoveToNextNodeInPath();
                return;
            }
            
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = _path[0].transform.position + offset;
            Vector3 direction = GetLookDirection();
            float remainingDistance = Vector3.Distance(currentPosition, targetPosition);
            float frameDistance = multiplier * speed * Time.deltaTime;
            if (frameDistance < remainingDistance)
            {
                Moving = true;
                transform.Translate(direction * frameDistance, Space.World);
            }
            else
            {
                transform.Translate(direction * remainingDistance, Space.World);
                MoveToNextNodeInPath();
            }
        }

        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            if (_path.Count > 0)
            {
                Handles.DrawLine(transform.position, _path[0].transform.position, 2);
            }
            for (int i = 0; i <= _path.Count - 2; i++)
            {
                Waypoint a = _path[i];
                Waypoint b = _path[i + 1];
                Handles.DrawLine(a.transform.position, b.transform.position, 2);
            }
        }
    }
}