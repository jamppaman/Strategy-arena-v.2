using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpellSystem
{
    [CustomEditor(typeof(EffectCreator))]
    public class EffectEditor : Editor
    {
        EffectCreator effCr;
        SerializedObject GetThing;
        SerializedProperty attriList;

        void OnEnable()
        {
            effCr = (EffectCreator)target;
            GetThing = new SerializedObject(effCr);
            attriList = GetThing.FindProperty("effectArvot"); // Find the List in our script and create a refrence of it

        }

        public override void OnInspectorGUI()
        {
            GetThing.Update();
            EditorGUILayout.Space();

            // Default Attributes
            SerializedProperty effectName = GetThing.FindProperty("effectName");
            SerializedProperty effectDuration = GetThing.FindProperty("effectDuration");

            SerializedProperty isAtTurnStart = GetThing.FindProperty("isAtTurnStart");
            SerializedProperty canBeSilenced = GetThing.FindProperty("canBeSilenced");
            SerializedProperty stacks = GetThing.FindProperty("stacks");

            SerializedProperty targeting = GetThing.FindProperty("targeting");
            SerializedProperty friendlyFire = GetThing.FindProperty("friendlyFire");

            SerializedProperty effectIcon = GetThing.FindProperty("effectIcon");
            SerializedProperty description = GetThing.FindProperty("description");

            // ulkonäön asettelu
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Base variables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(effectName);
            EditorGUILayout.PropertyField(canBeSilenced);
            EditorGUILayout.PropertyField(isAtTurnStart);
            EditorGUILayout.PropertyField(stacks);
            EditorGUILayout.PropertyField(effectDuration);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Targeting variables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targeting);
            EditorGUILayout.PropertyField(friendlyFire);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fluff", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(effectIcon);
            EditorGUILayout.PropertyField(description);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add a new Attribute with a button", EditorStyles.boldLabel);

            // dont touch ------------------------------
            if (GUILayout.Button("Add Attribute"))
            {
                effCr.effectArvot.Add(new EffectCreator.EffectType());
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            // -----------------------------------------

            // Stuff inside List. edit with caution if you break this, i will break you!
            for (int i = 0; i < attriList.arraySize; i++)
            {
                SerializedProperty attriRefe = attriList.GetArrayElementAtIndex(i);
                SerializedProperty AttributeState = attriRefe.FindPropertyRelative("effect");
                EditorGUILayout.PropertyField(AttributeState);
                EditorGUILayout.Space();
                if (AttributeState != null)
                {
                    EffectCreator.EffectType.Effect state = (EffectCreator.EffectType.Effect)AttributeState.enumValueIndex;

                    //Add stuff Here
                    switch (state)
                    {
                        case EffectCreator.EffectType.Effect.apModify:
                            SerializedProperty apModify = attriRefe.FindPropertyRelative("apModify");
                            EditorGUILayout.PropertyField(apModify);
                            break;
                        case EffectCreator.EffectType.Effect.armorBuffPercent:
                            SerializedProperty armorModifyPercent = attriRefe.FindPropertyRelative("armorModifyPercent");
                            EditorGUILayout.PropertyField(armorModifyPercent);
                            break;
                        case EffectCreator.EffectType.Effect.armorBuffPlus:
                            SerializedProperty armorModifyPlus = attriRefe.FindPropertyRelative("armorModifyPlus");
                            EditorGUILayout.PropertyField(armorModifyPlus);
                            break;
                        case EffectCreator.EffectType.Effect.damageBuffPercent:
                            SerializedProperty damageModifyPercent = attriRefe.FindPropertyRelative("damageModifyPercent");
                            EditorGUILayout.PropertyField(damageModifyPercent);
                            break;
                        case EffectCreator.EffectType.Effect.damageBuffPlus:
                            SerializedProperty damageModifyPlus = attriRefe.FindPropertyRelative("damageModifyPlus");
                            EditorGUILayout.PropertyField(damageModifyPlus);
                            break;
                        case EffectCreator.EffectType.Effect.damageOverTime:
                            SerializedProperty damageOverTime = attriRefe.FindPropertyRelative("damageOverTime");
                            EditorGUILayout.PropertyField(damageOverTime);
                            break;
                        case EffectCreator.EffectType.Effect.healModifier:
                            SerializedProperty healModify = attriRefe.FindPropertyRelative("healModify");
                            EditorGUILayout.PropertyField(healModify);
                            break;
                        case EffectCreator.EffectType.Effect.healOverTime:
                            SerializedProperty healOverTime = attriRefe.FindPropertyRelative("healOverTime");
                            EditorGUILayout.PropertyField(healOverTime);
                            break;
                        case EffectCreator.EffectType.Effect.heavy:
                            SerializedProperty heavyState = attriRefe.FindPropertyRelative("heavyState");
                            EditorGUILayout.PropertyField(heavyState);
                            break;
                        case EffectCreator.EffectType.Effect.immunity:
                            SerializedProperty immune = attriRefe.FindPropertyRelative("immune");
                            EditorGUILayout.PropertyField(immune);
                            break;
                        case EffectCreator.EffectType.Effect.mpModify:
                            SerializedProperty mpModify = attriRefe.FindPropertyRelative("mpModify");
                            EditorGUILayout.PropertyField(mpModify);
                            break;
                    }
                }
                // Dont touch anything after this------------------------------------
                if (GUILayout.Button("Remove attribute above"))
                {
                    attriList.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            GetThing.ApplyModifiedProperties();
        }
    }
}

