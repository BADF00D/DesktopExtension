using System.Windows;

namespace DesktopExtension.Interop
{
    public class WindowRectangle
    {
        public int Left { get; }
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }

        public WindowRectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString()
        {
            return
                $"{nameof(Left)}: {Left}, {nameof(Top)}: {Top}, {nameof(Right)}: {Right}, {nameof(Bottom)}: {Bottom}";
        }

        public void Deconstruct(out int left, out int top, out int right, out int bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
        }

        public bool IsEmpty => Left == Right || Bottom == Top;

        public bool IsOffScreen()
        {
            var width = SystemParameters.VirtualScreenWidth;
            var height = SystemParameters.VirtualScreenHeight;

            return Left > width || Right < 0 || Top > height || Bottom < 0;
        }
    }
}