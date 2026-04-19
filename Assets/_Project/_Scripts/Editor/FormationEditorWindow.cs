using System;
using UnityEditor;
using UnityEngine;

namespace ACT.Scripts.Editor
{
    public class FormationEditorWindow : EditorWindow
    {
        //Тут храним ScriptableObject-ы формаций:
        private const string DefaultFolder = "Assets/_Project/Configurations/Formations";
        private const int MaxGridSize = 16;
        private const int MinGridSize = 1;
        private const float GridLabelWidth = 50f;
        private const float CellWidth = 100f;
        private const float CellHeight = 60f;
        private const float CellSpacing = 8f;
        private const float PaletteWidth = 200f;

        private FormationDataSO _formationData;
        private Vector2 _scrollPosition;

        [MenuItem("Window/Game/Formation Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<FormationEditorWindow>("Formation Editor");
            window.minSize = new Vector2(640, 520);
            window.Initialize();
        }

        private void Initialize()
        {
            if (_formationData == null)
            {
                _formationData = CreateInstance<FormationDataSO>();
                _formationData.Resize(6, 6);
                _formationData.SetName(""); //Имя по умолчанию пустое, должно быть уникальным
            }
        }

        private void OnEnable() => Initialize();

        private void OnGUI()
        {
            Initialize();

            EditorGUILayout.LabelField("Formation Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawFormationSettings();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            DrawGridEditor();
            DrawUnitPalette();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            DrawCreateButton();
        }

        // -----------------------------
        // SETTINGS
        // -----------------------------
        private void DrawFormationSettings()
        {
            EditorGUILayout.BeginVertical("box");

            string newName = EditorGUILayout.TextField("Formation Name", _formationData.FormationName);
            if (newName != _formationData.FormationName)
                _formationData.SetName(newName);

            EditorGUILayout.BeginHorizontal();
            int newColumns = EditorGUILayout.IntField("Columns", _formationData.Columns);
            int newRows = EditorGUILayout.IntField("Rows", _formationData.Rows);
            EditorGUILayout.EndHorizontal();

            if (newColumns != _formationData.Columns || newRows != _formationData.Rows)
            {
                newColumns = Mathf.Clamp(newColumns, MinGridSize, MaxGridSize);
                newRows = Mathf.Clamp(newRows, MinGridSize, MaxGridSize);
                _formationData.Resize(newColumns, newRows);
            }

            EditorGUILayout.HelpBox("Перетащите юнита из палитры в ячейку сетки. Для удаления юнита нажать крестик.", MessageType.Info);

            EditorGUILayout.EndVertical();
        }

        // -----------------------------
        // GRID
        // -----------------------------
        private void DrawGridEditor()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("Formation Grid", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            float totalGridWidth = GridLabelWidth + _formationData.Columns * CellWidth + (_formationData.Columns - 1) * CellSpacing + 16f;
            float totalGridHeight = _formationData.Rows * CellHeight + (_formationData.Rows - 1) * CellSpacing + 20f;

            Rect gridRect = GUILayoutUtility.GetRect(totalGridWidth, totalGridHeight, GUILayout.ExpandWidth(true));
            DrawGrid(gridRect);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawGrid(Rect gridRect)
        {
            var evt = Event.current;

            Handles.color = Color.grey;
            EditorGUI.DrawRect(gridRect, new Color(0.12f, 0.12f, 0.12f, 0.95f));
            Handles.DrawSolidRectangleWithOutline(gridRect, Color.clear, Color.grey);

            var labelStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };

            for (int row = 0; row < _formationData.Rows; row++)
            {
                float y = gridRect.y + row * (CellHeight + CellSpacing);

                var rowLabelRect = new Rect(gridRect.x + 4f, y, GridLabelWidth - 8f, CellHeight);
                EditorGUI.LabelField(rowLabelRect, $"Row {row}", labelStyle);

                for (int column = 0; column < _formationData.Columns; column++)
                {
                    float x = gridRect.x + GridLabelWidth + column * (CellWidth + CellSpacing);
                    var cellRect = new Rect(x, y, CellWidth, CellHeight);
                    DrawCell(cellRect, _formationData.GetCell(column, row), column, row, evt, labelStyle);
                }
            }
        }

        private void DrawCell(Rect cellRect, FormationCell cell, int column, int row, Event evt, GUIStyle labelStyle)
        {
            GUI.Box(cellRect, GUIContent.none, EditorStyles.helpBox);

            if (cell.HasUnit)
            {
                Color unitColor = GetUnitColor(cell.UnitType);
                EditorGUI.DrawRect(new Rect(cellRect.x + 2f, cellRect.y + 2f, cellRect.width - 4f, cellRect.height - 4f), unitColor);

                EditorGUI.LabelField(new Rect(cellRect.x + 8f, cellRect.y + 8f, cellRect.width - 40f, 20f), cell.UnitType.ToString(), labelStyle);

                if (GUI.Button(new Rect(cellRect.xMax - 24f, cellRect.y + 4f, 20f, 20f), "×", EditorStyles.label))
                    _formationData.SetCell(column, row, FormationCell.Empty);
            }
            else
            {
                EditorGUI.LabelField(cellRect, "Empty", EditorStyles.centeredGreyMiniLabel);
            }

            // Context menu
            if (evt.type == EventType.ContextClick && cellRect.Contains(evt.mousePosition))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Clear"), false, () => _formationData.SetCell(column, row, FormationCell.Empty));
                menu.ShowAsContext();
                evt.Use();
            }

            // Таскаем юнитов:
            if (evt.type == EventType.DragUpdated && cellRect.Contains(evt.mousePosition))
            {
                if (DragAndDrop.GetGenericData("FormationUnitType") is UnitTypes)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.Use();
                }
            }

            if (evt.type == EventType.DragPerform && cellRect.Contains(evt.mousePosition))
            {
                if (DragAndDrop.GetGenericData("FormationUnitType") is UnitTypes dropType)
                {
                    _formationData.SetCell(column, row, new FormationCell { HasUnit = true, UnitType = dropType });
                    DragAndDrop.AcceptDrag();
                    evt.Use();
                }
            }
        }

        // -----------------------------
        // PALETTE
        // -----------------------------
        private void DrawUnitPalette()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(PaletteWidth), GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("Unit Palette", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            var evt = Event.current;

            foreach (UnitTypes unitType in Enum.GetValues(typeof(UnitTypes)))
            {
                Color unitColor = GetUnitColor(unitType);
                var paletteRect = GUILayoutUtility.GetRect(PaletteWidth - 12f, 32f);

                EditorGUI.DrawRect(paletteRect, unitColor);
                EditorGUI.LabelField(new Rect(paletteRect.x + 8f, paletteRect.y, paletteRect.width - 16f, paletteRect.height),
                    unitType.ToString(), new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.white } });

                if (evt.type == EventType.MouseDown && paletteRect.Contains(evt.mousePosition))
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.SetGenericData("FormationUnitType", unitType);
                    DragAndDrop.StartDrag(unitType.ToString());
                    evt.Use();
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        // -----------------------------
        // CREATE BUTTON
        // -----------------------------
        private void DrawCreateButton()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create Formation", GUILayout.Height(32), GUILayout.Width(180)))
                {
                    TryCreateFormation();
                }
            }
        }

        private void TryCreateFormation()
        {
            // 1) Проверка имени:
            if (string.IsNullOrWhiteSpace(_formationData.FormationName))
            {
                EditorUtility.DisplayDialog("Ошибка", "Введите название формации.", "OK");
                return;
            }

            // 2) Проверка, что формация не пустая:
            if (!HasAnyUnitsPlaced())
            {
                EditorUtility.DisplayDialog("Ошибка", "Формация не содержит ни одного юнита.", "OK");
                return;
            }

            // 3) Проверка существования ассета
            if (!AssetDatabase.IsValidFolder(DefaultFolder))
                AssetDatabase.CreateFolder("Assets/_Project/Configurations", "Formations");

            string basePath = $"{DefaultFolder}/{_formationData.FormationName}.asset";

            if (AssetDatabase.LoadAssetAtPath<FormationDataSO>(basePath) != null)
            {
                EditorUtility.DisplayDialog("Ошибка", $"Формация '{_formationData.FormationName}' уже существует.", "OK");
                return;
            }

            // 4) Создание ассета
            string finalPath = AssetDatabase.GenerateUniqueAssetPath(basePath);

            var asset = CreateInstance<FormationDataSO>();
            asset.SetName(_formationData.FormationName);
            asset.Resize(_formationData.Columns, _formationData.Rows);

            for (int row = 0; row < _formationData.Rows; row++)
                for (int col = 0; col < _formationData.Columns; col++)
                    asset.SetCell(col, row, _formationData.GetCell(col, row));

            AssetDatabase.CreateAsset(asset, finalPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = asset;

            Debug.Log($"Formation created: {finalPath}");

            // 5) Закрыть окно
            Close();
        }

        private bool HasAnyUnitsPlaced()
        {
            for (int row = 0; row < _formationData.Rows; row++)
                for (int col = 0; col < _formationData.Columns; col++)
                    if (_formationData.GetCell(col, row).HasUnit)
                        return true;

            return false;
        }

        // -----------------------------
        // COLORS
        // Тут цвета фиксированные для интуитивности,
        // TODO: при добавлении новых видов юнитов новые юниты будут серыми. Надо подумать...
        // -----------------------------
        private static Color GetUnitColor(UnitTypes type)
        {
            return type switch
            {
                UnitTypes.Warlord => new Color(0.76f, 0.22f, 0.20f),
                UnitTypes.KingsGuard => new Color(0.82f, 0.40f, 0.16f),
                UnitTypes.Champion => new Color(0.96f, 0.68f, 0.20f),
                UnitTypes.RoyalPaladin => new Color(0.40f, 0.72f, 0.26f),
                UnitTypes.Paladin => new Color(0.20f, 0.64f, 0.74f),
                UnitTypes.Knight => new Color(0.28f, 0.38f, 0.92f),
                UnitTypes.Guardian => new Color(0.48f, 0.24f, 0.70f),
                UnitTypes.Raider => new Color(0.82f, 0.28f, 0.70f),
                UnitTypes.Warrior => new Color(0.40f, 0.72f, 0.40f),
                UnitTypes.Soldier => new Color(0.24f, 0.55f, 0.30f),
                UnitTypes.Spearman => new Color(0.26f, 0.54f, 0.74f),
                UnitTypes.Recruit => new Color(0.56f, 0.56f, 0.56f),
                _ => Color.gray,
            };
        }
    }
}