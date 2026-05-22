using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using System.Collections.Concurrent;

/*
    Data objects exclusively for producing Chants, to be refined later
    
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
public class GlyphMetre
{
    int trackNumber;
    int glyphID;
    int startIndex;
    int endIndex;

    public GlyphMetre(int track, int ID, int start, int end)
    {
        trackNumber = track;
        glyphID = ID;
        startIndex = start;
        endIndex = end;
    }

    // shallow input verification function, further checks TBD in ChantBuilder as well
    public static bool VerifyInput(int track, int ID, int start, int end) { return true; }

    public int ID {get => glyphID; }
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
public class ChantBuilder : MonoBehaviour
{
    [SerializeField] public float bpm;
    [SerializeField] public int totalSubsections;
    [SerializeField] public float totalDuration; // ulong?
    [SerializeField] public int sampleRate;

    [SerializeField] public EventReference fmodEventReference;
    public EventReference ChantEventReference { get => fmodEventReference; }

    List<GlyphEventHandler> glyphTracks;

    // thread-safety
    private ConcurrentQueue<GlyphEventHandler> pending;

    #region MonoBehaviour
    private void Awake()
    {
        glyphTracks = new List<GlyphEventHandler>();
        pending = new ConcurrentQueue<GlyphEventHandler>();
    }

    private void Start()
    {
        // set sample rate
        RuntimeManager.CoreSystem.getSoftwareFormat(out sampleRate, out _, out _);
    }

    private void Update()
    {
        while (pending.TryDequeue(out GlyphEventHandler track))
        {
            track.Dispose();
            glyphTracks.Remove(track);
            if (glyphTracks.Count == 0)
            {
                // something goin on yo!
            }
        }
    }
    #endregion

    public void BuildChant(List<GlyphMetre> chant)
    {
        ClearSession();
        ulong nextBeat = GetNextBeatDSPClock();

        for (int i = 0; i < glyphTracks.Count; i++)
        {
            // need to check row->row match and other stuff
            GlyphEventHandler track = new GlyphEventHandler(GlyphDatabase.GetGlyph(chant[i].ID), fmodEventReference);
            track.OnStopped += OnTrackStopped;
            track.ScheduleOnBeat(nextBeat);
            glyphTracks.Add(track);
        }
    }

    private void OnTrackStopped(GlyphEventHandler track)
    {
        pending.Enqueue(track);
    }

    private void ClearSession()
    {
        glyphTracks.Clear();
    }

    private ulong GetNextBeatDSPClock()
    {
        RuntimeManager.CoreSystem.getMasterChannelGroup(out var master);
        master.getDSPClock(out ulong clock, out _);

        ulong samplesPerBeat = (ulong)(sampleRate * (60.0 / bpm));
        return clock + (samplesPerBeat - (clock % samplesPerBeat));
    }
}