using UnityEngine;
using System.Collections.Generic;

/*
    Data object exclusively for producing Chants, to be refined later
    
    1st idea:
    {
        chantLengthInTime,                  ---> 3.0seconds
        chantDivisions,                     ---> 6 divisions  
        [glyphID, startindex, endindex],    ---> [1, 0, 5] --> play rune 1 for 6 subdivisions 3.0seconds
        [glyphID, startindex, endindex]     ---> [2, 3, 5] --> play rune 2 for 3 subdivisions 1.5seconds
    }

    Notes: END INDEX IS INCLUSIVE 0-1 is one subdivision ... 5-6 is last subdivision
    n-1 --> n (chantDivisions)
*/
public struct GlyphQueue
{
    
}

/*
    Wrapper for packaging all Audio and Visual effect composites returned from queued Glyph store
    Heavy FMOD integration here

    Recommended Research Order
        FMOD + Unity integration basics — RuntimeManager, EventInstance lifecycle
        Programmer Sounds — the callback pattern is non-obvious but essential
        DSP Clock and Scheduled Sounds — critical for beat-accurate triggering
        Studio Event parameters — wiring your SO data to FMOD params
        Bus/VCA routing — once layers play correctly, get the mix right
*/
public class Chant
{

    public Chant() {}
    public static Chant ChantFromPayload(GlyphQueue payload) { return null; }
}