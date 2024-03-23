using System;

namespace MathLib;

public static class Angles
{
    public static double Rad2Deg(double radians)
    {
        double degrees = (180 / Math.PI) * radians;
        return (degrees);
    }

    public static double Deg2Rad(double degrees)
    {
        double radians = (degrees / 180) * Math.PI;
        return (radians);
    }
}
