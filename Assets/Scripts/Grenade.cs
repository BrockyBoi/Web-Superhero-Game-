using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	public static int enemiesHit;
	public static int grenadeCount;

	// Use this for initialization
	void Start () {
		grenadeCount++;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		if (c.gameObject.name == "Ground") 
		{
			Explode ();
		}
	}

	void Explode()
	{
		SuperPowerAttackBothDirectionsGetHits((int)Player.Attacks.AOE, 2);
		grenadeCount--;

		if (grenadeCount == 0) 
		{
			Debug.Log (enemiesHit);
			AchievementSystem.controller.CheckMaxHits (enemiesHit);
			enemiesHit = 0;
		}
		Destroy (gameObject, .5f);
	}

	int SuperPowerAttackBothDirectionsGetHits(int powerUsed, float attackDistance)
	{
		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.left, attackDistance, LayerMask.GetMask("Enemy"));
		RaycastHit2D[] hit2 = Physics2D.RaycastAll(transform.position, Vector2.right, attackDistance, LayerMask.GetMask("Enemy"));

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i])
			{
				Enemy enemy = hit [i].collider.gameObject.GetComponent<Enemy> ();

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
				
				enemy.TakeDamage (powerUsed, Player.playerSingleton.GetLevel(), Vector2.left);
			}
		}
		for(int i = 0; i < hit2.Length; i++)
		{
			if (hit2[i])
			{
				Enemy enemy = hit2 [i].collider.gameObject.GetComponent<Enemy> ();

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
				
				enemy.TakeDamage (powerUsed, Player.playerSingleton.GetLevel(), Vector2.right);
			}
		}

		return enemiesHit;
	}
}
