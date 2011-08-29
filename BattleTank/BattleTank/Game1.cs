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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Members
        StateManager _stateManager;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1360;
            this.graphics.PreferredBackBufferHeight = 760;
            this.graphics.IsFullScreen = false;
            this.IsMouseVisible = false;
            Window.Title = "BattleTank v0.5.1";
            Content.RootDirectory = "Content";

            _stateManager = new StateManager(this);
            Components.Add(_stateManager);
            Components.Add(new GamerServicesComponent(this));

            Services.AddService(typeof(ContentManager), Content);
            Services.AddService(typeof(StateManager), _stateManager);
            Services.AddService(typeof(GraphicsDeviceManager), graphics);


        }

        protected override void Initialize()
        {
            _stateManager.Estados.Add(GameStates.PlayingState, new Playing(this));
            _stateManager.Estados.Add(GameStates.MainMenu, new MainMenu(this));

            _stateManager._estadoActual = GameStates.MainMenu;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Brown);           
            base.Draw(gameTime);
        }
    }
}
