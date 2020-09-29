using System;
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
        [NonSerialized]
        GUIStyle nodeStyle = null;
        [NonSerialized]
        DialogueNode draggingNode = null;
        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;

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
                    DrawConnection(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
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

        private void DrawNode(DialogueNode node)
    {
      GUILayout.BeginArea(node.rect, nodeStyle);
      EditorGUI.BeginChangeCheck();

      string newText = EditorGUILayout.TextField(node.text);

      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
        node.text = newText;
      }

      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Add New"))
      {
        creatingNode = node;
      }
      DrawLinkButtons(node);
      if (GUILayout.Button("Delete"))
      {
        deletingNode = node;
      }

      GUILayout.EndHorizontal();

      GUILayout.EndArea();
    }

    private void DrawLinkButtons(DialogueNode node)
    {
        if (linkingParentNode == null)
        {
            if (GUILayout.Button("Link"))
            {
                linkingParentNode = node;
            }
        }
        else if (node == linkingParentNode)
        {
            if (GUILayout.Button("Cancle"))
            {
                linkingParentNode = null;
            }
        }
        else if (linkingParentNode.children.Contains(node.uniqueID))
        {
            if (GUILayout.Button("Unlink"))
            {
                Undo.RecordObject(selectedDialogue, "Delete Dialogue Link");
                linkingParentNode.children.Remove(node.uniqueID);
                linkingParentNode = null;
            }
        }
        else 
        {
            if (GUILayout.Button("Child"))
            {
                Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                linkingParentNode.children.Add(node.uniqueID);
                linkingParentNode = null;
            }
        }
    }

    private void DrawConnection(DialogueNode node)
        {
            Vector2 startPosition = new Vector2(
                node.rect.xMax - nodeStyle.border.right / 2f,
                node.rect.center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector2 endPosition = new Vector2(
                    childNode.rect.xMin + nodeStyle.border.left/2f,
                    childNode.rect.center.y);
                Vector2 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.5f;

                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset, 
                    Color.white, null, 4f);
            }
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
