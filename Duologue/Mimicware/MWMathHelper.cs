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
        /// Given a vector and an origin, computes the angle between them
        /// </summary>
        /// <param name="vector">The vector</param>
        /// <param name="origin">The origin</param>
        /// <returns>Float, angle in radians</returns>
        public static float ComputeAngleAgainstX(Vector2 vector, Vector2 origin)
        {
            return ComputeAngleAgainstX(vector - origin);
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

        public static float LimitToRange(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static int LimitToRange(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        /// <summary>
        /// Rotate a vector by an angle
        /// </summary>
        /// <param name="offset">The vector to rotate</param>
        /// <param name="p">The angle in radians</param>
        /// <returns>the rotated vector</returns>
        internal static Vector2 RotateVectorByRadians(Vector2 offset, float p)
        {
            float rot = ComputeAngleAgainstX(offset);
            float len = offset.Length();

            offset = new Vector2(
                len * (float)Math.Cos((double)(rot + p)),
                len * (float)Math.Sin((double)(rot + p)));

            return offset;
        }

        /// <summary>
        /// Basic coin toss, returns either true for false
        /// </summary>
        internal static bool CoinToss()
        {
            return rand.Next(0, 2) == 0;
        }
        #endregion
    }
}
