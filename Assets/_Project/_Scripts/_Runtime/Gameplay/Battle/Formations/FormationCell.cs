using System;

namespace ACT.Scripts
{
    // ============================
    //  Описание ячейки сетки формации
    // ============================
    [Serializable]
    public struct FormationCell
    {
        public bool HasUnit;
        public UnitTypes UnitType;

        public static FormationCell Empty =>
            new FormationCell { HasUnit = false, UnitType = 0};
    }
}