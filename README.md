# Wayfinding
*Version: 1.1.3*
## Description: 
AI pathfinding with referencable waypoints
## Package Mirrors: 
[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODg3LnBuZw==/original/npRUfq.png'>](https://github.com/Iron-Mountain-Software/wayfinding)[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODkyLnBuZw==/original/Fq0ORM.png'>](https://www.npmjs.com/package/com.iron-mountain.wayfinding)[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODk4LnBuZw==/original/Rv4m96.png'>](https://iron-mountain.itch.io/wayfinding)
---
## Key Scripts & Components: 
1. public class **Waypoint** : MonoBehaviour
   * Properties: 
      * public WaypointReference ***Reference***  { get; set; }
      * public List<Waypoint> ***Neighbors***  { get; }
   * Methods: 
      * public void ***AddNeighbor***(Waypoint neighbor)
      * public void ***RemoveNeighbor***(Waypoint neighbor)
1. public static class **WaypointManager**
1. public class **WaypointReference** : ScriptableObject
   * Properties: 
      * public String ***ID***  { get; set; }
      * public String ***Name***  { get; }
   * Methods: 
      * public virtual void ***Reset***()
1. public class **WaypointTraverser** : MonoBehaviour
   * Actions: 
      * public event Action ***OnMovingChanged*** 
      * public event Action ***OnCurrentWaypointChanged*** 
   * Properties: 
      * public float ***Speed***  { get; set; }
      * public float ***Multiplier***  { get; set; }
      * public Vector3 ***Offset***  { get; set; }
      * public Boolean ***Rotate***  { get; set; }
      * public float ***RotationMultiplier***  { get; set; }
      * public Boolean ***Moving***  { get; }
      * public Waypoint ***CurrentWaypoint***  { get; set; }
      * public Waypoint ***DestinationWaypoint***  { get; set; }
   * Methods: 
      * public void ***Initialize***(Waypoint start, Waypoint destination)
      * public Vector3 ***GetDirection***()
1. public class **WaypointTraverserAnimator** : MonoBehaviour
   * Properties: 
      * public Boolean ***AnimatorBoolValue***  { get; }
   * Methods: 
      * public void ***SetBool***(Boolean value)
### Data Structures
1. public class **FastPriorityQueueNode**
   * Properties: 
      * public float ***Priority***  { get; }
      * public Int32 ***QueueIndex***  { get; }
      * public Object ***Queue***  { get; }
1. public class **FastPriorityQueue`1**
   * Properties: 
      * public Int32 ***Count***  { get; }
      * public Int32 ***MaxSize***  { get; }
      * public T ***First***  { get; }
   * Methods: 
      * public virtual void ***Clear***()
      * public virtual Boolean ***Contains***(T node)
      * public virtual void ***Enqueue***(T node, float priority)
      * public virtual T ***Dequeue***()
      * public virtual void ***Resize***(Int32 maxNodes)
      * public virtual void ***UpdatePriority***(T node, float priority)
      * public virtual void ***Remove***(T node)
      * public virtual void ***ResetNode***(T node)
      * public virtual IEnumerator`1 ***GetEnumerator***()
      * public Boolean ***IsValidQueue***()
1. public class **GenericPriorityQueueNode`1**
   * Properties: 
      * public TPriority ***Priority***  { get; }
      * public Int32 ***QueueIndex***  { get; }
      * public Int64 ***InsertionIndex***  { get; }
      * public Object ***Queue***  { get; }
1. public class **GenericPriorityQueue`2**
   * Properties: 
      * public Int32 ***Count***  { get; }
      * public Int32 ***MaxSize***  { get; }
      * public TItem ***First***  { get; }
   * Methods: 
      * public virtual void ***Clear***()
      * public virtual Boolean ***Contains***(TItem node)
      * public virtual void ***Enqueue***(TItem node, TPriority priority)
      * public virtual TItem ***Dequeue***()
      * public virtual void ***Resize***(Int32 maxNodes)
      * public virtual void ***UpdatePriority***(TItem node, TPriority priority)
      * public virtual void ***Remove***(TItem node)
      * public virtual void ***ResetNode***(TItem node)
      * public virtual IEnumerator`1 ***GetEnumerator***()
      * public Boolean ***IsValidQueue***()
1. public interface **IPriorityQueue`2**
   * Properties: 
      * public TItem ***First***  { get; }
      * public Int32 ***Count***  { get; }
   * Methods: 
      * public abstract void ***Enqueue***(TItem node, TPriority priority)
      * public abstract TItem ***Dequeue***()
      * public abstract void ***Clear***()
      * public abstract Boolean ***Contains***(TItem node)
      * public abstract void ***Remove***(TItem node)
      * public abstract void ***UpdatePriority***(TItem node, TPriority priority)
1. public class **SimplePriorityQueue`1** : SimplePriorityQueue`2
1. public class **SimplePriorityQueue`2**
   * Properties: 
      * public Int32 ***Count***  { get; }
      * public TItem ***First***  { get; }
   * Methods: 
      * public virtual void ***Clear***()
      * public virtual Boolean ***Contains***(TItem item)
      * public virtual TItem ***Dequeue***()
      * public virtual void ***Enqueue***(TItem item, TPriority priority)
      * public Boolean ***EnqueueWithoutDuplicates***(TItem item, TPriority priority)
      * public virtual void ***Remove***(TItem item)
      * public virtual void ***UpdatePriority***(TItem item, TPriority priority)
      * public TPriority ***GetPriority***(TItem item)
      * public Boolean ***TryFirst***(TItem& first)
      * public Boolean ***TryDequeue***(TItem& first)
      * public Boolean ***TryRemove***(TItem item)
      * public Boolean ***TryUpdatePriority***(TItem item, TPriority priority)
      * public Boolean ***TryGetPriority***(TItem item, TPriority& priority)
      * public virtual IEnumerator`1 ***GetEnumerator***()
      * public Boolean ***IsValidQueue***()
1. public class **StablePriorityQueueNode** : FastPriorityQueueNode
   * Properties: 
      * public Int64 ***InsertionIndex***  { get; }
1. public class **StablePriorityQueue`1**
   * Properties: 
      * public Int32 ***Count***  { get; }
      * public Int32 ***MaxSize***  { get; }
      * public T ***First***  { get; }
   * Methods: 
      * public virtual void ***Clear***()
      * public virtual Boolean ***Contains***(T node)
      * public virtual void ***Enqueue***(T node, float priority)
      * public virtual T ***Dequeue***()
      * public virtual void ***Resize***(Int32 maxNodes)
      * public virtual void ***UpdatePriority***(T node, float priority)
      * public virtual void ***Remove***(T node)
      * public virtual void ***ResetNode***(T node)
      * public virtual IEnumerator`1 ***GetEnumerator***()
      * public Boolean ***IsValidQueue***()
