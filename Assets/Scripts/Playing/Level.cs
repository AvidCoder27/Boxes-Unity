using System;
using System.Collections.Generic;
using Unity.Mathematics;

public class Level
{
    private const double PI = Math.PI;
    private const float RadiusMultiplier = 3.8f;
    public static readonly float DistanceBetweenFloors = 7f;

    // Floors is a list of "floors." A floor is just a list of columns.
    public List<List<BoxStruct[]>> Floors { get; private set; }
    public int NumberOfColumns { get; private set; }
    public int NumberOfFloors { get; private set; }
    public double Radius { get; private set; }
    public double AngleBetweenColumns { get; private set; }

    public Level(List<List<BoxStruct[]>> map)
    {
        Floors = map;
        NumberOfFloors = map.Count;
        // get the # of columns on the top floor (index 0)
        NumberOfColumns = Floors[0].Count;

        Radius = NumberOfColumns / PI * RadiusMultiplier;
        AngleBetweenColumns = 2 * PI / NumberOfColumns;
    }

    public double2 CalculateCoordinatesForColumn(double column, double distanceFromCircle = 0)
    {
        double angle = column * AngleBetweenColumns;
        double radius = Radius - distanceFromCircle;
        double x = radius * Math.Cos(angle);
        double y = radius * Math.Sin(angle);
        return new double2(x, y);
    }

    public double CalculateCameraAngleForColumnInDegrees(double column)
    {
        return -360 * column / NumberOfColumns + 90;
    }
}
