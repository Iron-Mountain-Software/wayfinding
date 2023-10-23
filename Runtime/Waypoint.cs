using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IronMountain.Wayfinding
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] private WaypointReference reference;
        [SerializeField] private List<Waypoint> neighbors = new ();

        public WaypointReference Reference
        {
            get => reference;
            set => reference = value;
        }

        public List<Waypoint> Neighbors => neighbors;

        private void OnEnable() => WaypointManager.Waypoints.Add(this);
        private void OnDisable() => WaypointManager.Waypoints.Remove(this);

        public void AddNeighbor(Waypoint neighbor)
        {
            if (neighbor && !neighbors.Contains(neighbor)) neighbors.Add(neighbor);
        }
        
        public void RemoveNeighbor(Waypoint neighbor)
        {
            if (neighbors.Contains(neighbor)) neighbors.Remove(neighbor);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            RefreshNeighbors();
            RefreshName();
        }

        [ContextMenu("Refresh Neighbors")]
        private void RefreshNeighbors()
        {
            neighbors = neighbors.Distinct().ToList();
            neighbors.RemoveAll(test => !test);
            neighbors.RemoveAll(test => test == this);
            foreach (Waypoint neighbor in neighbors)
            {
                neighbor.AddNeighbor(this);
            }
            foreach (Waypoint node in WaypointManager.Waypoints)
            {
                if (node == this) continue;
                if (node.Neighbors.Contains(this) && !neighbors.Contains(node))
                {
                    node.RemoveNeighbor(this);
                }
            }
        }
        
        [ContextMenu("Refresh Name")]
        private void RefreshName()
        {
            name = reference ? reference.name : name;
        }

        private void OnDrawGizmos()
        {
            if (WaypointManager.Waypoints.Count > 0 && WaypointManager.Waypoints[0] == this) WaypointManager.DrawGizmos();
        }

#endif
        
    }
}