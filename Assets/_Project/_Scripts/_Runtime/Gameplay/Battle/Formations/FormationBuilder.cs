using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ACT.Runtime.Gameplay.Units;

namespace ACT.Runtime.Gameplay.Battle.Formations
{
    // ============================
    //  FORMATION BUILDER
    // ============================
    public class FormationBuilder
    {
        private readonly Dictionary<UnitTypes, UnitConfigSO> _configs;

        public FormationBuilder(Dictionary<UnitTypes, UnitConfigSO> configs)
        {
            _configs = configs;
        }

        public FormationRuntimeData BuildRandom(
            List<UnitTypes> units,
            int? maxColumns = null,
            int? maxRows = null)
        {
            units = units
                .OrderByDescending(t => _configs[t].PowerScore)
                .ToList();

            int count = units.Count;

            // Нет ограничений почти прямоугольная формация.
            // Будут вставлены все юниты что посчитанны для заданной мощи:
            if (maxColumns == null && maxRows == null)
            {
                int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
                int rows = Mathf.CeilToInt(count / (float)columns);
                return BuildTight(units, columns, rows);
            }

            // Ограничиваем количество колонн, чтобы влезало по ширине:
            if (maxColumns != null && maxRows == null)
            {
                int columns = Mathf.Clamp(count, 1, maxColumns.Value);
                int rows = Mathf.CeilToInt(count / (float)columns);
                return BuildTight(units, columns, rows);
            }

            // Ограничиваем количество рядов, чтобы влезало по длине:
            if (maxColumns == null && maxRows != null)
            {
                int rows = Mathf.Clamp(count, 1, maxRows.Value);
                int columns = Mathf.CeilToInt(count / (float)rows);
                return BuildTight(units, columns, rows);
            }

            // Ограничение и по рядам, и по колоннам. В этом случае, если юнитов будет больше,
            //чем вместимость сетки, будет выданно предупреждение:
            return BuildCenteredWithinBounds(units, maxColumns.Value, maxRows.Value);
        }

        private FormationRuntimeData BuildTight(List<UnitTypes> units, int columns, int rows)
        {
            var data = new FormationRuntimeData(rows, columns);

            int index = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (index >= units.Count)
                        return data;

                    data.Set(c, r, units[index]);
                    index++;
                }
            }

            return data;
        }

        private FormationRuntimeData BuildCenteredWithinBounds(
            List<UnitTypes> units,
            int maxColumns,
            int maxRows)
        {
            int capacity = maxColumns * maxRows;
            int countToPlace = units.Count;

            if (countToPlace > capacity)
            {
                Debug.LogWarning(
                    $"FormationBuilder: юнитов ({countToPlace}) больше, чем вместимость " +
                    $"формации ({capacity}). Лишние юниты будут отброшены.");
                countToPlace = capacity;
            }

            // Считаем "плотный" прямоугольник, который влезет в эти границы
            int tightColumns = Mathf.Min(maxColumns, countToPlace);
            int tightRows = Mathf.CeilToInt(countToPlace / (float)tightColumns);

            // Центрируем внутри maxColumns × maxRows
            int colOffset = (maxColumns - tightColumns) / 2;
            int rowOffset = (maxRows - tightRows) / 2;

            var data = new FormationRuntimeData(maxRows, maxColumns);

            int index = 0;
            for (int r = 0; r < tightRows; r++)
            {
                for (int c = 0; c < tightColumns; c++)
                {
                    if (index >= countToPlace)
                        return data;

                    int col = c + colOffset;
                    int row = r + rowOffset;

                    data.Set(col, row, units[index]);
                    index++;
                }
            }

            return data;
        }
    }
}