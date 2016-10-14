using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	Vector2 forwardVector;
	int myPower;
	int myDistance;
	int level;

	public bool stunsEnemy;

	int enemiesHit;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetVariables(Vector2 forward, int power, int playerLevel, int distance)
	{
		forwardVector = forward;
		myPower = power;
		level = playerLevel;
		myDistance = distance;

		if (forward == Vector2.left) {
			transform.localScale = new Vector2 (-transform.localScale.x, transform.localScale.y);
		}
		StartCoroutine (Move ());
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.CompareTag ("Enemy"))
		{
			Enemy e = c.gameObject.GetComponent < Enemy >();
			if(e.CheckIfInvulnerable() == false)
				enemiesHit++;
			e.TakeDamage(myPower, level, forwardVector);

			if (stunsEnemy)
				e.StunEnemy (Color.yellow, 3);
				
		}
	}

	IEnumerator Move()
	{
		int steps = 0;
		float max = myDistance;

		while (steps < max) {
			//transform.position += new Vector3 (velocity * Time.deltaTime * forwardVector.x, 0);
			//transform.Translate(new Vector3(1.1f * forwardVector.x,0));
			transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + forwardVector.x, transform.position.y), 1.1f);
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
