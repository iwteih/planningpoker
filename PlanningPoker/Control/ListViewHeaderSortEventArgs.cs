using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace PlanningPoker.Control
{
    class ListViewHeaderSortEventArgs : RoutedEventArgs
    {
        private string header;
        private ListSortDirection direction;
        public ListViewHeaderSortEventArgs(RoutedEvent routedEvent, string header, ListSortDirection direction)
            : base(routedEvent)
        {
            this.header = header;
            this.direction = direction;
        }

        public ListSortDirection Direction
        {
            get { return this.direction; }
        }
        public string Header
        {
            get { return header; }
        }

    }
}
