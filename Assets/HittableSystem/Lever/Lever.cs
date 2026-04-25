using UnityEngine;
using UnityEngine.Playables;

public class LeverSaveData : ISaveData
{
    public bool switched;

    public LeverSaveData()
    {
        switched = false;
    }

    public LeverSaveData(bool toggled)
    {
        switched = toggled;
    }
}

public class LeverSaveModule : SaveModule<LeverSaveData>
{
    protected readonly Lever lever;

    public LeverSaveModule(Lever leverIn)
    {
        lever = leverIn;
    }

    protected override void ApplyTypedData(LeverSaveData data) => lever.LoadSave(data);
    protected override LeverSaveData CollectTypedData() => lever.CollectSaveData();
}

public class Lever : MonoBehaviour, IHittable
{
    // components
    [SerializeField] PlayableDirector director;

    // save data
    LeverSaveModule save;
    bool switched = false;

    // list of gameobjects we want to affect upon hit
    [SerializeField] Platform[] platforms;

    #region MonoBehaviour
    void Awake()
    {
        save = new LeverSaveModule(this);
        save.Initialize();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (switched)
        {
            // move lever down
            director.time = director.duration;
            director.Evaluate();
            director.Stop();

            // move platforms
            foreach (Platform p in platforms)
            {
                p.ToggledLever();
            }
        }
    }

    void OnDestroy()
    {
        save.DetachFromSaveManager();
    }
    #endregion

    #region IHittable Interface
    public void Hit(HitboxRecord hit)
    {
        // director
        director.Play();

        // move platforms
        foreach (Platform p in platforms)
        {
            p.ToggledLever();
        }
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
    #endregion

    #region Saveable Object
    public void LoadSave(LeverSaveData data)
    {
        switched = data.switched;
    }

    public LeverSaveData CollectSaveData()
    {
        return new LeverSaveData(switched);
    }
    #endregion
}
