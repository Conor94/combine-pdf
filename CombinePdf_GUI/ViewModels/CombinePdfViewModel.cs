using CombinePdf_GUI.Extensions;
using CombinePdf_GUI.Models;
using DotNetExtension;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using PrismBase.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CombinePdf_GUI.ViewModels
{
    public class CombinePdfViewModel : ViewModelBase
    {
        #region Fields
        // Commands
        private DelegateCommand mAddPdfCommand;
        private DelegateCommand mSelectFolderCommand;
        private DelegateCommand mSaveCommand;
        private DelegateCommand<System.Windows.Controls.ListView> mRemovePdfCommand;
        private DelegateCommand<IList> mPdfSelectedCommand;
        private DelegateCommand<System.Windows.Controls.ListView> mMoveUpCommand;
        private DelegateCommand<System.Windows.Controls.ListView> mMoveDownCommand;
        private DelegateCommand<System.Windows.Controls.ListView> mSelectAllPdfsCommand;
        private DelegateCommand<System.Windows.Controls.ListView> mUnselectAllPdfsCommand;
        private DelegateCommand<System.Windows.Input.KeyEventArgs> mKeyUpCommand;
        private DelegateCommand<System.Windows.Input.KeyEventArgs> mKeyDownCommand;
        private DelegateCommand<System.Windows.DragEventArgs> mPdfDropCommand;
        // Data
        private ObservableCollection<Pdf> mPdfList;
        private IEnumerable<Pdf> mSelectedPdfs;
        private string mSelectedFolderPath;
        private string mSelectedFilename;
        private bool mIsOpenPdfAfterCombine;
        private bool mIsOpenFileExplorerAfterCombine;
        private bool mIsShiftPressed;
        private bool mIsCtrlPressed;
        private bool mIsSelectAll;
        private bool mIsSelectionHandled;
        #endregion

        #region Properties
        // Commands
        public DelegateCommand AddPdfCommand
        {
            get => mAddPdfCommand ?? (mAddPdfCommand = new DelegateCommand(AddPdfExecute));
            set => mAddPdfCommand = value;
        }
        public DelegateCommand SelectFolderCommand
        {
            get => mSelectFolderCommand ?? (mSelectFolderCommand = new DelegateCommand(SelectFolderExecute));
            set => mSelectFolderCommand = value;
        }
        public DelegateCommand SaveCommand
        {
            get => mSaveCommand ?? (mSaveCommand = new DelegateCommand(SaveExecute, SaveCanExecute));
            set => mSaveCommand = value;
        }
        public DelegateCommand<System.Windows.Controls.ListView> RemovePdfCommand
        {
            get => mRemovePdfCommand ?? (mRemovePdfCommand = new DelegateCommand<System.Windows.Controls.ListView>(RemovePdfExecute, RemovePdfCanExecute));
            set => mRemovePdfCommand = value;
        }
        public DelegateCommand<IList> PdfSelectedCommand
        {
            get => mPdfSelectedCommand ?? (mPdfSelectedCommand = new DelegateCommand<IList>(PdfSelectedExecute));
            set => mPdfSelectedCommand = value;
        }
        public DelegateCommand<System.Windows.Controls.ListView> MoveUpCommand
        {
            get => mMoveUpCommand ?? (mMoveUpCommand = new DelegateCommand<System.Windows.Controls.ListView>(MoveUpExecute, MoveUpCanExecute));
            set => mMoveUpCommand = value;
        }
        public DelegateCommand<System.Windows.Controls.ListView> MoveDownCommand
        {
            get => mMoveDownCommand ?? (mMoveDownCommand = new DelegateCommand<System.Windows.Controls.ListView>(MoveDownExecute, MoveDownCanExecute));
            set => mMoveDownCommand = value;
        }
        public DelegateCommand<System.Windows.Controls.ListView> SelectAllPdfsCommand
        {
            get => mSelectAllPdfsCommand ?? (mSelectAllPdfsCommand = new DelegateCommand<System.Windows.Controls.ListView>(SelectAllPdfsExecute, SelectAllPdfsCanExecute));
            set => mSelectAllPdfsCommand = value;
        }
        public DelegateCommand<System.Windows.Controls.ListView> UnselectAllPdfsCommand
        {
            get => mUnselectAllPdfsCommand ?? (mUnselectAllPdfsCommand = new DelegateCommand<System.Windows.Controls.ListView>(UnselectAllPdfsExecute, UnselectAllPdfsCanExecute));
            set => mUnselectAllPdfsCommand = value;
        }
        public DelegateCommand<System.Windows.Input.KeyEventArgs> KeyUpCommand
        {
            get => mKeyUpCommand ?? (mKeyUpCommand = new DelegateCommand<System.Windows.Input.KeyEventArgs>(KeyUpExecute));
            set => mKeyUpCommand = value;
        }
        public DelegateCommand<System.Windows.Input.KeyEventArgs> KeyDownCommand
        {
            get => mKeyDownCommand ?? (mKeyDownCommand = new DelegateCommand<System.Windows.Input.KeyEventArgs>(KeyDownExecute));
            set => mKeyDownCommand = value;
        }
        public DelegateCommand<System.Windows.DragEventArgs> PdfDropCommand
        {
            get => mPdfDropCommand ?? (mPdfDropCommand = new DelegateCommand<System.Windows.DragEventArgs>(PdfDropExecute));
            set => mPdfDropCommand = value;
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
        public string SelectedFolderPath
        {
            get => mSelectedFolderPath;
            set
            {
                SetProperty(ref mSelectedFolderPath, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public string SelectedFilename
        {
            get => mSelectedFilename;
            set
            {
                SetProperty(ref mSelectedFilename, value);
                SaveCommand.RaiseCanExecuteChanged();
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
        public bool IsCtrlPressed
        {
            get => mIsCtrlPressed;
            set => SetProperty(ref mIsCtrlPressed, value);
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
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog()
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

        private void SelectFolderExecute()
        {
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {
                AllowPropertyEditing = false,
                IsFolderPicker = true,
                Title = "Select Folder"
            };

            Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult dialogResult = dialog.ShowDialog();
            if (dialogResult == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                SelectedFolderPath = dialog.FileName;
            }
        }

        private void SaveExecute()
        {
            if (Directory.Exists(SelectedFolderPath) == false)
            {
                System.Windows.MessageBox.Show("The selected folder path does not exist.",
                                               "Invalid folder path",
                                               System.Windows.MessageBoxButton.OK,
                                               System.Windows.MessageBoxImage.Error);
                return;
            }


            using (PdfDocument combinedPdf = new PdfDocument())
            {
                foreach (Pdf pdf in PdfList)
                {
                    combinedPdf.AddPdf(pdf.Document);
                }
                string fullFilePath = $@"{SelectedFolderPath}\{SelectedFilename}.pdf";
                combinedPdf.Save(fullFilePath);

                using (Process p = new Process())
                {
                    if (IsOpenFileExplorerAfterCombine)
                    {
                        // Open the folder in File Explorer
                        p.StartInfo = new ProcessStartInfo($"cmd.exe")
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            Arguments = $"/c explorer {Path.GetDirectoryName(SelectedFolderPath)}"
                        };
                        p.Start();
                    }
                    if (IsOpenPdfAfterCombine)
                    {
                        // Open the PDF
                        p.StartInfo = new ProcessStartInfo(fullFilePath);
                        p.Start();
                    }
                }
            }
        }
        private bool SaveCanExecute()
        {
            return PdfList.Count > 0 &&
                   string.IsNullOrWhiteSpace(SelectedFolderPath) == false &&
                   string.IsNullOrWhiteSpace(SelectedFilename) == false;
        }

        private void RemovePdfExecute(System.Windows.Controls.ListView listView)
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
        private bool RemovePdfCanExecute(System.Windows.Controls.ListView listView)
        {
            return listView.SelectedItems.Count > 0;
        }

        private void PdfSelectedExecute(IList tmpSelectedPdfs)
        {
            if (!mIsSelectAll)
            {
                if (IsShiftPressed && !IsCtrlPressed)
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
                else if (!IsCtrlPressed && !IsShiftPressed)
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

                mIsSelectionHandled = false;
            }
            else
            {
                mIsSelectAll = false;
            }

            SelectedPdfs = new List<Pdf>(tmpSelectedPdfs.Cast<Pdf>());
        }

        private void MoveUpExecute(System.Windows.Controls.ListView listView)
        {
            // Keep a list of the selected backups
            List<Pdf> selectedPdfs = listView.SelectedItems.Cast<Pdf>().ToList();

            // Move the selected PDFs up
            PdfList.MoveUp(selectedPdfs);

            // Reselect the PDFs
            ReselectPdfs(selectedPdfs);
            listView.Focus();
        }
        private bool MoveUpCanExecute(System.Windows.Controls.ListView listview)
        {
            return PdfList.Count > 0;
        }

        private void MoveDownExecute(System.Windows.Controls.ListView listView)
        {
            // Keep a list of the selected backups            
            List<Pdf> selectedPdfs = listView.SelectedItems.Cast<Pdf>().ToList();

            // Move the selected PDFs down
            PdfList = PdfList.MoveDown(selectedPdfs);

            // Reselect the PDFs
            ReselectPdfs(selectedPdfs);
            listView.Focus();
        }
        private bool MoveDownCanExecute(System.Windows.Controls.ListView listView)
        {
            return PdfList.Count > 0;
        }

        private void SelectAllPdfsExecute(System.Windows.Controls.ListView listView)
        {
            mIsSelectAll = true;
            listView.SelectAll();
            listView.Focus();
        }
        private bool SelectAllPdfsCanExecute(System.Windows.Controls.ListView listView)
        {
            return PdfList.Count > 0;
        }

        private void UnselectAllPdfsExecute(System.Windows.Controls.ListView listView)
        {
            listView.UnselectAll();
            listView.Focus();
        }
        private bool UnselectAllPdfsCanExecute(System.Windows.Controls.ListView listView)
        {
            return PdfList.Count > 0;
        }

        private void KeyUpExecute(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftShift || e.Key == System.Windows.Input.Key.RightShift)
            {
                IsShiftPressed = false;
            }
            else if (e.Key == System.Windows.Input.Key.LeftCtrl || e.Key == System.Windows.Input.Key.RightCtrl)
            {
                IsCtrlPressed = false;
            }
        }

        private void KeyDownExecute(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftShift || e.Key == System.Windows.Input.Key.RightShift)
            {
                IsShiftPressed = true;
            }
            else if (e.Key == System.Windows.Input.Key.LeftCtrl || e.Key == System.Windows.Input.Key.RightCtrl)
            {
                IsCtrlPressed = true;
            }
        }

        private void PdfDropExecute(System.Windows.DragEventArgs e)
        {
            if (e.Data is System.Windows.DataObject data)
            {
                if (data.ContainsFileDropList())
                {
                    StringCollection files = data.GetFileDropList();

                    int fileCount = files.Count;
                    for (int i = 0; i < fileCount; i++)
                    {
                        Pdf pdf = new Pdf(files[i]);
                        PdfList.Add(pdf);
                    }
                }
            }
        }
        #endregion

        #region Event handlers
        private void SelectedPdfs_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
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