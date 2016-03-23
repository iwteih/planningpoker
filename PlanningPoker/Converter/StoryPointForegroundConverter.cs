using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PlanningPoker.Converter
{
    /// <summary>
    /// Color story point based on subtask status.
    /// 1 both parent & subtasks have story point, show default color
    /// 2 parent does not have story point
    ///   2.1 all subtasks have story point, show DarkKhaki
    ///   2.2 partial substasks have story point, show Coral 
    /// 3 Parent has story point
    ///   3.1 all substask have story point, show default color
    ///   3.2 partial subtasks have story point, show DodgerBlue 
    /// </summary>
    class StoryPointColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values == null || values.Length < 2)
            {
                return string.Empty;
            }

            Story story = values[1] as Story;

            if (story == null)
            {
                return SystemColors.ControlTextBrush;
            }
            bool parentNotSet = string.IsNullOrEmpty(story.StoryPoint);
            bool subTaskNotSet = false;

            if(story.HasSubTasks)
            {
                 subTaskNotSet = story.SubTasks.Any(f => string.IsNullOrEmpty(f.StoryPoint));
            }

            if (parentNotSet)
            { 
                if(subTaskNotSet)
                {
                    return Brushes.Coral;
                }
                return Brushes.DarkKhaki;
            }
            else
            {
                if(subTaskNotSet)
                {
                    return Brushes.DodgerBlue;
                }
            }
            return SystemColors.ControlTextBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
