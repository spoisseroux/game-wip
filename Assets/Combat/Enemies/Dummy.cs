using UnityEngine;

public class Dummy : MonoBehaviour, IHittable
{
    [SerializeField] GameObject damagePopUp;
    [SerializeField] Vector3 worldSpaceGUIAdjustment;

    public float health = 100;

    public void Hit(HitboxRecord hitboxRecord)
    {
        DamagePayload dmg = hitboxRecord.context.damage;
        health -= dmg.baseDamage;
        
        GameObject popup = Instantiate(damagePopUp, this.transform.position + worldSpaceGUIAdjustment, Quaternion.identity);
        popup.GetComponent<DamagePopUp>().Setup(dmg);

        Debug.Log("Ow my health! I lost: " + dmg.baseDamage + " health!");
        return;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
