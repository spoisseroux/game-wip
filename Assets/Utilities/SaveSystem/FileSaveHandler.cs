using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;

public class FileSaveHandler
{
    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings { 
        TypeNameHandling = TypeNameHandling.Auto, /* or TypeNameHandling.Objects */
        Formatting = Formatting.Indented
    };

    private bool debug;

    private string saveFileName = "testsave";
    private string suffix = ".json";

    private int MAX_SAVE_FILES_ALLOWED = 1;

    public FileSaveHandler()
    {
        debug = false;
    }

    public FileSaveHandler(bool config)
    {
        debug = config;
    }

    public void Save(int saveNumber, SerializeableDictionary<string, ISaveData> save)
    {
        Debug.Log("FileSaveHandler::Save() --> Begin call");
        // create filename .../save1.json
        string path = Path.Combine(Application.persistentDataPath + saveFileName + /*saveNumber + */ suffix);
        Debug.Log(path);
        try
        {
            Debug.Log("FileSaveHandler::Save() --> Try block");
            // create directory if nonexistent
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // serialize data
            string jsonSave = JsonConvert.SerializeObject(save, jsonSettings);

            // write to file
            using (FileStream stream = new FileStream(path, FileMode.Create) /* new StreamWriter(Application.persistentDataPath + "/Save.json") */)
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    Debug.Log("FileSaveHandler::Save() --> Writing to: " + path);
                    writer.Write(jsonSave);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred trying to save file: " + path + "\n" + e);
        }
    }

    public SerializeableDictionary<string, ISaveData> Load(int saveNumber)
    {
        string path = Path.Combine(Application.persistentDataPath + saveFileName + suffix);
        SerializeableDictionary<string, ISaveData> loadedData = null;

        Debug.Log("FileSaveHandler::Load() --> attempting to open save data from file: " + path);

        // make sure it exists
        if (File.Exists(path))
        {
            Debug.Log("Trying to read save file at: " + path);
            try
            {
                string jsonData = "";
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonData = reader.ReadToEnd();
                    }
                }

                // deserialize into save data format
                loadedData = JsonConvert.DeserializeObject<SerializeableDictionary<string, ISaveData>>(jsonData, jsonSettings);
                Debug.Log(loadedData);
            }
            catch (Exception e)
            {
                Debug.LogError("Error opening save file at: " + path + "\n" + e);
            }
        }
        return loadedData;
    }
}