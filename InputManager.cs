using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ODBank
{
    /// <summary>
    /// Manages all sort of input. Allows assigning keys to specific actions
    /// </summary>
    class InputManager
    {
        /// <summary>
        /// Ingame action that can be assigned to keys
        /// </summary>
        public enum Action
        {
            Up,
            Down,
            Left,
            Right,
            Boost,
            Stab,
            Debug,
            ScrollIn,
            ScrollOut,
        }

        /// <summary>
        /// Keyboard key assignment
        /// </summary>
        public Dictionary<Action, AllKeys> KeyboardDefinition { get; set; }

        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        public Vector2 MousePosition { get; private set; }

        /// <summary>
        /// Indicates if the game is on top and active
        /// </summary>
        public bool GameActive
        {
            get
            {
                return SceneManager.Game.IsActive;
            }
        }

        /// <summary>
        /// Indicates if the mouse is on the screen
        /// </summary>
        public bool MouseOnScreen
        {
            get
            {
                return SceneManager.Game.GraphicsDevice.Viewport.Bounds.Contains(MousePosition);
            }
        }

        /// <summary>
        /// Indicates if the mouse is on the screen and the game window is active
        /// </summary>
        public bool MouseActive
        {
            get
            {
                return GameActive && MouseOnScreen;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the [mouse was clicked this frame].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mouse was clicked this frame]; otherwise, <c>false</c>.
        /// </value>
        public bool MouseClicked { get; private set; }

        

        private KeyboardState _newKS = Keyboard.GetState();
        private KeyboardState _oldKS = Keyboard.GetState();

        private MouseState _newMS = Mouse.GetState();
        private MouseState _oldMS = Mouse.GetState();

        public InputManager()
        {
            KeyboardDefinition = new Dictionary<Action, AllKeys>();
        }

        /// <summary>
        /// Loads the default key assignment.
        /// </summary>
        public void LoadDefault()
        {
            KeyboardDefinition.Add(Action.Up, AllKeys.W);
            KeyboardDefinition.Add(Action.Down, AllKeys.S);
            KeyboardDefinition.Add(Action.Left, AllKeys.A);
            KeyboardDefinition.Add(Action.Right, AllKeys.D);
            KeyboardDefinition.Add(Action.Stab, AllKeys.LMB);
            KeyboardDefinition.Add(Action.Boost, AllKeys.RMB);
            KeyboardDefinition.Add(Action.ScrollIn, AllKeys.SCRIn);
            KeyboardDefinition.Add(Action.ScrollOut, AllKeys.SCROut);

            KeyboardDefinition.Add(Action.Debug, AllKeys.Y);
        }

        /// <summary>
        /// Determines if the given action is pressed currently
        /// </summary>
        public bool IsActionDown(Action Action)
        {
            var key = (AllKeys)GetKey(Action);
            if(key != AllKeys.None)
            {
                if (key == AllKeys.LMB)
                    return _newMS.LeftButton == ButtonState.Pressed;
                else if (key == AllKeys.MMB)
                    return _newMS.MiddleButton == ButtonState.Pressed;
                else if (key == AllKeys.RMB)
                    return _newMS.RightButton == ButtonState.Pressed;
                else if (key == AllKeys.SCRIn)
                    return _newMS.ScrollWheelValue > _oldMS.ScrollWheelValue;
                else if (key == AllKeys.SCROut)
                    return _newMS.ScrollWheelValue < _oldMS.ScrollWheelValue;
                else
                    return _newKS.IsKeyDown((Keys)key);
            }
                else
                    return false;
        }

        /// <summary>
        /// Determines if the given action was first pressed in this frame (oldks = false newks = true)
        /// </summary>
        public bool IsActionPressed(Action Action)
        {
            var key = (AllKeys)GetKey(Action);
            if (key != AllKeys.None)
            {
                if (key == AllKeys.LMB)
                    return _newMS.LeftButton == ButtonState.Pressed && _oldMS.LeftButton == ButtonState.Released;
                else if (key == AllKeys.MMB)
                    return _newMS.MiddleButton == ButtonState.Pressed && _oldMS.MiddleButton == ButtonState.Released;
                else if (key == AllKeys.LMB)
                    return _newMS.RightButton == ButtonState.Pressed && _oldMS.RightButton == ButtonState.Released;
                else if (key == AllKeys.SCRIn)
                    return _newMS.ScrollWheelValue > _oldMS.ScrollWheelValue;
                else if (key == AllKeys.SCROut)
                    return _newMS.ScrollWheelValue < _oldMS.ScrollWheelValue;
                else
                    return _newKS.IsKeyDown((Keys)key) && _oldKS.IsKeyUp((Keys)key);
            }
            else
                return false;
        }

        public int GetKey(Action Action)
        {
            if (KeyboardDefinition.ContainsKey(Action))
                return (int)KeyboardDefinition[Action];
            else
                return (int)AllKeys.None;

        }
        
        /// <summary>
        /// Used to update states
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            _oldKS = _newKS;
            _newKS = Keyboard.GetState();

            _oldMS = _newMS;
            _newMS = Mouse.GetState();

            if (_newMS.LeftButton == ButtonState.Pressed && _oldMS.LeftButton == ButtonState.Released)
                MouseClicked = true;
            else
                MouseClicked = false;

            MousePosition = _newMS.Position.ToVector2();
        }

        public enum AllKeys
        {
            None = 0,
            Back = 8,
            Tab = 9,
            Enter = 13,
            Pause = 19,
            CapsLock = 20,
            Kana = 21,
            Kanji = 25,
            Escape = 27,
            ImeConvert = 28,
            ImeNoConvert = 29,
            Space = 32,
            PageUp = 33,
            PageDown = 34,
            End = 35,
            Home = 36,
            Left = 37,
            Up = 38,
            Right = 39,
            Down = 40,
            Select = 41,
            Print = 42,
            Execute = 43,
            PrintScreen = 44,
            Insert = 45,
            Delete = 46,
            Help = 47,
            D0 = 48,
            D1 = 49,
            D2 = 50,
            D3 = 51,
            D4 = 52,
            D5 = 53,
            D6 = 54,
            D7 = 55,
            D8 = 56,
            D9 = 57,
            A = 65,
            B = 66,
            C = 67,
            D = 68,
            E = 69,
            F = 70,
            G = 71,
            H = 72,
            I = 73,
            J = 74,
            K = 75,
            L = 76,
            M = 77,
            N = 78,
            O = 79,
            P = 80,
            Q = 81,
            R = 82,
            S = 83,
            T = 84,
            U = 85,
            V = 86,
            W = 87,
            X = 88,
            Y = 89,
            Z = 90,
            LeftWindows = 91,
            RightWindows = 92,
            Apps = 93,
            Sleep = 95,
            NumPad0 = 96,
            NumPad1 = 97,
            NumPad2 = 98,
            NumPad3 = 99,
            NumPad4 = 100,
            NumPad5 = 101,
            NumPad6 = 102,
            NumPad7 = 103,
            NumPad8 = 104,
            NumPad9 = 105,
            Multiply = 106,
            Add = 107,
            Separator = 108,
            Subtract = 109,
            Decimal = 110,
            Divide = 111,
            F1 = 112,
            F2 = 113,
            F3 = 114,
            F4 = 115,
            F5 = 116,
            F6 = 117,
            F7 = 118,
            F8 = 119,
            F9 = 120,
            F10 = 121,
            F11 = 122,
            F12 = 123,
            F13 = 124,
            F14 = 125,
            F15 = 126,
            F16 = 127,
            F17 = 128,
            F18 = 129,
            F19 = 130,
            F20 = 131,
            F21 = 132,
            F22 = 133,
            F23 = 134,
            F24 = 135,
            NumLock = 144,
            Scroll = 145,
            LeftShift = 160,
            RightShift = 161,
            LeftControl = 162,
            RightControl = 163,
            LeftAlt = 164,
            RightAlt = 165,
            BrowserBack = 166,
            BrowserForward = 167,
            BrowserRefresh = 168,
            BrowserStop = 169,
            BrowserSearch = 170,
            BrowserFavorites = 171,
            BrowserHome = 172,
            VolumeMute = 173,
            VolumeDown = 174,
            VolumeUp = 175,
            MediaNextTrack = 176,
            MediaPreviousTrack = 177,
            MediaStop = 178,
            MediaPlayPause = 179,
            LaunchMail = 180,
            SelectMedia = 181,
            LaunchApplication1 = 182,
            LaunchApplication2 = 183,
            OemSemicolon = 186,
            OemPlus = 187,
            OemComma = 188,
            OemMinus = 189,
            OemPeriod = 190,
            OemQuestion = 191,
            OemTilde = 192,
            ChatPadGreen = 202,
            ChatPadOrange = 203,
            OemOpenBrackets = 219,
            OemPipe = 220,
            OemCloseBrackets = 221,
            OemQuotes = 222,
            Oem8 = 223,
            OemBackslash = 226,
            ProcessKey = 229,
            OemCopy = 242,
            OemAuto = 243,
            OemEnlW = 244,
            Attn = 246,
            Crsel = 247,
            Exsel = 248,
            EraseEof = 249,
            Play = 250,
            Zoom = 251,
            Pa1 = 253,
            OemClear = 254,

            LMB = 255,
            MMB = 256,
            RMB = 257,
            SCRIn = 258,
            SCROut = 259,
        }
    }
}
