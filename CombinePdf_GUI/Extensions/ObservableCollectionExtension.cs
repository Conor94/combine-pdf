using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CombinePdf_GUI.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static void MoveUp<T>(this ObservableCollection<T> items, List<T> selectedItems)
        {
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                if (selectedItems.Contains(items[i]))
                {
                    // Don't rearrange the first element
                    if (i != 0)
                    {
                        T previousItem = items[i - 1];
                        items[i - 1] = items[i];
                        items[i] = previousItem;
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
                        T nextItem = items[i + 1];
                        items[i + 1] = items[i];
                        items[i] = nextItem;
                    }
                    else if (i == (count - 1))
                    {
                        // Add the last element to the front
                        items = new ObservableCollection<T>(items.Prepend(items[count - 1]));
                        // Remove the last element
                        items.RemoveAt(count);
                        break; // Stop moving items down. An item has been removed from the bottom of the list, which causes everything above it to move down.
                    }
                }
            }
            return items;
        }
    }
}
