using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Level
{
    private const float PI = Mathf.PI;
    private const float RadiusMultiplier = 3.8f;
    public static readonly float DistanceBetweenFloors = 7f;

    // Floors is a list of "floors." A floor is just a list of columns. A column is actually just a two element array of box structs.
    public List<List<BoxStruct[]>> Floors { get; private set; }
    public int NumberOfColumns { get; private set; }
    public int NumberOfFloors { get; private set; }
    public float Radius { get; private set; }
    public float AngleBetweenColumns { get; private set; }

    public Level(List<List<BoxStruct[]>> map)
    {
        Floors = map;
        NumberOfFloors = map.Count;
        // get the # of columns on the top floor (index 0)
        NumberOfColumns = Floors[0].Count;

        Radius = NumberOfColumns / PI * RadiusMultiplier;
        AngleBetweenColumns = -2 * PI / NumberOfColumns;
    }

    public float2 CalculateCoordinatesForColumn(float column, float distanceFromCircle = 0)
    {
        float angle = column * AngleBetweenColumns;
        float radius = Radius - distanceFromCircle;
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        return new float2(x, y);
    }

    public float CalculateCameraAngleForColumnInDegrees(float column)
    {
        return (360 * column / NumberOfColumns) + 90;
    }
}
