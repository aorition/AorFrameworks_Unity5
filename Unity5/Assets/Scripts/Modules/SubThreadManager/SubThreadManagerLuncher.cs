using UnityEngine;
public class SubThreadManagerLuncher : MonoBehaviour
{

    public GameObject Root;
    private void Awake()
    {
        if (!SubThreadManager.HasInstance)
        {
            SubThreadManager.CreateInstance(Root ? Root : gameObject);
        }
    }

}