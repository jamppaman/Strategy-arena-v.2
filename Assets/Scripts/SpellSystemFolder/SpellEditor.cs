using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SpellSystem
{
    [CustomEditor(typeof(SpellCreator))]
    public class SpellEditor : Editor
    {
        SpellCreator creto;
        SerializedObject GetTarget;
        SerializedProperty attriList;

        void OnEnable()
        {
            creto = (SpellCreator)target;
            GetTarget = new SerializedObject(creto);
            attriList = GetTarget.FindProperty("spellArvot"); // Find the List in our script and create a refrence of it

        }

        public override void OnInspectorGUI()
        {
            GetTarget.Update();
            EditorGUILayout.Space();
            // Default Attributes
            SerializedProperty spellName = GetTarget.FindProperty("spellName");
            SerializedProperty apCost = GetTarget.FindProperty("spellApCost");
            SerializedProperty rangeMin = GetTarget.FindProperty("rangeMin");
            SerializedProperty rangeMax = GetTarget.FindProperty("rangeMax");
            SerializedProperty areaOfEffect = GetTarget.FindProperty("areaOfEffect");
            SerializedProperty areaType = GetTarget.FindProperty("areaType");
            SerializedProperty rangeType = GetTarget.FindProperty("rangeType");
            SerializedProperty needLineOfSight = GetTarget.FindProperty("needLineOfSight");
            SerializedProperty needTarget = GetTarget.FindProperty("needTarget");
            SerializedProperty needFreeSquare = GetTarget.FindProperty("needFreeSquare");

            // cooldown stuff
            SerializedProperty spellInitialCooldown = GetTarget.FindProperty("spellInitialCooldown");
            SerializedProperty spellCooldown = GetTarget.FindProperty("spellCooldown");
            SerializedProperty spellCastPerturn = GetTarget.FindProperty("spellCastPerturn");
            SerializedProperty castPerTarget = GetTarget.FindProperty("castPerTarget");


            // fluff
            SerializedProperty spellIcon = GetTarget.FindProperty("spellIcon");
            SerializedProperty spellSound = GetTarget.FindProperty("spellSound");
            SerializedProperty spellDescription = GetTarget.FindProperty("description");


            // ulkonäön asettelu
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Base variables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spellName);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(apCost);
            EditorGUILayout.PropertyField(rangeType);
            EditorGUILayout.PropertyField(areaType);
            EditorGUILayout.PropertyField(areaOfEffect);
            EditorGUILayout.PropertyField(rangeMax);
            EditorGUILayout.PropertyField(rangeMin);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Targeting variables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(needLineOfSight);
            EditorGUILayout.PropertyField(needTarget);
            EditorGUILayout.PropertyField(needFreeSquare);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Cooldown variables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spellInitialCooldown);
            EditorGUILayout.PropertyField(spellCooldown);
            EditorGUILayout.PropertyField(spellCastPerturn);
            EditorGUILayout.PropertyField(castPerTarget);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fluff", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spellIcon);
            EditorGUILayout.PropertyField(spellSound);
            EditorGUILayout.PropertyField(spellDescription);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add a new Attribute with a button", EditorStyles.boldLabel);
            // dont touch ------------------------------
            if (GUILayout.Button("Add Attribute"))
            {
                creto.spellArvot.Add(new SpellCreator.SpellAttribute());
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            // -----------------------------------------

            // Stuff inside List. edit with caution if you break this, i will break you!
            for (int i = 0; i < attriList.arraySize; i++)
            {
                SerializedProperty attriRefe = attriList.GetArrayElementAtIndex(i);
                SerializedProperty isSingleUse = attriRefe.FindPropertyRelative("isSingleUse");
                EditorGUILayout.PropertyField(isSingleUse);
                SerializedProperty AttributeState = attriRefe.FindPropertyRelative("attributeType");
                EditorGUILayout.PropertyField(AttributeState);
                EditorGUILayout.Space();
                if (AttributeState != null)
                {
                    SpellCreator.SpellAttribute.AttributeType state = (SpellCreator.SpellAttribute.AttributeType)AttributeState.enumValueIndex;

                    //Add stuff Here
                    switch (state)
                    {
                        case SpellCreator.SpellAttribute.AttributeType.damage:
                            SerializedProperty spellDamageMin = attriRefe.FindPropertyRelative("spellDamageMin");
                            SerializedProperty spellDamageMax = attriRefe.FindPropertyRelative("spellDamageMax");
                            SerializedProperty hurtsAlly = attriRefe.FindPropertyRelative("hurtsAlly");
                            EditorGUILayout.PropertyField(spellDamageMin);
                            EditorGUILayout.PropertyField(spellDamageMax);
                            EditorGUILayout.PropertyField(hurtsAlly);
                            break;

                        case SpellCreator.SpellAttribute.AttributeType.steal:
                            SerializedProperty stealDamageMin = attriRefe.FindPropertyRelative("stealDamageMin");
                            SerializedProperty stealDamageMax = attriRefe.FindPropertyRelative("stealDamageMax");
                            SerializedProperty stealHurtsAlly = attriRefe.FindPropertyRelative("stealHurtsAlly");
                            EditorGUILayout.PropertyField(stealDamageMin);
                            EditorGUILayout.PropertyField(stealDamageMax);
                            EditorGUILayout.PropertyField(stealHurtsAlly);
                            break;

                        case SpellCreator.SpellAttribute.AttributeType.heal:
                            SerializedProperty spellHealMin = attriRefe.FindPropertyRelative("spellHealMin");
                            SerializedProperty spellHealMax = attriRefe.FindPropertyRelative("spellHealMax");
                            SerializedProperty healsAll = attriRefe.FindPropertyRelative("healsAll");
                            EditorGUILayout.PropertyField(spellHealMin);
                            EditorGUILayout.PropertyField(spellHealMax);
                            EditorGUILayout.PropertyField(healsAll);
                            break;

                        case SpellCreator.SpellAttribute.AttributeType.walk:
                            SerializedProperty movemenPoints = attriRefe.FindPropertyRelative("movemenPoints");
                            SerializedProperty moveTowards = attriRefe.FindPropertyRelative("moveTowards");
                            EditorGUILayout.PropertyField(movemenPoints);
                            EditorGUILayout.PropertyField(moveTowards);
                            break;

                        case SpellCreator.SpellAttribute.AttributeType.teleport:
                            SerializedProperty switchWithTarget = attriRefe.FindPropertyRelative("switchWithTarget");
                            EditorGUILayout.PropertyField(switchWithTarget);
                            break;

                        case SpellCreator.SpellAttribute.AttributeType.pullpush:
                            SerializedProperty spellPullPush = attriRefe.FindPropertyRelative("spellPullPush");
                            SerializedProperty mySpellPullPushType = attriRefe.FindPropertyRelative("mySpellPullPushType");
                            SerializedProperty isItPull = attriRefe.FindPropertyRelative("isItPull");
                            SerializedProperty wallSpat = attriRefe.FindPropertyRelative("wallSplatDmg");
                            EditorGUILayout.PropertyField(mySpellPullPushType);
                            EditorGUILayout.PropertyField(spellPullPush);
                            EditorGUILayout.PropertyField(isItPull);
                            EditorGUILayout.PropertyField(wallSpat);
                            break;

                        case SpellCreator.SpellAttribute.AttributeType.effect:
                            SerializedProperty effect = attriRefe.FindPropertyRelative("effect");
                            SerializedProperty effectOnCaster = attriRefe.FindPropertyRelative("effectOnCaster");
                            SerializedProperty effectOnTarget = attriRefe.FindPropertyRelative("effectOnTarget");
                            EditorGUILayout.PropertyField(effect);
                            EditorGUILayout.PropertyField(effectOnCaster);
                            EditorGUILayout.PropertyField(effectOnTarget);
                            break;

                        case SpellCreator.SpellAttribute.AttributeType.silence:
                            SerializedProperty silenceAmount = attriRefe.FindPropertyRelative("silenceAmount");
                            EditorGUILayout.PropertyField(silenceAmount);
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
            GetTarget.ApplyModifiedProperties();
        }
    }
}