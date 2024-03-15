using System.Linq;
using tNavigatorModels.Project.Schedule;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void InitSchedule()
        {
            var schedule_inc = new List<string>();
            void AddRange(IEnumerable<string> rows) => schedule_inc.AddRange([..rows, "\n"]);
            void Add(string row) => AddRange([row]);

            AddRange([
                "RPTSCHED",
                "'WELLS=2' 'SUMMARY=2' 'fip=3' 'RESTART=1' 'WELSPECS' 'CPU=2' /",
                "",
                "INCLUDE",
                $"'INCLUDE/{Path.GetFileName(launcherConfig.WellTrackPath)}' /",
            ]);

            AddRange(["WELSPECS", ..project.Boreholes.Select(borehole => $"   '{borehole.Name}'   1*  2*  /"), "/"]);

            Add(Schedule.ScriptsTNavString(launcherConfig.ScriptDirPath, launcherConfig.ResultDirPath));

            AddRange(
            [
                "COMPDATMD",
                ..project.Boreholes.SelectMany(borehole => project.Schedule.Events.AddPerforationEvents
                    .Where(p => p.BoreholeName == borehole.Name)
                    .Select(perforation => perforation.COMPDATMDString())),
                "/"
            ]);

            // В *.data указана дата старта её нельзя указывать в DATES
            var eventsCalendar = project.Schedule.Events.GetAllEvents()
                .GroupBy(e => e.Step)
                .ToDictionary(events => events.Key, events => events.ToArray());

            for (int i = 1; i < project.Schedule.CurrentStep; i++)
            {
                eventsCalendar.TryGetValue(i, out var todayEvents);
                Add(Schedule.DateTNavString(i));

                foreach (var grouping in (todayEvents ?? []).GroupBy(e => e.EventTNavName))
                {
                    AddRange([grouping.Key, ..grouping.Select(e => e.TNavString()), "/", ""]);
                }
            }

            AddRange([Schedule.DateTNavString(project.Schedule.CurrentStep), "END"]);

            File.WriteAllText(launcherConfig.SchedulePath, string.Join('\n', schedule_inc));
        }
    }
}