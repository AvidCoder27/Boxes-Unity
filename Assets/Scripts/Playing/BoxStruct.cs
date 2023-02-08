public struct BoxStruct
{
    public bool isOpen;
    public Box.Contents contents;

    /// <summary>
    /// the color of the key in the box
    /// </summary>
    public Key.Color keyColor;

    /// <summary>
    /// the color of the lock on the box
    /// </summary>
    public Key.Color lockColor;

    /// <summary>
    /// Create a representation of a box
    /// </summary>
    /// <param name="isOpen">is the box open?</param>
    /// <param name="contents">the contents of the box</param>
    public BoxStruct(bool isOpen, Box.Contents contents = Box.Contents.None)
    {
        this.isOpen = isOpen;
        this.contents = contents;
        keyColor = Key.Color.Undefined;
        lockColor = Key.Color.Undefined;
    }

    /// <summary>
    /// Create a representation of a box
    /// </summary>
    /// <param name="isOpen">is the box open?</param>
    /// <param name="contents">the contents of the box</param>
    /// <param name="keyColor">the color of the key inside the box</param>
    /// <param name="lockColor">the color of the lock on the box</param>
    public BoxStruct(bool isOpen, Box.Contents contents, Key.Color keyColor, Key.Color lockColor)
    {
        this.isOpen = isOpen;
        this.contents = contents;
        this.keyColor = keyColor;
        this.lockColor = lockColor;
    }
}