using UnityEngine;
using UnityEditor;
using ACT.Runtime.Gameplay.Battle.Formations;
using ACT.Runtime.Gameplay.Units;

namespace ACT.Editor
{
    [CustomEditor(typeof(FormationDataSO))]
    public class FormationDataSOInspector : UnityEditor.Editor
    {
        //Цвет пустой ячейки:
        private static readonly Color EmptyColor = new Color(0.25f, 0.25f, 0.25f);

        // Минимальный и максимальный размер ячейки
        private const int MinCellSize = 28;
        private const int MaxCellSize = 120;

        public override void OnInspectorGUI()
        {
            FormationDataSO formation = (FormationDataSO)target;

            DrawHeader(formation);
            DrawGrid(formation);

            if (GUI.changed)
                EditorUtility.SetDirty(formation);
        }

        private void DrawHeader(FormationDataSO formation)
        {
            EditorGUILayout.LabelField("Описание формации:", EditorStyles.boldLabel);

            string newName = EditorGUILayout.TextField("Имя формации:", formation.name);
            if (newName != formation.name)
            {
                formation.SetName(newName);
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(formation), newName);
            }

            int newCols = EditorGUILayout.IntField("Колонны:", formation.Columns);
            int newRows = EditorGUILayout.IntField("Ряды:", formation.Rows);

            if (newCols != formation.Columns || newRows != formation.Rows)
            {
                Undo.RecordObject(formation, "Resize Formation");
                formation.Resize(newCols, newRows);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("ФОРМАЦИЯ:", EditorStyles.boldLabel);
        }

        private void DrawGrid(FormationDataSO formation)
        {
            // Меняем размер ячейки в зависимости от размера окна в инспекторе:
            float inspectorWidth = EditorGUIUtility.currentViewWidth - 40f;
            float cellSize = Mathf.Clamp(inspectorWidth / formation.Columns, MinCellSize, MaxCellSize);

            for (int row = 0; row < formation.Rows; row++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int col = 0; col < formation.Columns; col++)
                {
                    DrawCell(formation, col, row, cellSize);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawCell(FormationDataSO formation, int col, int row, float cellSize)
        {
            FormationCell cell = formation.GetCell(col, row);

            Color cellColor = cell.HasUnit
                ? GetColorForUnit(cell.UnitType)
                : EmptyColor;

            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = Color.white },
                fontSize = Mathf.Clamp((int)(cellSize / 4f), 8, 18),
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = cellColor;

            string label = cell.HasUnit ? cell.UnitType.ToString() : "";

            if (GUILayout.Button(label, style, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
            {
                ShowCellMenu(formation, col, row, cell);
            }

            GUI.backgroundColor = prev;
        }

        private void ShowCellMenu(FormationDataSO formation, int col, int row, FormationCell cell)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Empty"), !cell.HasUnit, () =>
            {
                Undo.RecordObject(formation, "Clear Cell");
                formation.SetCell(col, row, FormationCell.Empty);
                EditorUtility.SetDirty(formation);
            });

            menu.AddSeparator("");

            foreach (UnitTypes type in System.Enum.GetValues(typeof(UnitTypes)))
            {
                menu.AddItem(new GUIContent(type.ToString()), cell.HasUnit && cell.UnitType == type, () =>
                {
                    Undo.RecordObject(formation, "Set Unit Type");
                    formation.SetCell(col, row, new FormationCell
                    {
                        HasUnit = true,
                        UnitType = type
                    });
                    EditorUtility.SetDirty(formation);
                });
            }

            menu.ShowAsContext();
        }

        //Генерим случайные цвета:
        private Color GetColorForUnit(UnitTypes type)
        {
            int hash = type.ToString().GetHashCode();
            Random.InitState(hash);

            return new Color(
                Random.Range(0.35f, 1f),
                Random.Range(0.35f, 1f),
                Random.Range(0.35f, 1f)
            );
        }
    }
}