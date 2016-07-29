using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    protected Vector3 forwardDirection;
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

    protected bool onFire;

    protected Rigidbody2D rb2d;

    SuperPowerController powerController;

    protected void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        EnableCanMove();


        transform.SetParent(GameObject.Find("Enemies").transform);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
    }

    // Use this for initialization
    protected void Start () {
        powerController = SuperPowerController.controller;
        EnemySpawner.controller.AddEnemy(enemyNumber);
    }

    // Update is called once per frame
    protected void Update () {
        Movement();
	}

    protected void Movement()
    {
        if (!canMove || !Player.playerSingleton.CheckAlive())
            return;

        //rb2d.MovePosition(transform.position + forwardDirection * speed * Time.deltaTime);
        //transform.position = new Vector2(transform.position.x + forwardDirection.x * speed * Time.deltaTime, transform.position.y);
        transform.Translate(CheckPlayerPos() * speed * Time.deltaTime);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, forwardDirection, attackDistance, LayerMask.GetMask("Player"));

        if(hit)
        {
            Attack(hit);
        }
    }

    protected Vector3 CheckPlayerPos()
    {
        if (transform.position.x < Player.playerSingleton.GetXPos())
        {
            forwardDirection = Vector3.right;
        }
        else forwardDirection = Vector3.left;

        return forwardDirection;
    }

    protected IEnumerator Disappear()
    {
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        while(sR.color.a > 0)
        {
            sR.color = new Color(sR.color.r,sR.color.g,sR.color.b,sR.color.a - .006f);
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
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, sR.color.a - .006f);
            yield return null;
        }
        Destroy(gameObject);
    }

	void ChangeColor(Color c)
	{
		GetComponent<SpriteRenderer> ().color = c;
	}

	protected void StunEnemy(Color c, float time)
	{
		canMove = false;

		ChangeColor (c);

		Invoke ("UnstunEnemy", time);
	}

	protected void UnstunEnemy()
	{
		StartCoroutine (Recover ());

		ChangeColor (Color.red);
	}
		

    protected void Attack(RaycastHit2D hit)
    {
        DisableCanMove();

        hit.collider.gameObject.GetComponent<Player>().TakeDamage(damage);

        StartCoroutine(Recover(2.5f));
    }

    protected void EnableCanMove()
    {
        canMove = true;
    }

    protected void DisableCanMove()
    {
        canMove = false;
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

        health -= damageTaken;

        if (health <= 0)
        {
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
				GetHit (30, 50, 50, 75, dir);
				break;
			case ((int)SuperPowerController.PowerNames.ParagonMelee):
				GetHit (20, 30, 10, 15, dir);
				break;
			case ((int)SuperPowerController.PowerNames.SpeedMelee):
				GetHit (5, 10, 1, 2, dir);
				break;
			case ((int)SuperPowerController.PowerNames.Fire):
				SetOnFire (dmg, 3);
				break;
			case ((int)SuperPowerController.PowerNames.Pistol):
				GetHit (5, 10, 0, 0, dir);
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
				GetHit (25, 30, 25, 30, dir);
				break;
			case ((int)SuperPowerController.PowerNames.FreezeBreath):
				StunEnemy (Color.blue, 4);
				break;
			case ((int)SuperPowerController.PowerNames.WindGust):
				GetHit (5, 10, 40, 50, dir);
				break;
			case ((int)SuperPowerController.PowerNames.Shotgun):
				GetHit (25, 30, 1, 10, dir);
				break;
			default:
				break;
			}
			break;
		case ((int)Player.Attacks.Long):
			switch (Player.playerSingleton.GetPower (2)) {
			case ((int)SuperPowerController.PowerNames.ShoulderCharge):
				GetHit (10, 20, 50, 50, dir);
				break;
			case ((int)SuperPowerController.PowerNames.RockThrow):
				GetHit (30, 45, 10, 20, dir);
				break;
			case ((int)SuperPowerController.PowerNames.HeatVision):
				GetComponent<SpriteRenderer> ().color = Color.black;
				break;
			case ((int)SuperPowerController.PowerNames.DashAttack):
				GetHit (30, 50, 10, 10, dir);
				break;
			case ((int)SuperPowerController.PowerNames.Sniper):
				GetHit (40, 60, 0, 0, dir);
				break;
			default:
				break;
			}
			break;
		case ((int)Player.Attacks.AOE):
			switch (Player.playerSingleton.GetPower (3)) {
			case ((int)SuperPowerController.PowerNames.GroundSmash):
				GetHit (15, 30, 25, 30, dir);
				break;
			case ((int)SuperPowerController.PowerNames.Lightning):
				StunEnemy (Color.yellow, 8);
				break;
			case ((int)SuperPowerController.PowerNames.Jump):
				GetHit (2, 10, 35, 60, dir);
				break;
			case ((int)SuperPowerController.PowerNames.MapDash):
				GetHit (50, 75, 20, 30, dir);
				break;
			case ((int)SuperPowerController.PowerNames.Grenades):
				GetHit (20, 25, 30, 35, dir);
				break;
			default:
				break;
			}
			break;
		default:
			break;
		}
    }

	protected void GetHit(int x1, int x2, int y1, int y2, int direction)
	{
		int dmg = Player.playerSingleton.GetLevel ();
		rb2d.AddForce (new Vector2 (Random.Range (dmg * direction * x1, dmg * direction * x2), Random.Range (dmg * y1, dmg * y2)), ForceMode2D.Impulse);

		StartCoroutine (Recover ());
	}

    protected IEnumerator TakeFireDamage(float damage, float timeLeft)
    {
        float time = 0;
        while(time < timeLeft)
        {
            time += Time.deltaTime;
            health -= damage;
            yield return null;
        }
        onFire = false;
    }

    protected void SetOnFire(float damage, float time)
    {
        onFire = true;
        StartCoroutine(TakeFireDamage(damage, time));
    }

    protected void Die(float dmg, int powerType, Vector3 directionHit)
    {
        rb2d.AddTorque(Random.Range(dmg * directionHit.x * 20, dmg * directionHit.x * 40));

        CheckDeathType(powerType);

        rb2d.freezeRotation = false;

		XPController.controller.AddXP (enemyNumber);
        EnemySpawner.controller.SubtractEnemy(enemyNumber);
		AchievementSystem.controller.AddKill (enemyNumber);

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
			StartCoroutine(Disappear(Player.playerSingleton.GetLevel()));
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
}
