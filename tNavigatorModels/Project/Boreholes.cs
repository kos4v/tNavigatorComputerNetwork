namespace tNavigatorModels.Project
{
    public record Borehole(
        string Name,
        CoordinatePoint[] Coordinates
    )
    {
        public string TNavString() => $"   '{Name}'   1*  2*  /";
    };


    public record CoordinatePoint(int X, int Y, double Z, int OrderNumber);
}