namespace WinterEngine;

public static class Mathf
{
	// Instead of storing all our sin/cos values in a giant array like 3DSage did
	// we just convert them on the fly instead.
	public static double Rad2Deg(double radians) {
		double degrees = (180 / Math.PI) * radians;
    	return (degrees);
	}

	public static double Deg2Rad(double degrees) {
		double radians = (degrees / 180) * Math.PI;
    	return (radians);
	}
}