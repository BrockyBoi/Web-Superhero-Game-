using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
    public Transform player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Follow();
	}

    void Follow()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);
    }
}
