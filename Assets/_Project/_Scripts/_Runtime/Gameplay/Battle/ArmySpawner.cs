using System.Collections.Generic;
using ACT.Runtime.Gameplay.Battle.Formations;
using ACT.Runtime.Gameplay.Units;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle
{
    // ============================
    //  ARMY SPAWNER
    // ============================
    public class ArmySpawner
    {
        private readonly UnitObjectPool _pool;

        public ArmySpawner(UnitObjectPool pool)
        {
            _pool = pool;
        }

        public List<Unit> SpawnArmy(
            ArmyTypes armyType,
            IFormationData formation,
            Color color,
            Transform parent,
            Vector3 origin, 
            out float totalPower)
        {
            List<Unit> units = new();

            Vector3 facing = armyType == ArmyTypes.Invaders ? Vector3.left : Vector3.right;
            float sideSign = armyType == ArmyTypes.Invaders ? 1f : -1f;

            float halfHeight = (formation.Rows - 1) * formation.CellSpacing.y * 0.5f;
            Vector3 armyOrigin = origin + Vector3.right * sideSign * (halfHeight + 4f);

            for (int row = 0; row < formation.Rows; row++)
            {
                for (int col = 0; col < formation.Columns; col++)
                {
                    var cell = formation.GetCell(col, row);
                    if (!cell.HasUnit)
                        continue;

                    float width = (formation.Columns - 1) * formation.CellSpacing.x;
                    float height = (formation.Rows - 1) * formation.CellSpacing.y;

                    Vector2 local = new Vector2(
                        col * formation.CellSpacing.x - width * 0.5f,
                        row * formation.CellSpacing.y - height * 0.5f
                    );

                    Vector3 worldPos = armyOrigin + new Vector3(local.y * sideSign, 0f, local.x);

                    Unit unit = _pool.Get(cell.UnitType, parent);
                    if (unit == null)
                        continue;

                    unit.ArmyType = armyType;
                    ApplyColor(unit.transform, color);
                    unit.transform.SetPositionAndRotation(worldPos, Quaternion.LookRotation(facing, Vector3.up));
                    unit.Initialize();
                    unit.name = $"{armyType}_{row}_{col}_{cell.UnitType}";
                    units.Add(unit);
                }
            }

            totalPower = 0f;
            foreach (var unit in units)
            {
                if (unit != null)
                    totalPower += unit.PowerScore;
            }

            return units;
        }

        private void ApplyColor(Transform unitTransform, Color color)
        {
            Transform modelTransform = unitTransform.GetChild(0).GetChild(0);
            if (modelTransform != null)
            {
                if (modelTransform.childCount > 1 &&
                    modelTransform.GetChild(1).name == "Model")
                {
                    var renderer = modelTransform.GetChild(1).GetComponentInChildren<Renderer>();
                    if (renderer != null)
                        renderer.material.color = color;
                }
            }
        }
    }
}