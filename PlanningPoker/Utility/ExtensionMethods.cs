using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PlanningPoker.Utility
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    yield return (T)child;

                foreach (var descendant in GetVisualChildren<T>(child))
                    yield return descendant;
            }
        }

    }
}
