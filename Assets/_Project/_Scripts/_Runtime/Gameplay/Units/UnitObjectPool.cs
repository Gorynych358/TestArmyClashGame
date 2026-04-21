using UnityEngine;
using System.Collections.Generic;
using System;

namespace ACT.Scripts
{
    public class UnitObjectPool
    {
        private Transform _poolStorage;
        private readonly IUnitFactory _factory;
        private readonly Dictionary<UnitTypes, Queue<Unit>> _pool = new();

        public UnitObjectPool(IUnitFactory factory)
        {
            _factory = factory;

            foreach (UnitTypes type in Enum.GetValues(typeof(UnitTypes)))
                _pool[type] = new Queue<Unit>();
        }

        public void InitializePool(int prewarmCount, Transform poolStorage)
        {
            _poolStorage = poolStorage;
            Prewarm(prewarmCount, _poolStorage);
        }

        public Unit Get(UnitTypes type, Transform parent)
        {
            Unit unit;
            if (_pool[type].Count > 0)
                unit = _pool[type].Dequeue();
            else
                unit = _factory.Create(type, parent); 
            
            unit.transform.parent = parent;
            unit.gameObject.SetActive(true);
            return unit;
        }

        public void Return(Unit unit)
        {
            unit.gameObject.SetActive(false);
            unit.transform.parent = _poolStorage;
            _pool[unit.UnitType].Enqueue(unit);
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
    }
}
