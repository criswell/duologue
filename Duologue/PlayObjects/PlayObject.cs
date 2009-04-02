#region File description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Mimicware.Graphics;
using Mimicware.Manager;
#endregion

namespace Duologue.PlayObjects
{
    /// <summary>
    /// The major play object types
    /// </summary>
    public enum MajorPlayObjectType
    {
        Player,
        PlayerBullet,
        Enemy,
        Other,
    }
    /// <summary>
    /// The various types of play objects
    /// </summary>
    public enum TypesOfPlayObjects
    {
        Player,
        PlayerBullet,
        Enemy_Buzzsaw,
        Enemy_Wiggles,
        Enemy_Spitter,
        Enemy_Gloop,
        Enemy_StaticGloop,
        Enemy_KingGloop,
        Enemy_Pyre,
        Enemy_Ember,
        Enemy_UncleanRot,
        Enemy_Mirthworm,
        Enemy_AnnMoeba,
        Enemy_ProtoNora,
        Enemy_RollingRock,
        Enemy_Maggot,
        Enemy_Roggles,
        Enemy_Spawner,
        Enemy_Placeholder,
    }

    /// <summary>
    /// Defines the basic play object class all other play objects are derived
    /// from.
    /// </summary>
    public abstract class PlayObject
    {
        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// Is this player object initialized?
        /// </summary>
        public bool Initialized;
        /// <summary>
        /// Determines if this play object is considered "firm" in the environment
        /// </summary>
        public bool IsFirm;

        /// <summary>
        /// The type of play object this play object is
        /// </summary>
        public TypesOfPlayObjects MyType;

        /// <summary>
        /// The major type of play object we are
        /// </summary>
        public MajorPlayObjectType MajorType;

        /// <summary>
        /// Write-only property for setting the current asset manager
        /// Must be set before component is added to the game.
        /// </summary>
        public AssetManager AssetManager;

        /// <summary>
        /// Write-only property for setting the current render srpite instance
        /// Must be set before component is added to the game.
        /// </summary>
        public RenderSprite RenderSprite;

        /// <summary>
        /// Write-only property for setting the current graphics device
        /// Must be set before component is added to the game.
        /// </summary>
        public GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The current position of the play object
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Determines if this playObject is alive
        /// </summary>
        public bool Alive;

        /// <summary>
        /// The radius of this playobject
        /// </summary>
        public float Radius;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a new PlayObject
        /// </summary>
        public PlayObject()
        {
            AssetManager = InstanceManager.AssetManager;
            RenderSprite = InstanceManager.RenderSprite;
            GraphicsDevice = InstanceManager.GraphicsDevice;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Call to set the assetmanager.
        /// </summary>
        /// <param name="manager">The AssetManager</param>
        public virtual void SetAssetManager(AssetManager manager)
        {
            AssetManager = manager;
        }

        /// <summary>
        /// Call to set the render sprite
        /// </summary>
        /// <param name="render">The RenderSprite</param>
        public virtual void SetRenderSprite(RenderSprite render)
        {
            RenderSprite = render;
        }

        /// <summary>
        /// Call to set the GraphicsDevice
        /// </summary>
        /// <param name="device">The GraphicsDevice</param>
        public virtual void SetGraphicsDevice(GraphicsDevice device)
        {
            GraphicsDevice = device;
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// This should will be called every update at the begining
        /// </summary>
        public abstract bool StartOffset();

        /// <summary>
        /// Called for every play object this object might interact with
        /// </summary>
        /// <param name="pobj"></param>
        public abstract bool UpdateOffset(PlayObject pobj);

        /// <summary>
        /// Apply the offset
        /// </summary>
        public abstract bool ApplyOffset();

        /// <summary>
        /// Get the filenames for the images I need to have pre-cached
        /// </summary>
        public abstract String[] GetFilenames();

        /// <summary>
        /// Triggers a hit between objects
        /// </summary>
        /// <param name="pobj"></param>
        public abstract bool TriggerHit(PlayObject pobj);

        public abstract void Draw(GameTime gameTime);

        public abstract void Update(GameTime gameTime);
        #endregion
    }
}
