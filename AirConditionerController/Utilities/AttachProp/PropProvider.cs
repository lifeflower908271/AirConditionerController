using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.AttachProp
{
    public static class PropProvider
    {
        #region 样式索引
        public static readonly DependencyProperty StyleIndexProperty =
            DependencyProperty.RegisterAttached("StyleIndex", typeof(int), typeof(PropProvider), new FrameworkPropertyMetadata(null));
        public static int GetStyleIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(StyleIndexProperty);
        }

        public static void SetStyleIndex(DependencyObject obj, int value)
        {
            obj.SetValue(StyleIndexProperty, value);
        }


        #endregion

    }
}
