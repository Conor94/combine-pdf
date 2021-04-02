using DotNetExtension;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;

namespace CombinePdf_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check for arguments
            if (args != null && args.Length > 0)
            {
                // Check for a output filename
                string outputFilename = "";
                int index = Array.IndexOf(args, "-o");
                if (index != -1 && (args.Length > index + 1))
                {
                    outputFilename = args[index + 1];
                }
                else
                {
                    outputFilename = @".\combined.pdf";
                }

                // Combine the PDFs
                PdfDocument combinedPdf = new PdfDocument();                
                foreach (string arg in args)
                {
                    if ((PdfReader.TestPdfFile(arg) != 0) && (arg != outputFilename))
                    {
                        PdfDocument pdf = PdfReader.Open(arg, PdfDocumentOpenMode.Import);
                        combinedPdf.AddPdf(pdf);
                    }                
                }

                // Save the PDF
                if (combinedPdf.PageCount > 0)
                {
                    combinedPdf.Save(outputFilename); 
                }
                else
                {
                    Console.WriteLine("No PDF files to merge.");
                }
            }
            else
            {
                Console.WriteLine("No arguments entered.");
            }
        }
    }
}
