using System.Diagnostics;
using System.Drawing;
using Ws2812LedController.Core.Colors;
using Ws2812LedController.Core.Model;
using Ws2812LedController.Core.Utils;

namespace Ws2812LedController.Core;

public class LedSegment
{
    /* Absolute start index relative to the complete strip */
    public int AbsStart { get; }
    /* Absolute end index */
    public int AbsEnd => AbsStart + Width - 1;
    
    public int Width { get; }
    public string Id => $"{AbsStart}..{AbsEnd}";
    public Color[] State => Strip.Canvas.State[AbsStart..AbsEnd];
    public BitmapWrapper Canvas { get; }

    public bool Enabled { set; get; } = true;
    public LedLayer[] Layers { get; } = new LedLayer[typeof(LayerId).GetEnumNames().Length];

    /* Configurable properties */
    public bool UseGammaCorrection { set; get; }
    public bool InvertX
    {
        set
        {
            foreach (var layer in Layers)
            {
                layer.InvertX = value;
            }
        }
        get => Layers.All(l => l.InvertX);
    }
    public byte MaxBrightness
    {
        set
        {
            _maxBrightness = value;
            Strip.Canvas.RedrawBuffer(AbsStart, Width, value);
        }
        get => _maxBrightness;
    }

    public LedStrip Strip { get; }

    private byte _maxBrightness = 255;

    public LedSegment(int absStart, int length, LedStrip strip)
    {
        AbsStart = absStart;
        Width = length;
        Strip = strip;
        Canvas = new BitmapWrapper(Width);

        for (var i = 0; i < Layers.Length; i++)
        {
            Layers[i] = new LedLayer(length);
        }
    }

    public void MuteSection(bool mute, int start, int end, bool absolute)
    {
        if (absolute)
        {
            start = ToRelativeIndex(start);
            end = ToRelativeIndex(end);
        }

        for (var i = start; i <= end; i++)
        {
            Canvas.MutedPixels[i] = mute;
        }
    }  
    
    public bool ContainsAbsolutePixel(int absoluteIndex)
    {
        return absoluteIndex >= AbsStart && absoluteIndex <= AbsEnd;
    }

    public int ToAbsoluteIndex(int relativeIndex)
    {
        return AbsStart + relativeIndex;
    }
    
    public int ToRelativeIndex(int absoluteIndex)
    {
        Debug.Assert(ContainsAbsolutePixel(absoluteIndex), $"Absolute index '{absoluteIndex}' is out of range for this segment ({Id})");
        return absoluteIndex - AbsStart;
    }
}