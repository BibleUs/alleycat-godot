using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Motion;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.View
{
    public class HeadMountedViewFactory : TurretLikeFactory<HeadMountedView>
    {
        [Node]
        public Option<Camera> Camera { get; set; }

        [Export]
        public float Offset { get; set; } = 0.02f;

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Export]
        public HeadMountedView.StabilizeMode Stabilization { get; set; } =
            HeadMountedView.StabilizeMode.WhileMoving;

        [Export(PropertyHint.ExpRange, "0,1")]
        public float MinStabilization { get; set; } = 0.2f;

        [Export(PropertyHint.ExpRange, "0,1")]
        public float MaxStabilization { get; set; } = 0.8f;

        [Export(PropertyHint.ExpRange, "0.1,5")]
        public float TransitionTime { get; set; } = 2f;

        [Export(PropertyHint.ExpRange, "0.1,5")]
        public float VelocityThreshold { get; set; } = 0.2f;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxDofDistance { get; set; } = 5f;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxFocalDistance { get; set; } = 2f;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float FocusRange { get; set; } = 3f;

        [Export(PropertyHint.ExpRange, "10,1000")]
        public float FocusSpeed { get; set; } = 100f;

        [Export]
        public Curve NeckRotationCurve { get; set; }

        [Node("Rotation")]
        public Option<IInputBindings> RotationInput { get; set; }

        [Node("Deactivate")]
        public Option<IInputBindings> DeactivateInput { get; set; }

        [Export] private NodePath _camera;

        [Export] private NodePath _character;

        public HeadMountedViewFactory()
        {
            MinYaw = -95;
            MaxYaw = 95;
            MinPitch = -70;
            MaxPitch = 80;

            Active = false;
        }

        protected override Validation<string, HeadMountedView> CreateService(
            Range<float> yawRange, Range<float> pitchRange, ILoggerFactory loggerFactory)
        {
            return new HeadMountedView(
                Camera.IfNone(() => GetViewport().GetCamera()),
                RotationInput,
                DeactivateInput,
                yawRange,
                pitchRange,
                ProcessMode,
                this,
                Active,
                loggerFactory)
            {
                Offset = Offset,
                MaxDofDistance = MaxDofDistance,
                MaxFocalDistance = MaxFocalDistance,
                FocusRange = FocusRange,
                FocusSpeed = FocusSpeed,
                Stabilization = Stabilization,
                MinStabilization = MinStabilization,
                MaxStabilization = MaxStabilization,
                VelocityThreshold = VelocityThreshold,
                TransitionTime = TransitionTime,
                NeckRotationCurve = NeckRotationCurve
            };
        }
    }
}
