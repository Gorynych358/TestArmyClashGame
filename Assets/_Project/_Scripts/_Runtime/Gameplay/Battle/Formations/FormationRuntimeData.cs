using ACT.Runtime.Gameplay.Units;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle.Formations
{
    // ============================
    //  Формация которая создаётся в рантайме
    // ============================
    public class FormationRuntimeData : IFormationData
    {
        public int Rows { get; }
        public int Columns { get; }
        public Vector2 CellSpacing { get; set; } = new Vector2(2f, 2f);

        public FormationCell[] Cells;

        public FormationRuntimeData(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Cells = new FormationCell[rows * columns];
        }

        public FormationCell GetCell(int column, int row)
        {
            return Cells[row * Columns + column];
        }

        public void Set(int column, int row, UnitTypes type)
        {
            Cells[row * Columns + column] = new FormationCell
            {
                HasUnit = true,
                UnitType = type
            };
        }
    }
}