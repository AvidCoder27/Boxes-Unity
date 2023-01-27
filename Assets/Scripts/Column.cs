public struct Column
{
    public bool IsTopBoxOpen;
    public Box.Contents TopBoxContents;
    public bool IsBottomBoxOpen;
    public Box.Contents BottomBoxContents;

    public Column(bool isTopBoxOpen, bool isBottomBoxOpen, Box.Contents topBoxContents = Box.Contents.None, Box.Contents bottomBoxContents = Box.Contents.None)
    {
        IsTopBoxOpen = isTopBoxOpen;
        TopBoxContents = topBoxContents;
        IsBottomBoxOpen = isBottomBoxOpen;
        BottomBoxContents = bottomBoxContents;
    }

    /// <summary>
    /// Returns whether or not the specified box (top or bottom) is open. <br></br>
    /// 0 for bottom box, 1 for top box
    /// </summary>
    /// <param name="index">index of the box, 0 for bottom, 1 for top</param>
    /// <returns>true if the box at index is open</returns>
    /// <exception cref="System.Exception">Exception if the index is neither 0 nor 1</exception>
    public bool GetIsOpenFromIndex(int index)
    {
        if (index < 0 || index > 1)
            throw new System.Exception("Index out of range for Box state: must be 0 or 1");

        return index == 0 ? IsBottomBoxOpen : IsTopBoxOpen;
    }
    public Box.Contents GetContentsFromIndex(int index)
    {
        if (index < 0 || index > 1)
            throw new System.Exception("Index out of range for Box state: must be 0 or 1");

        return index == 0 ? BottomBoxContents : TopBoxContents;
    }
}
