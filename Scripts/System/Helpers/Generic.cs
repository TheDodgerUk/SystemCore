using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class Generic
    {
        public static float GetCorrectRotateAmount(float fromY, float toY)
        {
            float clockWise = 0f;
            float counterClockWise = 0f;

            if (fromY <= toY)
            {
                clockWise = toY - fromY;
                counterClockWise = fromY + (360 - toY);
            }
            else
            {
                clockWise = (360 - fromY) + toY;
                counterClockWise = fromY - toY;
            }
            if (clockWise <= counterClockWise)
            {
                return clockWise;
            }
            else
            {
                return -counterClockWise;
            }
        }

        public static float GetRotateAmountABS(float fromY, float toY)
        {
            return Mathf.Abs(GetCorrectRotateAmount(fromY, toY));
        }

        public static float GetSlidingAmount(float fromPos, float toPos)
        {
            float slidingAmount = 0.0f;
            slidingAmount = toPos - fromPos;
            return slidingAmount;
        }

        public static void Swap<t>(ref t value1, ref t value2)
        {
            t temp = value1;
            value1 = value2;
            value2 = temp;
        }

    }
}
