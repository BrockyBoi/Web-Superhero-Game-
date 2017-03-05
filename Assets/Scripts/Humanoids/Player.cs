using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	#region variables
    public static Player playerSingleton;

	protected Animator anim;

	protected float defaultXScale;

    protected Transform boundary1;
    protected Transform boundary2;
	public Transform attackPoint;

	public GameObject rockPrefab;
	public GameObject wavePrefab;
	public GameObject grenadePrefab;
	public GameObject clapPrefab;

	protected bool currentlyAttacking;
    protected bool canAttack;
    protected bool canMove;


    protected float[] attackRates;
	protected float[] defaultAttackRates = new float[4];
    protected float[] attackTimes;

    //Has 3 numbers which represent close range, medium range, and long range attacks
    public static float[] attackDistances;

    public int health;
	int starterHealth;


    Rigidbody2D rb2d;

    Vector2 forwardVector;

	bool alive = true;

    public bool godMode;
	public bool tutorial;

    public enum Attacks { Short, Medium, Long, AOE };

    int[] availablePowers = new int[4];

	AudioSource myAudio;

	//Upgradable variables
	int damage;
	int maxHealth;
	float healthRegen;
	[SerializeField]
	public float hSpeed = 5;

	#endregion 

    void Awake()
    {

        //TankMelee, SonicClap, ShoulderCharge, GroundSmash,
        //Fire, Wave, RockThrow, Lightning,
        //ParagonMelee, FreezeBreath, HeatVision, Jump,
        //SpeedMelee, WindGust, DashAttack, MapDash,
        //Bat, Shotgun, Sniper, Grenades
        attackRates = new float[20] { 2, 5, 10, 25,
                                     .2f, 3, 6, 10,
                                     1.5f, 4, 8, 30,
                                     .8f, 3.5f, 6, 20,
                                     1, 3.5f, 7, 20
                                    };

		GetPowers ();

        attackTimes = new float[4] { 0, 0, 0, 0};
        attackDistances = new float[3] { 5, 15, 30 };

        if (playerSingleton == null)
        {
            playerSingleton = this;
        }
        else Destroy(gameObject);

        rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator> ();

		forwardVector = Vector3.right;

		maxHealth = health;
		starterHealth = health;
    }

	void Start () {
        alive = true;
        canMove = true;
		canAttack = true;
		InitializeSliders ();
		InitializeBoundaries ();

		defaultXScale = transform.localScale.x;

		Invoke ("RegainHealth", 5);
    }

	void Update () {
		float horizontal = Input.GetAxisRaw ("Horizontal"); 
		anim.SetFloat ("speed", Mathf.Abs(horizontal));

		Movement(horizontal * Time.deltaTime * hSpeed);

		CheckAttackInput ();

    }

	void CheckAttackInput()
	{
		if (currentlyAttacking)
			return;
		
		if(Input.GetKey(KeyCode.Q))
		{
			if (!GameCanvas.controller.GetTutorialMode () || GameCanvas.controller.CheckIfOnSpecificPosition (GameCanvas.TutorialPosition.PressQ)) {
				TryAttack (0);
			}
		}
		else if (Input.GetKey(KeyCode.W))
		{
			if ((GameCanvas.controller.CheckIfOnSpecificPosition (GameCanvas.TutorialPosition.PressW) && GameCanvas.controller.GetTutorialMode())
				|| (!GameCanvas.controller.GetTutorialMode () && XPController.controller.GetLevel() > 1)) {
				TryAttack (1);
			}
		}
		else if (Input.GetKey(KeyCode.E))
		{
			if ((GameCanvas.controller.CheckIfOnSpecificPosition (GameCanvas.TutorialPosition.PressE) && GameCanvas.controller.GetTutorialMode())
				|| (!GameCanvas.controller.GetTutorialMode () && XPController.controller.GetLevel() > 2)) {
				TryAttack (2);
			}
				//SelectAttack(2);
		}
		else if (Input.GetKey(KeyCode.R)&& XPController.controller.GetLevel() > 3)
		{
			if (!GameCanvas.controller.GetTutorialMode () || GameCanvas.controller.CheckIfOnSpecificPosition (GameCanvas.TutorialPosition.PressR)
			   || GameCanvas.controller.CheckIfOnSpecificPosition (GameCanvas.TutorialPosition.Lvl1AOE)
			   || GameCanvas.controller.CheckIfOnSpecificPosition (GameCanvas.TutorialPosition.Lvl10AOE)
				|| (!GameCanvas.controller.GetTutorialMode() && XPController.controller.GetLevel() > 3)) {
				TryAttack (3);
			}
				//SelectAttack(3);
		}
	}

	void TryAttack(int attackNum)
	{
		if (CheckIfCanAttack (attackNum)) {
			anim.SetTrigger ("attackNum" + attackNum.ToString ());
			currentlyAttacking = true;
		}
	}

	bool CheckIfCanAttack(int num)
	{
		if (!canAttack || currentlyAttacking || !CheckAttackTime (num))
			return false;

		return true;
	}

	bool CheckAttackTime(int num)
	{
		if (Time.time < attackTimes [num]) {
			return false;
		}
		return true;
	}

	void IncreaseAttackTime(int num)
	{
		attackTimes [num] = Time.time + attackRates [availablePowers [num]];
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
		if (!alive || !canMove || currentlyAttacking || GameCanvas.controller.GetTutorialMode())
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

		for (int i = 0; i < 4; i++) {
			defaultAttackRates [i] = attackRates[availablePowers[i]];
		}
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
		IncreaseAttackTime (attackNum);

		switch (attackNum) {
		//Short Attacks
		case (0):
			switch (availablePowers [0]) {
			case ((int)SuperPowerController.PowerNames.TankMelee):
				if (NormalAttack (0) > 0)
				{
					StartCoroutine(BrieflySlowTime());
					PlayThisSound (attackNum);
				}
				else
					PlaySound (SoundController.controller.whoosh);
				break;
			case ((int)SuperPowerController.PowerNames.ParagonMelee):
				if(NormalAttack (0) > 0)
				{
					//StartCoroutine(BrieflySlowTime());
					PlaySound(SoundController.controller.punch);
				}
				else
					PlaySound (SoundController.controller.whoosh);
				break;
			case ((int)SuperPowerController.PowerNames.SpeedMelee):
				if(NormalAttack (0) > 0)
					PlayThisSound (attackNum);
				else
					PlaySound (SoundController.controller.whoosh);
				break;
			case ((int)SuperPowerController.PowerNames.Fire):
				PlaySound (SoundController.controller.flamethrower);
				NormalAttack (0);
				break;
			case ((int)SuperPowerController.PowerNames.Bat):
				NormalAttack (0);
				PlayThisSound (attackNum);
				break;
			default:
				break;
			}
			break;
		//Medium Attacks
		case (1):
			switch (availablePowers [1]) {
			case ((int)SuperPowerController.PowerNames.SonicClap):
				ProjectileSpawn (clapPrefab, 1);
				break;
			case ((int)SuperPowerController.PowerNames.Wave):
				NormalAttack (1);
				SpawnWaterAttack ();
				PlaySound (SoundController.controller.wave);
				break;
			case ((int)SuperPowerController.PowerNames.FreezeBreath):
				NormalAttack (1);
				IceBreathSprite (1);
				PlaySound(SoundController.controller.freezeBreath);
				break;
			case ((int)SuperPowerController.PowerNames.WindGust):
				NormalAttack (1);
				PlayThisSound (attackNum);
				break;
			case ((int)SuperPowerController.PowerNames.Shotgun):
				NormalAttack (1);
				PlayThisSound (attackNum);
				break;
			default:
				break;
			}
			break;
			//Long attacks
		case (2):
			switch (availablePowers [2]) {
			case ((int)SuperPowerController.PowerNames.ShoulderCharge):
				StartCoroutine (ShoulderCharge ());
				break;
			case ((int)SuperPowerController.PowerNames.RockThrow):
				ProjectileSpawn (rockPrefab, 2);
				PlaySound (SoundController.controller.rockSummon);
				break;
			case ((int)SuperPowerController.PowerNames.HeatVision):

				if(NormalAttack (2) > 0)
				{
					//StartCoroutine(BrieflySlowTime());
				}
				PlaySound (SoundController.controller.heatVision);
				//Turn on heat vision sprite
				LaserVisionSprite(1);
				break;
			case ((int)SuperPowerController.PowerNames.DashAttack):
				StartCoroutine (DashAttack ());
				break;
			case ((int)SuperPowerController.PowerNames.Sniper):
				NormalAttack (2);
				PlayThisSound (attackNum);
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
				PlayThisSound (attackNum);
				break;
			case ((int)SuperPowerController.PowerNames.Lightning):
				PlaySound (SoundController.controller.lightningStrike);
				NormalAttack (3);
				break;
			case ((int)SuperPowerController.PowerNames.Jump):
				StartCoroutine (JumpAttack ());
				return;
			case ((int)SuperPowerController.PowerNames.MapDash):
				StartCoroutine (MapDash ());
				break;
			case ((int)SuperPowerController.PowerNames.Grenades):
				ThrowGrenades ();
				break;
			default:
				break;
			}
			break;
		default:
			break;
		}

		//currentlyAttacking = false;
		//GameCanvas.controller.UpdateAttackSlider (attackNum);
		GameCanvas.controller.UpdateAttackImage(attackNum);
	}

	int NormalAttack(int powerNum)
	{
		int hits = 0;

		if (powerNum != 3)
			hits = SuperPowerAttack (powerNum, attackDistances [powerNum]);
		else {
			float distance = Mathf.Max (ShortAttackDistance() + 2, UpgradeController.controller.GetUpgradeLevel(UpgradeController.Upgrades.Damage) * 2.5f);
			hits = SuperPowerAttackBothDirections (powerNum, distance);
		}

		//if(hits > 0)
			//StartCoroutine(BrieflySlowTime());
		return hits;
	}

	IEnumerator BrieflySlowTime()
	{
		Time.timeScale = .1f;
		//float endTime = Time.realtimeSinceStartup + .25f;
		//while(Time.realtimeSinceStartup < endTime)
		//	yield return null;

		while(Time.timeScale < 1)
		{
			Time.timeScale += Time.deltaTime * 5;
			yield return null;
		}
	}

	void NoLongerAttacking()
	{
		currentlyAttacking = false;
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

	void PlayThisSound(int thisAttack)
	{
		SoundController.controller.PlaySoundInList (thisAttack);
	}

	void PlaySound(AudioClip clip)
	{
		SoundController.controller.PlaySound (clip);
	}
		
	int SuperPowerAttack(int powerUsed, float attackDistance)
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
				
				enemy.TakeDamage (powerUsed, damage, forwardVector);
			}
        }
			
		AchievementSystem.controller.CheckMaxHits (enemiesHit);

		return enemiesHit;
    }

    int SuperPowerAttackBothDirections(int powerUsed, float attackDistance)
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

				enemy.TakeDamage (powerUsed, damage, forwardVector);

            }
        }

        for(int i = 0; i < hit2.Length; i++)
        {
            if (hit2[i])
            {
				Enemy enemy = hit2 [i].collider.gameObject.GetComponent<Enemy> ();

				if (enemy.CheckIfInvulnerable () == false)
					enemiesHit++;

				enemy.TakeDamage (powerUsed, damage, -forwardVector);


            }
        }

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
		return enemiesHit;
    }

	void LaserVisionSprite(int i)
	{
		if (SuperPowerController.controller.GetSuperHero () != (int)SuperPowerController.SuperHero.Paragon)
			return;
		bool b = (i == 0) ? false : true;

		transform.GetChild (1).GetComponent<SpriteRenderer> ().enabled = b;
	}

	void IceBreathSprite(int i)
	{
		if (SuperPowerController.controller.GetSuperHero () != (int)SuperPowerController.SuperHero.Paragon) 
			return;

		bool b = (i == 0) ? false : true;

		transform.GetChild (2).GetComponent<SpriteRenderer> ().enabled = b;
	}

    IEnumerator JumpAttack()
    {
		canAttack = false;
		int force = Mathf.Max (UpgradeController.controller.GetUpgradeLevel(UpgradeController.Upgrades.Damage) * 85, 350);
		rb2d.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
		PlaySound (SoundController.controller.capeWhoosh);

        yield return new WaitForSeconds(1f);
		RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2)), Vector2.down, 2f, LayerMask.GetMask("Default"));
        while (!hit)           
        {
			hit = Physics2D.Raycast(transform.position, Vector2.down, 10, LayerMask.GetMask("Default"));
			Debug.DrawRay (transform.position, Vector2.down, Color.green);

			if (Vector2.Distance(rb2d.velocity, Vector2.zero) < .5f) {
				anim.SetTrigger ("HitPeak");
				rb2d.AddForce(new Vector2(0, -force * 3), ForceMode2D.Impulse);
				PlaySound (SoundController.controller.capeWhoosh);
			}
            yield return null;
        }

		anim.SetTrigger ("HitGround");
		FollowPlayer.MainCamera.CameraShake ();
		PlaySound (SoundController.controller.groundSmash);
		NormalAttack (3);
		canAttack = true;
		GameCanvas.controller.UpdateAttackSlider (3);
		NoLongerAttacking ();
		GameCanvas.controller.UpdateAttackImage(3);
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
			enemiesHit = SuperPowerAttack((int)Attacks.Long, LargeAttackDistance() / max);
            steps++;
            yield return null;
            
        }
        canMove = true;

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
		GameCanvas.controller.UpdateAttackImage(3);
    }

	IEnumerator MapDash()
	{
		int enemiesHit = 0;


		Vector3 startingPos = transform.position;
		canMove = false;
		while (CheckIfInBoundaries ()) {
			rb2d.MovePosition(new Vector2(transform.position.x - 3f, transform.position.y));
			enemiesHit += SuperPowerAttack ((int)Attacks.AOE, -3f);
			yield return null;
		}
		transform.position += new Vector3 (5, 0);
		SoundController.controller.PlaySoundInList (GetComponent<AudioSource>(), SuperPowerController.PowerNames.DashAttack);
		while (CheckIfInBoundaries ()) {
			rb2d.MovePosition(new Vector2(transform.position.x + 3f, transform.position.y));
			enemiesHit += SuperPowerAttack ((int)Attacks.AOE, 3f);
			yield return null;
		}
		SoundController.controller.PlaySoundInList (GetComponent<AudioSource>(), SuperPowerController.PowerNames.DashAttack);
		while (Vector3.Distance(transform.position, startingPos) > 2) {
			rb2d.MovePosition(new Vector2(transform.position.x - 3f, transform.position.y));
			enemiesHit += SuperPowerAttack ((int)Attacks.AOE, -3f);
			yield return null;
		}
		canMove = true;

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
		GameCanvas.controller.UpdateAttackImage(3);
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
			enemiesHit += SuperPowerAttack((int)Attacks.Long, ShortAttackDistance());
            steps++;
            yield return null;

        }
        canMove = true;

		AchievementSystem.controller.CheckMaxHits (enemiesHit);
    }

	void ProjectileSpawn(GameObject prefab, int powerNumber)
	{
		GameObject projectile = Instantiate (prefab, transform.position, Quaternion.identity) as GameObject;
		projectile.GetComponent<Projectile> ().SetVariables (forwardVector, (int)Attacks.Long, XPController.controller.GetLevel(), (int)attackDistances[powerNumber]);
	}

	void ThrowGrenades()
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

	void SpawnWaterAttack()
	{
		GameObject wave = Instantiate (wavePrefab, attackPoint.position, Quaternion.identity) as GameObject;
		Vector3 originalScale = transform.localScale;
		wave.transform.SetParent (transform);
		wave.transform.localScale = new Vector3 ((originalScale.x  / transform.localScale.x), (originalScale.y / transform.localScale.x), originalScale.z);
		DestroyObject (wave, 1.167f);
	}

    public void TakeDamage(int num)
    {
        if (godMode)
            return;

        health -= num;

        if(health <= 0)
        {
            alive = false;
			anim.SetBool ("dead", true);
			AchievementSystem.controller.PlayerDeath (SuperPowerController.controller.GetSuperHero ());
			enabled = false;
        }

    }

	void RegainHealth()
	{
		health = Mathf.Min (health + (int)healthRegen, maxHealth);

		Invoke ("RegainHealth", 5);
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

	public float GetAttackTime(int num)
	{
		return attackRates[availablePowers[num]];
	}

	public void UpdatePowers(int damage, int maxHealth, float healthRegen, float powerRegen, float speed)
	{
		this.damage = 1 + damage * 2;

		this.maxHealth = Mathf.Max(starterHealth, starterHealth * (int)(maxHealth * .66f));
		GameCanvas.controller.AssignSliderMaxValues((int)GameCanvas.SliderNumbers.Health, this.maxHealth);
		this.healthRegen = 1 + healthRegen;

		for (int i = 0; i < 4; i++) {
			attackRates[availablePowers [i]] = ((5 - powerRegen + 1) / 5.0f) * defaultAttackRates[i];
			GameCanvas.controller.AssignSliderMaxValues (i, attackRates [availablePowers[i]]);
		}

		this.hSpeed = 7 + speed * 2;
	}
}
