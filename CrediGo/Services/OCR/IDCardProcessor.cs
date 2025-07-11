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
            var ocr = new OCRProcessor(_imagePath, _tessdataPath);
            var text = ocr.GetTextFromImage();
            var extractor = new DataExtractor(text);
            return extractor.ExtractJson();
        }
    }
}
