using System.Collections.ObjectModel;

namespace Merger;

using NPOI.HSSF.UserModel;
using OfficeOpenXml;

/// <summary>
/// Runs merging operations on Excel files
/// </summary>
public class Merger
{
    public Merger()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public void MergeFiles(ObservableCollection<string> selectedFiles, string outputPath)
    {
        using var outputPackage = new ExcelPackage();
        var outputSheet = outputPackage.Workbook.Worksheets.Add("Results");
            
        // Write column headers
        outputSheet.Cells[1, 1].Value = "ISO NUMBERS";
        outputSheet.Cells[1, 2].Value = "FW_NUMBERS";
        outputSheet.Cells[1, 3].Value = "FW_INCHES";
        outputSheet.Cells[1, 4].Value = "SW_NUMBERS";
        outputSheet.Cells[1, 5].Value = "SW_INCHES";
        outputSheet.Cells[1, 6].Value = "TOTAL_NUMBERS";
        outputSheet.Cells[1, 7].Value = "TOTAL_INCHES";
            
        // Set row
        var currentRow = 2;

        foreach (var filePath in selectedFiles)
        {
            if (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                // Read with EPPlus
                using var inputPackage = new ExcelPackage(new FileInfo(filePath));
                var inputSheet = inputPackage.Workbook.Worksheets[0];
                for (var row = 2; row <= inputSheet.Dimension.Rows; ++row)
                {
                    for (var col = 1; col <= 7; ++col)
                    {
                        outputSheet.Cells[currentRow, col].Value = inputSheet.Cells[row, col].Text;
                    }

                    currentRow++;
                }
            }
            else if (filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var workbook = new HSSFWorkbook(stream);
                var inputSheet = workbook.GetSheetAt(0);

                for (var row = 1; row <= inputSheet.LastRowNum; ++row)
                {
                    var inputRow = inputSheet.GetRow(row);
                    if (inputRow == null) continue;

                    for (var col = 1; col < 7; ++col)
                    {
                        outputSheet.Cells[currentRow, col].Value = inputRow.GetCell(col)?.ToString();
                    }
                }
            }
                
            // Save output file
            outputPackage.SaveAs(new FileInfo(outputPath));
        }
    }
}