using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public static Player playerSingleton;

	protected Animator anim;

	protected float defaultXScale;

    protected Transform boundary1;
    protected Transform boundary2;
	public Transform attackPoint;

    public float hSpeed = 5;

	public GameObject rockPrefab;
	public GameObject wavePrefab;
	public GameObject grenadePrefab;
	public GameObject clapPrefab;

    protected bool canAttack;
    protected bool canMove;

    protected float[] attackRates;
    protected float[] attackTimes;

    //Has 3 numbers which represent close range, medium range, and long range attacks
    public static float[] attackDistances;

    [Range(0,250)]
    public int health = 100;
	int maxHealth;

    Rigidbody2D rb2d;

    Vector2 forwardVector;

    bool alive = true;

    public bool godMode;
	public bool tutorial;

    [Range (1,10)]
    public int playerLevel;

    public enum Attacks { Short, Medium, Long, AOE };

    int[] availablePowers = new int[4];

	public bool knockback;

	AudioSource audio;

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
        attackDistances = new float[3] { 15, 30, 50 };

        if (playerSingleton == null)
        {
            playerSingleton = this;
        }
        else Destroy(gameObject);

        rb2d = GetComponent<Rigidbody2D>();
		audio = gameObject.AddComponent<AudioSource> ();
		anim = GetComponent<Animator> ();

		forwardVector = Vector3.right;

		maxHealth = health;

    }

	// Use this for initialization
	void Start () {
        alive = true;
        canMove = true;
		canAttack = true;
		GetPowers ();
		InitializeSliders ();
		InitializeBoundaries ();

		defaultXScale = transform.localScale.x;


    }
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxisRaw ("Horizontal"); 
		anim.SetFloat ("speed", Mathf.Abs(horizontal));

		Movement(horizontal * Time.deltaTime * hSpeed);

		CheckAttackInput ();
    }

	void CheckAttackInput()
	{

		if(Input.GetKey(KeyCode.Q))
		{
			if (!GameCanvas.controller.GetTutorialMode () || GameCanvas.controller.CheckIfOnSpecificPosition (GameCanvas.TutorialPosition.PressQ))
				TryAttack (0);
		}
		else if (Input.GetKey(KeyCode.W))
		{
			if(!GameCanvas.controller.GetTutorialMode() || GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.PressW))
				TryAttack (1);
		}
		else if (Input.GetKey(KeyCode.E))
		{
			if(!GameCanvas.controller.GetTutorialMode() || GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.PressE))
				TryAttack (2);
				//SelectAttack(2);
		}
		else if (Input.GetKey(KeyCode.R))
		{
			if(!GameCanvas.controller.GetTutorialMode() || GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.PressR)
				|| GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.Lvl1AOE)
				|| GameCanvas.controller.CheckIfOnSpecificPosition(GameCanvas.TutorialPosition.Lvl10AOE))
				TryAttack (3);
				//SelectAttack(3);
		}
	}

	private void TryAttack(int attackNum)
	{
		if (CheckIfCanAttack (attackNum)) {
			anim.SetTrigger ("attackNum" + attackNum.ToString ());
			IncreaseAttackTime (attackNum);
		}
	}

	bool CheckIfCanAttack(int num)
	{
		if (!canAttack || !CheckAttackTime (num))
			return false;

		return true;
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

    void Movement(float h)
    {
		if (!alive || !canMove || GameCanvas.controller.GetTutorialMode())
            return;

        if (h != 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + h, transform.position.y),1);
            CheckBoundaries();
            CheckForward(h);
        }
    }

    bool CheckIfInBoundaries()
    {
        float xPos = transform.position.x;
        if (xPos < 0)
        {
            if (xPos < boundary1.position.x + 35)
            {
                return false;
            }
        }
        else
        {
            if (xPos > boundary2.position.x - 35)
            {
                return false;
            }
        }

        return true;
    }

	void InitializeBoundaries()
	{
		boundary1 = GameCanvas.controller.GrabBoundary1 ();
		boundary2 = GameCanvas.controller.GrabBoundary2 ();
	}

    void CheckBoundaries()
    {
        float xPos = transform.position.x;
        if (xPos < 0)
        {
			if (xPos < boundary1.position.x + 35)
            {
				xPos = boundary1.position.x + 35;
                transform.position = new Vector3(xPos, transform.position.y, 0);
            }
        }
        else
        {
			if (xPos > boundary2.position.x - 35)
            {
				xPos = boundary2.position.x - 35;
                transform.position = new Vector3(xPos, transform.position.y, 0);
            }
        }
    }

	void GetPowers()
	{
		availablePowers = SuperPowerController.controller.GetAvailablePowers();
	}

	void InitializeSliders()
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
		switch (attackNum) {
		//Short Attacks
		case (0):
			switch (availablePowers [0]) {
			case ((int)SuperPowerController.PowerNames.TankMelee):
				NormalAttack (0);
				break;
			case ((int)SuperPowerController.PowerNames.ParagonMelee):
				NormalAttack (0);
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
			break;
		//Medium Attacks
		case (1):
			switch (availablePowers [1]) {
			case ((int)SuperPowerController.PowerNames.SonicClap):
				ProjectileSpawn (clapPrefab, 1);
				break;
			case ((int)SuperPowerController.PowerNames.Wave):
				ProjectileSpawn (wavePrefab, 1);
				break;
			case ((int)SuperPowerController.PowerNames.FreezeBreath):
				NormalAttack (1);
				break;
			case ((int)SuperPowerController.PowerNames.WindGust):
				NormalAttack (1);
				break;
			case ((int)SuperPowerController.PowerNames.Shotgun):
				NormalAttack (1);
				break;
			default:
				break;
			}
			break;
		case (2):
			switch (availablePowers [2]) {
			case ((int)SuperPowerController.PowerNames.ShoulderCharge):
				StartCoroutine (ShoulderCharge ());
				break;
			case ((int)SuperPowerController.PowerNames.RockThrow):
				ProjectileSpawn (rockPrefab, 2);
				break;
			case ((int)SuperPowerController.PowerNames.HeatVision):
				NormalAttack (2);
				break;
			case ((int)SuperPowerController.PowerNames.DashAttack):
				StartCoroutine (DashAttack ());
				break;
			case ((int)SuperPowerController.PowerNames.Sniper):
				NormalAttack (2);
				break;
			default:
				break;
			}
			break;
		case (3):
			switch (availablePowers [3]) {
			case ((int)SuperPowerController.PowerNames.GroundSmash):
				FollowPlayer.MainCamera.CameraShake ();
				NormalAttack (3);
				break;
			case ((int)SuperPowerController.PowerNames.Lightning):
				break;
			case ((int)SuperPowerController.PowerNames.Jump):
				StartCoroutine (JumpAttack ());
				//IncreaseAttackTime ((int)Attacks.AOE);
				return;
			case ((int)SuperPowerController.PowerNames.MapDash):
				StartCoroutine (MapDash ());
				//IncreaseAttackTime ((int)Attacks.AOE);
				break;
			case ((int)SuperPowerController.PowerNames.Grenades):
				GrenadeMove ();
				//IncreaseAttackTime ((int)Attacks.AOE);
				break;
			default:
				break;
			}
			break;
		default:
			break;
		}
		//SoundController.controller.PlaySoundInList (audio, attackNum);
		GameCanvas.controller.UpdateAttackSlider (attackNum);
	}

	void NormalAttack(int powerNum)
	{
		if (powerNum != 3)
			SuperPowerAttack (powerNum, attackDistances [powerNum]);
		else
			SuperPowerAttackBothDirections (powerNum, playerLevel * 2);
	}
		

    void EnableCanAttack()
    {
        canAttack = true;
    }

    void DisableCanAttack()
    {
        canAttack = false;
    }

	void DisableCanAttack(float num)
	{
		canAttack = false;
		Invoke ("EnableCanAttack", num);
	}

    public int GetPower(int num)
    {
        return availablePowers[num];
    }


	void SuperPowerAttack(int powerUsed, float attackDistance)
	{
		int enemiesHit = 0;

		RaycastHit2D[] hit = Physics2D.RaycastAll(attackPoint.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
        Debug.DrawRay(transform.position, forwardVector, Color.green);

        for (int i = 0; i < hit.Length; i++)
        {
			if (hit [i]) {
				Enemy enemy = hit [i].collider.gameObject.GetComponent<Enemy> ();

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;
				
				enemy.TakeDamage (powerUsed, playerLevel, forwardVector);
			}
        }

		AchievementSystem.controller.CheckMaxHits (hit.Length);
    }

	int SuperPowerAttackGetHits(int powerUsed, float attackDistance)
	{
		int enemiesHit = 0;

		RaycastHit2D[] hit = Physics2D.RaycastAll(attackPoint.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
		Debug.DrawRay(transform.position, forwardVector, Color.green);

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

		return enemiesHit;
	}

    void SuperPowerAttackBothDirections(int powerUsed, float attackDistance)
    {
		int enemiesHit = 0;

		RaycastHit2D[] hit = Physics2D.RaycastAll(attackPoint.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
		RaycastHit2D[] hit2 = Physics2D.RaycastAll(attackPoint.position, -forwardVector, attackDistance, LayerMask.GetMask("Enemy"));

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

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
    }

	int SuperPowerAttackBothDirectionsGetHits(int powerUsed, float attackDistance)
	{
		int enemiesHit = 0;

		RaycastHit2D[] hit = Physics2D.RaycastAll(attackPoint.position, forwardVector, attackDistance, LayerMask.GetMask("Enemy"));
		RaycastHit2D[] hit2 = Physics2D.RaycastAll(attackPoint.position, -forwardVector, attackDistance, LayerMask.GetMask("Enemy"));

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
		rb2d.AddForce(new Vector2(0, Player.playerSingleton.GetLevel() * 55), ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
		RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2)), Vector2.down, 2f, LayerMask.GetMask("Default"));
        while (!hit)           
        {
			hit = Physics2D.Raycast(transform.position, Vector2.down, 10, LayerMask.GetMask("Default"));

			if (Vector2.Distance(rb2d.velocity, Vector2.zero) < .5f) {
				anim.SetTrigger ("HitPeak");
				rb2d.AddForce(new Vector2(0, -Player.playerSingleton.GetLevel() * 150), ForceMode2D.Impulse);
			}
            yield return null;
        }

		anim.SetTrigger ("HitGround");
		FollowPlayer.MainCamera.CameraShake ();
        SuperPowerAttackBothDirections((int)Attacks.AOE,LargeAttackDistance());
		canAttack = true;
		GameCanvas.controller.UpdateAttackSlider (3);
    }

    IEnumerator DashAttack()
    {
		int enemiesHit = 0;

        canMove = false;
        int steps = 0;
		int max = 20;

        while (steps < max)
        {
            if (CheckIfInBoundaries())
            {
				rb2d.MovePosition(new Vector2(transform.position.x + ((LargeAttackDistance() + 5) / max * forwardVector.x), transform.position.y));
            }
			enemiesHit = SuperPowerAttackGetHits((int)Attacks.Long, LargeAttackDistance() / max);
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
		SoundController.controller.PlaySoundInList (audio, SuperPowerController.PowerNames.DashAttack);
		while (CheckIfInBoundaries ()) {
			rb2d.MovePosition(new Vector2(transform.position.x + 3f, transform.position.y));
			enemiesHit += SuperPowerAttackGetHits ((int)Attacks.AOE, 3f);
			yield return null;
		}
		SoundController.controller.PlaySoundInList (audio, SuperPowerController.PowerNames.DashAttack);
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

	void ProjectileSpawn(GameObject prefab, int powerNumber)
	{
		GameObject projectile = Instantiate (prefab, transform.position, Quaternion.identity) as GameObject;
		projectile.GetComponent<Projectile> ().SetVariables (forwardVector, (int)Attacks.Long, playerLevel, (int)attackDistances[powerNumber]);

		//IncreaseAttackTime (powerNumber);

		Debug.Log (prefab.name);
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

		if (knockback) {
			DisableCanAttack (.8f);
			anim.SetTrigger ("takeHit");
		}

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

	public int GetMaxHealth()
	{
		return maxHealth;
	}

	public bool AtFullHealth()
	{
		if (maxHealth == health)
			return true;

		return false;
	}

	public Vector3 GetLocation()
	{
		return new Vector3 (transform.position.x, transform.position.y, -10);
	}
	
}
