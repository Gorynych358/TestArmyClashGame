using System.Threading;
using Cysharp.Threading.Tasks;

namespace ACT.Scripts
{
    //Расширение-адаптер для EventBus для использования с UniTask.
    public static class EventBusExtensions
    {
        public static UniTask<T> WaitForEvent<T>(this IEventBus eventBus, CancellationToken ct)
        where T : struct, IEvent
        {
            var tcs = new UniTaskCompletionSource<T>();

            eventBus.Subscribe<T>(EventHandler);

            void EventHandler(T evt)
            {
                eventBus.Unsubscribe<T>(EventHandler);
                tcs.TrySetResult(evt);
            }

            ct.Register(() =>
            {
                eventBus.Unsubscribe<T>(EventHandler);
                tcs.TrySetCanceled(ct);
            });

            return tcs.Task;
        }
    }
}