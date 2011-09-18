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
    class PrePlaystate : State
    {
        #region members
        Game _game;
        SpriteBatch _spriteBatch;
        SpriteFont _font;
        Texture2D tanque1, tanque2, tanque3, tanque4;
        Texture2D _marco;
        float _anguloRotacion;
        TimeSpan _previousTick, _tickTime, _selectTick, _previousSelectTick;
        bool[] _numMandos;
        Texture2D[] _tanques;
        int[] sel;
        bool[] ready;
        bool preparados = true;
        List<Player> _listaPlayers;
        Playing playingState;
        Map mapa;
        Colision _colision;
        #endregion 

        public PrePlaystate (Game1 game) : base(game)
        {
            _game = game;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _tickTime = TimeSpan.FromSeconds(.5f);
            _selectTick = TimeSpan.FromSeconds(.2f);
        }

        public override void Initialize()
        {
            preparados = false;
            _anguloRotacion = 0f;
            _numMandos = new bool[4] { false, false, false, false };
            ready = new bool[4] { false, false, false, false };
            _tanques = new Texture2D[4];
            _listaPlayers = new List<Player>();
            sel = new int[4];
            playingState = (Playing)Manager.Estados[GameStates.PlayingState];

            _colision = new Colision();

            for (int i = 0; i < 4; i++)
            {
                sel[i] = 0;
            }

            tanque1 = _tanques[sel[0]];
            tanque2 = _tanques[sel[1]];
            tanque3 = _tanques[sel[2]];
            tanque4 = _tanques[sel[3]];
            AsignarControles();
        }

        public override void LoadContent()
        {
            //Fuente de letra
            _font = Content.Load<SpriteFont>("fonts/gameFont");

            //Sprites de tanques
            _tanques[0] = Content.Load<Texture2D>("graphics/Chars/tanque1");
            _tanques[1] = Content.Load<Texture2D>("graphics/Chars/tanque2");
            _tanques[2] = Content.Load<Texture2D>("graphics/Chars/tanque3");
            _tanques[3] = Content.Load<Texture2D>("graphics/Chars/tanque4");

            //Marco de seleccion de tanque
            _marco = Content.Load<Texture2D>("graphics/Gui/Marco");

            //Mapa
            mapa = Content.Load<Map>("map/mapa3");
            _colision.Initialize(mapa, _game);

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _anguloRotacion += 1;
            preparados = true;
            //Seleccion tanques
            for (int i=0; i<4; i++)
            {
                if (!ready[i] && _numMandos[i]) //Si no esta listo y el mando esta conectado
                {
                    if (_listaPlayers[i]._controller.Derecha())
                    {
                        if (gameTime.TotalGameTime - _previousSelectTick > _selectTick)
                        {
                            _previousSelectTick = gameTime.TotalGameTime;
                            if (sel[i] == 0)
                            {
                                sel[i] = 3;
                            }
                            else
                                sel[i]--;
                        }
                    }
                    else if (_listaPlayers[i]._controller.Izquierda())
                    {
                        if (gameTime.TotalGameTime - _previousSelectTick > _selectTick)
                        {
                            _previousSelectTick = gameTime.TotalGameTime;
                            if (sel[i] == 3)
                            {
                                sel[i] = 0;
                            }
                            else
                                sel[i]++;
                        }
                    }
                    else if (_listaPlayers[i]._controller.Start())
                    {
                        if (gameTime.TotalGameTime - _previousSelectTick > _selectTick)
                        {
                            _previousSelectTick = gameTime.TotalGameTime;
                            _previousTick = gameTime.TotalGameTime;
                            _listaPlayers[i].Initialize(_tanques[sel[i]]);
                            ready[i] = true;
                        }
                    }
                }
            }

            for (int i = 0; i < 4 && preparados; i++)
            {
                if ((!ready[i] && _numMandos[i]))
                {
                    preparados = false;
                }
                else if ((ready[i] && _numMandos[i]))
                {
                    preparados = true;
                }
            }


            //Start!
            if ((_listaPlayers[0]._controller.Start() || _listaPlayers[1]._controller.Start()) && preparados)
            {
                if (gameTime.TotalGameTime - _previousTick > _tickTime)
                {
                    _previousTick = gameTime.TotalGameTime;
                    _colision._listaPlayers = _listaPlayers;
                    playingState.MailBox(_listaPlayers, mapa);
                    playingState.Initialize();
                    playingState.LoadContent();
                    Manager._estadoActual = GameStates.PlayingState;
                }
            }
        }

        private void AsignarControles()
        {
            Player p1 = new Player(_game);
            _listaPlayers.Add(p1);
            //Detectar controles conectados;
            _listaPlayers[0]._controller = new Controller("teclado");
            _listaPlayers[0]._colision=_colision;
            _listaPlayers[0]._pos = new Vector2(75, 320);
            _listaPlayers[0]._texturaProyectil = Content.Load<Texture2D>("graphics/Proyectil/Proyectil");
            _listaPlayers[0]._rumble = new Rumble(_listaPlayers[0]._controller);
            _numMandos[0] = true;

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                Player p2 = new Player(_game);
                _listaPlayers.Add(p2);
                _listaPlayers[1]._controller = new Controller("mando1");
                _listaPlayers[1]._colision = _colision;
                _listaPlayers[1]._pos = new Vector2(850, 320);
                _listaPlayers[1]._texturaProyectil = Content.Load<Texture2D>("graphics/Proyectil/Proyectil");
                _listaPlayers[1]._rumble = new Rumble(_listaPlayers[1]._controller);
                _numMandos[1] = true;
            }
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                Player p3 = new Player(_game);
                _listaPlayers.Add(p3);
                _listaPlayers[2]._controller = new Controller("mando2");
                _listaPlayers[2]._colision = _colision;
                _listaPlayers[2]._pos = new Vector2(850, 320);
                _listaPlayers[2]._texturaProyectil = Content.Load<Texture2D>("graphics/Proyectil/Proyectil");
                _listaPlayers[2]._rumble = new Rumble(_listaPlayers[2]._controller);
                _numMandos[2] = true;
            }
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
            {
                Player p4 = new Player(_game);
                _listaPlayers.Add(p4);
                _listaPlayers[3]._controller = new Controller("mando3");
                _listaPlayers[3]._colision = _colision;
                _listaPlayers[3]._pos = new Vector2(850, 320);
                _listaPlayers[3]._texturaProyectil = Content.Load<Texture2D>("graphics/Proyectil/Proyectil");
                _listaPlayers[3]._rumble = new Rumble(_listaPlayers[3]._controller);
                _numMandos[3] = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            _spriteBatch.DrawString(_font, "Seleccion de tanques", new Vector2(500, 0), Color.White);

            if(preparados)
                _spriteBatch.DrawString(_font, "A luchar ...", new Vector2(600, 380), Color.White);

            //Pintar marcos de selección
            if (ready[0])
            {
                _spriteBatch.Draw(_marco, new Vector2(300, 200), null, Color.White, MathHelper.ToRadians(-_anguloRotacion), new Vector2(_tanques[sel[0]].Width / 2, _tanques[sel[0]].Height / 2), 4, SpriteEffects.None, 0);
            }
            if (ready[1])
            {
                _spriteBatch.Draw(_marco, new Vector2(1000, 200), null, Color.White, MathHelper.ToRadians(-_anguloRotacion), new Vector2(_tanques[sel[0]].Width / 2, _tanques[sel[0]].Height / 2), 4, SpriteEffects.None, 0);
            }
            if (ready[2])
            {
                _spriteBatch.Draw(_marco, new Vector2(300, 600), null, Color.White, MathHelper.ToRadians(-_anguloRotacion), new Vector2(_tanques[sel[0]].Width / 2, _tanques[sel[0]].Height / 2), 4, SpriteEffects.None, 0);
            }
            if (ready[3])
            {
                _spriteBatch.Draw(_marco, new Vector2(1000, 600), null, Color.White, MathHelper.ToRadians(-_anguloRotacion), new Vector2(_tanques[sel[0]].Width / 2, _tanques[sel[0]].Height / 2), 4, SpriteEffects.None, 0);
            }

            //Pintar los tanques
            _spriteBatch.Draw(_tanques[sel[0]], new Vector2(300, 200), null, Color.White, MathHelper.ToRadians(_anguloRotacion), new Vector2(_tanques[sel[0]].Width / 2, _tanques[sel[0]].Height / 2), 3, SpriteEffects.None, 0);
            _spriteBatch.Draw(_tanques[sel[1]], new Vector2(1000, 200), null, Color.White, MathHelper.ToRadians(_anguloRotacion), new Vector2(_tanques[sel[1]].Width / 2, _tanques[sel[1]].Height / 2), 3, SpriteEffects.None, 0);
            //_spriteBatch.Draw(_tanques[sel[2]], new Vector2(300, 600), null, Color.White, MathHelper.ToRadians(_anguloRotacion), new Vector2(_tanques[sel[2]].Width / 2, _tanques[sel[2]].Height / 2), 3, SpriteEffects.None, 0);
            //_spriteBatch.Draw(_tanques[sel[3]], new Vector2(1000, 600), null, Color.White, MathHelper.ToRadians(_anguloRotacion), new Vector2(_tanques[sel[3]].Width / 2, _tanques[sel[3]].Height / 2), 3, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_font, "Jugador 3 pulsa Start", new Vector2(200, 600), Color.White);
            _spriteBatch.DrawString(_font, "Jugador 4 pulsa Start", new Vector2(900, 600), Color.White);

            _spriteBatch.End();
        }
    }
}
