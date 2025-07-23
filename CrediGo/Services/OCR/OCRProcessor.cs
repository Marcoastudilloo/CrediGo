using Tesseract;

namespace CrediGo.Services.OCR
{
    public class OCRProcessor
    {
        private readonly string _imagePath;
        private readonly string _tessdataPath;

        public OCRProcessor(string imagePath, string tessdataPath)
        {
            _imagePath = imagePath;
            _tessdataPath = tessdataPath;
        }

        public string GetTextFromImage()
        {
            try
            {
                var trainedDataPath = Path.Combine(_tessdataPath, "spa.traineddata");
                Console.WriteLine($"[DEBUG] Tessdata path: {_tessdataPath}");
                Console.WriteLine($"[DEBUG] Archivo existe: {File.Exists(trainedDataPath)}");

                using var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default);
                using var img = Pix.LoadFromFile(_imagePath);
                using var page = engine.Process(img);
                return page.GetText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Excepción en OCRProcessor.GetTextFromImage: " + ex.ToString());
                throw;
            }
        }



    }
}
