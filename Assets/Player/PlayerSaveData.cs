using UnityEngine;

public class PlayerSaveData : SaveableObject
{
    // parent player object
    [SerializeField] PlayerManager player;

    // save data
    private class PlayerSave : ISaveData
    {
        public string json { get; set; }
    }
    private PlayerSave saveData;
}