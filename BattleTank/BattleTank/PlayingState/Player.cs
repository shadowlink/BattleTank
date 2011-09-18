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
using TiledLib;

namespace BattleTank
{
    public class Player : Entidad
    {
        #region Members
        SpriteBatch _spriteBatch;
        public Controller _controller;
        public KeyboardState _currentKeyboardState, _previousKeyboardState;
        public GamePadState _currentGamePadState, _previousGamePadState;    
        public Texture2D _texturaProyectil { get; set; }
        public Colision _colision;
        private List<Proyectil> _listProyectil;
        private TimeSpan _previousFireTime;
        public enum _estado{arriba,abajo,derecha,izquierda, muerto};
        public _estado _estadoActual;
        public Game _game;      
        public Animacion _animacion;
        private Rectangle _sourceRect;
        private Rectangle _destinationRect;
        private int _actualFrame;
        public int frameWidth;
        public int frameHeight;
        private TimeSpan _previousRotation;
        private TimeSpan _tiempoRotacion;
        public Rumble _rumble;
        //Sombras
        private byte shadowAlpha;
        private int shadowOffsetX;
        private int shadowOffsetY;
        private float shadowRotation;
        Color _colorSombra;
        //Sonidos
        SoundEffect _disparo;
        #endregion

        #region Propiedades
        public float _vida;
        private TimeSpan _fireTime;
        #endregion

        public Player(Game game) : base(game)
        {
            //SpriteBatch de dibujado
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            //Lista que contiene todos los proyectiles en pantalla
            _listProyectil=new List<Proyectil>();
            //Cadencia de disparo
            _fireTime = TimeSpan.FromSeconds(.30f);
            //Estado actual del jugador
            _estadoActual = _estado.derecha;
            //Vida del jugador
            _vida = 179;
            //Sigue viva la entidad?
            _activo = true;
            //Instancia del juego
            _game=game;
            //Animacion del jugador
            _animacion = new Animacion();
            //Velocidad de movimiento
            _speed = 2.0f;

            //Sombras
            shadowAlpha = 50;
            _colorSombra = new Color(0, 0, 0, shadowAlpha);
            shadowOffsetX = 5;
            shadowOffsetY = -5;
            shadowRotation = -1f;

            //Datos para el cuadrado de colision
            posOff = new Vector2(6, 1);
            widthOff = 56;
            heightOff = 55;
            //Datos para el cuadrado de frame
            _actualFrame = 1;
            frameWidth = 58;
            frameHeight = 58;

            _anguloRotacion = 0;
            _tiempoRotacion = TimeSpan.FromSeconds(.20f);

            //Frame de la secuencia que se va a dibujar
            _sourceRect = new Rectangle(0, 0, frameWidth, frameHeight);
        }

        public void Initialize(Texture2D textura)
        {
            //Datos de color de la textura
            _texturaFrame = textura;
            textureData = new Color[_texturaFrame.Width*_texturaFrame.Height];
            _texturaFrame.GetData(textureData);
            //_animacion.Initialize(_textura, _pos, 60, 60, 4, 0 , Color.White, .0f, false);

            //Sonidos
            _disparo = _game.Content.Load<SoundEffect>("sound/SFX/Disparo");
        }

        protected void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {   
            _incremento = new Vector2((float)Math.Sin(MathHelper.ToRadians(_anguloRotacion)), (float)Math.Cos(MathHelper.ToRadians(_anguloRotacion)));

            _previouPos = _pos;

            if (_activo)
            {
                _rumble.UpdateRumble(gameTime);
                if (_controller.Izquierda())
                {
                    _previousRotation = gameTime.TotalGameTime;
                    _anguloRotacion += 2;
                    if (_anguloRotacion >= 360)
                    {
                        _anguloRotacion = 0;
                    }
                    if (_colision.updateColision(this, this))
                    {
                        _anguloRotacion -= 2;
                        if (_anguloRotacion < 0)
                        {
                            _anguloRotacion = 360;
                        }
                    }
                }
                if (_controller.Derecha())
                {
                    _previousRotation = gameTime.TotalGameTime;
                    _anguloRotacion -= 2;
                    if (_anguloRotacion <= 0)
                    {
                        _anguloRotacion = 360;
                    }
                    if (_colision.updateColision(this, this))
                    {
                        _anguloRotacion += 2;
                        if (_anguloRotacion > 360)
                        {
                            _anguloRotacion = 0;
                        }
                    }
                }
                if (_controller.Accelerar())
                {
                    _pos += _incremento * _speed;
                    if (_colision.updateColision(this, this))
                    {
                        _pos -= _incremento * _speed;
                    }
                }
                else if (_controller.Retroceder())
                {
                    _pos -= _incremento * _speed;
                    if (_colision.updateColision(this, this))
                    {
                        _pos += _incremento * _speed;
                    }
                }
                if (_controller.Disparar())
                {
                    if (gameTime.TotalGameTime - _previousFireTime > _fireTime)
                    {
                        _disparo.Play();
                        _previousFireTime = gameTime.TotalGameTime;
                        nuevoProyectil();
                    }
                }
            }

            for (int i = _listProyectil.Count - 1; i >= 0; i--)
            {
                _listProyectil[i].Update(gameTime);
                if (_listProyectil[i]._activo == false)
                {
                    _listProyectil.RemoveAt(i);
                }
            }

            _animacion.Update(gameTime);
            _sourceRect = new Rectangle((frameWidth * _actualFrame) + _actualFrame + 1, 1, frameWidth, frameHeight);
            _destinationRect = new Rectangle((int)_pos.X, (int)_pos.Y, widthOff, heightOff);
        }

        private void nuevoProyectil()
        {
            Proyectil _proyectil = new Proyectil(_game);
            _proyectil.Initialize(this, _texturaProyectil, _texturaProyectil, 20.0f, new Vector2(_pos.X-7, _pos.Y-7), _incremento, _colision);
            _listProyectil.Add(_proyectil);
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            for (int i = 0; i < _listProyectil.Count; i++)
            {
                _listProyectil[i].Draw(gameTime);
            }

            if (_activo)
            {
                _spriteBatch.Draw(_texturaFrame, _pos, null, Color.White, MathHelper.ToRadians(-_anguloRotacion) ,new Vector2(_texturaFrame.Width/2, _texturaFrame.Height/2), 1.0f, SpriteEffects.None, 1f);
                _spriteBatch.Draw(_texturaFrame, new Vector2(_pos.X + shadowOffsetX, _pos.Y - shadowOffsetY), null, _colorSombra, MathHelper.ToRadians(-_anguloRotacion), new Vector2(_texturaFrame.Width / 2, _texturaFrame.Height/2), 1f, SpriteEffects.None, 0.5f);
            }
            _spriteBatch.End();
        }
    }
}
