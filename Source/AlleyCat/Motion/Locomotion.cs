using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Motion
{
    public abstract class Locomotion<T> : GameNode, ILocomotion where T : Spatial
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public T Target { get; }

        public Vector3 Velocity => _velocity.Value;

        public Vector3 RotationalVelocity => _rotationalVelocity.Value;

        public IObservable<Vector3> OnVelocityChange => _velocity.AsObservable();

        public IObservable<Vector3> OnRotationalVelocityChange => _rotationalVelocity.AsObservable();

        public abstract ProcessMode ProcessMode { get; }

        public IObservable<float> OnProcess => TimeSource.OnProcess;

        public IObservable<float> OnPhysicsProcess => TimeSource.OnPhysicsProcess;

        public IScheduler Scheduler => TimeSource.Scheduler;

        public IScheduler PhysicsScheduler => TimeSource.PhysicsScheduler;

        protected ITimeSource TimeSource { get; }

        private readonly BehaviorSubject<bool> _active;

        private readonly BehaviorSubject<Vector3> _velocity;

        private readonly BehaviorSubject<Vector3> _rotationalVelocity;

        private Vector3 _requestedMovement;

        private Vector3 _requestedRotation;

        protected Locomotion(
            T target,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(target, nameof(target)).IsNotNull();
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Target = target;
            TimeSource = timeSource;

            _active = CreateSubject(active);
            _velocity = CreateSubject(Vector3.Zero);
            _rotationalVelocity = CreateSubject(Vector3.Zero);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnActiveStateChange
                .Where(v => !v && Valid)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(_ => this.Stop(), this);

            this.OnProcess(ProcessMode)
                .Where(_ => Active && Valid)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(ProcessLoop, this);

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                OnVelocityChange
                    .DistinctUntilChanged()
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(v => this.LogTrace("Velocity changed = {}.", v), this);

                OnRotationalVelocityChange
                    .DistinctUntilChanged()
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(v => this.LogTrace("Rotational velocity changed = {}.", v), this);
            }
        }

        public void Move(Vector3 velocity) => _requestedMovement = velocity;

        public void Rotate(Vector3 velocity) => _requestedRotation = velocity;

        protected abstract void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity);

        protected virtual void ProcessLoop(float delta)
        {
            var before = Target.GlobalTransform;

            Process(delta, _requestedMovement, _requestedRotation);

            var after = Target.GlobalTransform;

            if (delta <= 0) return;

            var currentVelocity = (Target.ToLocal(after.origin) - Target.ToLocal(before.origin)) / delta;
            var currentRotationalVelocity = (before.basis.Inverse() * after.basis).GetEuler() / delta;

            _velocity.OnNext(currentVelocity);
            _rotationalVelocity.OnNext(currentRotationalVelocity);
        }
    }
}
