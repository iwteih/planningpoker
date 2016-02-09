using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace PlanningPoker.Control
{
    class ListViewArrowAdorner : System.Windows.Controls.Control
    {
        // Using a DependencyProperty as the backing store for SortDirection. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(ListViewArrowAdorner));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adorner"></param>
        static ListViewArrowAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListViewArrowAdorner), new FrameworkPropertyMetadata(typeof(ListViewArrowAdorner)));
        }

        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }
    }
}
