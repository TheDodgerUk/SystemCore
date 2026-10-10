using UnityEngine;

public static class InputUtility
{
    private const float EPSILON = 0.0001f;
    public static bool LinePlaneIntersectionDualDirection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {
        //calculate the distance between the linePoint and the line-plane intersection point
        float dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        float dotDenominator = Vector3.Dot(lineVec, planeNormal);
        //line and plane are not parallel
        if (IsZero(dotDenominator) == false)
        {
            float length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            var vector = lineVec.normalized * length;

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;
            return true;
        }
        //output not valid
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public static bool LinePlaneIntersectionSingleDirection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {
        //calculate the distance between the linePoint and the line-plane intersection point
        float dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        float dotDenominator = Vector3.Dot(lineVec, planeNormal);
        //line and plane are not parallel
        if (IsZero(dotDenominator) == false && dotDenominator < 0)   //dotDenominator < 0 means it collides with the plane
        {
            float length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            var vector = lineVec.normalized * length;

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;
            return true;
        }
        //output not valid
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    private static bool IsZero(float amount )
    {
        amount = Mathf.Abs(amount);
        return (amount < EPSILON);
    }
}
