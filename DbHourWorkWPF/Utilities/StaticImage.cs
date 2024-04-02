using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace DbHourWorkWPF.Utilities
{
    public class StaticImage : Image
    {
        static StaticImage()
        {
            //Отслеживание смены исходной картинки
            Image.SourceProperty.OverrideMetadata(
                typeof(StaticImage),
                new FrameworkPropertyMetadata(SourceChanged));
        }

        private static void SourceChanged(DependencyObject obj,
                                            DependencyPropertyChangedEventArgs e)
        {
            var image = obj as StaticImage;
            if (image == null) return;

            //Поправка размера картинки под текущее разрешение
            image.Width = image.Source.Width * Render.PixelSize;
            image.Height = image.Source.Height * Render.PixelSize;
        }
    }
    public static class Render
    {
        static Render()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Static;
            var dpiProperty = typeof(SystemParameters).GetProperty("Dpi", flags);

            Dpi = (int)dpiProperty.GetValue(null, null);
            PixelSize = 96.0 / Dpi;
        }

        //Размер физического пикселя в виртуальных единицах
        public static double PixelSize { get; private set; }

        //Текущее разрешение
        public static int Dpi { get; private set; }
    }
}
