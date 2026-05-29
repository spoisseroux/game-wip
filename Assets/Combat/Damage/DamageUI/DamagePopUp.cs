using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    // Unity objects
    public TextMeshProUGUI text;
    public Color textColor;

    // configs
    public float verticalMoveSpeed = 0.5f;
    public float lifetime = 2f;

    // timer
    CountdownTimer timer;


    #region MonoBehaviour 
    private void Awake()
    {
        timer = new CountdownTimer(lifetime);
    }

    private void Update()
    {
        transform.position += new Vector3(0.0f, verticalMoveSpeed * Time.deltaTime, 0.0f);
        timer.Tick(Time.deltaTime);
        
        if (timer.progress <= 0.0f)
            Destroy(this.gameObject); // maybe make this a coroutine after
    }
    #endregion

    #region Helpers
    public void Setup(DamagePayload damage)
    {
        // set up text
        int dmg = (int)damage.baseDamage;
        text.SetText(dmg.ToString());
        // color

        timer.Start();
    }
    #endregion
}