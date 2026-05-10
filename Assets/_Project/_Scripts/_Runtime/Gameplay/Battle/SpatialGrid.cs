using System.Collections.Generic;
using UnityEngine;

namespace ACT.Scripts
{
    public class SpatialGrid
    {
        private readonly float _cellSize;
        private readonly Dictionary<Vector2Int, List<Unit>> _cells = new();

        public SpatialGrid(float cellSize)
        {
            _cellSize = cellSize;
        }

        public void Clear() => _cells.Clear();

        public void Build(List<Unit> defenders, List<Unit> enemies)
        {
            AddArmy(defenders);
            AddArmy(enemies);
        }

        private void AddArmy(List<Unit> units)
        {
            foreach (var unit in units)
            {
                if (unit == null || !unit.gameObject.activeSelf)
                    continue;

                Vector3 pos = unit.Transform.position;
                Vector2Int cell = GetCell(pos);

                if (!_cells.TryGetValue(cell, out var list))
                {
                    list = new List<Unit>(8);
                    _cells[cell] = list;
                }

                list.Add(unit);
            }
        }

        private Vector2Int GetCell(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / _cellSize),
                Mathf.FloorToInt(pos.z / _cellSize)
            );
        }

        public void FillNeighbors(IUnitContext requester, List<Unit> result)
        {
            result.Clear();

            Vector3 position = requester.Transform.position;
            Vector2Int cell = GetCell(position);

            for (int x = -1; x <= 1; x++)
            for (int z = -1; z <= 1; z++)
            {
                Vector2Int c = new Vector2Int(cell.x + x, cell.y + z);

                if (_cells.TryGetValue(c, out var list))
                {
                    for (int i = 0; i < list.Count; i++)
                        result.Add(list[i]);
                }
            }
        }
    }
}