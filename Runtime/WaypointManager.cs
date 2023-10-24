using System.Collections.Generic;
using IronMountain.Wayfinding.DataStructures;
using UnityEditor;
using UnityEngine;

namespace IronMountain.Wayfinding
{
    public static class WaypointManager
    {
        public static readonly List<Waypoint> Waypoints = new ();
        public static readonly List<WaypointReference> WaypointReferences = new ();

        public static bool ShouldDrawLines = true;
        public static bool ShouldDrawDiscs = true;
        public static bool ShouldDrawLabels = false;

        public static Waypoint GetWaypoint(string id)
        {
            return Waypoints.Find(test => 
                test 
                && test.Reference 
                && test.Reference.ID == id);
        }
        
        public static Waypoint GetWaypoint(WaypointReference reference)
        {
            return reference 
                ? Waypoints.Find(test => test && test.Reference == reference)
                : null;
        }

        public static Waypoint GetClosestWaypointTo(Vector3 position)
        {
            Waypoint closestWaypoint = null;
            float closestDistance = Mathf.Infinity;
            foreach (Waypoint waypoint in Waypoints)
            {
                if (!waypoint) continue;
                float testDistance = Vector3.Distance(waypoint.transform.position, position);
                if (testDistance < closestDistance)
                {
                    closestWaypoint = waypoint;
                    closestDistance = testDistance;
                }
            }
            return closestWaypoint;
        }
        
        public static Waypoint GetClosestWaypointTo(Vector3 position, float idealDistance)
        {
            Waypoint bestWaypoint = null;
            float bestDelta = Mathf.Infinity;
            foreach (Waypoint waypoint in Waypoints)
            {
                if (!waypoint) continue;
                float testDistance = Vector3.Distance(waypoint.transform.position, position);
                float testDelta = Mathf.Abs(testDistance - idealDistance);
                if (testDelta < bestDelta)
                {
                    bestWaypoint = waypoint;
                    bestDelta = testDelta;
                }
            }
            return bestWaypoint;
        }

        public static List<Waypoint> GetShortestPath(Waypoint start, Waypoint end)
        {
            List<Waypoint> path = new List<Waypoint>();

            SimplePriorityQueue<Waypoint> openSet = new SimplePriorityQueue<Waypoint>();
            openSet.Enqueue(start, Mathf.Infinity);

            Dictionary<Waypoint, Waypoint> previousNodeMapping = new Dictionary<Waypoint, Waypoint>();
            
            Dictionary<Waypoint, float> costToNodeFromStart = new Dictionary<Waypoint, float>();

            foreach (Waypoint node in Waypoints)
            {
                costToNodeFromStart.Add(node, Mathf.Infinity);
            }
            
            costToNodeFromStart[start] = 0;
            
            while (openSet.Count > 0)
            {
                Waypoint current = openSet.Dequeue();
                if (current == end) return ReconstructPath(previousNodeMapping, current);
                foreach (Waypoint neighbor in current.Neighbors)
                {
                    float tentativeGScore = costToNodeFromStart[current] + Distance(current, neighbor);
                    if (tentativeGScore < costToNodeFromStart[neighbor])
                    {
                        previousNodeMapping[neighbor] = current;
                        costToNodeFromStart[neighbor] = tentativeGScore;
                        float fScore = tentativeGScore + Distance(neighbor, end);
                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore);
                        }
                        else openSet.UpdatePriority(neighbor, fScore);
                    }
                }
            }
            return path;
        }
        
        private static float Distance(Waypoint a, Waypoint b)
        {
            if (!a || !b) return Mathf.Infinity;
            return Vector3.Distance(a.transform.position, b.transform.position);
        }
        
        private static List<Waypoint> ReconstructPath(Dictionary<Waypoint, Waypoint> previousNodeMapping, Waypoint current)
        {
            List<Waypoint> path = new List<Waypoint> {current};
            while (previousNodeMapping.ContainsKey(current))
            {
                current = previousNodeMapping[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
        
        private static GUIStyle _guiStyle;
        private static GUIStyleState _gUIStyleState;

        private static GUIStyle GUIStyle => _guiStyle ??= new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter
        };
        
        private static GUIStyleState GUIStyleState => _gUIStyleState ??= new GUIStyleState
        {
            textColor = Color.black
        };
        
#if UNITY_EDITOR

        public static void DrawGizmos()
        {
            if (ShouldDrawLines) DrawLines();
            if (ShouldDrawDiscs) DrawDiscs();
            if (ShouldDrawLabels) DrawLabels();
        }

        private static void DrawLines()
        {
            foreach (var waypoint in Waypoints)
            {
                if (!waypoint) continue;
                foreach (Waypoint neighbor in waypoint.Neighbors)
                {
                    if (!neighbor) continue;
                    Vector3 p1 = waypoint.transform.position;
                    Vector3 p2 = neighbor.transform.position;
                    if (p1.x < p2.x 
                        || p1.x == p2.x && p1.y < p2.y
                        || p1.x == p2.x && p1.y == p2.y && p1.z < p2.z) continue;
                    Handles.color = Color.white;
                    Handles.DrawLine(p1, p2, 1);
                }
            }
        }

        private static void DrawDiscs()
        {
            foreach (var waypoint in Waypoints)
            {
                if (!waypoint) continue;
                Handles.color = Color.white;
                Handles.DrawSolidDisc(waypoint.transform.position, Vector3.up, .1f);
            }
        }

        private static void DrawLabels()
        {
            GUIStyle.alignment = TextAnchor.UpperCenter;
            GUIStyle.normal = GUIStyleState;
            GUIStyle.normal.background = Texture2D.whiteTexture;
            GUIStyleState.textColor = Color.red;
            foreach (var waypoint in Waypoints)
            {
                if (!waypoint) continue;
                Handles.Label(
                    waypoint.transform.position, 
                    waypoint.Reference ? waypoint.Reference.Name : string.Empty,
                    GUIStyle);
            }
        }

#endif
        
    }
}