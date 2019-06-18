using UnityEngine;

namespace MagicOnionExample
{
    public class MagicOnionConnectionHandler : MonoBehaviour
    {
        static MagicOnionConnectionHandler Instance;

        void Awake()
        {
            if (Instance != null && Instance != this && Instance.gameObject != null)
            {
                DestroyImmediate(Instance.gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        void OnApplicationQuit()
        {
            MagicOnionNetwork.DisconnectAsync();
        }
    }
}
