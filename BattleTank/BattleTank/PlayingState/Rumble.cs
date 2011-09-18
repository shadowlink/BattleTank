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
    public class Rumble
    {
        #region Members
        TimeSpan _rumbleTime, _previousRumble;
        Controller _controlador;
        #endregion

        public Rumble(Controller controlador)
        {
            _controlador = controlador;
            _rumbleTime = TimeSpan.FromSeconds(0.7f);
        }

        public void RumbleDisparo()
        {
            _controlador.Vibrar(1.0f);
        }

        public void RumbleMuerte()
        {
            _controlador.Vibrar(2.0f);
        }

        public void UpdateRumble(GameTime gametime)
        {
            if (gametime.TotalGameTime - _previousRumble > _rumbleTime)
            {
                _previousRumble = gametime.TotalGameTime;
                _controlador.Vibrar(0f);
            }
        }
    }
}
