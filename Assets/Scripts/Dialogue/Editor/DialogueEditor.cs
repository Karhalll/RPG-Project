using System.Collections;
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
				foreach (DialogueNode node in selectedDialogue.GetAllNodes())
				{
					OnGUINode(node);
				}
			}
		}

		private void OnGUINode(DialogueNode node)
		{
			GUILayout.BeginArea(node.position, nodeStyle);
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
	}
}
