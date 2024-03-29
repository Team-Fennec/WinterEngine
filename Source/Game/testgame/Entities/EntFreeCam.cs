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

            Vector3 delta2D = new Vector3(
                (float)Math.Cos(Angles.Deg2Rad(Transform.LocalEulerRotation.Z)) * 12,
                (float)Math.Sin(Angles.Deg2Rad(Transform.LocalEulerRotation.Z)) * 12,
                (float)Math.Sin(Angles.Deg2Rad(Transform.LocalEulerRotation.X)) * 16
            );

            // input
            if (InputManager.ActionCheck("MoveUp"))
            {
                Transform.LocalPosition.X += (float)(delta2D.X * deltaTime);
                Transform.LocalPosition.Y -= (float)(delta2D.Y * deltaTime);
                Transform.LocalPosition.Z -= (float)(delta2D.Z * deltaTime);
            }
            if (InputManager.ActionCheck("MoveDown"))
            {
                Transform.LocalPosition.X -= (float)(delta2D.X * deltaTime);
                Transform.LocalPosition.Y += (float)(delta2D.Y * deltaTime);
                Transform.LocalPosition.Z += (float)(delta2D.Z * deltaTime);
            }
            if (InputManager.ActionCheck("MoveLeft"))
            {
                Transform.LocalPosition.X += (float)(delta2D.Y * deltaTime);
                Transform.LocalPosition.Y += (float)(delta2D.X * deltaTime);
            }
            if (InputManager.ActionCheck("MoveRight"))
            {
                Transform.LocalPosition.X -= (float)(delta2D.Y * deltaTime);
                Transform.LocalPosition.Y -= (float)(delta2D.X * deltaTime);
            }
            if (InputManager.ActionCheck("MoveUpV"))
            {
                Transform.LocalPosition.Z += (float)(6 * deltaTime);
            }
            if (InputManager.ActionCheck("MoveDownV"))
            {
                Transform.LocalPosition.Z -= (float)(6 * deltaTime);
            }

            Transform.LocalEulerRotation.X -= (float)((InputManager.GetMouseDelta().Y * 30.0) * deltaTime);
            Transform.LocalEulerRotation.Z -= (float)((InputManager.GetMouseDelta().X * 30.0) * deltaTime);
            Transform.LocalEulerRotation.X = Math.Clamp(Transform.LocalEulerRotation.X, -90.0f, 90.0f);

            // set the camera to our position
            SceneManager.ActiveCamera.Position = Transform.Position;
            SceneManager.ActiveCamera.Rotation = Transform.EulerRotation;
        }
    }
}
