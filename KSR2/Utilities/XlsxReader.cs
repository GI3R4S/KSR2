using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Utilities
{
    public static class XlsxReader
    {
        public static List<Record> ReadXlsx(string aPath)
        {
            List<Record> records = new List<Record>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            FileStream stream = File.Open(aPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader;
            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            var dataSet = excelReader.AsDataSet();

            DataTable dataTable = dataSet.Tables[0];

            for (int i = 1; i < dataTable.Rows.Count; ++i)
            {
                Record recordBuilder = new Record();
                var currentRow = dataTable.Rows[i];

                recordBuilder.Date = (DateTime)currentRow[0];
                recordBuilder.MinimalTemperature = float.Parse(currentRow[1].ToString());
                recordBuilder.MaximalTemperature = float.Parse(currentRow[2].ToString());
                recordBuilder.Rainfall = float.Parse(currentRow[3].ToString());
                recordBuilder.Evaporation = float.Parse(currentRow[4].ToString());
                recordBuilder.Sunshine = float.Parse(currentRow[5].ToString());
                recordBuilder.WindGustSpeed = float.Parse(currentRow[6].ToString());
                recordBuilder.WindSpeed = int.Parse(currentRow[7].ToString());
                recordBuilder.Humidity = int.Parse(currentRow[8].ToString());
                recordBuilder.Pressure = float.Parse(currentRow[9].ToString());
                recordBuilder.Cloud = int.Parse(currentRow[10].ToString());
                recordBuilder.Temperature = float.Parse(currentRow[11].ToString());
                recordBuilder.RiskMm = float.Parse(currentRow[12].ToString());

                records.Add(recordBuilder);
            }
            return records;
        }
    }
}
