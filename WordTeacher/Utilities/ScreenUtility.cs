using System.Windows;

namespace WordTeacher.Utilities
{
    public static class ScreenUtility
    {
        public static Point GetTopCenterPoint()
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            return new Point(desktopWorkingArea.Width / 2, 0);
        }
    }
}
