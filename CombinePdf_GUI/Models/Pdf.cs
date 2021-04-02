using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PrismBase.Mvvm;
using System.IO;
using System.Windows.Controls;

namespace CombinePdf_GUI.Models
{
    public class Pdf : ModelBase
    {
        #region Fields and properties
        private string mFilename;
        private PdfDocument mPdfDocument;
        private bool mIsSelected;

        public string Filename
        {
            get => mFilename;
            set => SetProperty(ref mFilename, value);
        }
        public PdfDocument Document
        {
            get => mPdfDocument;
            set => SetProperty(ref mPdfDocument, value);
        }
        public bool IsSelected
        {
            get => mIsSelected;
            set => SetProperty(ref mIsSelected, value);
        }
        #endregion

        #region Constructor
        public Pdf(string filename)
        {
            Filename = Path.GetFileName(filename);
            Document = PdfReader.Open(filename, PdfDocumentOpenMode.Import);
        }
        #endregion
    }
}
