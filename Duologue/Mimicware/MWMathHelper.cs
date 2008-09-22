#region File info
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Mimicware
{
    class MWMathHelper
    {
        #region Properties
        #endregion

        #region Fields
        #endregion

        #region Constructor
        /// <summary>
        /// Not to be called
        /// </summary>
        private MWMathHelper()
        {
            throw new Exception("Hah! Hah!");
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Given a vector, computes its angle against the X axis
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Float, angle in radians</returns>
        public static float ComputeAngleAgainstX(Vector2 vector)
        {
            float dotVector = Vector2.Dot(vector, Vector2.UnitX);
            float rotation = (float)Math.Acos((double)(dotVector / vector.Length()));
            if (vector.Y < 0)
                rotation *= -1;
            return rotation;
        }
        #endregion
    }
}
