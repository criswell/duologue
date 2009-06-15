#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware.Manager;
using Mimicware.Graphics;
using Mimicware;
using Mimicware.Fx;
// Duologue
using Duologue;
using Duologue.Audio;
using Duologue.State;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    public class CreditsScreenManager : GameScreen, IService
    {
        #region Constants
        private const double timeBackGroundChange = 4.3;
        #endregion

        #region Fields
        private DuologueGame localGame;
        private CreditsScreen creditsScreen;

        private double timer_BackgroundChange;
        private Teletype teletype;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public CreditsScreenManager(Game game)
            : base(game)
        {
            localGame = (DuologueGame)game;
            creditsScreen = new CreditsScreen(game, this);
            creditsScreen.DrawOrder = 4;
            creditsScreen.Enabled = false;
            creditsScreen.Visible = false;
            localGame.Components.Add(creditsScreen);
            timer_BackgroundChange = 0;
            teletype = null;
        }
        protected override void InitializeConstants()
        {
            MyComponents.Add(creditsScreen);

            this.SetEnable(false);
            this.SetVisible(false);
        }
        #endregion

        #region Public
        public override void ScreenEntrance(GameTime gameTime)
        {
            creditsScreen.ResetAll();
            timer_BackgroundChange = 0;
            SetRandomParallax();
            base.ScreenEntrance(gameTime);
        }

        /*
        public override void ScreenExit(GameTime gameTime)
        {
            creditsScreen.ResetAll();
            base.ScreenExit(gameTime);
        }*/

        public void QuitScreen()
        {
            if (teletype == null)
                teletype = ServiceLocator.GetService<Teletype>();
            teletype.FlushEntries();
            LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
            creditsScreen.Enabled = false;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (InstanceManager.InputManager.NewButtonPressed(Buttons.Back) || 
                InstanceManager.InputManager.NewButtonPressed(Buttons.B))
                QuitScreen();

            timer_BackgroundChange += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer_BackgroundChange > timeBackGroundChange)
            {
                timer_BackgroundChange = 0;
                LocalInstanceManager.Background.NextBackground();
                SetRandomParallax();
            }
            base.Update(gameTime);
        }

        private void SetRandomParallax()
        {
            ParallaxElement pe = new ParallaxElement();
            pe.Intensity = 1;
            pe.Speed = 0.5f;
            pe.Tint = new Color(
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255));

            LocalInstanceManager.Background.SetParallaxElement(pe, true);
            pe.Tint = new Color(
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255));
            LocalInstanceManager.Background.SetParallaxElement(pe, false);
        }
        #endregion
    }
}
