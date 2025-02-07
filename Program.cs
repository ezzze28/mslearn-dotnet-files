using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesSummaryDir = Path.Combine(currentDirectory, "salesSummaryDir");
Directory.CreateDirectory(salesSummaryDir);

var salesFiles = FindSalesFiles(storesDirectory);
GenerateSalesSummary(salesFiles, Path.Combine(salesSummaryDir, "SalesSummary.txt"));

IEnumerable<string> FindSalesFiles(string rootFolder)
{
    List<string> salesFiles = new List<string>();

    var directories = Directory.GetDirectories(rootFolder);

    foreach (var dir in directories)
    {
        var foundFiles = Directory.EnumerateFiles(dir, "sales.json", SearchOption.TopDirectoryOnly);

        foreach (var file in foundFiles)
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

void GenerateSalesSummary(IEnumerable<string> salesFiles, string summaryFilePath)
{
    StringBuilder report = new StringBuilder();
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");

    double totalSales = 0;
    report.AppendLine("\n Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        if (data != null)
        {
            totalSales += data.Total;
            report.AppendLine($"  {Path.GetFileName(Path.GetDirectoryName(file))}/{Path.GetFileName(file)}: {data.Total.ToString("C")}");
        }
        else
        {
            report.AppendLine($"  {Path.GetFileName(Path.GetDirectoryName(file))}/{Path.GetFileName(file)}: Format error");
        }
    }

    report.Insert(2, $"\n Total Sales: {totalSales.ToString("C")}\n");

    File.WriteAllText(summaryFilePath, report.ToString());
}

record SalesData(double Total);
