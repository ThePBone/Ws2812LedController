using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using Ws2812AudioReactiveClient.FastLedCompatibility;
using Ws2812LedController.Core;
using Ws2812LedController.Core.Effects.Base;
using Ws2812LedController.Core.FastLedCompatibility;
using Ws2812LedController.Core.Model;
using Ws2812LedController.Core.Utils;

namespace Ws2812AudioReactiveClient.Effects;

public class FlickerReactiveEffect : BaseAudioReactiveEffect
{
    public override string Description => "Flicker LEDs based on volume peaks";
    public override int Speed { set; get; } = 1000 / 60;

    public Color Color { set; get; } = Color.DarkRed;
    public double Threshold { set; get; } = 2000;
    
    protected override async Task<int> PerformFrameAsync(LedSegmentGroup segment, LayerId layer)
    {
        var count = NextSample();
        if (count < 1)
        {
            goto NEXT_FRAME;
        }

        var isPeak = IsPeak(Threshold);
        

        /* Fade to black by x */ 
        for(var i = 0; i < segment.Width; ++i) 
        {
            segment.SetPixel(i, Scale.nscale8x3(segment.PixelAt(i, layer), 255 - /*fadeBy*/ 45),layer);
        } 
        
        if (isPeak)
        {
            segment.Clear(Color, layer);
        }
        
        CancellationMethod.NextCycle();
        
        NEXT_FRAME:
        return Speed;
    }
}