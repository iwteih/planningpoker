using log4net;
using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace PlanningPoker.Control
{
    // http://www.codeproject.com/Articles/137209/Binding-and-styling-text-to-a-RichTextBox-in-WPF
    public class RichTextBoxBindable : RichTextBox
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly DependencyProperty DocumentProperty = 
            DependencyProperty.Register("Story", typeof(Story),
            typeof(RichTextBoxBindable), new FrameworkPropertyMetadata
                (null, new PropertyChangedCallback(OnStoryChanged)));

        public Story Story
        {
            get
            {
                return (Story)this.GetValue(DocumentProperty);
            }

            set
            {
                this.SetValue(DocumentProperty, value);
            }
        }

        private static void OnStoryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RichTextBox rtb = (RichTextBox)obj;
            //rtb.Document = (string)args.NewValue;
            Story story = args.NewValue as Story;

            if(story == null)
            {
                log.Warn("story is null");
                return;
            }

            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            paragraph.FontSize = 15;

            //var icon = new System.Windows.Controls.Image()
            //{
            //    Source = new BitmapImage(new Uri(story.IssueTypeIcon))
            //};
            //paragraph.Inlines.Add(icon);
            Hyperlink hyperLink = new Hyperlink(new Run(story.Title));
            hyperLink.NavigateUri = new Uri(story.URL);
            hyperLink.RequestNavigate += hyperLink_RequestNavigate;
            paragraph.Inlines.Add(hyperLink);
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Run(string.Format("Assignee: {0}", story.Assignee)));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Run(story.Summary));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Run(story.Description));
            document.Blocks.Add(paragraph);

            rtb.Document = document;
        }

        static void hyperLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
