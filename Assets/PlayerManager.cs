using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Transform startline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(player, startline);
    }

}
