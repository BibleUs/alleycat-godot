﻿using System;

namespace AlleyCat.Event
{
    /// <summary>
    /// Adapted from UniRx (https://github.com/neuecc/UniRx).
    /// </summary>
    public interface IReadOnlyReactiveProperty<out T> : IObservable<T>
    {
        T Value { get; }

        bool HasValue { get; }
    }
}
