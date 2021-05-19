using System;

namespace PrismBase.Mvvm
{
    public abstract class ModelBase : DataErrorBindableBase, IDisposable
    {
        private bool mIsSelected;
        public bool IsSelected
        {
            get => mIsSelected;
            set => SetProperty(ref mIsSelected, value);
        }


        public ModelBase() : base()
        {
            mIsSelected = false;
        }


        // IDisposable
        protected bool mIsDisposed;

        public void Dispose() => Dispose(true);

        protected abstract void Dispose(bool disposing);
    }
}
