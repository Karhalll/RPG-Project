﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace RPG.Dialogue.Editor
{
	public class DialogueEditor : EditorWindow
	{
		Dialogue selectedDialogue = null;
		GUIStyle nodeStyle = null;
		DialogueNode draggingNode = null;
		Vector2 draggingOffset;

		[MenuItem("Window/Dialogue Editor")]
		public static void ShowEditorWindow()
		{
			GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
		}

		[OnOpenAssetAttribute(1)]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
			if (dialogue  != null)
			{
				ShowEditorWindow();
				return true;
			}
			return false;
		}

		private void OnEnable()
		{
			Selection.selectionChanged += OnSelectionChange;

			nodeStyle = new GUIStyle();
			nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
			nodeStyle.padding = new RectOffset(20, 20, 20, 20);
			nodeStyle.border = new RectOffset(12, 12, 12, 12);
		}

		private void OnSelectionChange()
		{
			Dialogue dialogue = Selection.activeObject as Dialogue;
			if (dialogue != null)
			{
				selectedDialogue = dialogue;
				Repaint();
			}
		}

		private void OnGUI()
		{
			if (selectedDialogue == null)
			{
				EditorGUILayout.LabelField("No Dialogue Selected");
			}
			else
			{
				ProcessEvents();
				foreach (DialogueNode node in selectedDialogue.GetAllNodes())
				{
					OnGUINode(node);
				}
			}
		}

		private void ProcessEvents()
		{
			if (Event.current.type == EventType.MouseDown && draggingNode == null)
			{
				draggingNode = GetNodeAtPoint(Event.current.mousePosition);
				if (draggingNode != null)
				{
					draggingOffset =  draggingNode.rect.position - Event.current.mousePosition;
				}
			}
			else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
			{
        		Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
				draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
        		GUI.changed = true;
			}
			else if (Event.current.type == EventType.MouseUp && draggingNode != null)
			{
        		draggingNode = null;
			}
		}

		private void OnGUINode(DialogueNode node)
		{
			GUILayout.BeginArea(node.rect, nodeStyle);
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Node:");
			string newText = EditorGUILayout.TextField(node.text);
			string newUniqueID = EditorGUILayout.TextField(node.uniqueID);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
				node.text = newText;
				node.uniqueID = newUniqueID;
			}

			GUILayout.EndArea();
		}

		private DialogueNode GetNodeAtPoint(Vector2 point)
		{
			DialogueNode foundValue = null;
			foreach (DialogueNode node in selectedDialogue.GetAllNodes())
			{
				if (node.rect.Contains(point))
				{
          			foundValue = node;
				}
			}
			return foundValue;
		}
	}
}
