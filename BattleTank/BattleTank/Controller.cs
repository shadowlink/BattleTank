using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BattleTank
{
    public class Controller
    {
        #region members
        GamePadState gamepad;
        KeyboardState keyboard;
        PlayerIndex numplayer;
        bool _tecladoSelect;
        #endregion

        public Controller(String tipo)
        {
            _tecladoSelect = false;
            if (tipo == "teclado")
            {
                _tecladoSelect = true;
            }
            if (tipo == "mando1")
            {
                numplayer = PlayerIndex.One;
            }
            else if (tipo == "mando2")
            {
                numplayer = PlayerIndex.Two;
            }
            else if (tipo == "mando3")
            {
                numplayer = PlayerIndex.Three;
            }
            else if (tipo == "mando4")
            {
                numplayer = PlayerIndex.Four;
            }
        }

        public bool Arriba()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.W);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.LeftThumbstickUp);
            }
        }

        public bool Abajo()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.S);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.LeftThumbstickDown);
            }
        }

        public bool Derecha()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.D);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.LeftThumbstickRight);
            }
        }

        public bool Izquierda()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.A);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.LeftThumbstickLeft);
            }
        }

        public bool Accelerar()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.W);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.A);
            }
        }

        public bool Retroceder()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.S);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.B);
            }
        }

        public bool Disparar()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.Space);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.RightTrigger);
            }
        }

        public bool Start()
        {
            if (_tecladoSelect)
            {
                keyboard = Keyboard.GetState();
                return keyboard.IsKeyDown(Keys.Enter);
            }
            else
            {
                gamepad = GamePad.GetState(numplayer);
                return gamepad.IsButtonDown(Buttons.Start);
            }
        }

        public void Vibrar(float intensidad)
        {
            if (!_tecladoSelect)
            {
                GamePad.SetVibration(numplayer, intensidad, intensidad);
            }
        }
    }
}
