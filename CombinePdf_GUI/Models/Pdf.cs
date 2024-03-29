﻿using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PrismMvvmBase.Bindable;
using System.IO;

namespace CombinePdf_GUI.Models
{
    public class Pdf : ModelBase
    {
        #region Fields and properties
        private string mFilename;
        private PdfDocument mPdfDocument;

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
