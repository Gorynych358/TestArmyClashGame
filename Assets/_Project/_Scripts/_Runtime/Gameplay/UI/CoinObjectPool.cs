using System.Collections.Generic;
using UnityEngine;

namespace ACT.Scripts
{
    public class CoinObjectPool
    {
        private readonly GameObject _prefab;
        private readonly RectTransform _poolStorage;
        private readonly Stack<RectTransform> _pool = new();

        public CoinObjectPool(GameObject prefab, RectTransform poolStorage, int prewarmCount)
        {
            _prefab = prefab;
            _poolStorage = poolStorage;

            for (int i = 0; i < prewarmCount; i++)
                _pool.Push(Create());
        }

        private RectTransform Create()
        {
            var inst = Object.Instantiate(_prefab, _poolStorage);
            inst.gameObject.SetActive(false);
            return inst.GetComponent<RectTransform>();
        }

        public RectTransform Get()
        {
            var coin = _pool.Count > 0 ? _pool.Pop() : Create();
            coin.gameObject.SetActive(true);
            return coin;
        }

        public void Return(RectTransform coin)
        {
            coin.gameObject.SetActive(false);
            coin.SetParent(_poolStorage, false);
            _pool.Push(coin);
        }
    }
}
