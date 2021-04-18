﻿using CombinePdf_GUI.Models;
using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using PrismBase.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using DotNetExtension;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using CombinePdf_GUI.Extensions;
using System.Diagnostics;
using System;
using System.Windows.Input;

namespace CombinePdf_GUI.ViewModels
{
    public class CombinePdfViewModel : ViewModelBase
    {
        #region Fields and properties        
        // Commands
        private DelegateCommand mAddPdfCommand;
        private DelegateCommand mCombinePdfCommand;
        private DelegateCommand<ListView> mRemovePdfCommand;
        private DelegateCommand<IList> mPdfSelectedCommand;
        private DelegateCommand<ListView> mMoveUpCommand;
        private DelegateCommand<ListView> mMoveDownCommand;
        private DelegateCommand<ListView> mSelectAllPdfsCommand;
        private DelegateCommand<ListView> mUnselectAllPdfsCommand;
        private DelegateCommand<KeyEventArgs> mKeyUpCommand;
        private DelegateCommand<KeyEventArgs> mKeyDownCommand;
        // Data
        private ObservableCollection<Pdf> mPdfList;
        private IEnumerable<Pdf> mSelectedPdfs;
        private bool mIsOpenPdfAfterCombine;
        private bool mIsOpenFileExplorerAfterCombine;
        private bool mIsShiftPressed;
        private bool mIsSelectionHandled;
        private bool mIsSelectAll;

        // Commands
        public DelegateCommand AddPdfCommand
        {
            get => mAddPdfCommand ?? (mAddPdfCommand = new DelegateCommand(AddPdfExecute));
            set => mAddPdfCommand = value;
        }
        public DelegateCommand CombinePdfCommand
        {
            get => mCombinePdfCommand ?? (mCombinePdfCommand = new DelegateCommand(CombinePdfExecute, CombinePdfCanExecute));
            set => mCombinePdfCommand = value;
        }
        public DelegateCommand<ListView> RemovePdfCommand
        {
            get => mRemovePdfCommand ?? (mRemovePdfCommand = new DelegateCommand<ListView>(RemovePdfExecute, RemovePdfCanExecute));
            set => mRemovePdfCommand = value;
        }
        public DelegateCommand<IList> PdfSelectedCommand
        {
            get => mPdfSelectedCommand ?? (mPdfSelectedCommand = new DelegateCommand<IList>(PdfSelectedExecute));
            set => mPdfSelectedCommand = value;
        }
        public DelegateCommand<ListView> MoveUpCommand
        {
            get => mMoveUpCommand ?? (mMoveUpCommand = new DelegateCommand<ListView>(MoveUpExecute, MoveUpCanExecute));
            set => mMoveUpCommand = value;
        }
        public DelegateCommand<ListView> MoveDownCommand
        {
            get => mMoveDownCommand ?? (mMoveDownCommand = new DelegateCommand<ListView>(MoveDownExecute, MoveDownCanExecute));
            set => mMoveDownCommand = value;
        }
        public DelegateCommand<ListView> SelectAllPdfsCommand
        {
            get => mSelectAllPdfsCommand ?? (mSelectAllPdfsCommand = new DelegateCommand<ListView>(SelectAllPdfsExecute, SelectAllPdfsCanExecute));
            set => mSelectAllPdfsCommand = value;
        }
        public DelegateCommand<ListView> UnselectAllPdfsCommand
        {
            get => mUnselectAllPdfsCommand ?? (mUnselectAllPdfsCommand = new DelegateCommand<ListView>(UnselectAllPdfsExecute, UnselectAllPdfsCanExecute));
            set => mUnselectAllPdfsCommand = value;
        }
        public DelegateCommand<KeyEventArgs> KeyUpCommand
        {
            get => mKeyUpCommand ?? (mKeyUpCommand = new DelegateCommand<KeyEventArgs>(KeyUpExecute));
            set => mKeyUpCommand = value;
        }
        public DelegateCommand<KeyEventArgs> KeyDownCommand
        {
            get => mKeyDownCommand ?? (mKeyDownCommand = new DelegateCommand<KeyEventArgs>(KeyDownExecute));
            set => mKeyDownCommand = value;
        }

        // Data
        public ObservableCollection<Pdf> PdfList
        {
            get => mPdfList;
            set => SetProperty(ref mPdfList, value);
        }
        public IEnumerable<Pdf> SelectedPdfs
        {
            get => mSelectedPdfs;
            set
            {
                //SetProperty(ref mSelectedPdfs, value);
                mSelectedPdfs = value;
                RemovePdfCommand.RaiseCanExecuteChanged();
            }
        }
        public bool IsOpenPdfAfterCombine
        {
            get => mIsOpenPdfAfterCombine;
            set => SetProperty(ref mIsOpenPdfAfterCombine, value);
        }
        public bool IsOpenFileExplorerAfterCombine
        {
            get => mIsOpenFileExplorerAfterCombine;
            set => SetProperty(ref mIsOpenFileExplorerAfterCombine, value);
        }
        public bool IsShiftPressed
        {
            get => mIsShiftPressed;
            set => SetProperty(ref mIsShiftPressed, value);
        }
        #endregion

        #region Constructor
        public CombinePdfViewModel(IEventAggregator eventAggregator, IContainerProvider container) : base(eventAggregator, container)
        {
            PdfList = new ObservableCollection<Pdf>();
            PdfList.CollectionChanged += SelectedPdfs_OnCollectionChanged;

            mIsSelectionHandled = false;
            mIsSelectAll = false;
        }
        #endregion

        #region Command methods
        private void AddPdfExecute()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Select PDF",
                Filter = "Adobe PDF Files|*.pdf",
                Multiselect = true,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (string filename in dialog.FileNames)
                {
                    // Add the filename to the listview if the file exists and is a PDF
                    if (File.Exists(filename) && PdfReader.TestPdfFile(filename) != 0)
                    {
                        PdfList.Add(new Pdf(filename));
                    }
                }
            }
        }

        private void CombinePdfExecute()
        {
            // Loop through all PDF
            using (PdfDocument combinedPdf = new PdfDocument())
            {
                foreach (Pdf pdf in PdfList)
                {
                    combinedPdf.AddPdf(pdf.Document);
                }

                SaveFileDialog dialog = new SaveFileDialog()
                {
                    Title = "Save PDF",
                    Filter = "Adobe PDF Files|*.pdf",
                    CheckPathExists = true
                };

                if (dialog.ShowDialog() == true)
                {
                    combinedPdf.Save(dialog.FileName);

                    using (Process p = new Process())
                    {
                        if (IsOpenFileExplorerAfterCombine)
                        {
                            // Open the folder in File Explorer
                            p.StartInfo = new ProcessStartInfo($"cmd.exe")
                            {
                                WindowStyle = ProcessWindowStyle.Hidden,
                                Arguments = $"/c explorer {Path.GetDirectoryName(dialog.FileName)}"
                            };
                            p.Start();
                        }
                        if (IsOpenPdfAfterCombine)
                        {
                            // Open the PDF
                            p.StartInfo = new ProcessStartInfo(dialog.FileName);
                            p.Start();
                        }
                    }
                }
            }
        }
        private bool CombinePdfCanExecute()
        {
            return PdfList.Count > 0;
        }

        private void RemovePdfExecute(ListView listView)
        {
            IEnumerable<Pdf> selectedPdfs = listView.SelectedItems.Cast<Pdf>();
            foreach (Pdf pdf in selectedPdfs.ToList())
            {
                pdf.Dispose();
                PdfList.Remove(pdf);
            }
            GC.Collect();
            listView.Focus();
        }
        private bool RemovePdfCanExecute(ListView listView)
        {
            return listView.SelectedItems.Count > 0;
        }

        private void PdfSelectedExecute(IList tmpSelectedPdfs)
        {
            if (!mIsSelectAll)
            {
                if (!IsShiftPressed)
                {
                    // Unselect all PDFs except the most recently selected (most recently selected is the last element in the list)
                    if (SelectedPdfs != null &&
                        tmpSelectedPdfs.Count > 0 &&
                        SelectedPdfs.Contains(tmpSelectedPdfs[0]) &&
                        !mIsSelectionHandled)
                    {
                        List<Pdf> list = SelectedPdfs.ToList();
                        list.Remove((Pdf)tmpSelectedPdfs[0]);
                        SelectedPdfs = list;
                        ((Pdf)tmpSelectedPdfs[0]).IsSelected = false;
                    }
                    else if (SelectedPdfs != null && SelectedPdfs.Count() > 0)
                    {
                        mIsSelectionHandled = true;
                        tmpSelectedPdfs.Add(SelectedPdfs.ElementAt(0));
                    }
                }
                else
                {
                    List<Pdf> pdfList = PdfList.ToList();
                    int firstIndex = pdfList.IndexOf((Pdf)tmpSelectedPdfs[0]);
                    int lastIndex = pdfList.IndexOf((Pdf)tmpSelectedPdfs[tmpSelectedPdfs.Count - 1]);
                    if (firstIndex < lastIndex)
                    {
                        // Select all PDFs between the first and last index
                        while (firstIndex < lastIndex)
                        {
                            PdfList.ElementAt(firstIndex).IsSelected = true;
                            firstIndex++;
                        }
                    }
                    else
                    {
                        // Select all PDFs between the first and last index
                        while (firstIndex > lastIndex)
                        {
                            PdfList.ElementAt(firstIndex).IsSelected = true;
                            firstIndex--;
                        }
                    }
                }

                mIsSelectionHandled = false;
            }
            else
            {
                mIsSelectAll = false;
            }
            
            SelectedPdfs = new List<Pdf>(tmpSelectedPdfs.Cast<Pdf>()); 
        }

        private void MoveUpExecute(ListView listView)
        {
            // Keep a list of the selected backups
            List<Pdf> selectedPdfs = listView.SelectedItems.Cast<Pdf>().ToList();

            // Move the selected PDFs up
            PdfList.MoveUp(selectedPdfs);

            // Reselect the PDFs
            ReselectPdfs(selectedPdfs);
            listView.Focus();
        }
        private bool MoveUpCanExecute(ListView listview)
        {
            return PdfList.Count > 0;
        }

        private void MoveDownExecute(ListView listView)
        {
            // Keep a list of the selected backups            
            List<Pdf> selectedPdfs = listView.SelectedItems.Cast<Pdf>().ToList();

            // Move the selected PDFs down
            PdfList = PdfList.MoveDown(selectedPdfs);

            // Reselect the PDFs
            ReselectPdfs(selectedPdfs);
            listView.Focus();
        }
        private bool MoveDownCanExecute(ListView listView)
        {
            return PdfList.Count > 0;
        }

        private void SelectAllPdfsExecute(ListView listView)
        {
            mIsSelectAll = true;
            listView.SelectAll();
            listView.Focus();
        }
        private bool SelectAllPdfsCanExecute(ListView listView)
        {
            return PdfList.Count > 0;
        }

        private void UnselectAllPdfsExecute(ListView listView)
        {
            listView.UnselectAll();
            listView.Focus();
        }
        private bool UnselectAllPdfsCanExecute(ListView listView)
        {
            return PdfList.Count > 0;
        }

        private void KeyUpExecute(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                IsShiftPressed = false;
            }
        }

        private void KeyDownExecute(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                IsShiftPressed = true;
            }
        }
        #endregion

        #region Event handlers
        private void SelectedPdfs_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CombinePdfCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Helpers
        private static void ReselectPdfs(List<Pdf> pdfs)
        {
            for (int i = 0; i < pdfs.Count; i++)
            {
                pdfs[i].IsSelected = true;
            }
        }
        #endregion
    }
}