using tNavigatorModels.Project.Schedule;
using tNavigatorModels.Project.Schedule.Events;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void InitSchedule()
        {
            var scheduleInc = new List<string>();
            void AddRange(IEnumerable<string> rows) => scheduleInc.AddRange([..rows, "\n"]);
            void Add(string row) => AddRange([row]);

            var allEvents = project.Schedule.Events.GetAllEvents();
            var scriptStartStep = (allEvents?.OfType<OpenPerforationEvent>().MinBy(c => c.Step)?.Step ?? 0) + 1;

            AddRange([
                "RPTSCHED",
                "'WELLS=2' 'SUMMARY=2' 'fip=3' 'RESTART=1' 'WELSPECS' 'CPU=2' /",
                "",
                "INCLUDE\r\n'INCLUDE/DynamicModel_VFP.inc' /",
                "INCLUDE",
                $"'INCLUDE/{Path.GetFileName(launcherConfig.WellTrackPath)}' /",
                "",
                "WELSPECS",
                ..project.Boreholes.Select(b => b.TNavString()),
                "/",
                "",
                Schedule.ScriptsTNavString(launcherConfig.ScriptDirPath, launcherConfig.ResultDirPath, scriptStartStep)
            ]);

            var eventsCalendar = allEvents.GroupBy(e => e.Step)
                .ToDictionary(events => events.Key, events => events.ToArray());

            // i = 1 т.к. В *.data указана дата старта её нельзя указывать в DATES
            IBaseEvent? firstEvent = null;
            for (int i = 1; i < project.Schedule.CurrentStep; i++)
            {
                eventsCalendar.TryGetValue(i, out var todayEvents);
                if (todayEvents is null && firstEvent is null)
                {
                    continue;
                }
                firstEvent ??= todayEvents!.First();

                Add(Schedule.DateTNavString(i));
                // Schedule.GetEventPriority позволяет объявить перфорации до взаимодействия с ними
                // Так же указывает прочий порядок добавление событий
                foreach (var grouping in (todayEvents ?? []).GroupBy(e => e.EventTNavName).OrderBy(GetPriority))
                {
                    string[] dateRange = [grouping.Key, .. grouping.Select(e => e.TNavString()), "/", ""];
                    AddRange(dateRange);
                }
            }

            AddRange([Schedule.DateTNavString(project.Schedule.CurrentStep), "END"]);
            File.WriteAllText(launcherConfig.SchedulePath, string.Join('\n', scheduleInc));
            return;

            int GetPriority(IEnumerable<object> c) => Schedule.GetEventPriority(c.First());
        }
    }
}