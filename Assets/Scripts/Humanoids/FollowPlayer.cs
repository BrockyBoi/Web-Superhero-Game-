using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	public static FollowPlayer MainCamera;

    public Transform player;

	Vector3 velocity = Vector3.zero;

	void Awake()
	{
		MainCamera = this;
	}

	// Use this for initialization
	void Start () {
		player = Player.playerSingleton.transform;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Follow();
	}

    void Follow()
    {
		transform.position = Vector3.SmoothDamp (transform.position, new Vector3(player.position.x, player.position.y, -10), ref velocity, .01f);
    }

	public void CameraShake()
	{
		StartCoroutine (CameraShakeEnumerator ());
	}

	IEnumerator CameraShakeEnumerator()
	{
		float shakeTime = .35f;

		while (shakeTime > 0) {
			Vector2 newSpot = Random.insideUnitCircle * 1.5f;

			transform.position = new Vector3 (transform.position.x + newSpot.x, transform.position.y + newSpot.y, -10);

			shakeTime -= Time.deltaTime;
			yield return null;
		}
	}
}
