using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;

    [SerializeField] float time;

    public void ToggledLever()
    {
        StartCoroutine(MovePlatform());
    }

    public void MoveToEndPosition()
    {
        this.transform.position = end.position;
        this.transform.rotation = end.rotation;
    }

    public IEnumerator MovePlatform()
    {
        float elapsed = 0.0f;
        while (elapsed <= time)
        {
            this.transform.position = Vector3.Lerp(start.position, end.position, elapsed / time);
            this.transform.rotation = Quaternion.Lerp(start.rotation, end.rotation, elapsed / time);
            elapsed += Time.deltaTime;

            yield return null;
        }
        
        // sanity check to make sure we actually got there
        transform.position = end.position;
        transform.rotation = end.rotation;
        yield return null;
    }
}