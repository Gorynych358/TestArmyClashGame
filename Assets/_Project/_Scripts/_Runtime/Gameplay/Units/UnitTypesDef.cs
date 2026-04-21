using UnityEngine;

namespace ACT.Scripts
{
    //Перечисления типов модификаторов.
    //Названия типов просто для удобства, не влияют на логику.
    //Новые типы добавляем тут:
    public enum UnitTypes
    {
        //Элита:
        Warlord, 
        KingsGuard,
        Champion,
        RoyalPaladin,
        //Гвардия:
        Paladin,
        Knight,
        Guardian,
        Raider,
        //Солдаты:
        Warrior,
        Soldier,
        Spearman,
        Recruit
    }
}
