using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;
using System;

public static class HMSExcelHelper
{
    public static string[,] ReadExcel(string path)
    {
        string[,] result = null;

        using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            result = ToStringArray(sheet.Cells.Value);

        }
        return result;
    }

    private static string[,] ToStringArray(object arg)
    {
        string[,] result = null;

        if (arg is Array)
        {
            int rank = ((Array)arg).Rank;
            if (rank == 2)
            {
                int columnCount = ((Array)arg).GetLength(1) - 1;
                int rowCount = ((Array)arg).GetLength(0);
                result = new string[rowCount, columnCount];

                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        var value = ((Array)arg).GetValue(i, j);
                        if (value != null)
                            result[i, j] = value.ToString();
                    }
                }
            }
        }

        return result;
    }
}
