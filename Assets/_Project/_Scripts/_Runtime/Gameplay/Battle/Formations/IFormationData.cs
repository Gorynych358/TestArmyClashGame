using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle.Formations
{
    public interface IFormationData
    {
        int Rows { get; }
        int Columns { get; }
        Vector2 CellSpacing { get; }
        FormationCell GetCell(int column, int row);
    }
}
