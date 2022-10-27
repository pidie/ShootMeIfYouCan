using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Waypoints;

namespace PiDieEditor
{
	#if UNITY_EDITOR
	public class WaypointManagerWindow : EditorWindow
	{
		[MenuItem("Tools/Waypoint Editor")]
		public static void Open() => GetWindow<WaypointManagerWindow>("Waypoint Editor");

		[SerializeField] private Transform waypointRoot;

		private List<int> unusedDigits = new ();
		private bool canResetWaypointIndices;

		private void OnGUI()
		{
			var obj = new SerializedObject(this);

			EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

			if (waypointRoot == null)
			{
				var root = GameObject.Find("Waypoints");

				if (root == null)
					EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.",
						MessageType.Warning);
				else
					waypointRoot = root.transform;
			}
			else
			{
				EditorGUILayout.BeginVertical("box");
				DrawButtons();
				EditorGUILayout.EndVertical();
			}

			obj.ApplyModifiedProperties();
		}

		private void DrawButtons()
		{
			if (GUILayout.Button("Create Waypoint"))
				CreateWaypoint();
			
			if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
			{
				GUI.backgroundColor = Color.yellow;
				
				if (GUILayout.Button("Create Waypoint Before"))
					CreateWaypointBefore();
				if (GUILayout.Button("Create Waypoint After"))
					CreateWaypointAfter();

				var allAreWaypoints = Selection.transforms.All(obj => obj.GetComponent<Waypoint>());

				if (allAreWaypoints)
				{
					GUI.backgroundColor = Color.green;

					switch (Selection.transforms.Length)
					{
						case 2:
						{
							if (GUILayout.Button("Add Waypoint To Path Before"))
								AddWaypointToPathBefore();
							if (GUILayout.Button("Add Waypoint To Path After"))
								AddWaypointToPathAfter();
							if (GUILayout.Button(new GUIContent("Connect via Branch", "The active waypoint becomes a branch of the other waypoint.")))
								ConnectWaypointsViaBranch();
							break;
						}
						case 3 when Selection.transforms.All(t => t.GetComponent<Waypoint>()):
						{
							var unselectedWaypoints = (from transform in Selection.transforms 
								where transform != Selection.activeTransform 
								select transform.GetComponent<Waypoint>()).ToList();

							if (unselectedWaypoints[0].NextWaypoint == unselectedWaypoints[1])
								if (GUILayout.Button("Add Waypoint To Path Between"))
									AddWaypointToPathBetween(unselectedWaypoints[0], unselectedWaypoints[1]);
							if (unselectedWaypoints[1].NextWaypoint == unselectedWaypoints[0])
								if (GUILayout.Button("Add Waypoint To Path Between"))
									AddWaypointToPathBetween(unselectedWaypoints[1], unselectedWaypoints[0]);
							break;
						}
					}
				}

				GUI.backgroundColor = Color.blue;

				if (GUILayout.Button("Split Path Before"))
					SplitPathBefore();
				if (GUILayout.Button("Split Path After"))
					SplitPathAfter();

				GUI.backgroundColor = Color.cyan;
				
				if (GUILayout.Button("Create Branch Waypoint"))
					CreateBranch();

				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Remove Waypoint"))
					RemoveWaypoint();
				if (GUILayout.Button(new GUIContent("Decouple Waypoint", "Removes all references to and from this waypoint.")))
					DecoupleWaypoint();
			}

			if (unusedDigits.Count > 0)
			{
				GUI.backgroundColor = default;
				canResetWaypointIndices = GUILayout.Toggle(canResetWaypointIndices, "Reset Waypoint Indices");
				
				if (canResetWaypointIndices)
				{
					EditorGUILayout.HelpBox("Please verify that you want to reset all waypoint indices.", MessageType.Error);
					GUI.backgroundColor = Color.black;
					if (GUILayout.Button("CONFIRM: OK"))
					{
						canResetWaypointIndices = false;
						unusedDigits.Clear();
					}
				}
			}
		}

		private void CreateWaypoint()
		{
			int waypointNumber;
			
			if (unusedDigits.Count > 0)
			{
				waypointNumber = unusedDigits[0];
				unusedDigits.RemoveAt(0);
			}
			else
				waypointNumber = waypointRoot.childCount;
			
			var waypointObject = new GameObject($"Waypoint {waypointNumber}", typeof(Waypoint));
			waypointObject.transform.SetParent(waypointRoot, false);

			var waypoint = waypointObject.GetComponent<Waypoint>();
			if (waypointRoot.childCount > 1)
			{
				waypoint.PreviousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
				waypoint.PreviousWaypoint.NextWaypoint = waypoint;
				waypoint.transform.position = waypoint.PreviousWaypoint.transform.position;
				waypoint.transform.forward = waypoint.PreviousWaypoint.transform.forward;
			}

			Selection.activeGameObject = waypoint.gameObject;
		}

		private void CreateWaypointBefore()
		{
			int waypointNumber;
			
			if (unusedDigits.Count > 0)
			{
				waypointNumber = unusedDigits[0];
				unusedDigits.Remove(0);
			}
			else
				waypointNumber = waypointRoot.childCount;
			
			var waypointObject = new GameObject($"Waypoint {waypointNumber}", typeof(Waypoint));
			waypointObject.transform.SetParent(waypointRoot, false);

			var newWaypoint = waypointObject.GetComponent<Waypoint>();

			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

			waypointObject.transform.position = selectedWaypoint.transform.position;
			waypointObject.transform.forward = selectedWaypoint.transform.forward;

			if (selectedWaypoint.PreviousWaypoint != null)
			{
				newWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
				selectedWaypoint.PreviousWaypoint.NextWaypoint = newWaypoint;
			}

			newWaypoint.NextWaypoint = selectedWaypoint;

			selectedWaypoint.PreviousWaypoint = newWaypoint;
			
			newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

			Selection.activeGameObject = newWaypoint.gameObject;
		}

		private void CreateWaypointAfter()
		{
			int waypointNumber;
			
			if (unusedDigits.Count > 0)
			{
				waypointNumber = unusedDigits[0];
				unusedDigits.Remove(0);
			}
			else
				waypointNumber = waypointRoot.childCount;
			
			var waypointObject = new GameObject($"Waypoint {waypointNumber}", typeof(Waypoint));
			waypointObject.transform.SetParent(waypointRoot, false);

			var newWaypoint = waypointObject.GetComponent<Waypoint>();

			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

			waypointObject.transform.position = selectedWaypoint.transform.position;
			waypointObject.transform.forward = selectedWaypoint.transform.forward;

			if (selectedWaypoint.NextWaypoint != null)
			{
				selectedWaypoint.NextWaypoint.PreviousWaypoint = newWaypoint;
				newWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
			}

			newWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint.NextWaypoint;
			selectedWaypoint.NextWaypoint = newWaypoint;
			
			newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

			Selection.activeGameObject = newWaypoint.gameObject;
		}

		private static void AddWaypointToPathBefore()
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
			Waypoint inPathWaypoint = default;

			foreach (var obj in Selection.transforms)
			{
				var objWaypoint = obj.GetComponent<Waypoint>();
				if (objWaypoint != selectedWaypoint)
				{
					if (objWaypoint)
						inPathWaypoint = objWaypoint;
				}
			}

			if (inPathWaypoint == null)
			{
				EditorGUILayout.HelpBox("Waypoint in path does not exist.", MessageType.Error);
				return;
			}

			if (inPathWaypoint.PreviousWaypoint != null)
			{
				selectedWaypoint.PreviousWaypoint = inPathWaypoint.PreviousWaypoint;
				inPathWaypoint.PreviousWaypoint.NextWaypoint = selectedWaypoint;
			}

			inPathWaypoint.PreviousWaypoint = selectedWaypoint;
			selectedWaypoint.NextWaypoint = inPathWaypoint;
		}

		private static void AddWaypointToPathAfter()
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
			Waypoint inPathWaypoint = default;

			foreach (var obj in Selection.transforms)
			{
				var objWaypoint = obj.GetComponent<Waypoint>();
				if (objWaypoint != selectedWaypoint)
				{
					if (objWaypoint)
						inPathWaypoint = objWaypoint;
				}
			}

			if (inPathWaypoint == null)
			{
				EditorGUILayout.HelpBox("Waypoint in path does not exist.", MessageType.Error);
				return;
			}

			if (inPathWaypoint.NextWaypoint != null)
			{
				selectedWaypoint.NextWaypoint = inPathWaypoint.NextWaypoint;
				inPathWaypoint.NextWaypoint.PreviousWaypoint = selectedWaypoint;
			}

			inPathWaypoint.NextWaypoint = selectedWaypoint;
			selectedWaypoint.PreviousWaypoint = inPathWaypoint;
		}

		private static void AddWaypointToPathBetween(Waypoint previousWaypoint, Waypoint nextWaypoint)
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

			selectedWaypoint.PreviousWaypoint = previousWaypoint;
			selectedWaypoint.NextWaypoint = nextWaypoint;
			previousWaypoint.NextWaypoint = selectedWaypoint;
			nextWaypoint.PreviousWaypoint = selectedWaypoint;
		}

		private static void ConnectWaypointsViaBranch()
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
			Waypoint otherWaypoint = null;

			foreach (var transform in Selection.transforms)
			{
				if (transform != Selection.activeTransform)
					otherWaypoint = transform.GetComponent<Waypoint>();
			}

			if (otherWaypoint != null)
			{
				otherWaypoint.Branches.Add(selectedWaypoint);
				selectedWaypoint.Branches.Add(otherWaypoint);
			}
		}

		private void CreateBranch()
		{
			int waypointNumber;
			
			if (unusedDigits.Count > 0)
			{
				waypointNumber = unusedDigits[0];
				unusedDigits.Remove(0);
			}
			else
				waypointNumber = waypointRoot.childCount;
			
			var waypointObject = new GameObject($"Waypoint {waypointNumber}", typeof(Waypoint));
			waypointObject.transform.SetParent(waypointRoot, false);

			var waypoint = waypointObject.GetComponent<Waypoint>();

			var branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
			branchedFrom.Branches.Add(waypoint);
			waypoint.Branches.Add(branchedFrom);

			waypoint.transform.position = branchedFrom.transform.position;
			waypoint.transform.forward = branchedFrom.transform.forward;

			Selection.activeGameObject = waypoint.gameObject;
		}

		private static void SplitPathBefore()
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

			if (selectedWaypoint.PreviousWaypoint != null)
			{	
				selectedWaypoint.PreviousWaypoint.NextWaypoint = default;
				selectedWaypoint.PreviousWaypoint = default;
			}
		}

		private static void SplitPathAfter()
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

			if (selectedWaypoint.NextWaypoint)
			{
				selectedWaypoint.NextWaypoint.PreviousWaypoint = default;
				selectedWaypoint.NextWaypoint = default;
			}
		}

		private void RemoveWaypoint()
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

			if (selectedWaypoint.NextWaypoint != null)
				selectedWaypoint.NextWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;

			if (selectedWaypoint.PreviousWaypoint != null)
			{
				selectedWaypoint.PreviousWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
				Selection.activeGameObject = selectedWaypoint.PreviousWaypoint.gameObject;
			}
			
			unusedDigits.Add(int.Parse(selectedWaypoint.name.Split(" ")[1]));
			unusedDigits.Sort();
			
			DestroyImmediate(selectedWaypoint.gameObject);
		}

		private static void DecoupleWaypoint()
		{
			var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

			if (selectedWaypoint.PreviousWaypoint != null)
			{
				selectedWaypoint.PreviousWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
				selectedWaypoint.PreviousWaypoint.Branches.Remove(selectedWaypoint);
			}

			if (selectedWaypoint.NextWaypoint != null)
			{
				selectedWaypoint.NextWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
				selectedWaypoint.NextWaypoint.Branches.Remove(selectedWaypoint);
			}
	
			selectedWaypoint.PreviousWaypoint = default;
			selectedWaypoint.NextWaypoint = default;
			selectedWaypoint.Branches.Clear();
		}
	}
	#endif
}