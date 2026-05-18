using UnityEngine;
using System.Collections.Generic;
using System;

namespace ACT.Runtime.Gameplay.Units
{
    public class UnitObjectPool : IDisposable
    {
        private Transform _poolStorage;
        private readonly IUnitFactory _factory;
        private readonly Dictionary<UnitTypes, Queue<Unit>> _pool = new();
        private bool _initialized;

        public UnitObjectPool(IUnitFactory factory)
        {
            _factory = factory;

            // Создаём очереди для всех типов заранее
            foreach (UnitTypes type in Enum.GetValues(typeof(UnitTypes)))
                _pool[type] = new Queue<Unit>();
        }

        public void InitializePool(int prewarmCount, Transform poolStorage)
        {
            _poolStorage = poolStorage;
            Prewarm(prewarmCount, _poolStorage);
            _initialized = true;
        }

        public Unit Get(UnitTypes type, Transform parent)
        {
            // Если пул не инициализирован — создаём напрямую
            if (!_initialized)
                return _factory.Create(type, parent);

            if (!_pool.TryGetValue(type, out var queue))
                return _factory.Create(type, parent);

            Unit unit;
            if (queue.Count > 0)
                unit = queue.Dequeue();
            else
                unit = _factory.Create(type, parent);

            unit.transform.parent = parent;
            unit.gameObject.SetActive(true);
            return unit;
        }

        public void Return(Unit unit)
        {
            if (unit == null || unit.gameObject == null)
                return;

            // Если пул не инициализирован — тихо уничтожаем объект
            if (!_initialized || _poolStorage == null)
            {
                UnityEngine.Object.Destroy(unit.gameObject);
                return;
            }

            var type = unit.UnitType;

            // Если тип отсутствует — уничтожаем объект
            if (!_pool.TryGetValue(type, out var queue))
            {
                UnityEngine.Object.Destroy(unit.gameObject);
                return;
            }
            
            unit.ResetUnit(); // Сброс состояния юнита перед возвратом в пул
            unit.gameObject.SetActive(false);
            unit.transform.parent = _poolStorage;
            queue.Enqueue(unit);
        }

        private void Prewarm(int prewarmCount, Transform unitParent)
        {
            foreach (UnitTypes type in Enum.GetValues(typeof(UnitTypes)))
            {
                for (int i = 0; i < prewarmCount; i++)
                {
                    var unit = _factory.Create(type, unitParent);
                    unit.gameObject.SetActive(false);
                    _pool[type].Enqueue(unit);
                }
            }
        }

        public void Dispose()
        {
            // Если пул не инициализирован — ничего не делаем
            if (!_initialized)
                return;

            foreach (var kvp in _pool)
            {
                var queue = kvp.Value;

                while (queue.Count > 0)
                {
                    var unit = queue.Dequeue();

                    if (unit != null && unit.gameObject != null)
                        UnityEngine.Object.Destroy(unit.gameObject);
                }
            }

            _pool.Clear();
            _initialized = false;
        }
    }
}