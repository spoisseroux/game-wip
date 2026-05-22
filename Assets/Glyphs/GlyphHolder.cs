using System.Collections.Generic;
using UnityEngine;

// have to figure out where we separate the act of generating a chant from the UI, 
// and then sequentially chanting
public class GlpyhHolder : MonoBehaviour
{
    // list of glyphs
    [SerializeField] List<GlyphDataSO> glyphs; 
    /*
        Could this be List<Glyph> where Glyph : IGlyph (funcs) and have GlyphDataSO data?
    */

    // UI Data, list of pushed glyphs

    #region MonoBehaviour
    private void Awake()
    {
        
    }

    // NEED TO REFACTOR INTO THE INPUT REQUESTS
    private void Update()
    {
        // m key for quick testing
        /*
        var keyboard = Keyboard.current;
        if (keyboard.mKey.wasPressedThisFrame)
        {
            Debug.Log("heyyy checking...");
            ExecuteChant(runes);
        }
        */
    }

    private void OnDrawGizmosSelected()
    {
        
    }
    #endregion

    #region Rune Container Additions
    // receive a rune from Altar
    public void BestowGlyph()
    {
        // add to list
    }
    #endregion

}
