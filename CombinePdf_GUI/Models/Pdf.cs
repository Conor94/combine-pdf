using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PrismBase.Mvvm;
using System.IO;

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
            mIsDisposed = false;
            Filename = Path.GetFileName(filename);
            Document = PdfReader.Open(filename, PdfDocumentOpenMode.Import);
        }

        public Pdf(Pdf pdf)
        {
            Filename = pdf.Filename;
            Document = pdf.Document;
            IsSelected = pdf.IsSelected;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (mIsDisposed)
            {
                return;
            }

            if (disposing)
            {
                Document.Dispose();
            }
            Document = null;

            mIsDisposed = true;            
        }
        #endregion
    }
}
