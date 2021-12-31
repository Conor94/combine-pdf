using PrismMvvmBase.Bindable;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CombinePdf_GUI.Extensions
{
    /// <summary>
    /// Adds methods to move an element up and down within an <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static class ObservableCollectionExtension
    {
        public static void MoveUp<T>(this ObservableCollection<T> items, List<T> selectedItems)
        {
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                if (selectedItems.Contains(items[i]))
                {                    
                    if (i != 0)
                    {
                        T previousItem = items[i - 1]; // keep a copy of the previous item
                        int previousItemIndex = items.IndexOf(items[i - 1]);
                        items.RemoveAt(previousItemIndex); // Remove the previous item
                        items.Insert(previousItemIndex + 1, previousItem);
                    }
                    else
                    {
                        // Add the first element to the end
                        items.Add(items[0]);
                        // Remove the first element
                        items.RemoveAt(0);
                        break; // Stop moving items down. An item has been removed from the bottom of the list, which causes everything above it to move down.
                    }
                }
            }
        }

        public static ObservableCollection<T> MoveDown<T>(this ObservableCollection<T> items, List<T> selectedItems)
        {
            int count = items.Count;
            for (int i = (count - 1); i >= 0; i--)
            {
                if (selectedItems.Contains(items[i]))
                {
                    if (i != (count - 1))
                    {
                        T currentItem = items[i];
                        int currentItemIndex = items.IndexOf(items[i]);
                        items.RemoveAt(currentItemIndex); // Remove the next item
                        items.Insert(currentItemIndex + 1, currentItem);
                    }
                    else if (i == (count - 1))
                    {
                        // Add the last element to the front
                        items.Insert(0, items[count - 1]);

                        // Remove the last element
                        items.RemoveAt(count);
                        break; // Stop moving items down. An item has been removed from the bottom of the list, which causes everything above it to move down.
                    }
                }
            }
            return items;
        }

        public static void ShiftSelectItem<T>(this ObservableCollection<T> items, List<T> selectedItems) where T : ModelBase
        {
            List<T> itemsList = items.ToList();

            // Get the indexes of the first and last Pdf
            int firstIndex = itemsList.IndexOf(selectedItems[0]);
            int lastIndex = itemsList.IndexOf(selectedItems[selectedItems.Count - 1]);

            if (firstIndex < lastIndex)
            {
                // Select all PDFs between the first and last index
                while (firstIndex < lastIndex)
                {
                    items.ElementAt(firstIndex).IsSelected = true;
                    firstIndex++;
                }
            }
            else
            {
                // Select all PDFs between the first and last index
                while (firstIndex > lastIndex)
                {
                    items.ElementAt(firstIndex).IsSelected = true;
                    firstIndex--;
                }
            }
        }

        /// <summary>Selects the most recently selected item and unselects all others.</summary>
        public static void SelectItem<T>(this ObservableCollection<T> items, List<T> selectedItems) where T : ModelBase
        {
            // Unselect all PDFs except the most recently selected (most recently selected is the last element in the list)
            if (items != null &&
                selectedItems.Count > 0 &&
                items.Contains(selectedItems[0]))
            {
                items.Remove(selectedItems[0]);
                selectedItems[0].IsSelected = false;
            }
            else if (items != null && items.Count() > 0)
            {
                selectedItems.Add(items.ElementAt(0));
            }
        }
    }
}
