public struct BoxStruct
{
    public bool IsOpen;
    public Box.Contents Contents;

    /// <summary>
    /// the color of the key in the box
    /// </summary>
    public Key.Colors KeyColor;

    /// <summary>
    /// the color of the lock on the box
    /// </summary>
    public Key.Colors LockColor;

    /// <summary>
    /// Create a representation of a box
    /// </summary>
    /// <param name="isOpen">is the box open?</param>
    /// <param name="contents">the contents of the box</param>
    public BoxStruct(bool isOpen, Box.Contents contents = Box.Contents.None)
    {
        IsOpen = isOpen;
        Contents = contents;
        KeyColor = Key.Colors.Undefined;
        LockColor = Key.Colors.Undefined;
    }

    /// <summary>
    /// Create a representation of a box
    /// </summary>
    /// <param name="isOpen">is the box open?</param>
    /// <param name="contents">the contents of the box</param>
    /// <param name="keyColor">the color of the key inside the box</param>
    /// <param name="lockColor">the color of the lock on the box</param>
    public BoxStruct(bool isOpen, Box.Contents contents, Key.Colors keyColor, Key.Colors lockColor)
    {
        IsOpen = isOpen;
        Contents = contents;
        KeyColor = keyColor;
        LockColor = lockColor;
    }
}