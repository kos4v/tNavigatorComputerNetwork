using System.Data;
using System.Globalization;
using tNavigatorModels;
using tNavigatorModels.Project.Schedule;
using tNavigatorModels.Result;
using Utils;

namespace tNavigatorLauncher.FileParsers
{
    public record Coordinate(double x, double y, double z)
    {
        public static double CalculateDistance(Coordinate point1, Coordinate point2)
        {
            double deltaX = point2.x - point1.x;
            double deltaY = point2.y - point1.y;
            double deltaZ = point2.z - point1.z;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }
    }

    public class BoreholeRow
    {
        public string BoreholeName { get; set; }
        public DateTime Date { get; set; }
        public Coordinate Coordinate { get; set; }
        public double Value { get; set; }
        public int Order { get; set; }
    }


    public partial class NavigatorFileController
    {
        public static Dictionary<string, Coordinate>? CoordinateDict { get; set; }

        public List<BoreholeData> GetDebit(EnumDataType dataType)
        {
            CoordinateDict ??=
                JsonUtil.Deserialize<Dictionary<string, Coordinate>>(File.ReadAllText(launcherConfig.CoordinatesPath));
            var debitDir = Path.Combine(launcherConfig.ResultDirPath, Schedule.DebitDirName[dataType]);
            if (!Directory.Exists(debitDir))
                return [];

            var debitFiles = Directory.GetFiles(debitDir);

            var boreholeRows = new List<BoreholeRow>();
            var start = 0;
            foreach (var debitFile in debitFiles)
            {
                var dt = Converter.ConvertCSVtoDataTable(debitFile);

                foreach (DataRow dataRow in dt.Rows)
                {
                    var a = dataRow[2].ToString()!.Split(':').Last();
                    var b = a.Split(',');
                    var c = b.Select(v => string.Join("", v.Where(char.IsDigit))).Select(v => Convert.ToInt16(v))
                        .ToArray();
                    var d = string.Join("_", c);

                    if (!CoordinateDict!.TryGetValue(d, out var coordinate))
                        continue;

                    var date = dt.Rows[0][1].ToString()!.Split('-').Select(v => Convert.ToInt16(v)).ToList();
                    double.TryParse((string)dataRow[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
                    boreholeRows.Add(new BoreholeRow
                    {
                        BoreholeName = dataRow[2].ToString()!.Split(':').First(),
                        Date = new DateTime(date[0], date[1], date[2]),
                        Coordinate = coordinate!,
                        Value = Math.Round(value, 5),
                        Order = start++,
                    });
                }
            }

            var boreholeDataCollection = boreholeRows
                .GroupBy(br => (br.BoreholeName, br.Date))
                .Select(group => new BoreholeData()
                {
                    DataType = dataType,
                    BoreholeName = group.Key.BoreholeName,
                    Date = group.Key.Date,
                    Value = group.OrderBy(c => c.Order).Select(v => v.Value).ToList(),
                    MD = CoordinateToMd(group.ToList()).ToList()
                })
                .ToList();

            return boreholeDataCollection;


            IEnumerable<double> CoordinateToMd(IEnumerable<BoreholeRow> boreholeRows)
            {
                var orderRows = boreholeRows.OrderBy(v => v.Order);
                var mdList = new List<double> { 0 };
                var startPoint = orderRows.First().Coordinate with { z = 0 };
                foreach (var boreholeRow in orderRows)
                {
                    mdList.Add(Coordinate.CalculateDistance(startPoint, boreholeRow.Coordinate) + mdList.Last());
                    startPoint = boreholeRow.Coordinate;
                }

                return mdList.Skip(1);
            }
        }
    }
}