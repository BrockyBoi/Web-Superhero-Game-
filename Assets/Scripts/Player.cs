using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public static Player playerSingleton;

    public GameObject firePrefab;
    public float defaultXScale;

    public Transform boundary1;
    public Transform boundary2;

    public float hSpeed = 5;
    public float jumpHeight = .3f;

	public GameObject rockPrefab;
	public GameObject wavePrefab;
	public GameObject grenadePrefab;

    bool canAttack;
    bool canMove;

    float[] attackRates;
    float[] attackTimes;

    //Has 3 numbers which represent close range, medium range, and long range attacks
    public static float[] attackDistances;

    [Range(0,100)]
    int health = 100;
	int maxHealth;

    Rigidbody2D rb2d;

    BoxCollider2D personalCollider;

    Vector2 forwardVector;

    bool alive = true;

    public bool godMode;
    [Range (1,10)]
    public int playerLevel;

    public enum Attacks { Short, Medium, Long, AOE };

    int[] availablePowers = new int[4];
    void Awake()
    {

        //TankMelee, SonicClap, ShoulderCharge, GroundSmash,
        //Fire, Wave, RockThrow, Lightning,
        //ParagonMelee, FreezeBreath, HeatVision, Jump,
        //SpeedMelee, WindGust, DashAttack, MapDash,
        //Pistol, Shotgun, Sniper, Grenades
        attackRates = new float[20] { 2, 5, 10, 25,
                                     .2f, 3, 6, 10,
                                     1.5f, 4, 8, 30,
                                     .8f, 3.5f, 6, 20,
                                     .5f, 3.5f, 7, 20
                                    };
        //Only need to check for short, medium, long, AOE attacks
        attackTimes = new float[4] { 0, 0, 0, 0};
        attackDistances = new float[3] { 2, 7, 15 };

        if (playerSingleton == null)
        {
            playerSingleton = this;
        }
        else Destroy(gameObject);

        rb2d = GetComponent<Rigidbody2D>();
        personalCollider = GetComponent<BoxCollider2D>();

		forwardVector = Vector3.right;
    }

	// Use this for initialization
	void Start () {
        alive = true;
        canMove = true;
		canAttack = true;
		GetPowers ();
		InitalizeSliders ();
		maxHealth = health;
    }
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * hSpeed;
        float vertical = Input.GetAxisRaw("Vertical");
        float jump = Input.GetAxisRaw("Jump");


        Movement(horizontal, vertical);
       // Attack(jump);

        if(Input.GetKey(KeyCode.Q))
        {
			if(!GameCanvas.controller.GetTutorialMode() || GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.PressQ))
            	SelectAttack(0);
        }
        else if (Input.GetKey(KeyCode.W))
        {
			if(!GameCanvas.controller.GetTutorialMode() || GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.PressW))
            SelectAttack(1);
        }
        else if (Input.GetKey(KeyCode.E))
        {
			if(!GameCanvas.controller.GetTutorialMode() || GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.PressE))
            SelectAttack(2);
        }
        else if (Input.GetKey(KeyCode.R))
        {
			if(!GameCanvas.controller.GetTutorialMode() || GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.PressR)
														|| GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.Lvl1AOE)
														|| GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.Lvl10AOE))
            SelectAttack(3);
        }
    }

    void CheckForward(float h)
    {
        if (!canMove)
            return;

        if (h > 0)
        {
            
            transform.localScale = new Vector2(defaultXScale, transform.localScale.y);
            forwardVector = Vector2.right;
        }
        else if (h < 0)
        {
            transform.localScale = new Vector2(-defaultXScale, transform.localScale.y);
            forwardVector = Vector2.left;
        }
    }

    void Movement(float h, float v)
    {
		if (!alive || !canMove || GameCanvas.controller.GetTutorialMode())
            return;

        if (h != 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + h, transform.position.y),1);
            CheckBoundaries();
            //transform.Translate(new Vector3(transform.position.x + h, transform.position.y));
            //transform.position = new Vector3(transform.position.x + h, transform.position.y);
            CheckForward(h);
        }

        //if (v != 0 && transform.position.y < 1)
        //    rb2d.AddForce(new Vector2(0, jumpHeight * powerController.GetPower(SuperPowerController.PowerNames.Jump)), ForceMode2D.Impulse);
    }

    bool CheckIfInBoundaries()
    {
        float xPos = transform.position.x;
        if (xPos < 0)
        {
            if (xPos < boundary1.position.x + 20)
            {
                return false;
            }
        }
        else
        {
            if (xPos > boundary2.position.x - 20)
            {
                return false;
            }
        }

        return true;
    }

    void CheckBoundaries()
    {
        float xPos = transform.position.x;
        if (xPos < 0)
        {
            if (xPos < boundary1.position.x + 20)
            {
                xPos = boundary1.position.x + 20;
                transform.position = new Vector3(xPos, transform.position.y, 0);
            }
        }
        else
        {
            if (xPos > boundary2.position.x - 20)
            {
                xPos = boundary2.position.x - 20;
                transform.position = new Vector3(xPos, transform.position.y, 0);
            }
        }
    }

	void GetPowers()
	{
		availablePowers = SuperPowerController.controller.GetAvailablePowers();
	}

	void InitalizeSliders()
	{
		for (int i = 0; i < 4; i++) {
			GameCanvas.controller.InitializeSlider (i, attackRates [availablePowers [i]]);
		}
	}

	public void FinishRechargingAttack(int num)
	{
		attackTimes [num] -= 30;
	}

    void SelectAttack(int attackNum)
	{
		if (!canAttack || !CheckAttackTime (attackNum))
			return;

		switch (attackNum) {
		//Short Attacks
		case (0):
			switch (availablePowers [0]) {
			case ((int)SuperPowerController.PowerNames.TankMelee):
				break;
			case ((int)SuperPowerController.PowerNames.ParagonMelee):
				break;
			case ((int)SuperPowerController.PowerNames.SpeedMelee):
				break;
			case ((int)SuperPowerController.PowerNames.Fire):
				break;
			case ((int)SuperPowerController.PowerNames.Pistol):
				break;
			default:
				break;
			}
			SuperPowerAttack (attackNum, ShortAttackDistance ());
			IncreaseAttackTime ((int)Attacks.Short);
			break;
		//Medium Attacks
		case (1):
			switch (availablePowers [1]) {
			case ((int)SuperPowerController.PowerNames.SonicClap):
				break;
			case ((int)SuperPowerController.PowerNames.Wave):
				WaveSpawn ();
				IncreaseAttackTime ((int)Attacks.Medium);
				GameCanvas.controller.UpdateAttackSlider (attackNum);
				return;
			case ((int)SuperPowerController.PowerNames.FreezeBreath):
				break;
			case ((int)SuperPowerController.PowerNames.WindGust):
				break;
			case ((int)SuperPowerController.PowerNames.Shotgun):
				break;
			default:
				break;
			}
			SuperPowerAttack (attackNum, MediumAttackDistance ());
			IncreaseAttackTime ((int)Attacks.Medium);
			break;
		case (2):
			switch (availablePowers [2]) {
			case ((int)SuperPowerController.PowerNames.ShoulderCharge):
				StartCoroutine (ShoulderCharge ());
				break;
			case ((int)SuperPowerController.PowerNames.RockThrow):
				RockThrow ();
				break;
			case ((int)SuperPowerController.PowerNames.HeatVision):
				SuperPowerAttack (attackNum, LargeAttackDistance ());
				break;
			case ((int)SuperPowerController.PowerNames.DashAttack):
				StartCoroutine (DashAttack ());
				break;
			case ((int)SuperPowerController.PowerNames.Sniper):
				SuperPowerAttack (attackNum, LargeAttackDistance ());
				break;
			default:
				break;
			}
			IncreaseAttackTime ((int)Attacks.Long);
			break;
		case (3):
			switch (availablePowers [3]) {
			case ((int)SuperPowerController.PowerNames.GroundSmash):
				break;
			case ((int)SuperPowerController.PowerNames.Lightning):
				break;
			case ((int)SuperPowerController.PowerNames.Jump):
				StartCoroutine (JumpAttack ());
				IncreaseAttackTime ((int)Attacks.AOE);
				return;
			case ((int)SuperPowerController.PowerNames.MapDash):
				StartCoroutine (MapDash ());
				IncreaseAttackTime ((int)Attacks.AOE);
				GameCanvas.controller.UpdateAttackSlider (attackNum);
				return;
			case ((int)SuperPowerController.PowerNames.Grenades):
				GrenadeMove ();
				IncreaseAttackTime ((int)Attacks.AOE);
				GameCanvas.controller.UpdateAttackSlider (attackNum);
				return;
			default:
				break;
			}
			SuperPowerAttackBothDirections (attackNum, playerLevel * 2);
			IncreaseAttackTime ((int)Attacks.AOE);
			break;
		default:
			break;
		}
		GameCanvas.controller.UpdateAttackSlider (attackNum);
	}

    void EnableCanAttack()
    {
        canAttack = true;
    }

    void DisableCanAttack()
    {
        canAttack = false;
    }

    public int GetPower(int num)
    {
        return availablePowers[num];
    }


	void SuperPowerAttack(int powerUsed, float attackDistance)
	{
		int enemiesHit = 0;

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
        Debug.DrawRay(transform.position, forwardVector, Color.green);

        for (int i = 0; i < hit.Length; i++)
        {
			if (hit [i]) {
				Enemy enemy = hit [i].collider.gameObject.GetComponent<Enemy> ();
				enemy.TakeDamage (powerUsed, playerLevel, forwardVector);

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
			}
        }

		AchievementSystem.controller.CheckMaxHits (hit.Length);
    }

	int SuperPowerAttackGetHits(int powerUsed, float attackDistance)
	{
		int enemiesHit = 0;

		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
		Debug.DrawRay(transform.position, forwardVector, Color.green);

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i])
			{
				Enemy enemy = hit [i].collider.gameObject.GetComponent<Enemy> ();
				enemy.TakeDamage (powerUsed, playerLevel, forwardVector);

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
			}
		}

		return enemiesHit;
	}

    void SuperPowerAttackBothDirections(int powerUsed, float attackDistance)
    {
		int enemiesHit = 0;

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
        RaycastHit2D[] hit2 = Physics2D.RaycastAll(transform.position, -forwardVector, attackDistance, LayerMask.GetMask("Enemy"));

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i])
            {
				Enemy enemy = hit [i].collider.gameObject.GetComponent<Enemy> ();
				enemy.TakeDamage (powerUsed, playerLevel, forwardVector);

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
            }
        }

        for(int i = 0; i < hit2.Length; i++)
        {
            if (hit2[i])
            {
				Enemy enemy = hit2 [i].collider.gameObject.GetComponent<Enemy> ();
				enemy.TakeDamage (powerUsed, playerLevel, -forwardVector);

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
            }
        }

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
    }

	int SuperPowerAttackBothDirectionsGetHits(int powerUsed, float attackDistance)
	{
		int enemiesHit = 0;

		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
		RaycastHit2D[] hit2 = Physics2D.RaycastAll(transform.position, -forwardVector, attackDistance, LayerMask.GetMask("Enemy"));

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i])
			{
				Enemy enemy = hit [i].collider.gameObject.GetComponent<Enemy> ();
				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
				enemy.TakeDamage (powerUsed, playerLevel, forwardVector);
			}
		}

		for(int i = 0; i < hit2.Length; i++)
		{
			if (hit2[i])
			{
				Enemy enemy = hit2 [i].collider.gameObject.GetComponent<Enemy> ();
				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
				enemy.TakeDamage (powerUsed, playerLevel, -forwardVector);
			}
		}

		return enemiesHit;
	}

    IEnumerator JumpAttack()
    {
		canAttack = false;
		rb2d.AddForce(new Vector2(0, Player.playerSingleton.GetLevel() * 6), ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1f, LayerMask.GetMask("Default"));
        while (!hit)           
        {
             hit = Physics2D.Raycast(transform.position, Vector3.down, 1f, LayerMask.GetMask("Default"));
            yield return null;
        }

        SuperPowerAttackBothDirections((int)Attacks.AOE,LargeAttackDistance());
		canAttack = true;
		GameCanvas.controller.UpdateAttackSlider (3);
    }

    IEnumerator DashAttack()
    {
        canMove = false;
        int steps = 0;
		int max = 20;

		int enemiesHit = 0;

        while (steps < max)
        {
            if (CheckIfInBoundaries())
            {
				rb2d.MovePosition(new Vector2(transform.position.x + ((LargeAttackDistance() + 5) / max * forwardVector.x), transform.position.y));
            }
			enemiesHit += SuperPowerAttackGetHits((int)Attacks.Long, LargeAttackDistance() / max);
            steps++;
            yield return null;
            
        }
        canMove = true;

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
    }

	IEnumerator MapDash()
	{
		int enemiesHit = 0;

		Vector3 startingPos = transform.position;
		canMove = false;
		while (CheckIfInBoundaries ()) {
			rb2d.MovePosition(new Vector2(transform.position.x - 3f, transform.position.y));
			enemiesHit += SuperPowerAttackGetHits ((int)Attacks.AOE, -3f);
			yield return null;
		}
		transform.position += new Vector3 (5, 0);
		while (CheckIfInBoundaries ()) {
			rb2d.MovePosition(new Vector2(transform.position.x + 3f, transform.position.y));
			enemiesHit += SuperPowerAttackGetHits ((int)Attacks.AOE, 3f);
			yield return null;
		}
		while (Vector3.Distance(transform.position, startingPos) > 2) {
			rb2d.MovePosition(new Vector2(transform.position.x - 3f, transform.position.y));
			enemiesHit += SuperPowerAttackGetHits ((int)Attacks.AOE, -3f);
			yield return null;
		}
		canMove = true;

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
	}

    IEnumerator ShoulderCharge()
    {
		int enemiesHit = 0;

        canMove = false;
        int steps = 0;
		int max = 60;

        while (steps < max)
        {
            if (CheckIfInBoundaries())
            {
				rb2d.MovePosition(new Vector2(GetXPos() + ((LargeAttackDistance() + 5) / max * forwardVector.x), transform.position.y));
            }
			enemiesHit += SuperPowerAttackGetHits((int)Attacks.Long, ShortAttackDistance());
            steps++;
            yield return null;

        }
        canMove = true;

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
    }

	void RockThrow()
	{
		GameObject rock = Instantiate (rockPrefab, transform.position, Quaternion.identity) as GameObject;
		rock.GetComponent<Projectile> ().SetVariables (forwardVector, (int)Attacks.Long, playerLevel, (int)LargeAttackDistance ());
	}

	void WaveSpawn()
	{
		GameObject wave = Instantiate (wavePrefab, transform.position, Quaternion.identity) as GameObject;
		wave.GetComponent<Projectile> ().SetVariables (forwardVector, (int)Attacks.Long, playerLevel, (int)MediumAttackDistance ());
	}

	void GrenadeMove()
	{
		for (int i = 0; i < 8; i++) {
			GameObject grenade = Instantiate (grenadePrefab, transform.position, Quaternion.identity) as GameObject;
			Rigidbody2D gRb2d = grenade.GetComponent<Rigidbody2D> ();

			Vector2 dir = new Vector2 (Random.Range (-.5f, .5f), 1);
			float force = Random.Range (500, 750);
			gRb2d.AddForce (dir * force);
			gRb2d.AddTorque (dir.x * force);
		}
	}

    public void TakeDamage(int num)
    {
        if (godMode)
            return;

        health -= num;

        if(health <= 0)
        {
            alive = false;
            Debug.Log("ded");
			AchievementSystem.controller.PlayerDeath (SuperPowerController.controller.GetSuperHero ());
        }
    }

    public static float ShortAttackDistance()
    {
        return attackDistances[0];
    }

	public static float MediumAttackDistance()
    {
        return attackDistances[1];
    }

	public static float LargeAttackDistance()
    {
        return attackDistances[2];
    }

    void IncreaseAttackTime(int num)
	{
		attackTimes [num] = Time.time + attackRates [availablePowers [num]];
	}

    bool CheckAttackTime(int num)
    {
		if (Time.time < attackTimes [num]) {
			return false;
		}
        return true;
    }

    public float GetXPos()
    {
        return transform.position.x;
    }

	public Vector3 GetForwardVector()
	{
		return forwardVector;
	}

    public bool CheckAlive()
    {
        return alive;
    }

	public int GetLevel()
	{
		return playerLevel;
	}

    public void LevelUp()
    {
        playerLevel++;
    }

	public void SetLevel(int num)
	{
		playerLevel = num;
	}

	public int GetHealth()
	{
		return health;
	}

	public bool AtFullHealth()
	{
		if (maxHealth == health)
			return true;

		return false;
	}
	
}
