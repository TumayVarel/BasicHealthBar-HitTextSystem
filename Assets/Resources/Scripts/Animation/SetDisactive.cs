using UnityEngine;

public class SetDisactive : MonoBehaviour
{
    /// <summary>HitText animation invokes this method at the end of its lifetime.</summary>
    public void SetDisActive() => gameObject.SetActive(false);
}
