using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace BusinessLayer.PivotHelpers
{
    public class PivotBase
    {
        public const long MILLION = 1000000;
        public const long BILLION = 1000*MILLION;
        public static string[] Ranges = new string[]{ "$0-$100 million", "$100-$500 million", "$500 million-$1 billion", "$1-$5 billion", "$5-$10 billion", "$10-$30 billion", "$30-$50 billion", "$50 billion and up" };
        public static long[] RangeNums = new long[]{ 0, 100*MILLION, 500*MILLION, 1*BILLION, 5*BILLION, 10*BILLION,30*BILLION, 50*BILLION, long.MaxValue };
        public static DataTable ProcessClientAssetData(DataTable table, string columnX, string columnY, string columnZ)
        {
            string clientIdentifier = "Client CASID";
            string totalClientsColumn = "Total Clients";
            string totalAssetsColumn = "Total Assets";
            int totalClientsFinal = 0;
            double totalAssetsFinal = 0;

            DataTable resultTable = new DataTable();
            if (columnX == "")
                columnX = table.Columns[0].ColumnName;
            resultTable.Columns.Add(columnY);

            List<string> columnXValues = new List<string>();

            foreach (var range in Ranges)
            {
                if (!columnXValues.Contains(range))
                {
                    columnXValues.Add(range);
                    resultTable.Columns.Add(range);
                }
            }
            resultTable.Columns.Add(totalClientsColumn);
            resultTable.Columns.Add(totalAssetsColumn);

            if (columnY != "" && columnZ != "")
            {
                List<string> columnYValues = new List<string>();
                foreach (DataRow dr in table.Rows)
                {
                    if (!columnYValues.Contains(dr[columnY].ToString()))
                        columnYValues.Add(dr[columnY].ToString());
                }
                columnYValues.Sort();

                DataRow TotalClientsRow = resultTable.NewRow();
                DataRow TotalAssetsRow = resultTable.NewRow();

                TotalClientsRow[columnY] = totalClientsColumn;
                TotalAssetsRow[columnY] = totalAssetsColumn;
                HashSet<long> clients = new HashSet<long>();

                foreach (string columnYValue in columnYValues)
                {
                    DataRow drReturn = resultTable.NewRow();
                    drReturn[0] = columnYValue;
                    DataRow[] rows = table.Select($"[{columnY}]='{columnYValue}'");
                    double totalAssets = 0;
                    int totalClients = 0;

                    foreach (DataRow dr in rows)
                    {
                        try 
                        { 
                            string rowColumnTitle = Ranges[FindRange(double.Parse(dr[columnX].ToString()))];
                            var tempAssets = double.Parse(dr[columnX].ToString());
                            totalAssets += tempAssets;
                            totalAssetsFinal += tempAssets;

                            try
                            {
                                if (TotalAssetsRow[rowColumnTitle].ToString() == "")
                                    TotalAssetsRow[rowColumnTitle] = ConvertToUsdMillion(double.Parse(dr[columnX].ToString()));
                                else
                                    TotalAssetsRow[rowColumnTitle] = ConvertToUsdMillion(ConvertFromUsdMillion(TotalAssetsRow[rowColumnTitle].ToString()) + double.Parse(dr[columnX].ToString()));
                            }
                            catch { }
                            foreach (DataColumn dc in resultTable.Columns)
                            {
                                if (dc.ColumnName == rowColumnTitle)
                                {
                                    if (!clients.Contains(long.Parse(dr[clientIdentifier].ToString())))
                                    {
                                        clients.Add(long.Parse(dr[clientIdentifier].ToString()));
                                        try
                                        {
                                            drReturn[rowColumnTitle] = long.Parse(drReturn[rowColumnTitle].ToString()) + 1;
                                        }
                                        catch
                                        {
                                            drReturn[rowColumnTitle] = 1;
                                        }
                                        totalClients++;
                                        totalClientsFinal++;
                                        if (TotalClientsRow[rowColumnTitle].ToString() == "")
                                            TotalClientsRow[rowColumnTitle] = 1;
                                        else
                                            TotalClientsRow[rowColumnTitle] = int.Parse(TotalClientsRow[rowColumnTitle].ToString()) + 1;
                                    }
                                }
                            }
                        }
                        catch {}
                    }
                    drReturn[totalClientsColumn] = totalClients;
                    drReturn[totalAssetsColumn] = ConvertToUsdMillion(totalAssets);
                    resultTable.Rows.Add(drReturn);
                }
                TotalClientsRow[totalClientsColumn] = totalClientsFinal;
                TotalAssetsRow[totalAssetsColumn] = ConvertToUsdMillion(totalAssetsFinal);

                resultTable.Rows.Add(TotalClientsRow);
                resultTable.Rows.Add(TotalAssetsRow);
            }
            
            var nullValue = "-";
            if (nullValue != "")
            {
                foreach (DataRow dr in resultTable.Rows)
                {
                    foreach (DataColumn dc in resultTable.Columns)
                    {
                        if (dr[dc.ColumnName].ToString() == "")
                            dr[dc.ColumnName] = nullValue;
                    }
                }
            }

            return resultTable;
        }

        public static int FindRange(double number)
        {
            if (number <= RangeNums[0]) return -1;

            for (int i = 0; i < RangeNums.Length-1; i++)
            { 
                if(number>RangeNums[i] && number <= RangeNums[i+1])
                {
                    return i;
                }
            }
            return -1;
        }
        public static string ConvertToUsdMillion(double number)
        {
            return $"${(long)(Math.Round(number / MILLION))}";
        }
        public static double ConvertFromUsdMillion(string number)
        {
            double.TryParse(number.Remove(number.IndexOf('$'),1).Trim(), out double result);
            return result*MILLION;
        }

    }
}
