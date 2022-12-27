using UnityEngine;
using UnityEditor;

namespace RadiantGI.Universal {

    [CustomEditor(typeof(RadiantVirtualEmitter))]
    public class RadiantVirtualEmitterEditor : Editor {

        SerializedProperty color, intensity;
        SerializedProperty addMaterialEmission, targetRenderer, emissionPropertyName, materialIndex;
        SerializedProperty boxCenter, boxSize;

        void OnEnable() {
            color = serializedObject.FindProperty("color");
            intensity = serializedObject.FindProperty("intensity");
            addMaterialEmission = serializedObject.FindProperty("addMaterialEmission");
            targetRenderer = serializedObject.FindProperty("targetRenderer");
            emissionPropertyName = serializedObject.FindProperty("emissionPropertyName");
            materialIndex = serializedObject.FindProperty("materialIndex");
            boxCenter = serializedObject.FindProperty("boxCenter");
            boxSize = serializedObject.FindProperty("boxSize");
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(color);
            EditorGUILayout.PropertyField(addMaterialEmission);
            if (addMaterialEmission.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(targetRenderer);
                EditorGUILayout.PropertyField(emissionPropertyName);
                EditorGUILayout.PropertyField(materialIndex);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(intensity);
            EditorGUILayout.PropertyField(boxCenter);
            EditorGUILayout.PropertyField(boxSize);

            serializedObject.ApplyModifiedProperties();

        }

    }


    public static class RadiantVirtualEmitterEditorExtension {

        [MenuItem("GameObject/Create Other/Radiant GI/Virtual Emitter")]
        static void CreateEmitter(MenuCommand menuCommand) {
            GameObject emitter = new GameObject("Radiant Virtual Emitter", typeof(RadiantVirtualEmitter));

            GameObjectUtility.SetParentAndAlign(emitter, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(emitter, "Create Virtual Emitter");
            Selection.activeObject = emitter;
        }

    }
}

