using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    // [SerializeField] private GameObject player;
    // public float minX, minZ, maxX, maxZ;

    private void Start()
    {
        // Vector3 startPosition = new Vector3(Random.Range(minX, maxX), 1, Random.Range(minZ, maxZ));
        // PhotonNetwork.Instantiate(player.name, startPosition, Quaternion.identity);
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
