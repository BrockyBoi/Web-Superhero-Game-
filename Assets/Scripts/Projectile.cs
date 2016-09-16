using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	Vector3 forwardVector;
	int myPower;
	int myDistance;
	int level;

	bool hasAllVariables;

	int enemiesHit;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (hasAllVariables) {
			StartCoroutine (Move ());
			hasAllVariables = false;
		}
	}

	public void SetVariables(Vector3 forward, int power, int playerLevel, int distance)
	{
		forwardVector = forward;
		myPower = power;
		level = playerLevel;
		myDistance = distance;
		hasAllVariables = true;
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.CompareTag ("Enemy"))
		{
			if(c.gameObject.GetComponent<Enemy>().CheckIfInvulnerable() == false)
				enemiesHit++;
			c.gameObject.GetComponent<Enemy>().TakeDamage(myPower, level, forwardVector);
		}
	}

	IEnumerator Move()
	{
		int steps = 0;
		float max = myDistance * 2;

		while (steps < max) {
			//transform.position += new Vector3 (velocity * Time.deltaTime * forwardVector.x, 0);
			transform.Translate(new Vector3(.5f * forwardVector.x,0));
			steps++;
			yield return null;
		}
		AchievementSystem.controller.CheckMaxHits (enemiesHit);
		StartCoroutine (Disappear ());

	}

	IEnumerator Disappear()
	{
		SpriteRenderer sR = GetComponent<SpriteRenderer> ();
		while (sR.color.a > 0) {
			sR.color = new Color (sR.color.r, sR.color.g, sR.color.b, sR.color.a - .06f);
			yield return null;
		}
		Destroy (gameObject);
	}
}
