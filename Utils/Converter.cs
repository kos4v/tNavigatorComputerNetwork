using System.Data;
using System.Text;

namespace Utils
{
    public class Converter
    {
        public static DataTable ConvertCSVtoDataTable(string strFilePath, char separator = ';')
        {

            DataTable dt = new DataTable();
            using StreamReader sr = new StreamReader(strFilePath);
            string[] headers = sr.ReadLine().Split(separator);
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(separator);
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}