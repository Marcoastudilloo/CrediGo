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
            Console.WriteLine($"[DEBUG] Tessdata path: {_tessdataPath}");
            Console.WriteLine($"[DEBUG] Archivo existe: {System.IO.File.Exists(Path.Combine(_tessdataPath, "spa.traineddata"))}");

            using var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default);
            using var img = Pix.LoadFromFile(_imagePath);
            using var page = engine.Process(img);
            return page.GetText();

        }
    }
}
