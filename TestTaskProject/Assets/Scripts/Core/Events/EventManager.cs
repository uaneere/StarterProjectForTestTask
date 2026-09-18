using System;
using System.Collections.Generic;

public class EventManager
{
    private readonly Dictionary<Type, Delegate> _subscribers = new();

    public void Subscribe<T>(Action<T> action)
    {
        _subscribers.TryGetValue(typeof(T), out var subscribers);
        _subscribers[typeof(T)] = Delegate.Combine(subscribers, action);
    }

    public void Unsubscribe<T>(Action<T> action)
    {
        if (!_subscribers.TryGetValue(typeof(T), out var subscribers)) return;

        subscribers = Delegate.Remove(subscribers, action);

        if (subscribers == null)
            _subscribers.Remove(typeof(T));
        else
            _subscribers[typeof(T)] = subscribers;
    }

    public void Publish<T>(T eventData)
    {
        if (_subscribers.TryGetValue(typeof(T), out var subscribers))
            ((Action<T>)subscribers)?.Invoke(eventData);
    }
}