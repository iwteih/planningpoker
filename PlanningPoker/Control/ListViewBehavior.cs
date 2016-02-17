using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using PlanningPoker.Utility;

namespace PlanningPoker.Control
{
    // http://www.cnblogs.com/nankezhishi/archive/2009/12/04/SortListView.html
    public class ListViewBehavior
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderSortProperty =
            DependencyProperty.RegisterAttached("HeaderSort", typeof(bool), typeof(ListViewBehavior), new UIPropertyMetadata(new PropertyChangedCallback(OnHeaderSortPropertyChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SortFieldProperty =
            DependencyProperty.RegisterAttached("SortField", typeof(string), typeof(ListViewBehavior));


        public static bool GetHeaderSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(HeaderSortProperty);
        }

        public static void SetHeaderSort(DependencyObject obj, bool value)
        {
            obj.SetValue(HeaderSortProperty, value);
        }

        public static string GetSortField(DependencyObject obj)
        {
            return (string)obj.GetValue(SortFieldProperty);
        }

        public static void SetSortField(DependencyObject obj, string value)
        {
            obj.SetValue(SortFieldProperty, value);
        }

        private static void OnHeaderSortPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView == null)
                throw new InvalidOperationException("HeaderSort Property can only be set on a ListView");

            if ((bool)e.NewValue)
            {
                listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnListViewHeaderClick));
            }
            else
            {
                listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnListViewHeaderClick));
            }
        }

        public static void cleanLagecySortInfo(ListView listView)
        {
            listView.Items.SortDescriptions.Clear();
            List<GridViewColumnHeader> headers = listView.GetVisualChildren<GridViewColumnHeader>().ToList();

            foreach (var header in headers)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(header);
                Adorner[] adorners = adornerLayer.GetAdorners(header);

                if (adorners == null)
                {
                    continue;
                }

                foreach (Adorner adorner in adorners)
                {
                    UIElementAdorner uiElementAdorner = adorner as UIElementAdorner;
                    if (uiElementAdorner != null && uiElementAdorner.Child is ListViewArrowAdorner)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
            }
        }

        private static ListSortDirection getLastSortDirection(GridViewColumnHeader header)
        {
            ListSortDirection direction = ListSortDirection.Ascending;

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(header);
            Adorner[] adorners = adornerLayer.GetAdorners(header);

            if (adorners != null)
            {
                foreach (var v in adorners)
                {
                    var adorner = v as UIElementAdorner;

                    if (adorner != null)
                    {
                        var decorator = adorner.Child as ListViewArrowAdorner;

                        if (decorator != null)
                        {
                            decorator.SortDirection = (ListSortDirection)(((int)decorator.SortDirection + 1) % 2);
                            direction = decorator.SortDirection;
                            break;
                        }
                    }
                }
            }

            return direction;
        }


        private static void OnListViewHeaderClick(object sender, RoutedEventArgs e)
        {
            ListView listView = e.Source as ListView;

            if (listView == null)
            {
                return;
            }

            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;

            if (header == null)
            {
                return;
            }

            if (listView.Items == null || listView.Items.Count == 0)
            {
                return;
            }

            ListSortDirection direction = getLastSortDirection(header);
            cleanLagecySortInfo(listView);

            var adornerToAdd = new ListViewArrowAdorner();
            adornerToAdd.SortDirection = direction;
            AdornerLayer.GetAdornerLayer(header).Add(new UIElementAdorner(header, adornerToAdd));

            SortDescription sortDescriptioin = new SortDescription()
            {
                Direction = direction,
                PropertyName = header.Column.GetValue(SortFieldProperty) as string ?? header.Column.Header as string
            };
            listView.Items.SortDescriptions.Add(sortDescriptioin);
        }
    }
}
