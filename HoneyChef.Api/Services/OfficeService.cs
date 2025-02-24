//using DevExpress.Spreadsheet;
//using DevExpress.XtraRichEdit;
//using IOITCore.Models.ViewModels;
//using IOITCore.Services.Interfaces;
//using System.Drawing;

//namespace IOITCore.Services
//{
//    public class OfficeService : IOfficeService
//    {
//        private static string GenerateFileName(string baseName, string extension) =>
//            $"{baseName}_{DateTime.Now:MM_yyyy_HHmmss}{extension}";

//        public string ExportWord(string file, List<WordDTO> replacements)
//        {
//            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
//                return "File template không tồn tại.";

//            if (replacements == null || replacements.Count == 0)
//                return "Không có dữ liệu được truyền vào.";

//            string outputFileName = GenerateFileName("res", ".docx");

//            try
//            {
//                using var wordProcessor = new RichEditDocumentServer();
//                wordProcessor.LoadDocumentTemplate(file, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

//                var document = wordProcessor.Document;
//                document.BeginUpdate();
//                replacements.ForEach(r => document.ReplaceAll(r.Key, r.Value, DevExpress.XtraRichEdit.API.Native.SearchOptions.None));
//                document.EndUpdate();

//                wordProcessor.SaveDocument(outputFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
//                return outputFileName;
//            }
//            catch (Exception ex)
//            {
//                return $"Lỗi: {ex.Message}";
//            }
//        }

//        public List<Dictionary<string, string>> ImportExcel(string filePath, int sheetIndex, int startRow, int columnCount)
//        {
//            if (!File.Exists(filePath))
//                throw new FileNotFoundException("Không tìm thấy tệp Excel.", filePath);

//            var dataList = new List<Dictionary<string, string>>();
//            using var workbook = new Workbook();
//            workbook.LoadDocument(filePath);

//            if (sheetIndex < 0 || sheetIndex >= workbook.Worksheets.Count)
//                throw new ArgumentOutOfRangeException(nameof(sheetIndex), "Chỉ mục sheet không hợp lệ.");

//            var sheet = workbook.Worksheets[sheetIndex];
//            int lastRow = sheet.Rows.LastUsedIndex;

//            if (startRow > lastRow)
//                throw new ArgumentException("startRow lớn hơn số dòng thực tế trong sheet.");

//            for (int row = startRow; row <= lastRow; row++)
//            {
//                var rowData = new Dictionary<string, string>();
//                for (int col = 0; col < Math.Min(columnCount, sheet.Columns.LastUsedIndex + 1); col++)
//                {
//                    rowData[$"col{col}"] = sheet.Cells[row, col].Value.IsEmpty ? "" : sheet.Cells[row, col].Value.ToString();
//                }
//                dataList.Add(rowData);
//            }
//            return dataList;
//        }

//        public MemoryStream ExportExcelTemplate<T>(string fileTemplate, int sheetNumber, int dataCol, int rowStart, List<T> data, List<string> fields)
//        {
//            ValidateExcelTemplate(fileTemplate, data, fields);
//            using var workbook = new Workbook();
//            workbook.LoadDocument(fileTemplate);
//            var sheet = workbook.Worksheets[sheetNumber];

//            int rowCounter = rowStart, recordIndex = 1;
//            foreach (var item in data)
//            {
//                PopulateRow(sheet, rowCounter++, dataCol, item, fields, recordIndex++);
//            }

//            return SaveWorkbookToStream(workbook);
//        }

//        public MemoryStream ExportExcelNoTemplate<T>(List<string> listHeader, int sheetNumber, int dataCol, int rowStart, List<T> data, List<string> fields)
//        {
//            using var workbook = new Workbook();
//            var sheet = workbook.Worksheets[sheetNumber];

//            PopulateHeader(sheet, rowStart, listHeader);
//            int rowCounter = rowStart + 1, recordIndex = 1;

//            foreach (var item in data)
//            {
//                PopulateRow(sheet, rowCounter++, dataCol, item, fields, recordIndex++);
//            }
//            return SaveWorkbookToStream(workbook);
//        }

//        private static void ValidateExcelTemplate<T>(string fileTemplate, List<T> data, List<string> fields)
//        {
//            if (string.IsNullOrWhiteSpace(fileTemplate) || !File.Exists(fileTemplate))
//                throw new FileNotFoundException("Không tìm thấy file template Excel.", fileTemplate);
//            if (data == null || data.Count == 0 || fields == null || fields.Count == 0)
//                throw new ArgumentException("Dữ liệu hoặc danh sách fields không hợp lệ.");
//        }

//        private static void PopulateHeader(Worksheet sheet, int rowStart, List<string> headers)
//        {
//            for (int i = 0; i <= headers.Count; i++)
//            {
//                var cell = sheet.Cells[rowStart, i];
//                cell.Value = i == 0 ? "STT" : headers[i - 1];
//                cell.Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);
//            }
//        }

//        private static void PopulateRow<T>(Worksheet sheet, int rowIndex, int dataCol, T item, List<string> fields, int recordIndex)
//        {
//            for (int col = 0; col < dataCol; col++)
//            {
//                var cell = sheet.Cells[rowIndex, col];
//                cell.Value = col == 0 ? recordIndex : GetPropertyValue(item, fields[col - 1]);
//                cell.CopyFrom(sheet.Cells[rowIndex - 1, col], PasteSpecial.Formats);
//            }
//        }

//        private static string GetPropertyValue<T>(T item, string propertyName)
//        {
//            return item.GetType().GetProperty(propertyName)?.GetValue(item, null)?.ToString() ?? "";
//        }

//        private static MemoryStream SaveWorkbookToStream(Workbook workbook)
//        {
//            var stream = new MemoryStream();
//            workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
//            stream.Position = 0;
//            return stream;
//        }
//    }
//}
