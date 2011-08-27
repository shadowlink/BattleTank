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
    public class StateManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        public Dictionary<GameStates, State> Estados;
        public GameStates _estadoActual;
        #endregion
        
        public StateManager(Game game) : base(game)
        {
            Estados = new Dictionary<GameStates, State>();
            _estadoActual = GameStates.PlayingState;
        }

        public override void Initialize()
        {
            State estado;
            if (Estados.TryGetValue(_estadoActual, out estado))
                estado.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            State estado;
            if (Estados.TryGetValue(_estadoActual, out estado))
                estado.LoadContent();
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            State estado;
            if (Estados.TryGetValue(_estadoActual, out estado))
                estado.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            State estado;
            if (Estados.TryGetValue(_estadoActual, out estado))
                estado.Draw(gameTime);
        }
    }
}
