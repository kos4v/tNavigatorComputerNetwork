namespace tNavigatorModels
{
    public record Team(string Name);
    public record TNvagigaorModel(string Url);
    public record TNvagigaorResult(
        string ProjectDirPath,
        string? Message);
    public record TNavigatorProject(
        Borehole[] Boreholes,
        Schedule.Schedule Schedule,
        Team Team        
        );
}
