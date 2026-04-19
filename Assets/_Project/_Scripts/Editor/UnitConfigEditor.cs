using UnityEngine;
    using UnityEditor;

namespace ACT.Scripts
{

    [CustomEditor(typeof(UnitConfigSO))]
    public class UnitConfigSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = (UnitConfigSO)target;

            EditorGUILayout.LabelField("Unit Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawPropertiesExcluding(serializedObject, "m_Script");

            if (config.Provider == null)
            {
                EditorGUILayout.HelpBox("Assign ModifierProvider to calculate final stats", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var colorMod = config.ColorMod;
            var sizeMod = config.SizeMod;
            var shapeMod = config.ShapeMod;

            if (colorMod == null || sizeMod == null || shapeMod == null)
            {
                EditorGUILayout.HelpBox("ModifierProvider does not contain required modifiers", MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            DrawPreviewPanel(config, colorMod);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Power Score", EditorStyles.boldLabel);
            DrawPowerScore(config);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Final Stats", EditorStyles.boldLabel);
            DrawStatBars(config);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPreviewPanel(UnitConfigSO config, ColorModifierSO colorMod)
        {
            float panelHeight = 100f;
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, panelHeight);
            EditorGUI.DrawRect(rect, colorMod.ColorDef);

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            string text =
                $"TYPE: {config.UnitType.ToString().ToUpper()}\n" +
                $"SHAPE: {config.Shape.ToString().ToUpper()}\n" +
                $"SIZE: {config.Size.ToString().ToUpper()}\n" +
                $"COLOR: {config.Color.ToString().ToUpper()}";

            GUI.Label(rect, text, style);
        }

        private void DrawPowerScore(UnitConfigSO config)
        {
            GUIStyle psStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 22,
                normal = { textColor = Color.cyan },
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.LabelField(config.PowerScore.ToString("0.0"), psStyle);
        }

        private void DrawStatBars(UnitConfigSO config)
        {
            DrawBar("HP", config.FinalHP, 500);
            DrawBar("ATK", config.FinalATK, 100);
            DrawBar("SPEED", config.FinalSPEED, 10);
            DrawBar("ATKSPD", config.FinalATKSPD, 5);
        }

        private void DrawBar(string label, float value, float maxValue)
        {
            float height = 24f;
            float width = EditorGUIUtility.currentViewWidth - 40f;

            Rect rect = GUILayoutUtility.GetRect(width, height);

            float fillPercent = Mathf.Clamp01(value / maxValue);
            Color barColor = GetBarColor(fillPercent);

            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            Rect fillRect = new Rect(rect.x, rect.y, rect.width * fillPercent, rect.height);
            EditorGUI.DrawRect(fillRect, barColor);

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(rect, $"{label}: {value}", style);
        }

        private Color GetBarColor(float percent)
        {
            if (percent > 0.66f) return new Color(0.2f, 0.8f, 0.2f);
            if (percent > 0.33f) return new Color(0.9f, 0.7f, 0.1f);
            return new Color(0.9f, 0.2f, 0.2f);
        }
    }
}
