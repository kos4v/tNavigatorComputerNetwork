using System.Net;

namespace tNavigatorModels.Project
{
    public record Team(string Name);

    public record ConsumeModel(string Url);

    public record Result(string ProjectDirPath, string? Message);

    public record Project(
        Borehole[] Boreholes,
        Schedule.Schedule Schedule,
        Team Team,
        string ResultAddress,
        string ConverterAddress
    );
}