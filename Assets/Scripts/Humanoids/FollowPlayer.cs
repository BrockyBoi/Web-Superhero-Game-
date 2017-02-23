using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    public static FollowPlayer MainCamera;

    public Transform player;

    Vector3 velocity = Vector3.zero;

    bool shaking;

    Transform[] boundaries;

    void Awake()
    {
        MainCamera = this;
    }

    // Use this for initialization
    void Start()
    {
        boundaries = new Transform[2];
        boundaries[0] = GameCanvas.controller.boundary1;
        boundaries[1] = GameCanvas.controller.boundary2;
        player = Player.playerSingleton.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (shaking)
            return;

        Follow();
    }

    void Follow()
    {
        if (Player.playerSingleton.transform.position.x - (Camera.main.orthographicSize) > boundaries[0].position.x + 35 && Player.playerSingleton.transform.position.x + (Camera.main.orthographicSize) < boundaries[1].position.x - 35)
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.position.x, player.position.y, -10), ref velocity, .0001f);
        else if (Player.playerSingleton.transform.position.x - Camera.main.orthographicSize < boundaries[0].position.x + 35)
            transform.position = new Vector3(boundaries[0].position.x + 35 + Camera.main.orthographicSize , transform.position.y, - 10);
        else if(Player.playerSingleton.transform.position.x + (Camera.main.orthographicSize) > boundaries[1].position.x - 35)
			transform.position = new Vector3(boundaries[1].position.x - 35 - Camera.main.orthographicSize , transform.position.y, -10);
    }

    public void CameraShake()
    {
        StartCoroutine(CameraShakeEnumerator());
    }

    IEnumerator CameraShakeEnumerator()
    {
        shaking = true;
        float shakeTime = .5f;
        while (shakeTime > 0)
        {
            Vector2 newSpot = Random.insideUnitCircle * .15f;

            transform.position = new Vector3(transform.position.x + newSpot.x, transform.position.y + newSpot.y, -10);

            shakeTime -= Time.deltaTime;
            yield return null;
        }
        shaking = false;
    }
}
