using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACT.Scripts
{
    [Serializable]
    public struct FormationCell
    {
        public bool HasUnit;
        public UnitTypes UnitType;

        public static FormationCell Empty => new FormationCell { HasUnit = false, UnitType = UnitTypes.Warlord };
    }

    [CreateAssetMenu(fileName = "FormationDataSO", menuName = "Configs/Battle/FormationDataSO")]
    public class FormationDataSO : ScriptableObject
    {
        [SerializeField] private string _formationName = "NewFormation";
        [SerializeField] private int _columns = 6;
        [SerializeField] private int _rows = 6;
        [SerializeField] private FormationCell[] _cells = Array.Empty<FormationCell>();
        [SerializeField] private Vector2 _cellSpacing = new Vector2(2f, 2f);

        public string FormationName => _formationName;
        public int Columns => _columns;
        public int Rows => _rows;
        public Vector2 CellSpacing => _cellSpacing;

        public IReadOnlyList<FormationCell> Cells => _cells;

        public FormationCell GetCell(int column, int row)
        {
            if (!IsValidIndex(column, row))
                return FormationCell.Empty;

            return _cells[row * _columns + column];
        }

        public void SetCell(int column, int row, FormationCell cell)
        {
            if (!IsValidIndex(column, row))
                return;

            _cells[row * _columns + column] = cell;
        }

        public void SetName(string formationName)
        {
            _formationName = formationName.Trim();
        }

        public void Resize(int columns, int rows)
        {
            columns = Mathf.Max(1, columns);
            rows = Mathf.Max(1, rows);

            if (columns == _columns && rows == _rows && _cells.Length == columns * rows)
                return;

            var newCells = new FormationCell[columns * rows];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (x < _columns && y < _rows && _cells != null && _cells.Length == _columns * _rows)
                    {
                        newCells[y * columns + x] = _cells[y * _columns + x];
                    }
                    else
                    {
                        newCells[y * columns + x] = FormationCell.Empty;
                    }
                }
            }

            _columns = columns;
            _rows = rows;
            _cells = newCells;
        }

        public Vector2 GetCellLocalPosition(int column, int row)
        {
            float width = (_columns - 1) * _cellSpacing.x;
            float height = (_rows - 1) * _cellSpacing.y;

            float x = column * _cellSpacing.x - width * 0.5f;
            float y = row * _cellSpacing.y - height * 0.5f;
            return new Vector2(x, y);
        }

        public bool IsValidIndex(int column, int row)
        {
            return column >= 0 && row >= 0 && column < _columns && row < _rows;
        }
    }
}
