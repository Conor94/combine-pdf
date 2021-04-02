using PdfSharp.Pdf;

namespace DotNetExtension
{
    public static class PdfDocumentExtension
    {
        public static void AddPdf(this PdfDocument pdf, PdfDocument pdfDocument)
        {
            foreach (PdfPage page in pdfDocument.Pages)
            {
                pdf.AddPage(page);
            }
        }
    }
}
