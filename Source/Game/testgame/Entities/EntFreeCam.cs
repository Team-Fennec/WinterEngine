using WinterEngine.SceneSystem;
using WinterEngine.SceneSystem.Attributes;
using WinterEngine.InputSystem;
using System.Numerics;
using MathLib;

namespace TestGame.Entities
{
    [EntityClass("ent_freecam")]
    internal class EntFreeCam : Entity
    {
        public override void Think(double deltaTime)
        {
            // update components
            base.Think(deltaTime);

            Vector2 delta2D = new Vector2(
                (float)Math.Cos(Angles.Deg2Rad(Transform.LocalEulerRotation.Z)),
                (float)Math.Sin(Angles.Deg2Rad(Transform.LocalEulerRotation.Z))
            );
            // input
            if (InputManager.ActionCheck("MoveUp"))
            {
                Transform.LocalPosition.X += delta2D.X;
                Transform.LocalPosition.Y -= delta2D.Y;
            }
            if (InputManager.ActionCheck("MoveDown"))
            {
                Transform.LocalPosition.X -= delta2D.X;
                Transform.LocalPosition.Y += delta2D.Y;
            }
            if (InputManager.ActionCheck("MoveLeft"))
            {
                Transform.LocalPosition.X -= delta2D.X;
                Transform.LocalPosition.Y -= delta2D.Y;
            }
            if (InputManager.ActionCheck("MoveRight"))
            {
                Transform.LocalPosition.X += delta2D.X;
                Transform.LocalPosition.Y += delta2D.Y;
            }

            if (InputManager.ActionCheck("LookLeft"))
            {
                Transform.LocalEulerRotation.Z += 5;
            }

            if (InputManager.ActionCheck("LookRight"))
            {
                Transform.LocalEulerRotation.Z -= 5;
            }

            // set the camera to our position
            SceneManager.ActiveCamera.Position = Transform.Position;
            SceneManager.ActiveCamera.Rotation = Transform.EulerRotation;
        }
    }
}
