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
    public enum MetalToothState
    {
        Running,
        Spawning,
        Fading
    }

    public class Enemy_MetalTooth :Enemy
    {
        #region Constants
        private const string filename_Base = "Enemies/MetalTooth-base";
        private const string filename_BaseLower = "Enemies/MetalTooth-base-lower";
        private const string filename_Blades = "Enemies/MetalTooth-blades";
        private const string filename_Shine = "Enemies/MetalTooth-shine";

        private const float maxSpeed = 2.4f;
        private const float minSpeed = 1.25f;
        private const float accel = 0.026f;
        private const double percentSlowDown = 0.75;

        //private const float fleeSpeed = 5.3f;

        private const double maxFleeDistanceMultiplier = 7.0;
        private const double minFleeDistanceMultiplier = 3.0;

        private const double timePerColorChange = 0.1f;

        private const float radiusMultiplier = 0.87f;

        // Deltas
        private const float delta_Rotation = 0.07f;
        private const float delta_ShineOffsetX = -0f;
        private const float delta_ShineOffsetY = -12f;

        /// <summary>
        /// The number of explosions when I die
        /// </summary>
        private int numberOfExplosions = 6;
        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 10;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 15;

        /// <summary>
        /// The hit point array
        /// </summary>
        static readonly byte[] hitPointArray = new byte[] 
        {
            (byte)128, 
            (byte)160, 
            (byte)192, 
            (byte)224, 
            (byte)255
        };

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 1000;

        /// <summary>
        /// The point value when my shields disintegrate if I were it at perfect beat
        /// </summary>
        private const int myShieldPointValue = 10;
        #endregion

        #region Properties
        #endregion

        #region Fields
        // Textures and related stuff
        private Texture2D texture_Base;
        private Texture2D texture_Blades;
        private Texture2D texture_Shine;
        private Texture2D texture_BaseLower;

        private Vector2 offset_Shine;
        private Vector2 center_Base;
        private Vector2 center_BaseLower;
        private Vector2 center_Blades;
        private Vector2 center_Shine;

        private Color[] myColor;
        private int currentColor;
        private Color shieldColor;

        // Animation stuff
        private float rotation_Blades;
        private float rotation_Eye;
        private double timeSinceSwitch;
        private MetalToothState currentState;
        private Vector2 nextPosition;
        private float travelLength;
        private float totalTravelLength;
        private float speed;

        // Audio stuff
        private AudioManager audio;
        #endregion

        #region Constructor / Init
        public Enemy_MetalTooth() : base() { }

        public Enemy_MetalTooth(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_MetalTooth;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            Initialized = false;
            offset_Shine = new Vector2(delta_ShineOffsetX, delta_ShineOffsetY);

            // Set the RealSize by hand
            RealSize = new Vector2(255, 260);
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
            if (hitPoints == null || (int)hitPoints == 0)
            {
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints * realHitPointMultiplier;
            CurrentHitPoints = (int)hitPoints * realHitPointMultiplier;
            audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        public override String[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_Base,
                filename_Blades,
                filename_Shine,
                filename_BaseLower,
            };
        }

        public void LoadAndInitialize()
        {
            texture_Base = InstanceManager.AssetManager.LoadTexture2D(filename_Base);
            texture_Blades = InstanceManager.AssetManager.LoadTexture2D(filename_Blades);
            texture_Shine = InstanceManager.AssetManager.LoadTexture2D(filename_Shine);
            texture_BaseLower = InstanceManager.AssetManager.LoadTexture2D(filename_BaseLower);

            center_Base = new Vector2(
                texture_Base.Width * 0.5f, texture_Base.Height * 0.5f);
            center_Blades = new Vector2(
                texture_Blades.Width * 0.5f, texture_Blades.Height * 0.5f);
            center_Shine = new Vector2(
                texture_Shine.Width * 0.5f, texture_Shine.Height * 0.5f);
            center_BaseLower = new Vector2(
                texture_BaseLower.Width * 0.5f, texture_BaseLower.Height * 0.5f);

            Radius = center_Blades.X * radiusMultiplier;

            myColor = new Color[]
            {
                GetMyColor(ColorState.Dark),
                GetMyColor(ColorState.Medium),
                GetMyColor(ColorState.Light)
            };

            currentColor = 0;

            SetShieldColor();

            rotation_Blades = 0f;
            rotation_Eye = 0f;
            timeSinceSwitch = 0.0;

            currentState = MetalToothState.Running;
            GetNextPosition(Vector2.Zero);

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private methods
        private void GetNextPosition(Vector2 vector2)
        {
            if (vector2 == Vector2.Zero)
            {
                nextPosition = new Vector2(
                    (float)MWMathHelper.GetRandomInRange(
                        InstanceManager.DefaultViewport.TitleSafeArea.X + center_BaseLower.X,
                        InstanceManager.DefaultViewport.TitleSafeArea.Right - center_BaseLower.X),
                    (float)MWMathHelper.GetRandomInRange(
                        InstanceManager.DefaultViewport.TitleSafeArea.Y + center_BaseLower.Y,
                        InstanceManager.DefaultViewport.TitleSafeArea.Bottom - center_BaseLower.Y));
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

        private void SetShieldColor()
        {
            shieldColor = GetMyColor(ColorState.Dark);
            int alphaIndex = (int)MathHelper.Lerp(0, myColor.Length - 1, ((float)CurrentHitPoints / (float)StartHitPoints));
            shieldColor.A = hitPointArray[alphaIndex];
        }
        #endregion

        #region Public overrides
        public override bool StartOffset()
        {
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }
                rotation_Eye = MWMathHelper.ComputeAngleAgainstX(Vector2.Negate(vToPlayer)) + MathHelper.PiOver2;
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            CurrentHitPoints--;
            if (CurrentHitPoints <= 0)
            {
                MyManager.TriggerPoints(
                    ((PlayerBullet)pobj).MyPlayerIndex,
                    myPointValue + hitPointMultiplier * StartHitPoints,
                    Position);
                Vector2 temp;
                for (int i = 0; i < numberOfExplosions; i++)
                {
                    temp = new Vector2(
                        (float)MWMathHelper.GetRandomInRange(-1.0, 1.0),
                        (float)MWMathHelper.GetRandomInRange(-1.0, 1.0));
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                        Position + temp * center_Base, myColor[currentColor]);
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                        Position + temp * center_Base, Color.WhiteSmoke);
                }
                Alive = false;
                LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                audio.PlayEffect(EffectID.BuzzDeath);
                return false;
            }
            else
            {
                TriggerShieldDisintegration(texture_Shine, myColor[currentColor], Position, 0f);
                MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myShieldPointValue, Position);
                audio.PlayEffect(EffectID.CokeBottle);
                SetShieldColor();
            }
            return true;
        }        
        #endregion

        #region Draw/Update
        public override void Draw(GameTime gameTime)
        {
            // Draw blades
            InstanceManager.RenderSprite.Draw(
                texture_Blades,
                Position,
                center_Blades,
                null,
                Color.White,
                rotation_Blades,
                1f,
                0f);

            // Draw lower base
            InstanceManager.RenderSprite.Draw(
                texture_BaseLower,
                Position,
                center_BaseLower,
                null,
                Color.White,
                0f,
                1f,
                0f);

            // Draw base
            InstanceManager.RenderSprite.Draw(
                texture_Base,
                Position,
                center_Base,
                null,
                myColor[currentColor],
                rotation_Eye,
                1f,
                0f);

            // Draw shine
            InstanceManager.RenderSprite.Draw(
                texture_Shine,
                Position + offset_Shine,
                center_Shine,
                null,
                shieldColor,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.Addititive);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;

            rotation_Blades += delta_Rotation;
            if (rotation_Blades > MathHelper.TwoPi)
                rotation_Blades = 0f;
            else if (rotation_Blades < 0f)
                rotation_Blades = MathHelper.TwoPi;

            switch (currentState)
            {
                case MetalToothState.Fading:
                    Update_Fading();
                    break;
                case MetalToothState.Spawning:
                    Update_Spawning();
                    break;
                default:
                    Update_Running();
                    break;
            }
        }
        #endregion

        #region Private Updates
        private void Update_Running()
        {
            if (travelLength / totalTravelLength < percentSlowDown)
            {
                if (speed < maxSpeed)
                    speed += accel;
            }
            else
            {
                if (speed > minSpeed)
                    speed -= accel;
            }

            Position += speed * Orientation;
            travelLength += speed;
            if (travelLength > totalTravelLength)
            {
                travelLength = 0;
                speed = 0;
                //timer_Thinking = 0;
                currentState = MetalToothState.Fading;
                GetNextPosition(Vector2.Zero);
            }
            timeSinceSwitch = 0.0;
        }

        private void Update_Spawning()
        {
            // Run through the other enemy objects, looking for dead ones

            // If no more dead ones, resume movement, else go back to fade
            //FIXME
            currentColor = 0;
            currentState = MetalToothState.Running;
        }

        private void Update_Fading()
        {
            if (timeSinceSwitch > timePerColorChange)
            {
                timeSinceSwitch = 0.0;
                currentColor++;
                if (currentColor >= myColor.Length)
                {
                    currentColor = myColor.Length - 1;
                    currentState = MetalToothState.Spawning;
                }
            }
        }
        #endregion
    }
}
