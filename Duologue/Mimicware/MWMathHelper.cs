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
        private static Random rand;
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

        /// <summary>
        /// Given a range, return a random double in that range
        /// </summary>
        /// <param name="lower">Lower bounds</param>
        /// <param name="upper">Upper bounds</param>
        /// <returns>Random double</returns>
        public static double GetRandomInRange(double lower, double upper)
        {
            if (rand == null)
                rand = new Random();
            return lower + rand.NextDouble() * (upper - lower);
        }

        /// <summary>
        /// Given a range, return a random integer.
        /// </summary>
        /// <param name="lower">Lower bounds</param>
        /// <param name="upper">Upper bounds</param>
        /// <returns>Random integer</returns>
        public static int GetRandomInRange(int lower, int upper)
        {
            if (rand == null)
                rand = new Random();
            return rand.Next(lower, upper);
        }
        #endregion
    }
}
