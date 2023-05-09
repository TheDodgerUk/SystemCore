using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreUtils
{
	public const float FeetToMeters = 0.3048f;
	public const float FeetToCentimeters = 30.48f;
	public const float InchesToMeters = 0.0254f;
	public const float InchesToCentimeters = 2.54f;
	public const float MetersToFeet = 3.28084f;
	public const float MetersToInches = 39.3701f;
	public const float CentimetersToFeet = 0.0328084f;
	public const float CentimetersToInches = 0.393701f;
	public const float KilometersToMiles = 0.621371f;
	public const float MilesToKilometers = 1.60934f;


	public static Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
	{
		var vVector1 = vPoint - vA;
		var vVector2 = (vB - vA).normalized;

		var d = Vector3.Distance(vA, vB);
		var t = Vector3.Dot(vVector2, vVector1);

		if (t <= 0)
			return vA;

		if (t >= d)
			return vB;

		var vVector3 = vVector2 * t;

		var vClosestPoint = vA + vVector3;

		return vClosestPoint;
	}

}
