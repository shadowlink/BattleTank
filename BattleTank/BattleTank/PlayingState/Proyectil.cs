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
    public class Proyectil : Entidad
    {
        #region Members
        SpriteBatch _spriteBatch;
        public Player _player;
        public Colision _colision;
        public float _anguloRotacion;
        #endregion

        public Proyectil(Game game): base(game)
        {
            _colision = new Colision();
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _pos = Vector2.Zero;
            _daño = 1;
            _speed = 20.0f;
            _activo = true;
            _anguloRotacion = 0;
        }

        public void Initialize(Player player, Texture2D textura, Texture texturaFrameProyectil, float daño, Vector2 pos, Vector2 incremento, Colision colision)
        {
            _player = player;
            _textura = textura;
            _daño = (int)daño;
            _pos = pos;
            _colision = colision;
            _incremento = incremento;

            //Datos de color de la textura
            _texturaFrame = textura;
            textureData = new Color[_texturaFrame.Width * _texturaFrame.Height];
            _texturaFrame.GetData(textureData);
        }

        protected void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            _pos += _incremento*8;
            if (_colision.updateColision(this, _player))
            {
                _activo=false;
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_texturaFrame, new Vector2(_pos.X, _pos.Y), null, Color.White);
            _spriteBatch.End();
        }
    }
}
