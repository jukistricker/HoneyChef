using IOITCore.Models.ViewModels;

namespace IOITCore.Services.Interfaces
{
    public interface IOfficeService
    {
        public string ExportWord(string path, List<WordDTO> replacements);
        public MemoryStream ExportExcelTemplate<T>(string filetemplate, int sheetNumber, int datacol, int rowstart, List<T> data, List<string> fields);
        public List<Dictionary<string, string>> ImportExcel(string filePath, int sheetIndex, int startRow, int columnCount);
        public MemoryStream ExportExcelNoTemplate<T>(List<string> listheader, int sheetNumber, int datacol, int rowstart, List<T> data, List<string> fields);
    }
}
