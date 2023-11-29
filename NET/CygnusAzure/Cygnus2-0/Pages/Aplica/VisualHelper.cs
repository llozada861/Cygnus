using Cygnus2_0.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Cygnus2_0.Pages.Aplica
{
    public class VisualHelper
    {
        //EnableRowsMoveProperty is used to enable rows moving by mouse drag and move in data grid
        //the only requirement is to ItemsSource collection of datagrid be a ObservableCollection or at least IList collection
        public static readonly DependencyProperty EnableRowsMoveProperty =
            DependencyProperty.RegisterAttached("EnableRowsMove", typeof(bool), typeof(VisualHelper), new PropertyMetadata(false, EnableRowsMoveChanged));

        //Private DraggedItemProperty attached property used only for EnableRowsMoveProperty
        private static readonly DependencyProperty DraggedItemProperty =
            DependencyProperty.RegisterAttached("DraggedItem", typeof(object), typeof(VisualHelper), new PropertyMetadata(null));

        public static bool GetEnableRowsMove(DataGrid obj)
        {
            return (bool)obj.GetValue(EnableRowsMoveProperty);
        }

        public static void SetEnableRowsMove(DataGrid obj, bool value)
        {
            obj.SetValue(EnableRowsMoveProperty, value);
        }

        private static object GetDraggedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(DraggedItemProperty);
        }

        private static void SetDraggedItem(DependencyObject obj, object value)
        {
            obj.SetValue(DraggedItemProperty, value);
        }

        private static void EnableRowsMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (d as DataGrid);
            if (grid == null) return;
            if ((bool)e.NewValue)
            {
                grid.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                grid.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
                grid.PreviewMouseMove += OnMouseMove;
            }
            else
            {
                grid.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                grid.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
                grid.PreviewMouseMove -= OnMouseMove;
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //find datagrid row by mouse point position
            var row = TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition((sender as DataGrid)));
            if (row == null || row.IsEditing) return;
            VisualHelper.SetDraggedItem(sender as DataGrid, row.Item);
        }

        private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var draggeditem = VisualHelper.GetDraggedItem(sender as DependencyObject);
            if (draggeditem == null) return;
            ExchangeItems(sender, (sender as DataGrid).SelectedItem);
            //select the dropped item
            (sender as DataGrid).SelectedItem = draggeditem;
            //reset
            VisualHelper.SetDraggedItem(sender as DataGrid, null);
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            var draggeditem = VisualHelper.GetDraggedItem(sender as DependencyObject);
            if (draggeditem == null) return;
            var row = TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition((sender as DataGrid)));
            if (row == null || row.IsEditing) return;
            ExchangeItems(sender, row.Item);
        }

        private static void ExchangeItems(object sender, object targetItem)
        {
            int nuIndex = 1;

            var draggeditem = VisualHelper.GetDraggedItem(sender as DependencyObject);
            if (draggeditem == null) return;
            if (targetItem != null && !ReferenceEquals(draggeditem, targetItem) &&
                ((Archivo)targetItem).FileName != ((Archivo)draggeditem).FileName
                )
            {
                var list = (sender as DataGrid).ItemsSource as IList;
                if (list == null)
                    throw new ApplicationException("EnableRowsMoveProperty requires the ItemsSource property of DataGrid to be at least IList inherited collection. Use ObservableCollection to have movements reflected in UI.");
                //get target index
                var targetIndex = list.IndexOf(targetItem);

                //remove the source from the list
                list.Remove(draggeditem);

                ((Archivo)draggeditem).Index = ((Archivo)targetItem).Index;

                //move source at the target's location
                list.Insert(targetIndex, draggeditem);

                foreach (Archivo item in list)
                {
                    item.Index = nuIndex;
                    nuIndex++;
                }

                try
                {
                    (sender as DataGrid).Items.Refresh();
                }
                catch { }
            }
        }

        public static T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        public static T TryFindFromPoint<T>(UIElement reference, Point point)
            where T : DependencyObject
        {
            var element = reference.InputHitTest(point) as DependencyObject;
            if (element == null) return null;
            if (element is T) return (T)element;
            return FindVisualParent<T>(element);
        }
    }    
}
