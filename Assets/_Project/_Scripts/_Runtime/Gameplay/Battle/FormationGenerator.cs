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

        public List<Unit> CreateArmy(
            ArmyTypes armyType, 
            FormationDataSO formationConfig, 
            Color armyColor,
            Transform parent = null, 
            Vector3 origin = default)
        {
            if (formationConfig == null)
                throw new ArgumentNullException(nameof(formationConfig));

            var units = new List<Unit>();
            Vector3 facingDirection = armyType == ArmyTypes.Invaders ? Vector3.left : Vector3.right;
            float sideSign = armyType == ArmyTypes.Invaders ? 1f : -1f;

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
                    unit.ArmyType = armyType;
                    ApplyColor(unit.transform, armyColor);
                    unit.transform.position = worldPosition;
                    unit.transform.rotation = Quaternion.LookRotation(facingDirection, Vector3.up);
                    unit.name = $"{armyType}_{row}_{column}_{cell.UnitType}";
                    unit.Initialize();
                    units.Add(unit);
                }
            }

            return units;
        }

        public List<Unit> CreateRandomArmy(
            ArmyTypes armyType, 
            int count, 
            Color armyColor, 
            Transform parent = null, 
            Vector3 origin = default)
        {
            var units = new List<Unit>();
            Vector3 facingDirection = armyType == ArmyTypes.Invaders ? Vector3.left : Vector3.right;

            for (int i = 0; i < count; i++)
            {
                var unit = _unitPool.Get((UnitTypes)UnityEngine.Random.Range(0, 11), parent);
                if (unit == null)
                    continue;
                unit.ArmyType = armyType;
                ApplyColor(unit.transform, armyColor);
                unit.transform.position = origin + new Vector3(0, 0, i * 2.5f);
                unit.transform.rotation = Quaternion.LookRotation(facingDirection, Vector3.up);
                unit.name = $"{armyType}_{i}_{unit.UnitType}";
                unit.Initialize();
                units.Add(unit);
            }

            return units;
        }
        
        //Применяем командный цвет к юнитам:
        private void ApplyColor(Transform unitTransform, Color color)
        {
            Transform modelTransform = unitTransform.GetChild(0).GetChild(0);
            if(modelTransform != null)
            {
                // Меняем цвет модели в цвет армии:
                if(modelTransform.childCount > 1 && 
                    modelTransform.GetChild(1).name == "Model")//Проверяем есть ли модель в контейнере юнита.
                {
                    var renderer = modelTransform.transform.GetChild(1).gameObject.GetComponentInChildren<Renderer>();
                    renderer.material.color = color;
                }
            }
        }
    }
}
