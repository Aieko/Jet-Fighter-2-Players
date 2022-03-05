using UnityEngine;

public abstract class Weapon
{
    protected float ReloadTime;
    protected float RateOfFire;
    protected float LastTimeFired;

    protected GameObject Bullet;
    protected AudioClip BulletAudioClip;

    public bool OutOfAmmo { get; protected set; }

    protected int CurrentAmmoAmount;
    protected int MaxAmmoAmount;
    protected int NumOfPlayer;

    protected Weapon(int numOfPlayer)
    {
        NumOfPlayer = numOfPlayer;
    }

    public abstract void Fire(Transform sceneObject, Quaternion rotation);

    protected void Reload()
    {
        CurrentAmmoAmount = MaxAmmoAmount;
        OutOfAmmo = false;
    }

    protected virtual bool CanFire()
    {

        if (OutOfAmmo || Time.time <= LastTimeFired + RateOfFire)
            return false;
        else return true;

    }

}

public class Weapon1 : Weapon
{
    public Weapon1(int numOfPlayer) : base(numOfPlayer)
    {
        Bullet = (GameObject)Resources.Load("Projectiles/Bullet");
        BulletAudioClip = (AudioClip) Resources.Load("Sounds/Minigun");
        ReloadTime = 2f;
        RateOfFire = 0.2f;
        MaxAmmoAmount = 5;
        CurrentAmmoAmount = MaxAmmoAmount;
    }

    public override void Fire(Transform sceneObject, Quaternion rotation)
    {
       
        if (!CanFire())
        {
            if (OutOfAmmo && Time.time >= LastTimeFired + ReloadTime)
            {
                Reload();
            }
            return;
        }
        GameManager.Instance.PlayFireSound(BulletAudioClip);
        var bullet = Object.Instantiate(Bullet, sceneObject.position, rotation);
        bullet.transform.GetComponent<Projectile>().Shoot(NumOfPlayer);
       // Debug.Log(CurrentAmmoAmount + ", " + MaxAmmoAmount + "Can fire? - " + CanFire());
        LastTimeFired = Time.time;
        CurrentAmmoAmount--;

        if (CurrentAmmoAmount <= 0)
        {
            OutOfAmmo = true;
        }
    }

}

public class Weapon2 : Weapon
{
    public Weapon2(int numOfPlayer) : base(numOfPlayer)
    {
        Bullet = (GameObject)Resources.Load("Projectiles/Rocket");
        BulletAudioClip = (AudioClip)Resources.Load("Sounds/Rocket");
        ReloadTime = 1.5f;
        MaxAmmoAmount = 2;
        CurrentAmmoAmount = MaxAmmoAmount;
       
    }

    public override void Fire(Transform sceneObject, Quaternion rotation)
    {
        if (!CanFire())
        {
            if (OutOfAmmo && Time.time >= LastTimeFired + ReloadTime)
            {
                Reload();
            }
            return;
        }

        var bullet = Object.Instantiate(Bullet,sceneObject.Find("LeftGun").position, rotation);
        bullet.transform.GetComponent<Projectile>().Shoot(NumOfPlayer);
        var bullet2 = Object.Instantiate(Bullet, sceneObject.Find("RightGun").position, rotation);
        bullet2.transform.GetComponent<Projectile>().Shoot(NumOfPlayer);
        CurrentAmmoAmount -= 2;
        LastTimeFired = Time.time;
        GameManager.Instance.PlayFireSound(BulletAudioClip);
        if (CurrentAmmoAmount <= 0)
        {
            OutOfAmmo = true;
        }

    }

}

public class Weapon3 : Weapon
{
    public Weapon3(int numOfPlayer) : base(numOfPlayer)
    {
        Bullet = (GameObject)Resources.Load("Projectiles/Bomb");
        BulletAudioClip = (AudioClip)Resources.Load("Sounds/Bomb");
        ReloadTime = 2f;
        MaxAmmoAmount = 1;
        CurrentAmmoAmount = MaxAmmoAmount;
    }

    public override void Fire(Transform sceneObject, Quaternion rotation)
    {
        if (!CanFire())
        {
            if (OutOfAmmo && Time.time >= LastTimeFired + ReloadTime)
            {
                Reload();
            }
            return;
        }

        var bullet = Object.Instantiate(Bullet, sceneObject.position, rotation);
        bullet.transform.GetComponent<Projectile>().Shoot(NumOfPlayer);
        LastTimeFired = Time.time;
        CurrentAmmoAmount--;
        GameManager.Instance.PlayFireSound(BulletAudioClip);
        if (CurrentAmmoAmount <= 0)
        {
            OutOfAmmo = true;
        }
    }

}

