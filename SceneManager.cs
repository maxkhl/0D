using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ODBank
{
    /// <summary>
    /// Used to manage scenes
    /// </summary>
    class SceneManager
    {
        /// <summary>
        /// Currently running scene. Set method is not thread safe!
        /// </summary>
        public Scene ActiveScene 
        { 
            get
            {
                return _ActiveScene;
            }
            set
            {
                //Dispose old scene
                if(_ActiveScene != null)
                    _ActiveScene.Dispose();

                _ActiveScene = value;

                //Load new scene
                if (_ActiveScene != null)
                    _ActiveScene.LoadContent();
            }
        }
        private Scene _ActiveScene;

        /// <summary>
        /// Attached game
        /// </summary>
        public static MainGame Game { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Game">Attached game</param>
        public SceneManager(MainGame HWGame)
        {
            Game = HWGame;
        }

        /// <summary>
        /// The currently active scene will be updated in here.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if (ActiveScene != null)
                ActiveScene.Update(gameTime);
        }

        /// <summary>
        /// The currently active scene will be drawn in here.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {            
            if (ActiveScene != null)
                ActiveScene.Draw(gameTime);
        }
    }
}
