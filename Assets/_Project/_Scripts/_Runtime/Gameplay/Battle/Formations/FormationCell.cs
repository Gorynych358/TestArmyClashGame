using System;
using ACT.Runtime.Gameplay.Units;

namespace ACT.Runtime.Gameplay.Battle.Formations
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