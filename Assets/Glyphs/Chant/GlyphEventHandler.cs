using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;

public class GlyphEventHandler : IDisposable
{
    // Add to GlyphEventHandle
    public event Action<GlyphEventHandler> OnStopped;

    public FMOD.Studio.EventInstance instance { get; private set; }
    public GlyphDataSO sourceGlyph { get; private set; }

    private GCHandle callbackPin; // pin so garbage collector doesn't clear this object
    private FMOD.Studio.EVENT_CALLBACK callback;
    private bool released = false;

    public GlyphEventHandler(GlyphDataSO glyph, EventReference fmodRef)
    {
        sourceGlyph = glyph;
        instance = RuntimeManager.CreateInstance(fmodRef);

        // pin callback to survive through GC
        callback = new FMOD.Studio.EVENT_CALLBACK(HandleEventCallback);
        callbackPin = GCHandle.Alloc(callback);

        // pass 'this' through as userdata so the static callback can reach it
        GCHandle selfPin = GCHandle.Alloc(this);
        instance.setUserData(GCHandle.ToIntPtr(selfPin));

        // apply our callback function and make it only respond to these two events
        instance.setCallback(callback,
            FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND |
            FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND |
            FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);

        ApplyDSPParams();
    }

    #region Helpers
    private void ApplyDSPParams()
    {
        // adjust these later to match DAW
        instance.setParameterByName("Pitch", sourceGlyph.pitch);
        instance.setParameterByName("Resonance", sourceGlyph.resonance);
    }

    private void OnSoundCreate(IntPtr paramPtr)
    {
        // read FMOD struct from unmanaged memory into C# object
        var param = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(
            paramPtr, 
            typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
        
        // create FMOD sound from glyph
        RuntimeManager.CoreSystem.createSound(
            Application.streamingAssetsPath + "/" + sourceGlyph.audioFileName,
            FMOD.MODE.CREATESTREAM | FMOD.MODE.LOOP_NORMAL,
            out FMOD.Sound sound); // is this what I want? why audio file name and not something else?
            /*
                RESULT System.createSound(
                    byte[] data,
                    MODE mode,
                    out Sound sound
                );

                this could be interesting
            */

        param.sound = sound.handle; // grab IntPtr objects from FMOD sound and pass through
        param.subsoundIndex = -1; // ???

        // write back to FMOD unmanaged memory
        Marshal.StructureToPtr(param, paramPtr, false);
    }

    private void OnSoundDestroy(IntPtr paramPtr)
    {
        // Release the sound when FMOD is done with it
        var param = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(
            paramPtr,
            typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)
        );

        var sound = new FMOD.Sound(param.sound);
        sound.release();
    }

    public void ScheduleOnBeat(ulong dspClockTarget)
    {
        instance.start();

        instance.getChannelGroup(out FMOD.ChannelGroup group);
        group.getDSPClock(out ulong _, out ulong parentClock);
        // pin playback clock to certain timestamp
        group.setDelay(dspClockTarget, 0, false);
    }
    #endregion

    #region FMOD Studio Callback
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))] // not sure what this does, think it handles callbacks btw managed and unmanaged data/environments??
    private static FMOD.RESULT HandleEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE eventType, IntPtr instancePtr, IntPtr paramPtr)
    {
        // recover 'this' from userdata ptr
        var eventInst = new FMOD.Studio.EventInstance(instancePtr);
        eventInst.getUserData(out IntPtr selfPtr);
        var handle = (GlyphEventHandler)GCHandle.FromIntPtr(selfPtr).Target;

        // switch on events
        switch (eventType)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
            {
                handle.OnSoundCreate(paramPtr);
                break;
            }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
            {
                handle.OnSoundDestroy(paramPtr);
                break;
            }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED:
            {
                handle.OnStopped?.Invoke(handle);
                break;
            }
        }

        return FMOD.RESULT.OK;
    }
    #endregion

    public void Dispose()
    {
        if (released)
            return;

        released = false;
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();

        // release garbage collection handle
        callbackPin.Free();
    }
}