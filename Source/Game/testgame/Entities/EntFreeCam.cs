using WinterEngine.SceneSystem;
using WinterEngine.SceneSystem.Attributes;
using WinterEngine.InputSystem;
using System.Numerics;
using MathLib;
using ImGuiNET;

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
                (float)Math.Cos(Angles.Deg2Rad(Transform.LocalEulerRotation.Z)) * 6,
                (float)Math.Sin(Angles.Deg2Rad(Transform.LocalEulerRotation.Z)) * 6
            );
            // input
            if (InputManager.ActionCheck("MoveUp"))
            {
                Transform.LocalPosition.X += (float)(delta2D.X * deltaTime);
                Transform.LocalPosition.Y -= (float)(delta2D.Y * deltaTime);
            }
            if (InputManager.ActionCheck("MoveDown"))
            {
                Transform.LocalPosition.X -= (float)(delta2D.X * deltaTime);
                Transform.LocalPosition.Y += (float)(delta2D.Y * deltaTime);
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

            if (InputManager.ActionCheck("LookLeft"))
            {
                Transform.LocalEulerRotation.Z -= (float)(50 * deltaTime);
            }

            if (InputManager.ActionCheck("LookRight"))
            {
                Transform.LocalEulerRotation.Z += (float)(50 * deltaTime);
            }

            // set the camera to our position
            SceneManager.ActiveCamera.Position = Transform.Position;
            SceneManager.ActiveCamera.Rotation = Transform.EulerRotation;

            ImGui.SetNextWindowPos(Vector2.Zero, ImGuiCond.Always);
            if (ImGui.Begin("freecam_transform", 
                ImGuiWindowFlags.NoDecoration
                |ImGuiWindowFlags.NoSavedSettings
                |ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text($"Position: {Transform.Position}");
                ImGui.Text($"Rotation: {Transform.EulerRotation}");

                ImGui.End();
            }
        }
    }
}
