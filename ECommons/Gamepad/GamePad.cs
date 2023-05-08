using Dalamud.Game.ClientState.GamePad;
using ECommons.DalamudServices;
using System.Collections.Generic;

namespace ECommons.Gamepad
{
    public static class GamePad
    {
        public static Dictionary<GamepadButtons, string> ControllerButtons = new Dictionary<GamepadButtons, string>()
        {
            { GamepadButtons.None, "None" },
            { GamepadButtons.DpadUp, "D-Pad Up"},
            { GamepadButtons.DpadLeft, "D-Pad Left" },
            { GamepadButtons.DpadDown, "D-Pad Down" },
            { GamepadButtons.DpadRight, "D-Pad Right" },
            { GamepadButtons.North, "△ / Y" },
            { GamepadButtons.West, "□ / X" },
            { GamepadButtons.South, "X / A" },
            { GamepadButtons.East, "○ / B" },
            { GamepadButtons.L1, "L1 / LB" },
            { GamepadButtons.L2, "L2 / LT" },
            { GamepadButtons.R1, "R1 / RB" },
            { GamepadButtons.R2, "R2 / RT" },
            { GamepadButtons.L3, "L3 / LS" },
            { GamepadButtons.R3, "R3 / RS" },
            { GamepadButtons.Start, "Options / Start" },
            { GamepadButtons.Select, "Share / Back" }
        };

        public static bool IsButtonPressed(GamepadButtons button) => Svc.GamepadState.Pressed(button) == 1;

        public static bool IsButtonHeld(GamepadButtons button) => Svc.GamepadState.Repeat(button) == 1; 

        public static bool IsButtonJustReleased(GamepadButtons button) => Svc.GamepadState.Released(button) == 1;
    }
}
