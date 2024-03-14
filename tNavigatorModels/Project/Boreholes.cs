namespace tNavigatorModels.Project
{
    public record Borehole(
        string Name,
        CoordinatePoint[] Coordinates,
        DateTime Date
    );


    public record CoordinatePoint(int X, int Y, int Z, int OrderNumber);
}