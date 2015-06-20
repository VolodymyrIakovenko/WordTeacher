using System.Windows;

namespace WordTeacher.Utilities
{
    public static class ScreenUtility
    {
        private static Rect _desktopWorkingArea = SystemParameters.WorkArea;

        public static Point GetTopCenterPoint()
        {
            return new Point(_desktopWorkingArea.Width / 2, 0);
        }

        public static double GetTopYAxisValue()
        {
            return _desktopWorkingArea.Top;
        }

        public static double GetWorkAreaLeft()
        {
            return _desktopWorkingArea.Left;
        }

        public static double GetWorkAreaRight()
        {
            return _desktopWorkingArea.Right;
        }
    }
}
