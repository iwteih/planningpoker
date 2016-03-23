using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PlanningPoker.Converter
{
    class StoryPointTextConverter: IMultiValueConverter
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
                return string.Empty;
            }

            if(!string.IsNullOrEmpty(story.StoryPoint))
            {
                return story.StoryPoint;
            }

            if (story.HasSubTasks)
            {
               string sum = story.CalcChildrenStoryPoints();

                if(Story.UnFlippedScore == sum)
                {
                    return string.Empty;
                }

                return sum;
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
