using System.Windows;
using System.Windows.Controls;
using DesktopExtension.Utils;

namespace DesktopExtension.Log
{
    public class NotificationDateTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NotificationTemplate { get; set; }



        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is INotification notification)
            {

                return NotificationTemplate;
            }

            return null;
        }
    }
}