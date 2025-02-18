using ClumsyWizard.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ClumsyWizard.CWEditor
{
    [CustomEditor(typeof(CW_CanvasLayer))]
    public class CW_CanvasLayerEditor : Editor
    {
        private CW_CanvasLayer canvasLayer;
        private CW_CanvasLayerDataSO dataHolder;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            canvasLayer = (CW_CanvasLayer)target;

            string[] guids = AssetDatabase.FindAssets("t:scriptableobject", new string[] { "Assets/_ClumsyWizard/Data" });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                dataHolder = (CW_CanvasLayerDataSO)AssetDatabase.LoadAssetAtPath(assetPath, typeof(CW_CanvasLayerDataSO));

                if (dataHolder != null)
                    break;
            }

            if (dataHolder != null)
            {
                SerializedProperty layerNameProp = serializedObject.FindProperty("LayerName");

                int selectionIndex = dataHolder.IndexOf(layerNameProp.stringValue);

                SerializedProperty layerProp = serializedObject.FindProperty("Layer");
                int selectedLayer = EditorGUILayout.Popup("Layer", selectionIndex, dataHolder.Layers.ToArray());

                layerNameProp.stringValue = dataHolder.ValueAt(selectedLayer);
                layerProp.intValue = selectedLayer;

                canvasLayer.UpdateCanvas();

                GUILayout.Space(20);

                if(GUILayout.Button("Edit Layers", GUILayout.Height(30.0f)))
                {
                    Selection.activeObject = dataHolder;
                }
            }
            else
            {
                GUILayout.Label("No file of type CW_CanvasLayerDataSO found at [Assets/_ClumsyWizard/Data]. Please Create this SO file.");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}