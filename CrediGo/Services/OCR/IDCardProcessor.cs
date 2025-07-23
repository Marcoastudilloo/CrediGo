using Tesseract;

namespace CrediGo.Services.OCR
{
    public class IDCardProcessor
    {
        private readonly string _imagePath;
        private readonly string _tessdataPath;

        public IDCardProcessor(string imagePath, string tessdataPath)
        {
            _imagePath = imagePath;
            _tessdataPath = tessdataPath;
        }

        public object ExtractJson()
        {
            try
            {
                var ocr = new OCRProcessor(_imagePath, _tessdataPath);
                var text = ocr.GetTextFromImage();
                var extractor = new DataExtractor(text);
                return extractor.ExtractJson();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Excepción en IDCardProcessor.ExtractJson: " + ex);
                throw;
            }
        }

        public string GetTextFromImage()
        {
            using var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default);
            using var img = Pix.LoadFromFile(_imagePath);
            using var page = engine.Process(img);
            return page.GetText();
        }

        public string GetTsv()
        {
            using var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default);
            using var img = Pix.LoadFromFile(_imagePath);
            using var page = engine.Process(img);
            return page.GetTsvText(1);
        }

    }
}
