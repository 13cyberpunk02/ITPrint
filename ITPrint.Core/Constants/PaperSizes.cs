namespace ITPrint.Core.Constants;

public static class PaperSizes
{
    // Размеры в миллиметрах
    public static class Dimensions
    {
        public static readonly (int Width, int Height) A4 = (210, 297);
        public static readonly (int Width, int Height) A3 = (297, 420);
        public static readonly (int Width, int Height) A2 = (420, 594);
        public static readonly (int Width, int Height) A1 = (594, 841);
        public static readonly (int Width, int Height) A0 = (841, 1189);
        public static readonly (int Width, int Height) A1x2 = (841, 1189);
        public static readonly (int Width, int Height) A1x3 = (841, 1783);
    }
    
    // Размеры в точках (points) для PDF (1mm = 2.83465 points)
    public static class Points
    {
        public static readonly (double Width, double Height) A4 = (595.28, 841.89);
        public static readonly (double Width, double Height) A3 = (841.89, 1190.55);
        public static readonly (double Width, double Height) A2 = (1190.55, 1683.78);
        public static readonly (double Width, double Height) A1 = (1683.78, 2383.94);
        public static readonly (double Width, double Height) A0 = (2383.94, 3370.39);
        public static readonly (double Width, double Height) A1x2 = (2383.94, 3370.39);
        public static readonly (double Width, double Height) A1x3 = (2383.94, 5055.12);
    }
}