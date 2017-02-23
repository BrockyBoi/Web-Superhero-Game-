using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	#region variables
	public Transform attackTransform;

    protected Vector3 forwardDirection;
	protected Vector3 originalScale;
    [SerializeField]
    protected float health;
    [SerializeField]
    [Range(0,10)]
    protected float speed;
    [SerializeField]
    protected int damage;

    [SerializeField]
    protected float attackDistance;

    public int enemyNumber;

    protected bool canMove = true;
    protected bool invulnerable;
	protected bool canAttack;

	protected bool stunned;
    protected bool onFire;
	protected bool dead;

    protected Rigidbody2D rb2d;

	protected bool tutorialMode;

	private Animator anim;
	#endregion

    protected void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator> ();
        transform.SetParent(GameObject.Find("Enemies").transform);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
    }

    protected void Start () {
		originalScale = transform.localScale;

		if (!tutorialMode) {
			EnemySpawner.controller.AddEnemy (enemyNumber);
			EnableCanMove();
		}
    }

    protected void Update () {
        Movement();
	}

    protected void Movement()
    {
		if (Player.playerSingleton.CheckAlive ()) {
			if (tutorialMode || !canMove)
				return;

			transform.position = Vector2.MoveTowards (transform.position, new Vector2 (transform.position.x + CheckPlayerPos ().x, transform.position.y), speed * Time.deltaTime);

			float dist = Vector2.Distance (attackTransform.position, Player.playerSingleton.transform.position);
			if (dist <= attackDistance) {
				DisableCanMove ();
				anim.SetTrigger ("attack");
			}
		} else
			transform.position = Vector2.MoveTowards (transform.position, new Vector2(transform.position.x + forwardDirection.x, transform.position.y), speed * Time.deltaTime);
    }

    protected Vector3 CheckPlayerPos()
    {
		if (transform.position.x < Player.playerSingleton.GetXPos ()) {
			forwardDirection = Vector3.right;
			transform.localScale = new Vector2 (originalScale.x, originalScale.y);
		} else {
			transform.localScale = new Vector2 (-originalScale.x, originalScale.y);
			forwardDirection = Vector3.left;
		}

        return forwardDirection;
    }

    protected IEnumerator Disappear()
    {
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        while(sR.color.a > 0)
        {
            sR.color = new Color(sR.color.r,sR.color.g,sR.color.b,sR.color.a - .019f);
            yield return null;
        }
        Destroy(gameObject);
    }

    protected IEnumerator Disappear(float time)
    {
        yield return new WaitForSeconds(time);
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        while (sR.color.a > 0)
        {
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, sR.color.a - .012f);
            yield return null;
        }
        Destroy(gameObject);
    }

	void ChangeColor(Color c)
	{
		GetComponent<SpriteRenderer> ().color = c;
	}

	public void StunEnemy(Color c, float time)
	{
		canMove = false;
		stunned = true;

		ChangeColor (c);

		if(health > 0)
			Invoke ("UnstunEnemy", time);
	}

	protected void UnstunEnemy()
	{
		StartCoroutine (Recover ());
		stunned = false;

		ChangeColor (Color.white);
	}

	public void SetTutorialMode(bool b)
	{
		tutorialMode = b;
	}

    protected void Attack()
    {
		if (tutorialMode || invulnerable || stunned)
			return;

			if(enemyNumber < 3)
			{

		
		if(Physics2D.Raycast(attackTransform.position, forwardDirection, attackDistance, LayerMask.GetMask("Player"))){
			Player.playerSingleton.TakeDamage (damage);
		} 
			}
			else{
				if(Physics2D.Raycast(attackTransform.position, forwardDirection, attackDistance / 2, LayerMask.GetMask("Player")) || Physics2D.Raycast(attackTransform.position, -forwardDirection, attackDistance / 2, LayerMask.GetMask("Player")))
				{
					Player.playerSingleton.TakeDamage(damage);
				}
			}

		if (canAttack) {
			anim.SetTrigger ("attack");
		} else {
			StartCoroutine (Recover ());
		}
    }

    protected void EnableCanMove()
    {
        canMove = true;
		anim.SetBool ("canMove", true);
    }

    protected void DisableCanMove()
    {
        canMove = false;
		anim.SetBool ("canMove", false);
    }

    protected void EnableInvulnerable()
    {
        invulnerable = true;
    }

    protected void DisableInvulnerable()
    {
        invulnerable = false;
    }

    public void TakeDamage(int powerType, float damageTaken, Vector3 directionHit)
    {
        if (invulnerable)
            return;

        CheckPowerHitBy(powerType, damageTaken, directionHit);

		DisableCanMove ();

        health -= damageTaken;

		anim.SetTrigger ("hit");

        if (!tutorialMode && health <= 0)
        {
			anim.SetBool ("dead", true);
            Die(damageTaken, powerType, directionHit);
        }
    }

    protected void CheckPowerHitBy(int powerHitBy, float dmg, Vector3 directionHit)
    {
		int dir = (int)directionHit.x;

		switch (powerHitBy) {
		case ((int)Player.Attacks.Short):
			switch (Player.playerSingleton.GetPower (0)) {
			case ((int)SuperPowerController.PowerNames.TankMelee):
				GetHit (30, 50, 50, 75, dir, dmg);
				break;
			case ((int)SuperPowerController.PowerNames.ParagonMelee):
				GetHit (50, 60, 10, 20, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.SpeedMelee):
				GetHit (5, 10, 1, 2, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.Fire):
				if(!onFire)
					SetOnFire (dmg, 3);
				break;
			case ((int)SuperPowerController.PowerNames.Bat):
				GetHit (20, 25, 0, 0, dir,dmg);
				break;
			default:
				break;
			}
			break;
		//Medium Attacks
		case ((int)Player.Attacks.Medium):
			switch (Player.playerSingleton.GetPower (1)) {
			case ((int)SuperPowerController.PowerNames.SonicClap):
				StunEnemy (Color.yellow, 2);
				break;
			case ((int)SuperPowerController.PowerNames.Wave):
				GetHit (25, 30, 25, 30, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.FreezeBreath):
				StunEnemy (Color.blue, dmg);
				break;
			case ((int)SuperPowerController.PowerNames.WindGust):
				GetHit (5, 10, 40, 50, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.Shotgun):
				GetHit (25, 30, 1, 10, dir,dmg);
				break;
			default:
				break;
			}
			break;
		case ((int)Player.Attacks.Long):
			switch (Player.playerSingleton.GetPower (2)) {
			case ((int)SuperPowerController.PowerNames.ShoulderCharge):
				GetHit (10, 20, 50, 50, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.RockThrow):
				GetHit (75, 100, 10, 20, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.HeatVision):
				GetHit (50, 60, 0, 0, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.DashAttack):
				GetHit (30, 50, 10, 10, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.Sniper):
				GetHit (100, 135, 0, 0, dir,dmg);
				break;
			default:
				break;
			}
			break;
		case ((int)Player.Attacks.AOE):
			switch (Player.playerSingleton.GetPower (3)) {
			case ((int)SuperPowerController.PowerNames.GroundSmash):
				GetHit (35, 40, 35, 40, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.Lightning):
				StunEnemy (Color.yellow, 8);
				break;
			case ((int)SuperPowerController.PowerNames.Jump):
				GetHit (15, 35, 45, 70, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.MapDash):
				GetHit (50, 75, 20, 30, dir,dmg);
				break;
			case ((int)SuperPowerController.PowerNames.Grenades):
				GetHit (20, 25, 30, 35, dir,dmg);
				break;
			default:
				break;
			}
			break;
		default:
			break;
		}
    }

	protected void GetHit(int x1, int x2, int y1, int y2, int direction, float dmg)
	{
		rb2d.AddForce (new Vector2 (Random.Range (dmg * direction * x1, dmg * direction * x2), Random.Range (dmg * y1, dmg * y2)), ForceMode2D.Impulse);

		if(health > 0)
			StartCoroutine (Recover ());
	}

	protected void SetOnFire(float damage, float time)
	{
		onFire = true;
		StartCoroutine(TakeFireDamage(damage, time));
	}

    protected IEnumerator TakeFireDamage(float damage, float timeLeft)
	{
        float time = 0;
        while(time < timeLeft)
        {
            time += Time.deltaTime;
			health -= damage * .05f;
			if (health <= 0) {
				Die (0, 0, Vector3.up);
				StopCoroutine (TakeFireDamage(damage, timeLeft));
			}
            yield return null;
        }
        onFire = false;
    }
		

    protected void Die(float dmg, int powerType, Vector3 directionHit)
    {
		rb2d.freezeRotation = false;
        rb2d.AddTorque(Random.Range(dmg * directionHit.x * 1000, dmg * directionHit.x * 2500));

		if(enemyNumber != 3)
        CheckDeathType(powerType);
		else StartCoroutine(Disappear());

		XPController.controller.AddXP (enemyNumber);
        EnemySpawner.controller.SubtractEnemy(enemyNumber);
		AchievementSystem.controller.KilledEnemy ((int)dmg, enemyNumber);

        enabled = false;
    }

    protected void CheckDeathType(int powerType)
    {
		switch(Player.playerSingleton.GetPower(powerType))
        {
            case ((int)SuperPowerController.PowerNames.HeatVision):
                StartCoroutine(Disappear());
                    break;
		case ((int)SuperPowerController.PowerNames.Fire):
                StartCoroutine(Disappear());
                break;
            default:
			StartCoroutine(Disappear(2));
                break;
        }
    }

    protected IEnumerator Recover()
    {
        EnableInvulnerable();
        DisableCanMove();
        while (rb2d.velocity.magnitude > 0)
        {
            yield return null;
        }
        DisableInvulnerable();
        EnableCanMove();

		if (tutorialMode)
			Destroy (gameObject);
    }

    protected IEnumerator Recover(float time)
    {
        yield return new WaitForSeconds(time);
        EnableInvulnerable();
        DisableCanMove();
        while (rb2d.velocity.magnitude > 0)
        {
            yield return null;
        }
        DisableInvulnerable();
        EnableCanMove();
    }

    protected void SetGravity(float num)
    {
        rb2d.gravityScale = num;
    }

	public bool CheckIfInvulnerable()
	{
		return invulnerable;
	}
}
