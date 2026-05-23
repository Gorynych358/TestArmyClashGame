using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACT.Runtime.Infrastructure.EventBus
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new();
        private readonly object _lock = new object();

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            lock (_lock)
            {
                if (!_subscribers.TryGetValue(eventType, out var list))
                {
                    list = new List<object>();
                    _subscribers[eventType] = list;
                }

                // Проверка на дублирование подписок
                if (!list.Contains(handler))
                {
                    list.Add(handler);
                }
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            lock (_lock)
            {
                if (_subscribers.TryGetValue(eventType, out var list))
                {
                    list.Remove(handler);

                    if (list.Count == 0)
                        _subscribers.Remove(eventType);
                }
            }
        }

        public void Publish<T>(T eventData) where T : IEvent
        {
            var eventType = typeof(T);

            List<object> listCopy;
            lock (_lock)
            {
                if (!_subscribers.TryGetValue(eventType, out var list))
                    return;

                listCopy = new List<object>(list);
            }

            foreach (var obj in listCopy)
            {
                try
                {
                    var handler = (Action<T>)obj;
                    handler(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception in event handler for {typeof(T)}: {ex.Message}");
                }
            }
        }
    }
}