#region File Description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
#endregion

namespace Mimicware.Manager
{
    /// <summary>
    /// Standard input manager
    /// </summary>
    public class InputManager
    {
        #region Constants
        public const int MaxInputs = 4;
        #endregion

        #region Fields
        private PlayerIndex lastPlayerIndex;
        #endregion

        #region Properties
        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        /// <summary>
        /// The last player index which was polled
        /// </summary>
        public PlayerIndex LastPlayerIndex
        {
            get { return lastPlayerIndex; }
        }
        #endregion

        #region Constructor / Init
        /// <summary>
        /// Constructs a new input manager.
        /// </summary>
        public InputManager()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Use to check if a key has been pressed between this and the last update
        /// </summary>
        public bool NewKeyPressed(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (NewKeyPressed(key, (PlayerIndex)i))
                {
                    lastPlayerIndex = (PlayerIndex)i;
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Use to check if a key has been pressed between this and the last update for
        /// a specific player
        /// </summary>
        public bool NewKeyPressed(Keys key, PlayerIndex playerIndex)
        {
            lastPlayerIndex = playerIndex;
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyUp(key));
        }


        /// <summary>
        /// Use to check if a controller button has been pressed between this and the
        /// last update
        /// </summary>
        public bool NewButtonPressed(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (NewButtonPressed(button, (PlayerIndex)i))
                {
                    lastPlayerIndex = (PlayerIndex)i;
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Use to check if a controller button has been pressed between this and the
        /// last update for a specific player
        /// </summary>
        public bool NewButtonPressed(Buttons button, PlayerIndex playerIndex)
        {
            lastPlayerIndex = playerIndex;
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonUp(button));
        }

        /// <summary>
        /// Use to check if a key is pressed
        /// </summary>
        public bool KeyPressed(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (KeyPressed(key, (PlayerIndex)i))
                {
                    lastPlayerIndex = (PlayerIndex)i;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Use to check if a key is pressed by a specific player
        /// </summary>
        public bool KeyPressed(Keys key, PlayerIndex playerIndex)
        {
            lastPlayerIndex = playerIndex;
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key));
        }

        /// <summary>
        /// Use to check if a button is currently being pressed
        /// </summary>
        public bool ButtonPressed(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (ButtonPressed(button, (PlayerIndex)i))
                {
                    lastPlayerIndex = (PlayerIndex)i;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Use to check if a button is currently being pressed by a specific player
        /// </summary>
        public bool ButtonPressed(Buttons button, PlayerIndex playerIndex)
        {
            lastPlayerIndex = playerIndex;
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button));
        }
        #endregion

        #region Update
        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.Circular);
            }
        }
        #endregion
    }
}
