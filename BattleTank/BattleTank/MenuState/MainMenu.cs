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
    public class MainMenu : State
    {
        #region Members
        Texture2D _titulo;
        Texture2D _jugar;
        Texture2D _opciones;
        Texture2D _salir;
        Texture2D _flecha;
        enum Opcion { Jugar, Opciones, Salir };
        Opcion _opcionActual;
        SpriteBatch _spriteBatch;
        TimeSpan _previousGameTime;
        TimeSpan _movementTime;
        SpriteFont _fuente;
        SoundEffect _menuSelect, _menuIter;
        Song _menuSound;
        bool _songStart;
        #endregion

        public MainMenu(Game game) : base(game)
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _opcionActual = Opcion.Jugar;
            _movementTime = TimeSpan.FromSeconds(.20f);
            _songStart = false;
        }

        public override void Initialize()
        {


        }

        public override void LoadContent()
        {
            //Texturas
            _titulo = Content.Load<Texture2D>("graphics/Gui/Titulo");
            _jugar = Content.Load<Texture2D>("graphics/Gui/Jugar");
            _opciones = Content.Load<Texture2D>("graphics/Gui/Opciones");
            _salir = Content.Load<Texture2D>("graphics/Gui/Salir");
            _flecha = Content.Load<Texture2D>("graphics/Gui/Flecha");
            _fuente = Content.Load<SpriteFont>("fonts/gameFont");

            //Sonidos
            _menuIter = Content.Load<SoundEffect>("sound/SFX/SoundIter");
            _menuSelect = Content.Load<SoundEffect>("sound/SFX/Soundselect");
            _menuSound = Content.Load<Song>("sound/Music/Menu");
            MediaPlayer.IsRepeating = true;
        }


        public override void Update(GameTime gameTime)
        {

            if (!_songStart)
            {
                MediaPlayer.Play(_menuSound);
                _songStart = true;
            }  

            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (gameTime.TotalGameTime - _previousGameTime > _movementTime)
                {
                    _previousGameTime = gameTime.TotalGameTime;
                    if (_opcionActual == Opcion.Salir)
                    {
                        _opcionActual = Opcion.Opciones;
                        _menuIter.Play();
                    }
                    else if(_opcionActual==Opcion.Opciones)
                    {
                        _opcionActual = Opcion.Jugar;
                        _menuIter.Play();
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (gameTime.TotalGameTime - _previousGameTime > _movementTime)
                {
                    _previousGameTime = gameTime.TotalGameTime;
                    if (_opcionActual == Opcion.Jugar)
                    {
                        _opcionActual = Opcion.Opciones;
                        _menuIter.Play();
                    }
                    else if(_opcionActual==Opcion.Opciones)
                    {
                        _opcionActual = Opcion.Salir;
                        _menuIter.Play();
                    }
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _menuSelect.Play();
                switch (_opcionActual)
                {
                    case Opcion.Jugar:
                        Manager.Estados[GameStates.PrePlayState].Initialize();
                        Manager.Estados[GameStates.PrePlayState].LoadContent();
                        Manager._estadoActual = GameStates.PrePlayState;
                        break;
                    case Opcion.Salir:
                        this.Game.Exit();
                        break;
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_titulo, new Vector2(150, 10), Color.Black);
            _spriteBatch.Draw(_jugar, new Vector2(620, 300), Color.Black);
            _spriteBatch.Draw(_opciones, new Vector2(590, 400), Color.Black);
            _spriteBatch.Draw(_salir, new Vector2(620, 500), Color.Black);

            if (_opcionActual == Opcion.Jugar)
            {
                _spriteBatch.Draw(_flecha, new Vector2(550, 300), Color.Black);
            }
            else if (_opcionActual == Opcion.Opciones)
            {
                _spriteBatch.Draw(_flecha, new Vector2(530, 400), Color.Black);
            }
            else if (_opcionActual == Opcion.Salir)
            {
                _spriteBatch.Draw(_flecha, new Vector2(550, 500), Color.Black);
            }

            _spriteBatch.DrawString(_fuente, "Copyright (C) 2011 ShadowLink", new Vector2(10, 738), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_fuente, "v 0.6", new Vector2(1315, 738), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            _spriteBatch.End();
        }
    }
}
