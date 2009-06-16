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
using Duologue.State;
using Duologue.Screens;
using Duologue.Audio;
#endregion

namespace Duologue.PlayObjects
{
    public enum RotState
    {
        Steady,
        FadeIn,
        FullOn,
        FadeOut,
        ScreamIn,
        RollOutTongue,
        Scream,
        RollInTongue,
        ScreamOut
    }

    public class Enemy_UncleanRot : Enemy
    {
        #region Constants
        // Filenames
        private const string filename_Base = "Enemies/unclean_rot/base-{0}";
        private const string filename_Outline = "Enemies/unclean_rot/outline_{0}";
        private const string filename_OutlineTongue = "Enemies/unclean_rot/outline-tongue-{0}";
        private const string filename_Skullcap = "Enemies/unclean_rot/skullcap-{0}";
        private const string filename_Static = "Enemies/unclean_rot/static-{0:D2}";

        private const string filename_Scream = "Audio/UncleanRot/scream";
        private const string filename_FaceOn = "Audio/UncleanRot/face_on";
        private const string filename_Explosion = "Audio/PlayerEffects/multiple-explosions-short";

        private const int numFrames_Body = 9;
        private const int numFrames_Tongue = 6;
        private const int numFrames_Static = 10;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 5;

        /// <summary>
        /// The time per frame for the static animation in steady state
        /// </summary>
        private const double deltaTime_StaticSteady = 0.1;

        /// <summary>
        /// The time per frame for the static animation when screaming
        /// </summary>
        private const double deltaTime_StaticScream = 0.01;

        /// <summary>
        /// The time it takes to fade in
        /// </summary>
        private const double deltaTime_FadeIn = 0.6;

        /// <summary>
        /// The time it takes to fade out
        /// </summary>
        private const double deltaTime_FadeOut = 0.6;

        /// <summary>
        /// The time it takes to scream in
        /// </summary>
        private const double deltaTime_ScreamIn = 0.25 / (double)numFrames_Body;

        /// <summary>
        /// The time it takes to scream out
        /// </summary>
        private const double deltaTime_ScreamOut = 0.25 / (double)numFrames_Body;

        /// <summary>
        /// The time the mob remains in a steady state
        /// </summary>
        private const double deltaTime_Steady = 2.0;

        /// <summary>
        /// The time the mob remains full on
        /// </summary>
        private const double deltaTime_FullOn = 0.5;

        /// <summary>
        /// The total time it takes to roll out the tongue
        /// </summary>
        private const double deltaTime_TongueRollOut = 0.4 / (double)numFrames_Tongue;

        /// <summary>
        /// The total time it takes to roll in the tongue
        /// </summary>
        private const double deltaTime_TongueRollIn = 0.3 / (double)numFrames_Tongue;

        /// <summary>
        /// The time per frame for a tongue roll
        /// </summary>
        private const double deltaTime_TongueRoll = 0.25;

        /// <summary>
        /// The number of tongue rolls per scream. This basically means that the total time screaming
        /// will be deltaTime_TongueRoll * numberOfTongueRolls
        /// </summary>
        private const int totalNumberOfTongueRolls = 5;

        /// <summary>
        /// This is the number of times we fade before we start a scream
        /// </summary>
        private const int totalNumberOfTimesToFade = 3;

        /// <summary>
        /// The defined radius of the mob
        /// </summary>
        private const float definedRadius = 60f;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 1000;

        /// <summary>
        /// The point value when my shields disintegrate if I were it at perfect beat
        /// </summary>
        private const int myShieldPointValue = 10;

        /// <summary>
        /// Volume settings
        /// </summary>
        private const float volume_FaceOn = 0.4f;
        private const float volume_Scream = 0.9f;
        private const float volume_Explosions = 0.97f;

        #region Forces/Attractions/Repulsions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 2f;

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.5f;

        /// <summary>
        /// The repulsion from the player if the player gets too close
        /// </summary>
        private const float playerRepluse = 3.5f;

        /// <summary>
        /// Min number of how many of our radius size away we're comfortable with the player
        /// </summary>
        private const float minPlayerComfortRadiusMultiplier = 5f;

        /// <summary>
        /// Max number of our radius we're comfortable with the player
        /// </summary>
        private const float maxPlayerComfortRadiusMultiplier = 5.5f;
        #endregion
        #endregion

        #region Fields
        private Texture2D[] texture_Base;
        private Texture2D[] texture_Outline;
        private Texture2D[] texture_OutlineTongue;
        private Texture2D[] texture_Skullcap;
        private Texture2D[] texture_Static;

        private SoundEffect sfx_Scream;
        private SoundEffect sfx_FaceOn;
        private SoundEffectInstance sfxi_Scream;
        private SoundEffectInstance sfxi_FaceOn;
        private SoundEffect sfx_Explode;

        private Color[] color_Steady;
        private Color[] color_CurrentColors;
        private Color[] color_Static;
        private Color color_Shield;
        private int currentStaticColor;

        private int color_ScreamBase;
        private int color_ScreamSkullcap;

        private Vector2 center_Body;
        private Vector2 center_Static;

        private int currentFrame_Body;
        private int currentFrame_Static;
        private int currentFrame_Tongue;

        private RotState currentState;

        private double timeSinceLastSwitch;
        private double timeSinceLastStaticFrame;
        private int numberOfTimesFaded;
        private int numberOfTongueRolls;

        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private PlayObject nearestPlayerObject;

        private bool isFleeing;

        private AudioManager audio;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy_UncleanRot() : base() { }

        public Enemy_UncleanRot(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_UncleanRot;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            RealSize = new Vector2(300, 280);
            Initialized = false;
            Alive = false;
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints,
            double spawnDelay)
        {
            Position = startPos;
            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            SpawnTimeDelay = spawnDelay;
            SpawnTimer = 0;
            if (hitPoints == null || hitPoints == 0)
            {
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints * realHitPointMultiplier;
            CurrentHitPoints = (int)hitPoints * realHitPointMultiplier;
            audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            texture_Base = new Texture2D[numFrames_Body];
            texture_Outline = new Texture2D[numFrames_Body];
            texture_Skullcap = new Texture2D[numFrames_Body];
            texture_OutlineTongue = new Texture2D[numFrames_Tongue];
            texture_Static = new Texture2D[numFrames_Static];
            sfx_Scream = InstanceManager.AssetManager.LoadSoundEffect(filename_Scream);
            sfx_FaceOn = InstanceManager.AssetManager.LoadSoundEffect(filename_FaceOn);
            sfx_Explode = InstanceManager.AssetManager.LoadSoundEffect(filename_Explosion);

            for (int i = 0; i < numFrames_Body; i++)
            {
                texture_Base[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Base, (i + 1).ToString()));
                texture_Outline[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Outline, (i + 1).ToString()));
                texture_Skullcap[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Skullcap, (i + 1).ToString()));
            }

            for (int i = 0; i < numFrames_Tongue; i++)
            {
                texture_OutlineTongue[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_OutlineTongue, (i + 1).ToString()));
            }

            center_Body = new Vector2(
                80f, texture_Base[0].Height / 2f);

            for (int i = 0; i < numFrames_Static; i++)
            {
                texture_Static[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Static, (i + 1)));
            }

            center_Static = new Vector2(
                texture_Static[0].Width / 2f, texture_Static[0].Height / 2f);

            // Init the colors
            color_CurrentColors = new Color[ColorState.NumberColorsPerPolarity];
            SetCurrentColors();

            color_Steady = new Color[2];
            color_Steady[0] = Color.Silver;
            color_Steady[1] = Color.LightSlateGray;

            color_Static = new Color[4];
            color_Static[0] = new Color(Color.Ivory, 200);
            color_Static[1] = new Color(Color.White, 200);
            color_Static[2] = new Color(Color.Thistle, 200);
            color_Static[3] = new Color(Color.Silver, 200);
            currentStaticColor = 0;

            color_Shield = new Color(Color.White, 128);

            color_ScreamBase = ColorState.Light;
            color_ScreamSkullcap = ColorState.Dark;

            // Init the basic variables
            Radius = definedRadius;
            currentFrame_Body = 0;
            currentFrame_Static = 0;
            currentFrame_Tongue = 0;
            currentState = RotState.Steady;
            timeSinceLastSwitch = 0;
            timeSinceLastStaticFrame = 0;
            numberOfTimesFaded = 0;
            numberOfTongueRolls = 0;
            isFleeing = false;

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Public overrides
        public override string[] GetTextureFilenames()
        {
            String[] filenames = new String[3*numFrames_Body + numFrames_Static + numFrames_Tongue];

            int t = 0;

            for (int i = 0; i < numFrames_Body; i++)
            {
                filenames[t] = String.Format(filename_Base, (i + 1).ToString());
                t++;
                filenames[t] = String.Format(filename_Outline, (i + 1).ToString());
                t++;
                filenames[t] = String.Format(filename_Skullcap, (i + 1).ToString());
                t++;
            }

            for (int i = 0; i < numFrames_Tongue; i++)
            {
                filenames[t] = String.Format(filename_OutlineTongue, (i + 1).ToString());
                t++;
            }

            for (int i = 0; i < numFrames_Static; i++)
            {
                filenames[t] = String.Format(filename_Static, (i + 1));
                t++;
            }

            return filenames;
        }

        public override string[] GetSFXFilenames()
        {
            return new String[]
            {
                filename_Scream,
                filename_FaceOn,
                filename_Explosion
            };
        }
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();
                if (len < nearestPlayerRadius)
                {
                    nearestPlayerRadius = len;
                    nearestPlayer = vToPlayer;
                    nearestPlayerObject = pobj;
                }
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                // Beam handling
                if (((Player)pobj).IsInBeam(this) == -1 && currentState != RotState.Steady)
                {
                    LocalInstanceManager.Steam.AddParticles(Position, GetMyColor());
                }
            }
            else if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                // Enemy
                Vector2 vToEnemy = pobj.Position - this.Position;
                float len = vToEnemy.Length();
                if (len < this.Radius + pobj.Radius)
                {
                    // Too close, BTFO
                    if (len == 0f)
                    {
                        // Well, bah, we're on top of each other!
                        vToEnemy = new Vector2(
                            (float)InstanceManager.Random.NextDouble() - 0.5f,
                            (float)InstanceManager.Random.NextDouble() - 0.5f);
                    }
                    vToEnemy = Vector2.Negate(vToEnemy);
                    vToEnemy.Normalize();
                    offset += standardEnemyRepulse * vToEnemy;
                }

                return true;
            }
            else
            {
                // Other

                return true;
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            // First, apply the player offset
            if (nearestPlayer.Length() > 0f)
            {
                // FIXME would be nice if this was more of a "turn" than a sudden jarring switch
                Orientation = Vector2.Negate(nearestPlayer);
                if (!(nearestPlayerRadius > minPlayerComfortRadiusMultiplier * Radius &&
                    nearestPlayerRadius < maxPlayerComfortRadiusMultiplier * Radius))
                {
                    // Default to attract, only repulse if too close
                    float modifier = playerAttract;
                    isFleeing = false;
                    if (nearestPlayerRadius < minPlayerComfortRadiusMultiplier * Radius)
                    {
                        // Too close, BTFO
                        modifier = playerRepluse;
                        isFleeing = true;
                    }

                    nearestPlayer += new Vector2(nearestPlayer.Y, -nearestPlayer.X);
                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += modifier * nearestPlayer;
                }
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
            }

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet && currentState != RotState.Steady)
            {
                CurrentHitPoints--;
                if (CurrentHitPoints <= 0)
                {
                    try
                    {
                        sfxi_FaceOn.Stop();
                    }
                    catch { }
                    
                    try
                    {
                        sfxi_Scream.Stop();
                    }
                    catch { }

                    sfx_Explode.Play(volume_Explosions);
                    for (int i = 0; i < 4; i++)
                    {
                        LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                            Position + (float)MWMathHelper.GetRandomInRange(-1.0, 1.0) * center_Static,
                            color_CurrentColors[MWMathHelper.GetRandomInRange(0, color_CurrentColors.Length)]);
                    }
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue, Position);
                    Alive = false;
                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    //audio.soundEffects.PlayEffect(EffectID.BuzzDeath);
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(texture_Static[currentFrame_Static], color_Shield, Position, 0f);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myShieldPointValue, Position);
                    //audio.soundEffects.PlayEffect(EffectID.CokeBottle);
                    audio.PlayEffect(EffectID.CokeBottle);
                    return true;
                }
            }
            return true;
        }
        #endregion

        #region Private methods
        private void SetCurrentColors()
        {
            color_CurrentColors[ColorState.Light] = GetMyColor(ColorState.Light);
            color_CurrentColors[ColorState.Medium] = GetMyColor(ColorState.Medium);
            color_CurrentColors[ColorState.Dark] = GetMyColor(ColorState.Dark);
        }
        #endregion

        #region Private Draw methods
        private void Draw_NormalFace(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Outline[currentFrame_Body],
                Position,
                center_Body,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_Steady(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Base[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_Steady[0],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Skullcap[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_Steady[1],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_FadeIn(GameTime gameTime, double deltaTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Base[currentFrame_Body],
                Position,
                center_Body,
                null,
                new Color(color_CurrentColors[ColorState.Medium],
                    (float)(timeSinceLastSwitch/deltaTime)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Skullcap[currentFrame_Body],
                Position,
                center_Body,
                null,
                new Color(color_CurrentColors[ColorState.Dark],
                    (float)(timeSinceLastSwitch / deltaTime)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_FadeOut(GameTime gameTime, double deltaTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Base[currentFrame_Body],
                Position,
                center_Body,
                null,
                new Color(color_CurrentColors[ColorState.Medium],
                    1f - (float)(timeSinceLastSwitch / deltaTime)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Skullcap[currentFrame_Body],
                Position,
                center_Body,
                null,
                new Color(color_CurrentColors[ColorState.Dark],
                    1f - (float)(timeSinceLastSwitch / deltaTime)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_FullOn(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Base[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_CurrentColors[ColorState.Medium],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Skullcap[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_CurrentColors[ColorState.Dark],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_Scream(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Base[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_CurrentColors[color_ScreamBase],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Skullcap[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_CurrentColors[color_ScreamSkullcap],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_OutlineTongue[currentFrame_Tongue],
                Position,
                center_Body,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }
        #endregion

        #region Private Update methods
        private void Update_FadeIn()
        {
            currentFrame_Body = 0;
            if (timeSinceLastSwitch >= deltaTime_FadeIn)
            {
                timeSinceLastSwitch = 0;
                currentState = RotState.FullOn;
            }
        }

        private void Update_FullOn()
        {
            currentFrame_Body = 0;
            if (timeSinceLastSwitch >= deltaTime_FullOn)
            {
                timeSinceLastSwitch = 0;
                currentState = RotState.FadeOut;
            }
        }

        private void Update_FadeOut()
        {
            currentFrame_Body = 0;
            if (timeSinceLastSwitch >= deltaTime_FadeOut)
            {
                timeSinceLastSwitch = 0;
                numberOfTimesFaded++;
                currentState = RotState.Steady;
            }
        }

        private void Update_ScreamIn()
        {
            if (timeSinceLastSwitch >= deltaTime_ScreamIn)
            {
                timeSinceLastSwitch = 0;
                currentFrame_Body++;
                if (currentFrame_Body == numFrames_Body)
                {
                    currentState = RotState.RollOutTongue;
                    currentFrame_Tongue = 0;
                    currentFrame_Body = (numFrames_Body - 1);
                }
            }
        }

        private void Update_ScreamOut()
        {
            if (timeSinceLastSwitch >= deltaTime_ScreamOut)
            {
                timeSinceLastSwitch = 0;
                currentFrame_Body--;
                if (currentFrame_Body < 0)
                {
                    currentState = RotState.Steady;
                    currentFrame_Tongue = 0;
                    currentFrame_Body = 0;
                }
            }
        }

        private void Update_Scream()
        {
            currentFrame_Body = (numFrames_Body - 1);
            if (timeSinceLastSwitch >= deltaTime_TongueRoll)
            {
                timeSinceLastSwitch = 0;
                if (numberOfTongueRolls >= totalNumberOfTongueRolls)
                {
                    currentState = RotState.ScreamOut;
                    numberOfTongueRolls = 0;
                    MyEnemyType = EnemyType.Leader;
                }
                else
                {
                    numberOfTongueRolls++;
                    if (currentFrame_Tongue != texture_OutlineTongue.Length - 1)
                    {
                        currentFrame_Tongue = texture_OutlineTongue.Length - 1;
                    }
                    else
                    {
                        currentFrame_Tongue = texture_OutlineTongue.Length - 2;
                    }

                    if (color_ScreamBase != ColorState.Medium)
                    {
                        color_ScreamBase = ColorState.Medium;
                        color_ScreamSkullcap = ColorState.Dark;
                    }
                    else
                    {
                        color_ScreamBase = ColorState.Light;
                        color_ScreamSkullcap = ColorState.Medium;
                    }
                }
            }
        }

        private void Update_RollOutTongue()
        {
            currentFrame_Body = (numFrames_Body - 1);
            if (timeSinceLastSwitch >= deltaTime_TongueRollOut)
            {
                timeSinceLastSwitch = 0;
                currentFrame_Tongue++;
                if (currentFrame_Tongue == numFrames_Tongue)
                {
                    currentFrame_Tongue = numFrames_Tongue - 1;
                    currentState = RotState.Scream;
                    color_ScreamBase = ColorState.Medium;
                    color_ScreamSkullcap = ColorState.Dark;
                    numberOfTongueRolls = 0;
                }
            }
        }

        private void Update_RollInTongue()
        {
            currentFrame_Body = (numFrames_Body - 1);
            if (timeSinceLastSwitch >= deltaTime_TongueRollIn)
            {
                timeSinceLastSwitch = 0;
                currentFrame_Tongue--;
                if (currentFrame_Tongue < 0)
                {
                    currentFrame_Tongue = 0;
                    currentState = RotState.Steady;
                    color_ScreamBase = ColorState.Medium;
                    color_ScreamSkullcap = ColorState.Dark;
                    numberOfTongueRolls = 0;
                }
            }
        }

        private void Update_Steady()
        {
            if (timeSinceLastSwitch >= deltaTime_Steady)
            {
                timeSinceLastSwitch = 0;
                if (numberOfTimesFaded >= totalNumberOfTimesToFade)
                {
                    currentState = RotState.ScreamIn;
                    numberOfTimesFaded = 0;
                    if (ColorPolarity == ColorPolarity.Positive)
                        ColorPolarity = ColorPolarity.Negative;
                    else
                        ColorPolarity = ColorPolarity.Positive;
                    SetCurrentColors();
                    MyEnemyType = EnemyType.Standard;
                    sfxi_Scream = sfx_Scream.Play(volume_Scream);
                }
                else
                {
                    currentState = RotState.FadeIn;
                    sfxi_FaceOn = sfx_FaceOn.Play(volume_FaceOn);
                }
            }
        }
        #endregion

        #region Draw/ Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the static
            InstanceManager.RenderSprite.Draw(
                texture_Static[currentFrame_Static],
                Position,
                center_Static,
                null,
                color_Static[currentStaticColor],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);

            switch (currentState)
            {
                case RotState.FadeIn:
                    Draw_Steady(gameTime);
                    Draw_FadeIn(gameTime, deltaTime_FadeIn);
                    Draw_NormalFace(gameTime);
                    break;
                case RotState.FadeOut:
                    Draw_Steady(gameTime);
                    Draw_FadeOut(gameTime, deltaTime_FadeOut);
                    Draw_NormalFace(gameTime);
                    break;
                case RotState.FullOn:
                    Draw_FullOn(gameTime);
                    Draw_NormalFace(gameTime);
                    break;
                case RotState.ScreamIn:
                    Draw_Steady(gameTime);
                    Draw_FadeIn(gameTime, deltaTime_ScreamIn);
                    Draw_NormalFace(gameTime);
                    break;
                case RotState.ScreamOut:
                    Draw_Steady(gameTime);
                    Draw_FadeOut(gameTime, deltaTime_ScreamOut);
                    Draw_NormalFace(gameTime);
                    break;
                case RotState.RollOutTongue:
                    Draw_Scream(gameTime);
                    break;
                case RotState.Scream:
                    Draw_Scream(gameTime);
                    break;
                default:
                    Draw_Steady(gameTime);
                    Draw_NormalFace(gameTime);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceLastSwitch += gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceLastStaticFrame += gameTime.ElapsedGameTime.TotalSeconds;
            if (currentState == RotState.Scream)
            {
                if (timeSinceLastStaticFrame > deltaTime_StaticScream)
                {
                    currentFrame_Static++;
                    timeSinceLastStaticFrame = 0;
                    if (currentFrame_Static >= numFrames_Static)
                    {
                        currentFrame_Static = 0;
                    }
                }
            }
            else
            {
                if (timeSinceLastStaticFrame > deltaTime_StaticSteady)
                {
                    currentFrame_Static++;
                    timeSinceLastStaticFrame = 0;
                    if (currentFrame_Static >= numFrames_Static)
                    {
                        currentFrame_Static = 0;
                    }
                }
            }

            switch (currentState)
            {
                case RotState.FadeIn:
                    Update_FadeIn();
                    break;
                case RotState.FullOn:
                    Update_FullOn();
                    break;
                case RotState.FadeOut:
                    Update_FadeOut();
                    break;
                case RotState.ScreamIn:
                    Update_ScreamIn();
                    break;
                case RotState.Scream:
                    Update_Scream();
                    break;
                case RotState.RollOutTongue:
                    Update_RollOutTongue();
                    break;
                case RotState.ScreamOut:
                    Update_ScreamOut();
                    break;
                case RotState.RollInTongue:
                    Update_RollInTongue();
                    break;
                default:
                    Update_Steady();                    
                    break;
            }
        }
        #endregion
    }
}
