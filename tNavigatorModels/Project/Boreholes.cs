namespace tNavigatorModels.Project
{
    public record Borehole(
        string Name,
        CoordinatePoint[] Coordinates
    );


    public record CoordinatePoint(int X, int Y, double Z, int OrderNumber);
}