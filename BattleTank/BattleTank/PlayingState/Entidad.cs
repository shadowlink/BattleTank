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
    public abstract class Entidad
    {
        #region OffSets
        public Vector2 posOff;
        public int widthOff;
        public int heightOff;
        #endregion

        #region Members
        public Color[] textureData;
        #endregion

        #region Propiedades
        public bool _activo;
        public float _speed { get; set; }
        public Vector2 _pos, _previouPos;
        public Vector2 _incremento;
        public Texture2D _textura;
        public Texture2D _texturaFrame;
        public float _anguloRotacion;
        public int _daño;
        #endregion

        public Entidad(Game game)
        {
            _daño=0;
        }
    }
}
