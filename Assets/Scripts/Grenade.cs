using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

	// Use this for initialization
	void Start () {

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
		SuperPowerAttackBothDirections ((int)Player.Attacks.AOE, 2);
		Destroy (gameObject, .5f);
	}

	void SuperPowerAttackBothDirections(int powerUsed, float attackDistance)
	{
		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.left, attackDistance, LayerMask.GetMask("Enemy"));
		RaycastHit2D[] hit2 = Physics2D.RaycastAll(transform.position, Vector2.right, attackDistance, LayerMask.GetMask("Enemy"));

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i])
			{
				hit[i].collider.gameObject.GetComponent<Enemy>().TakeDamage(powerUsed, Player.playerSingleton.GetLevel(), Vector2.left);
			}
		}

		for(int i = 0; i < hit2.Length; i++)
		{
			if (hit2[i])
			{
				hit2[i].collider.gameObject.GetComponent<Enemy>().TakeDamage(powerUsed, Player.playerSingleton.GetLevel(), Vector2.right);
			}
		}
	}
}
