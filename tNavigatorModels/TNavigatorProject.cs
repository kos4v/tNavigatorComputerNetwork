namespace tNavigatorModels
{
    public record Team(string Name);
    public record TNavigatorModel(string Url);
    public record TNavigatorResult(
        string ProjectDirPath,
        string? Message);
    public record TNavigatorProject(
        Borehole[] Boreholes,
        Schedule.Schedule Schedule,
        Team Team        
        );
}
