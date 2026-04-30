using UnityEngine;

namespace ACT.Scripts
{
    public interface IFormationData
    {
        int Rows { get; }
        int Columns { get; }
        Vector2 CellSpacing { get; }
        FormationCell GetCell(int column, int row);
    }
}
