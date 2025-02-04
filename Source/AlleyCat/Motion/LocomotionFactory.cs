using AlleyCat.Autowire;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public abstract class LocomotionFactory<TLocomotion, TTarget> : GameNodeFactory<TLocomotion>
        where TLocomotion : ILocomotion, IGameNode
        where TTarget : Spatial
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node]
        public Option<TTarget> Target { get; set; }

        [Export] private NodePath _targetPath;

        protected override Validation<string, TLocomotion> CreateService(ILoggerFactory loggerFactory)
        {
            return Target
                .ToValidation("Missing locomotion target.")
                .Bind(target => CreateService(target, loggerFactory));
        }

        protected abstract Validation<string, TLocomotion> CreateService(TTarget target, ILoggerFactory loggerFactory);
    }
}
