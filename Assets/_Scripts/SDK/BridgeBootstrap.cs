using UnityEngine;

public class BridgeBootstrap : MonoBehaviour
{
    private static bool _created;

    private void Awake()
    {
        if (_created)
        {
            Destroy(gameObject);
            return;
        }

        _created = true;
        DontDestroyOnLoad(gameObject);
    }
}