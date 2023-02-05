public struct Column
{
    public bool IsTopOpen;
    public Box.Contents TopContents;
    public Key.Color TopKey;
    public Key.Color TopLock;

    public bool IsBottomOpen;
    public Box.Contents BottomContents;
    public Key.Color BottomKey;
    public Key.Color BottomLock;

    public Column(
        bool isTopOpen,
        bool isBottomOpen,
        Box.Contents topContents = Box.Contents.None,
        Box.Contents bottomContents = Box.Contents.None,
        Key.Color topKey = Key.Color.Undefined,
        Key.Color bottomKey = Key.Color.Undefined,
        Key.Color topLock = Key.Color.Undefined,
        Key.Color bottomLock = Key.Color.Undefined)
    {
        IsTopOpen = isTopOpen;
        TopContents = topContents;
        TopKey = topKey;
        TopLock = topLock;

        IsBottomOpen = isBottomOpen;
        BottomContents = bottomContents;
        BottomKey = bottomKey;
        BottomLock = bottomLock;
    }

    /// <summary>
    /// Get open or closed status of a box in the column
    /// </summary>
    /// <param name="index">index of the box, 0 for bottom, 1 for top</param>
    /// <returns></returns>
    public bool GetIsOpenFromIndex(int index)
    {
        ValidateIndex(index);
        return index == 0 ? IsBottomOpen : IsTopOpen;
    }

    /// <summary>
    /// Get contents of a box in the column
    /// </summary>
    /// <param name="index">index of the box, 0 for bottom, 1 for top</param>
    /// <returns></returns>
    public Box.Contents GetContentsFromIndex(int index)
    {
        ValidateIndex(index);
        return index == 0 ? BottomContents : TopContents;
    }

    /// <summary>
    /// Get key color of a box in the column
    /// </summary>
    /// <param name="index">index of the box, 0 for bottom, 1 for top</param>
    /// <returns></returns>
    public Key.Color GetKeyColorFromIndex(int index)
    {
        ValidateIndex(index);
        return index == 0 ? BottomKey : TopKey;
    }

    public Key.Color GetLockColorFromIndex(int index)
    {
        ValidateIndex(index);
        return index == 0 ? BottomLock : TopLock;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index > 1)
        {
            throw new System.Exception("Index out of range for Box state: must be 0 or 1");
        }
    }
}