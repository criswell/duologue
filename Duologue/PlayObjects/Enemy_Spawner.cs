#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
// Duologue
using Duologue.Audio;
using Duologue.State;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    public enum SpawnerState
    {
        Moving,
        Fleeing,
        FlareUp,
        FlareDown,
        Scanning
    }
    public class Enemy_Spawner : Enemy
    {
        #region Constants
        private const string filename_Flare = "Enemies/spawner/flare";
        private const string filename_Base = "Enemies/spawner/spawner-base";
        private const string filename_Lights = "Enemies/spawner/spawner-lights{0}";

        private const int numOfLightLevels = 3;

        private const float scale_InnerRing = 0.68f;

        private const float delta_InnerRingNormal = MathHelper.PiOver4 * -0.034f;
        private const float delta_InnerRingFlee = MathHelper.PiOver4 * 0.09f;
        private const float delta_OuterRingNormal = MathHelper.PiOver4 * 0.02f;
        private const float delta_OuterRingFlee = MathHelper.PiOver4 * -0.05f;

        private const float maxSpeed = 2.4f;
        private const float accel = 0.026f;

        private const float fleeSpeed = 5.3f;

        private const double maxFleeDistanceMultiplier = 7.0;
        private const double minFleeDistanceMultiplier = 3.0;
        private const float radiusMultiplier = 0.78f;

        private const float minFlareSize = 0.1f;
        private const float maxFlareSzie = 0.89f;
        private const float delta_FlareSize = 0.09f;

        private const double timeBetweenColorSwitches = 0.33;
        private const double timeToThink = 0.202;
        private const double timeFlareUp = 0.25;
        private const double timeFlareDown = 0.1;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 100;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 50;
        #endregion

        #region Fields
        private Texture2D texture_Flare;
        private Texture2D texture_Base;
        private Texture2D[] texture_Lights;

        private Vector2 center_Flare;
        private Vector2 center_Base;

        private float rotation_InnerRing;
        private float rotation_OuterRing;

        private SpawnerState currentState;

        private Vector2 nextPosition;
        private float travelLength;
        private float totalTravelLength;
        private float speed;
        private float size_Flare;

        private Color color_Current;
        private Color color_Base;
        private int[] colorIndex_Lights;
        private Color[] color_PossibleLightColors;

        private double timer_ColorSwitchCountdown;
        private double timer_Thinking;
        private double timer_Flare;
        private TypesOfPlayObjects[] possibleNigglets;
        private List<TypesOfPlayObjects> weightedNiggletList;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_Spawner() : base() { }

        public Enemy_Spawner(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Spawner;
            MajorType = MajorPlayObjectType.Enemy;

            // Set the RealSize by hand
            RealSize = new Vector2(110, 110);
            Initialized = false;
            Alive = false;
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            Position = startPos;
            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null)
            {
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints * realHitPointMultiplier;
            //audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            texture_Flare = InstanceManager.AssetManager.LoadTexture2D(filename_Flare);
            texture_Base = InstanceManager.AssetManager.LoadTexture2D(filename_Base);
            texture_Lights = new Texture2D[numOfLightLevels];

            for (int i = 0; i < numOfLightLevels; i++)
            {
                texture_Lights[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Lights, (i + 1).ToString()));
            }

            center_Base = new Vector2(
                texture_Base.Width / 2f, texture_Base.Height / 2f);
            center_Flare = new Vector2(
                texture_Flare.Width / 2f, texture_Flare.Height / 2f);

            rotation_InnerRing = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);
            rotation_OuterRing = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);

            Radius = RealSize.Length() * radiusMultiplier;

            currentState = SpawnerState.Moving;
            speed = 0f;
            size_Flare = minFlareSize;

            color_Current = GetMyColor(ColorState.Light);
            color_Base = Color.Silver;
            colorIndex_Lights = new int[numOfLightLevels];
            color_PossibleLightColors = new Color[]{
                Color.MediumAquamarine,
                Color.MediumVioletRed,
                Color.OrangeRed,
                Color.GreenYellow,
                Color.DeepPink,
                Color.Yellow,
                Color.Violet,
                Color.Turquoise,
                Color.SteelBlue,
            };

            SetLightColors();
            timer_ColorSwitchCountdown = 0;

            GetNextPosition(Vector2.Zero);

            // To give more weight to specific types, list
            // them multiple times
            possibleNigglets = new TypesOfPlayObjects[]
            {
                TypesOfPlayObjects.Enemy_Buzzsaw,
                TypesOfPlayObjects.Enemy_Ember,
                TypesOfPlayObjects.Enemy_Gloop,
                TypesOfPlayObjects.Enemy_Maggot,
                TypesOfPlayObjects.Enemy_Mirthworm,
                TypesOfPlayObjects.Enemy_Mirthworm,
                TypesOfPlayObjects.Enemy_Mirthworm,
                TypesOfPlayObjects.Enemy_Maggot,
                TypesOfPlayObjects.Enemy_Roggles,
                TypesOfPlayObjects.Enemy_Spitter,
                TypesOfPlayObjects.Enemy_Wiggles,
            };

            weightedNiggletList = new List<TypesOfPlayObjects>();

            Alive = true;
            Initialized = true;
        }

        public override String[] GetFilenames()
        {
            String[] filenames = new String[numOfLightLevels + 2];

            for (int i = 0; i < numOfLightLevels; i++)
            {
                filenames[i] = String.Format(filename_Lights, (i+1).ToString());
            }

            filenames[filenames.Length - 2] = filename_Base;
            filenames[filenames.Length - 1] = filename_Flare;

            return filenames;
        }
        #endregion

        #region Private methods
        private void GetNextPosition(Vector2 vector2)
        {
            if (vector2 == Vector2.Zero)
            {
                nextPosition = new Vector2(
                    (float)MWMathHelper.GetRandomInRange(
                        InstanceManager.DefaultViewport.TitleSafeArea.X,
                        InstanceManager.DefaultViewport.TitleSafeArea.Right),
                    (float)MWMathHelper.GetRandomInRange(
                        InstanceManager.DefaultViewport.TitleSafeArea.Y,
                        InstanceManager.DefaultViewport.TitleSafeArea.Bottom));
            }
            else
            {
                vector2.Normalize();
                float distance = (float)MWMathHelper.GetRandomInRange(
                    minFleeDistanceMultiplier * Radius, maxFleeDistanceMultiplier * Radius);

                nextPosition = Position + distance * vector2;
            }

            Orientation = nextPosition - Position;
            totalTravelLength = Orientation.Length();
            Orientation.Normalize();
            travelLength = 0;
        }

        private void SetLightColors()
        {
            for (int i = 0; i < numOfLightLevels; i++)
            {
                colorIndex_Lights[i] = MWMathHelper.GetRandomInRange(0, color_PossibleLightColors.Length);
            }
        }

        /*
            __________________________________________________________________
            __________________________________________________________________
            ______________________________,..ucz..,___________________________
            ________________________ _,@{5E3[?}P~s[3X2,_______________________
            _____________________  ,{51R]F!?!h;:1c:7!c!P1.   _________________
            ___________________  ,{Et7!5lJ:.: !ciI.c;5}L:31.  ________________
            ___________________ :F:KrL.::.!=!:=zr;:3;CiH.::5i_________________
            _________________  ;1:]h;zzzzC!c;utL.nz5iuj?j:::!U  ______________
            _________________ .H.JlJ[VTC{ClD{2CU7C}P+P.;{L:::3. ______________
            _________________ :L:E)=l;}L!5[5}Z}F(C)/]:.!.Q.::J[.______________
            _________________ !L!E \.7;;}FF3TE[k;7!~!;.:;U.:::)L______________
            _________________  hiE    :U.F!:::!?(7:c(3ihtQ]c;J).______________
            _________________ :Utn:. \:L:ztzzn..:L;ni::;|hzCjJ( ______________
            _________________ :H\:). ==UtOtttntht.:Y.;=Utb!V;H) ______________
            _________________ 'KjK]J[h.7" :==c:.::tyzU{Ht:}Q$F` ______________
            _________________      G[sjJ.={3[F{L:[{2{U1H.@7   ________________
            ___________________ _,.3}H73FE}EFF+F1:::jH}B]F  __________________
            _________________  /1Hlp!F!h;;:::::::::J1E1:]R_ __________________
            _________________ :5!Ut5t:;.:.:ti..z;.!EiQ(:(F:c. ________________
            ________________.x\=:^1X!::z(hthi==CjDjh1UiJ(:i !nz.______________
            _________ __.ntP!.  -:!k.U[C!:!.:;:=tJ[H[UyG(O!\.C) !ft.,_ _______
            _______ _y1P7 :       .=TQ{P.j:.:.{3!L}g{R}2[FT3[L]J[Z77!Rj_______
            _______,1F):  !;.=7    :.g{E]H]U}n[3}HEF}PyE[../1FTFl-:   !L".__ _
            ___  gcEcu+<cpyL.pi::==FF::!}3l3iE!ItEiH1b1niE5ciFyuyscuyE=E(=(5_ 
            ___,;P|H("|JtL(Lib:b!:jJtYjb1HtcfU1Ht5tFtc]u.QiE)L(5ik=U)u1Utht.!L
            _  z':fOtU1JtDtUeU1Ojd1JthtO1dt:.CtX.x;Jtht43hcJzV)Q1UtL.D=U V(.tU

                                T H A T ' S   R A C I S T !
        */
        private void SpawnNigglet(GameTime gameTime)
        {
            weightedNiggletList.Clear();
            weightedNiggletList.AddRange(possibleNigglets);

            // Now, run through the living critters and see if we have any sorts of modifiers
            for (int i = 0; i < LocalInstanceManager.CurrentNumberEnemies; i++)
            {
                if (LocalInstanceManager.Enemies[i].Alive)
                {
                    switch (LocalInstanceManager.Enemies[i].MyType)
                    {
                        case TypesOfPlayObjects.Enemy_KingGloop:
                            for (int t = 0; t < weightedNiggletList.Count; t++)
                            {
                                weightedNiggletList.Add(TypesOfPlayObjects.Enemy_Gloop);
                            }
                            break;
                        case TypesOfPlayObjects.Enemy_Pyre:
                            for (int t = 0; t < weightedNiggletList.Count; t++)
                            {
                                weightedNiggletList.Add(TypesOfPlayObjects.Enemy_Ember);
                            }
                            break;
                        case TypesOfPlayObjects.Enemy_UncleanRot:
                            for (int t = 0; t < weightedNiggletList.Count; t++)
                            {
                                weightedNiggletList.Add(TypesOfPlayObjects.Enemy_StaticGloop);
                            }
                            break;
                        default:
                            // Nada
                            break;
                    }
                }
            }

            // Figure out what to spawn, and where to spawn it in the list
            TypesOfPlayObjects po = weightedNiggletList[
                MWMathHelper.GetRandomInRange(0, weightedNiggletList.Count)];

            int place = 0;
            bool added = false;
            while (place < LocalInstanceManager.CurrentNumberEnemies)
            {
                if (LocalInstanceManager.Enemies[place] == null)
                {
                    SpawnThisNigglet(po, place);
                    added = true;
                    break;
                }
                else if (!LocalInstanceManager.Enemies[place].Alive)
                {
                    SpawnThisNigglet(po, place);
                    added = true;
                    break;
                }

                place++;
            }
            if (added && place > LocalInstanceManager.CurrentNumberEnemies)
                LocalInstanceManager.CurrentNumberEnemies = place;

        }

        private void SpawnThisNigglet(TypesOfPlayObjects po, int place)
        {
            switch (po)
            {
                case TypesOfPlayObjects.Enemy_Buzzsaw:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Buzzsaw(MyManager);
                    break;
                case TypesOfPlayObjects.Enemy_Ember:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Ember(MyManager);
                    break;
                case TypesOfPlayObjects.Enemy_Gloop:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Gloop(MyManager);
                    break;
                case TypesOfPlayObjects.Enemy_Maggot:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Maggot(MyManager);
                    break;
                case TypesOfPlayObjects.Enemy_Mirthworm:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Mirthworm(MyManager);
                    break;
                case TypesOfPlayObjects.Enemy_Roggles:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Roggles(MyManager);
                    break;
                case TypesOfPlayObjects.Enemy_Spitter:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Spitter(MyManager);
                    break;
                case TypesOfPlayObjects.Enemy_StaticGloop:
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_StaticGloop(MyManager);
                    break;
                default:
                    // Default to wiggles
                    LocalInstanceManager.Enemies[place] =
                        new Enemy_Wiggles(MyManager);
                    break;
            }
            LocalInstanceManager.Enemies[place].Initialize(
                Position,
                Orientation,
                ColorState,
                ColorPolarity,
                StartHitPoints);
        }
        #endregion

        #region Public overrides
        public override bool StartOffset()
        {
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            return true;
        }

        public override bool ApplyOffset()
        {
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            return true;
        }
        #endregion

        #region Private Update Methods
        private void Update_Rotate()
        {
            if (currentState == SpawnerState.Fleeing)
            {
                rotation_InnerRing += delta_InnerRingFlee;
                rotation_OuterRing += delta_OuterRingFlee;
            }
            else
            {
                rotation_InnerRing += delta_InnerRingNormal;
                rotation_OuterRing += delta_OuterRingNormal;
            }

            if (rotation_InnerRing > MathHelper.TwoPi)
                rotation_InnerRing = 0;
            else if (rotation_InnerRing < 0)
                rotation_InnerRing = MathHelper.TwoPi;

            if (rotation_OuterRing > MathHelper.TwoPi)
                rotation_OuterRing = 0;
            else if (rotation_OuterRing < 0)
                rotation_OuterRing = MathHelper.TwoPi;
        }

        private void Update_Moving(GameTime gameTime)
        {
            if (speed < maxSpeed)
                speed += accel;

            Position += speed * Orientation;
            travelLength += speed;
            if (travelLength > totalTravelLength)
            {
                travelLength = 0;
                speed = 0;
                timer_Thinking = 0;
                currentState = SpawnerState.Scanning;
                GetNextPosition(Vector2.Zero);
            }
        }

        private void Update_Scanning(GameTime gameTime)
        {
            timer_Thinking += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer_Thinking > timeToThink)
            {
                timer_Thinking = 0;
                timer_Flare = 0;
                currentState = SpawnerState.FlareUp;
            }
        }

        private void Update_Fleeing(GameTime gameTime)
        {
            Position += fleeSpeed * Orientation;
            travelLength += fleeSpeed;
            if (travelLength > totalTravelLength)
            {
                travelLength = 0;
                speed = 0;
                GetNextPosition(Vector2.Zero);
                currentState = SpawnerState.Moving;
            }
        }

        private void Update_FlareDown(GameTime gameTime)
        {
            size_Flare = (maxFlareSzie - minFlareSize) * (float)(1.0 - timer_Flare / timeFlareUp) + minFlareSize;
            timer_Flare += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer_Flare > timeFlareDown)
            {
                timer_Flare = 0;
                currentState = SpawnerState.Moving;
                if (MWMathHelper.CoinToss())
                    ColorPolarity = ColorPolarity.Positive;
                else
                    ColorPolarity = ColorPolarity.Negative;
                color_Current = GetMyColor(ColorState.Light);
            }
        }

        private void Update_FlareUp(GameTime gameTime)
        {
            size_Flare = (maxFlareSzie - minFlareSize) * (float)(timer_Flare / timeFlareUp) + minFlareSize;
            timer_Flare += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer_Flare > timeFlareUp)
            {
                timer_Flare = 0;
                SpawnNigglet(gameTime);
                currentState = SpawnerState.FlareDown;
            }
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            if (currentState == SpawnerState.FlareUp || currentState == SpawnerState.FlareDown)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Flare,
                    Position,
                    center_Flare,
                    null,
                    color_Current,
                    0f,
                    size_Flare,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
            }

            // Draw base of the ship
            InstanceManager.RenderSprite.Draw(
                texture_Base,
                Position,
                center_Base,
                null,
                color_Base,
                rotation_OuterRing,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Base,
                Position,
                center_Base,
                null,
                color_Base,
                rotation_InnerRing,
                scale_InnerRing,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Draw the lights;
            for (int i = 0; i < numOfLightLevels; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Lights[i],
                    Position,
                    center_Base,
                    null,
                    color_PossibleLightColors[colorIndex_Lights[i]],
                    rotation_OuterRing,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                InstanceManager.RenderSprite.Draw(
                    texture_Lights[i],
                    Position,
                    center_Base,
                    null,
                    color_PossibleLightColors[colorIndex_Lights[i]],
                    rotation_InnerRing,
                    scale_InnerRing,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }
        }

        public override void Update(GameTime gameTime)
        {
            timer_ColorSwitchCountdown += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer_ColorSwitchCountdown > timeBetweenColorSwitches)
            {
                timer_ColorSwitchCountdown = 0;
                SetLightColors();
            }

            Update_Rotate();

            switch (currentState)
            {
                case SpawnerState.FlareUp:
                    Update_FlareUp(gameTime);
                    break;
                case SpawnerState.FlareDown:
                    Update_FlareDown(gameTime);
                    break;
                case SpawnerState.Fleeing:
                    Update_Fleeing(gameTime);
                    break;
                case SpawnerState.Scanning:
                    Update_Scanning(gameTime);
                    break;
                default:
                    // Plain old moving
                    Update_Moving(gameTime);
                    break;
            }
        }
        #endregion
    }
}
