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
        Player player, player2;
        Map map;
        Colision _colision;
        List<Player> _listaPlayers;
        SpriteFont _fuente;
        bool _activo;
        TimeSpan _visualTime;
        Texture2D _hud;
        Texture2D _hudVida;
        Texture2D _vidaPlayer1, _vidaPlayer2;
        Viewport _viewport;
        #endregion

        public Playing(Game game) : base(game)
        {
            _game = game;
            graphics = GraphicsManager;
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        public override void Initialize()
        {
            _visualTime = TimeSpan.FromSeconds(16.60f);
            _activo = true;
            _listaPlayers = new List<Player>();
            player = new Player(_game);
            player2 = new Player(_game);
            _colision = new Colision();
            _viewport = new Viewport();
            map = Content.Load<Map>("map/mapa3");
            _colision.Initialize(map, _game);

            _listaPlayers.Add(player);
            _listaPlayers.Add(player2);
        }

        public override void LoadContent()
        {
            //Fuentes
            _fuente = Content.Load<SpriteFont>("fonts/gameFont");
            
            //Hud
            _hud = CreateRectangle(200, 760, Color.Black);
            _hudVida = Content.Load<Texture2D>("graphics/Gui/HUDVida");
            _vidaPlayer1 = CreateRectangle(179, 49, Color.Red);
            _vidaPlayer2 = CreateRectangle(179, 49, Color.Red);

            //Colisiones
            _colision._listaPlayers = _listaPlayers;

            //Jugdores
            player._textura = Content.Load<Texture2D>("graphics/Chars/tanque1_1");
            //player._texturaFrame = Content.Load<Texture2D>("graphics/Chars/tanque");
            player._texturaProyectil = Content.Load<Texture2D>("graphics/Proyectil/Proyectil");
            player._pos = new Vector2(75, 320);
            player._colision = _colision;
            player._controlador = "teclado";

            player2._textura = Content.Load<Texture2D>("graphics/Chars/tanque2_2");
           // player._texturaFrame = Content.Load<Texture2D>("graphics/Chars/tanque2");
            player2._texturaProyectil = Content.Load<Texture2D>("graphics/Proyectil/proyectil");
            player2._pos = new Vector2(850, 320);
            player2._colision = _colision;
            player2._controlador = "mando";

            //Viewports
            _viewport.X = 200;
            _viewport.Y = 0;
            _viewport.Width = 960;
            _viewport.Height = 760;
            _viewport.MinDepth = 0;
            _viewport.MaxDepth = 0;

            player.Initialize(Content.Load<Texture2D>("graphics/Chars/tanque"));
            player2.Initialize(Content.Load<Texture2D>("graphics/Chars/tanque2"));
        }

        protected void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_activo)
            {
                //Actualizar mapa de colisiones en el jugador por si alguna elemento del mapa ha cambiado
                _colision.Update(map);
                player._colision = _colision;
                player2._colision = _colision;

                player.Update(gameTime);
                player2.Update(gameTime);

                if (player._vida > 0) 
                {
                    _vidaPlayer1 = CreateRectangle((int)player._vida, 49, Color.Red);
                }
                else
                {
                    _vidaPlayer1 = CreateRectangle(1, 49, Color.Red);
                    _activo = false;
                }

                if (player2._vida > 0)
                {
                    _vidaPlayer2 = CreateRectangle((int)player2._vida, 49, Color.Red);
                }
                else
                {
                    _vidaPlayer2 = CreateRectangle(1, 49, Color.Red);
                    _activo = false;
                }

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //(Manager.Estados[GameStates.PlayingState] as Playing).Initialize();
                //(Manager.Estados[GameStates.PlayingState] as Playing).LoadContent();
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
            GraphicsDevice.Clear(Color.Brown);

            spriteBatch.Begin();
            spriteBatch.Draw(_hud, Vector2.Zero, null, Color.Black);
            spriteBatch.Draw(_hud, new Vector2(1160, 0), null, Color.Black);
            spriteBatch.Draw(_vidaPlayer1, new Vector2(19, 40), null, Color.White);
            spriteBatch.Draw(_vidaPlayer2, new Vector2(1174, 40), null, Color.White);
            spriteBatch.Draw(_hudVida, new Vector2(5, 40), null, Color.White);
            spriteBatch.Draw(_hudVida, new Vector2(1160, 40), null, Color.White);
            spriteBatch.DrawString(_fuente, "Jugador 1: " + player._vida, new Vector2(10, 0), Color.White);
            spriteBatch.DrawString(_fuente, "Jugador 2: " + player2._vida, new Vector2(1170, 0), Color.White);
            spriteBatch.DrawString(_fuente, "Jugador 3: " + "0", new Vector2(10, 358), Color.White);
            spriteBatch.DrawString(_fuente, "Jugador 4: " + "0", new Vector2(1170, 358), Color.White);
            spriteBatch.End();

            graphics.GraphicsDevice.Viewport = _viewport;
            spriteBatch.Begin();
            if (_activo)
            {
                map.Draw(spriteBatch);
            }
            else
            {
                map.Draw(spriteBatch);
                if (player._vida == 0)
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
            player.Draw(gameTime);
            player2.Draw(gameTime);
            spriteBatch.End();
            graphics.GraphicsDevice.Viewport = original;
        }
    }
}