using System;



public static class ColourExtensions
{
    public struct colorRgbw
    {
        public uint red;
        public uint green;
        public uint blue;
        public uint white;
    };

    // The saturation is the colorfulness of a color relative to its own brightness.
    static uint Saturation(colorRgbw rgbw)
    {
        // Find the smallest of all three parameters.
        float low = Math.Min(rgbw.red, Math.Min(rgbw.green, rgbw.blue));
        // Find the highest of all three parameters.
        float high = Math.Max(rgbw.red, Math.Max(rgbw.green, rgbw.blue));
        // The difference between the last two variables
        // divided by the highest is the saturation.
        return (uint)Math.Round(100 * ((high - low) / high));
    }

    // Returns the value of White
    static uint GetWhite(colorRgbw rgbw)
    {
        return (255 - Saturation(rgbw)) / 255 * (rgbw.red + rgbw.green + rgbw.blue) / 3;
    }

    // Use this function for too bright emitters. It corrects the highest possible value.
    static uint GetWhite(colorRgbw rgbw, uint redMax, uint greenMax, uint blueMax)
    {
        // Set the maximum value for all colors.
        rgbw.red = (uint)((float)rgbw.red / 255.0 * (float)redMax);
        rgbw.green = (uint)((float)rgbw.green / 255.0 * (float)greenMax);
        rgbw.blue = (uint)((float)rgbw.blue / 255.0 * (float)blueMax);
        return (255 - Saturation(rgbw)) / 255 * (rgbw.red + rgbw.green + rgbw.blue) / 3;
    }

    // Example function.
    static colorRgbw rgbToRgbw(uint red, uint green, uint blue)
    {
        colorRgbw rgbw = new colorRgbw();
        rgbw.red = red;
        rgbw.green = green;
        rgbw.blue = blue;
        rgbw.white = GetWhite(rgbw);
        return rgbw;
    }

    // Example function with color correction.
    static colorRgbw rgbToRgbw(uint red, uint redMax,
                        uint green, uint greenMax,
                        uint blue, uint blueMax)
    {
        colorRgbw rgbw = new colorRgbw();
        rgbw.red = red;
        rgbw.green = green;
        rgbw.blue = blue;
        rgbw.white = GetWhite(rgbw, redMax, greenMax, blueMax);
        return rgbw;
    }

    public static UnityEngine.Color RgbwToColor(uint red, uint green, uint blue, uint white)
    {
        UnityEngine.Color newColor = new UnityEngine.Color();
        newColor.r = red/256f;
        newColor.g = green / 256f;
        newColor.b = blue / 256f;
        newColor.a = 1;
        newColor *= white / 256;
        newColor.a = 1;
        return newColor;
    }
}
