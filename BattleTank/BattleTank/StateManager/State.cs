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
    public abstract class State
    {
        #region Members
        StateManager manager;           //Manager de estados
        ContentManager content;         //ContentManager para los recursos del juego
        GraphicsDeviceManager graphics; //Graphics
        Game game;
        #endregion

        public Game Game
        {
            get { return game; }
        }

        public StateManager Manager
        {
            get { return manager; }
        }

        public ContentManager Content
        {
            get { return content; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return graphics.GraphicsDevice; }
        }

        public GraphicsDeviceManager GraphicsManager
        {
            get { return graphics; }
        }

        public State(Game game)
        {
            this.game = game;
            manager = game.Services.GetService(typeof(StateManager)) as StateManager;
            content = game.Services.GetService(typeof(ContentManager)) as ContentManager;
            graphics = game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
        }

        public abstract void Initialize();
        public abstract void LoadContent();
        /// Actualiza la logica del estado actual del juego
        public abstract void Update(GameTime gameTime);

        /// Pinta el estado en el juego
        public abstract void Draw(GameTime gameTime);
    }
}
