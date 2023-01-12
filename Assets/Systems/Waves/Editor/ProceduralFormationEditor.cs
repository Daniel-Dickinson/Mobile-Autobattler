using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TwoBears.Waves
{
    [CustomEditor(typeof(ProceduralFormation))]
    public class ProceduralFormationEditor : Editor
    {
        //Properties
        private SerializedProperty waves;

        //Foldouts
        private SerializedProperty foldoutOne;
        private SerializedProperty foldoutTwo;
        private SerializedProperty foldoutThree;
        private SerializedProperty foldoutFour;
        private SerializedProperty foldoutFive;
        private SerializedProperty foldoutSix;
        private SerializedProperty foldoutSeven;
        private SerializedProperty foldoutEight;
        private SerializedProperty foldoutNine;
        private SerializedProperty foldoutTen;

        //Editor
        private void OnEnable()
        {
            waves = serializedObject.FindProperty("waves");

            foldoutOne = serializedObject.FindProperty("foldoutOne");
            foldoutTwo = serializedObject.FindProperty("foldoutTwo");
            foldoutThree = serializedObject.FindProperty("foldoutThree");
            foldoutFour = serializedObject.FindProperty("foldoutFour");
            foldoutFive = serializedObject.FindProperty("foldoutFive");
            foldoutSix = serializedObject.FindProperty("foldoutSix");
            foldoutSeven = serializedObject.FindProperty("foldoutSeven");
            foldoutEight = serializedObject.FindProperty("foldoutEight");
            foldoutNine = serializedObject.FindProperty("foldoutNine");
            foldoutTen = serializedObject.FindProperty("foldoutTen");
        }
        public override void OnInspectorGUI()
        {
            //Must have 100 waves in array
            if (waves.arraySize != 100)
            {
                waves.arraySize = 100;
                serializedObject.ApplyModifiedProperties();
            }

            //Update
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            //Waves
            WavesGUI(foldoutOne, serializedObject.targetObject as ProceduralFormation, 0, 9);
            WavesGUI(foldoutTwo, serializedObject.targetObject as ProceduralFormation, 10, 19);
            WavesGUI(foldoutThree, serializedObject.targetObject as ProceduralFormation, 20, 29);
            WavesGUI(foldoutFour, serializedObject.targetObject as ProceduralFormation, 30, 39);
            WavesGUI(foldoutFive, serializedObject.targetObject as ProceduralFormation, 40, 49);
            WavesGUI(foldoutSix, serializedObject.targetObject as ProceduralFormation, 50, 59);
            WavesGUI(foldoutSeven, serializedObject.targetObject as ProceduralFormation, 60, 69);
            WavesGUI(foldoutEight, serializedObject.targetObject as ProceduralFormation, 70, 79);
            WavesGUI(foldoutNine, serializedObject.targetObject as ProceduralFormation, 80, 89);
            WavesGUI(foldoutTen, serializedObject.targetObject as ProceduralFormation, 90, 99);

            //Apply
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }

        //Waves GUI
        private void WavesGUI(SerializedProperty foldout, ProceduralFormation formation, int start, int end)
        {
            //Header
            using (new EditorGUILayout.HorizontalScope())
            {
                foldout.boolValue = EditorGUILayout.Foldout(foldout.boolValue, new GUIContent("Waves " + (start + 1) + " - " + (end + 1)), EditorStyles.foldoutHeader);
            }

            //Content
            if (foldout.boolValue)
            {
                EditorGUI.indentLevel++;
                for (int i = start; i <= end; i++) WaveGUI(formation, i);
                EditorGUI.indentLevel--;
            }

            //Spacer
            EditorGUILayout.Space();
        }
        private void WaveGUI(ProceduralFormation formation, int index)
        {
            //Grab properties
            SerializedProperty wave = waves.GetArrayElementAtIndex(index);
            SerializedProperty draft = wave.FindPropertyRelative("draft");
            SerializedProperty reward = wave.FindPropertyRelative("reward");
            SerializedProperty frontPoints = wave.FindPropertyRelative("frontPoints");
            SerializedProperty middlePoints = wave.FindPropertyRelative("middlePoints");
            SerializedProperty backPoints = wave.FindPropertyRelative("backPoints");
            

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wave " + (index + 1), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(draft);
            EditorGUILayout.PropertyField(reward);

            EditorGUILayout.PropertyField(frontPoints);
            EditorGUILayout.PropertyField(middlePoints);
            EditorGUILayout.PropertyField(backPoints);

            EditorGUILayout.LabelField("Total : " + (frontPoints.intValue + middlePoints.intValue + backPoints.intValue) + " / " + formation.GetPlayerPointsForWave(index).ToString());
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}