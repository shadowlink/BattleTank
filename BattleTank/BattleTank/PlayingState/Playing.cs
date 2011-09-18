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
    public class Playing : State
    {
        #region Members
        Game _game;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _fuente;
        bool _activo;
        TimeSpan _visualTime;
        Texture2D _hud;
        Texture2D _hudVida;
        Texture2D[] _vidaPlayer;
        Viewport _viewport;
        Map map;
        List<Player> _listaPlayers;
        Colision _colision;
        #endregion

        public Playing(Game game) : base(game)
        {
            _game = game;
            graphics = GraphicsManager;
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            _listaPlayers = new List<Player>();
        }

        public void MailBox(List<Player> players, Map mimapa)
        {
            _listaPlayers = players;
            map = mimapa;
        }

        public override void Initialize()
        {
            _visualTime = TimeSpan.FromSeconds(16.60f);
            _activo = true;
            _viewport = new Viewport();
            _vidaPlayer = new Texture2D[_listaPlayers.Count];
        }

        public override void LoadContent()
        {
            //Fuentes
            _fuente = Content.Load<SpriteFont>("fonts/gameFont");
            
            //Hud
            _hud = CreateRectangle(200, 760, Color.Black);
            _hudVida = Content.Load<Texture2D>("graphics/Gui/HUDVida");

            for (int i = 0; i < _vidaPlayer.Length; i++)
            {
                _vidaPlayer[i] = CreateRectangle(179, 49, Color.Red);
            }

            //Viewports
            _viewport.X = 200;
            _viewport.Y = 0;
            _viewport.Width = 960;
            _viewport.Height = 760;
            _viewport.MinDepth = 0;
            _viewport.MaxDepth = 1;
        }

        protected void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_activo)
            {
                for (int i = 0; i < _listaPlayers.Count; i++ )
                {
                    _listaPlayers[i].Update(gameTime);
                }

                for (int i = 0; i < _vidaPlayer.Length; i++)
                {
                    if (_listaPlayers[i]._vida>0)
                    {
                        _vidaPlayer[i] = CreateRectangle((int)_listaPlayers[i]._vida, 49, Color.Red);
                    }
                    else
                    {
                        _vidaPlayer[i] = CreateRectangle(1, 49, Color.Red);
                        _activo = false;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //(Manager.Estados[GameStates.PlayingState] as MainMenu).Initialize();
                //(Manager.Estados[GameStates.PlayingState] as MainMenu).LoadContent();
                Manager._estadoActual = GameStates.MainMenu;
            }
        }

        public Texture2D CreateRectangle(int lado1, int lado2, Color colorito)
        {
            Texture2D texture = new Texture2D(graphics.GraphicsDevice, lado1, lado2);
            Color[] dataColor = new Color[lado1 * lado2];

            for (int i = 0; i < dataColor.Length; ++i)
            {
                dataColor[i] = colorito;
            }

            texture.SetData(dataColor);
            return texture;
        }

        public override void Draw(GameTime gameTime)
        {
            Viewport original=graphics.GraphicsDevice.Viewport;
            GraphicsDevice.Clear(Color.White);
            
            spriteBatch.Begin();
            
            //Marcadores de vida gráficos
            spriteBatch.Draw(_hud, Vector2.Zero, null, Color.Black);
            spriteBatch.Draw(_hud, new Vector2(1160, 0), null, Color.Black);
            spriteBatch.Draw(_vidaPlayer[0], new Vector2(19, 40), null, Color.White);
            spriteBatch.Draw(_vidaPlayer[1], new Vector2(1174, 40), null, Color.White);
            spriteBatch.Draw(_hudVida, new Vector2(5, 40), null, Color.White);
            spriteBatch.Draw(_hudVida, new Vector2(1160, 40), null, Color.White);
            
            //Marcadores de vida numéricos
            spriteBatch.DrawString(_fuente, "Jugador 1: " + _listaPlayers[0]._vida, new Vector2(10, 0), Color.White);
            spriteBatch.DrawString(_fuente, "Jugador 2: " + _listaPlayers[1]._vida, new Vector2(1170, 0), Color.White);
            spriteBatch.DrawString(_fuente, "Jugador 3: " + "0", new Vector2(10, 358), Color.White);
            spriteBatch.DrawString(_fuente, "Jugador 4: " + "0", new Vector2(1170, 358), Color.White);
            spriteBatch.End();

            graphics.GraphicsDevice.Viewport = _viewport;
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            map.Draw(spriteBatch);
            if (!_activo)
            {
                if (_listaPlayers[0]._vida < 1)
                {
                    spriteBatch.DrawString(_fuente, "Jugador 2 gana! ", new Vector2(280, 384), Color.Yellow, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.DrawString(_fuente, "Jugador 1 gana! ", new Vector2(280, 384), Color.Yellow, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                }

                spriteBatch.DrawString(_fuente, "Pulsa Escape", new Vector2(380, 450), Color.Yellow);
                _activo = false;
            }
            spriteBatch.End();
            spriteBatch.Begin();
            for (int i = 0; i < _listaPlayers.Count; i++)
            {
                _listaPlayers[i].Draw(gameTime);
            }
            spriteBatch.End();
            graphics.GraphicsDevice.Viewport = original;
        }
    }
}