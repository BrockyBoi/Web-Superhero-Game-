using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	public static FollowPlayer MainCamera;

    public Transform player;

	Vector3 velocity = Vector3.zero;

	bool shaking;

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
		if (shaking)
			return;
		
        Follow();
	}

    void Follow()
    {
		transform.position = Vector3.SmoothDamp (transform.position, new Vector3(player.position.x, player.position.y, -10), ref velocity, .0001f);
    }

	public void CameraShake()
	{
		StartCoroutine (CameraShakeEnumerator ());
	}

	IEnumerator CameraShakeEnumerator()
	{
		shaking = true;
		float shakeTime = .5f;
		Debug.Log ("Does this even happen?");
		while (shakeTime > 0) {
			Vector2 newSpot = Random.insideUnitCircle * .15f;

			transform.position = new Vector3 (transform.position.x + newSpot.x, transform.position.y + newSpot.y, -10);

			shakeTime -= Time.deltaTime;
			yield return null;
		}
		shaking = false;
	}
}
