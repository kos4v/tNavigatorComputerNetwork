using System.Data;
using tNavigatorModels.Project.Schedule;
using tNavigatorModels.Result;
using Utils;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public List<BoreholeData> GetDebit(EnumDataType dataType)
        {
            var debitDir = Path.Combine(launcherConfig.ResultDirPath, Schedule.DebitDirName[dataType]);
            var debitFiles = Directory.GetFiles(debitDir);

            var boreholeDebitList = new List<BoreholeData>();

            foreach (var debitFile in debitFiles)
            {
                var dt = Converter.ConvertCSVtoDataTable(debitFile);
                var date = dt.Rows[0][1].ToString()!.Split('-').Select(v => Convert.ToInt16(v)).ToList();

                var boreholeData = new BoreholeData()
                {
                    Date = new DateTime(date[0], date[1], date[2]),
                    BoreholeName = dt.Rows[0][2].ToString()!.Split(':').First(),
                    DataType = dataType
                };

                foreach (DataRow dataRow in dt.Rows)
                {
                    var coordinate = dataRow[2].ToString()!.Split(':').Last().Split(',').Last();
                    var zCell = Convert.ToInt16(string.Join("", coordinate.Where(char.IsDigit)));
                    var value = Convert.ToDouble(((string)dataRow[3]).Replace('.', ','));
                    boreholeData.ZCell.Add(zCell);
                    boreholeData.Value.Add(Math.Round(value, 5));
                }

                boreholeDebitList.Add(boreholeData);
            }

            return boreholeDebitList;
        }
    }
}