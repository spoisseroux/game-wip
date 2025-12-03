// Object representing a glyph/sigil on a GameObject that reacts to Rune Activations
public interface IRune {
    public abstract void Chant();
    /*
        // create the cutscene
        // play sounds and sfx
        // play shaders and vfx
        // ... more?
    */

    public abstract void Activate(); 
    /*
        // alert any listeners for this rune's activation
        // buff owner
        // .... more?
    */
}


// MAYBE
// IRUNE --> physical object in game, so the object that reacts to chants/activations and displays shaders and sounds???, 
//       --> attach it on a specific GameObject that has the sprite and effect it there??

// RuneDataSO --> data for the RuneHolder to act upon, create player-centric audio, animations, effects?

// 