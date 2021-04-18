using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                        Type t = typeof(T);

                        T previousItem = (T)Activator.CreateInstance(t, items[i - 1]);
                        ////T previousItem = items[i - 1];
                        ////items[i - 1] = items[i];
                        ////items[i] = previousItem;
                        items[i - 1] = (T)Activator.CreateInstance(t, items[i]);
                        items[i] = (T)Activator.CreateInstance(t, previousItem);
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
                        Type t = typeof(T);

                        T nextItem = (T)Activator.CreateInstance(t, items[i + 1]);
                        ////T nextItem = items[i + 1];
                        ////items[i + 1] = items[i];
                        ////items[i] = nextItem;
                        items[i + 1] = (T)Activator.CreateInstance(t, items[i]);
                        items[i] = (T)Activator.CreateInstance(t, nextItem);
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
    }
}
