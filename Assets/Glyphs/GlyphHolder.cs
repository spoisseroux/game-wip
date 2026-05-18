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

    // subscribe and unsubscribe to rune UI events
    private void OnEnable()
    {
        // ui.OnChantSelected += ExecuteChant;
    }

    private void OnDisable()
    {
        // ui.OnChantSelected -= ExecuteChant;
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

    // load runes from save
    public void LoadRunes(List<int> save)
    {
        for (int i = 0; i < save.Count; i++)
        {
            
        }
    }

    public List<int> SetSavedRunes()
    {
        List<int> ids = new List<int>();
        /*
        for (int i = 0; i < runes.Count; i++)
        {
            ids.Add(runes[i].databaseID);
        }
        */

        return ids;
    }
    #endregion
}
