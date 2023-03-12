using System;
using System.Collections.Generic;

#pragma warning disable IDE1006 // Naming Styles
[Serializable]
public struct SerialLevel
{
    public int columns;
    public int floors;
    public List<SerialFloor> boxes;
    public Level GetLevel()
    {
        List<List<BoxStruct[]>> list = new();

        for (int f = 0; f < floors; f++)
        {
            list.Add(new List<BoxStruct[]>());
            for (int c = 0; c < columns; c++)
            {
                list[f].Add(new BoxStruct[]
                {
                    boxes[f].GetCol(c).bottom.GetBoxStruct(), boxes[f].GetCol(c).top.GetBoxStruct()
                });
            }
        }

        return new Level(list);
    }
}

[Serializable]
public struct SerialFloor
{
    public List<SerialColumn> floor;
    public SerialColumn GetCol(int index) => floor[index];
}

[Serializable]
public struct SerialColumn
{
    public SerialBox top;
    public SerialBox bottom;
}

[Serializable]
public struct SerialBox
{
    public bool open;
    public string contents;
    public string key_color;
    public string lock_color;

    public BoxStruct GetBoxStruct()
    {
        return new BoxStruct(open, Box.ContentsFromString(contents), Box.ColorFromString(key_color), Box.ColorFromString(lock_color));
    }
}

#pragma warning restore IDE1006 // Naming Styles