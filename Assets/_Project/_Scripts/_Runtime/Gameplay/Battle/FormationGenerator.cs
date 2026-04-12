using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACT.Scripts
{
    public class FormationGenerator
    {
        private readonly UnitObjectPool _unitPool;
        private readonly float _formationGap = 4f;

        public FormationGenerator(UnitObjectPool unitPool)
        {
            _unitPool = unitPool ?? throw new ArgumentNullException(nameof(unitPool));
        }

        public List<Unit> CreateArmy(ArmyTypes armyType, FormationDataSO formationConfig, Transform parent = null, Vector3 origin = default)
        {
            if (formationConfig == null)
                throw new ArgumentNullException(nameof(formationConfig));

            var units = new List<Unit>();
            Vector3 facingDirection = armyType == ArmyTypes.Inviders ? Vector3.left : Vector3.right;
            float sideSign = armyType == ArmyTypes.Inviders ? 1f : -1f;

            float formationHalfWidth = (formationConfig.Rows - 1) * formationConfig.CellSpacing.y * 0.5f;
            Vector3 armyOrigin = origin + Vector3.right * sideSign * (formationHalfWidth + _formationGap);

            for (int row = 0; row < formationConfig.Rows; row++)
            {
                for (int column = 0; column < formationConfig.Columns; column++)
                {
                    var cell = formationConfig.GetCell(column, row);
                    if (!cell.HasUnit)
                        continue;

                    Vector2 localCell = formationConfig.GetCellLocalPosition(column, row);
                    Vector2 rotatedCell = new Vector2(localCell.y, localCell.x);
                    Vector3 worldPosition = armyOrigin + new Vector3(rotatedCell.x * sideSign, 0f, rotatedCell.y);

                    Unit unit = _unitPool.Get(cell.UnitType, parent);
                    if (unit == null)
                        continue;

                    unit.transform.position = worldPosition;
                    unit.transform.rotation = Quaternion.LookRotation(facingDirection, Vector3.up);
                    unit.name = $"{armyType}_{row}_{column}_{cell.UnitType}";
                    units.Add(unit);
                }
            }

            return units;
        }

    }
}
