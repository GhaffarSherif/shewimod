using System;
using System.Collections.Generic;

namespace DuckGame.MyMod
{
    [BaggedProperty("isInDemo", true), EditorGroup("guns|explosives"), BaggedProperty("canSpawn", true), BaggedProperty("isOnlineCapable", true)]
    public class Nuke : Gun
    {
        private bool finsihedExploding = false;
        public StateBinding _timerBinding = new StateBinding("_timer", -1, false);

        public StateBinding _pinBinding = new StateBinding("_pin", -1, false);
        Random r;
        private SpriteMap _sprite;

        public bool _pin = true;

        public float _timer = 1.2f;

        private Duck _cookThrower;

        private float _cookTimeOnThrow;

        public bool pullOnImpact;

        private bool _explosionCreated;

        private bool _localDidExplode;

        private bool _didBonus;
        private float laserTimer = 6f;

        private static int grenade;

        public int gr;

        public int _explodeFrames = -1;

        public Duck cookThrower
        {
            get
            {
                return this._cookThrower;
            }
        }

        public float cookTimeOnThrow
        {
            get
            {
                return this._cookTimeOnThrow;
            }
        }

        public Nuke(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("/weapons/shewi.png"), 22, 32, false);
            base.graphic = this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 10f);
            base.bouncy = 0.6f;
            this.friction = 0.05f;
            this._editorName = "Nuke";
            this._bio = "When Sherif gets angry. Yep.";
			
        }

        public override void Initialize()
        {
            this.gr = Nuke.grenade;
            Nuke.grenade++;
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            this._pin = false;
            this._localDidExplode = true;
            if (!this._explosionCreated)
            {
                Graphics.flashAdd = 1.3f;
                Layer.Game.darken = 1.3f;
            }
            this.CreateExplosion(pos);
        }

        public void CreateExplosion(Vec2 pos)
        {
            if (!this._explosionCreated)
            {
                float cx = pos.x;
                float cy = pos.y - 2f;
                Level.Add(new ExplosionPart(cx, cy, true));
                int num = 6;
                if (Graphics.effectsLevel < 2)
                {
                    num = 3;
                }
                for (int i = 0; i < num; i++)
                {
                    float dir = (float)i * 60f + Rando.Float(-10f, 10f);
                    float dist = Rando.Float(12f, 20f);
                    ExplosionPart ins = new ExplosionPart(cx + (float)(System.Math.Cos((double)Maths.DegToRad(dir)) * (double)dist), cy - (float)(System.Math.Sin((double)Maths.DegToRad(dir)) * (double)dist), true);
                    Level.Add(ins);
                }
                this._explosionCreated = true;
                SFX.Play("explode", 1f, 0f, 0f, false);
            }
        }

        public override void Update()
        {
            base.Update();
            if (_explodeFrames <= 0f && laserTimer <=2f)
                laserTimer = laserTimer - 0.01f;

            if (laserTimer <= 1.5f)
            {
                if (laserTimer < 1.5f && laserTimer > 1.1f)
                {
                    Vec2 barrel = this.Offset(base.barrelOffset);
                    this.ApplyForce(new Vec2(1f, -4f));
                    this.hSpeed = 0.005f;
                    this.vSpeed += 0.003f;
                    QuadLaserBullet b = new QuadLaserBullet(barrel.x, barrel.y, barrelVector);
                    b.killThingType = base.GetType();
                    Level.Add(b);
                }
                else if (laserTimer < 1.1f && laserTimer > 0f)
                {
                    Vec2 barrel = this.Offset(base.barrelOffset);
                    //this.ApplyForce(new Vec2(1f, -4f));
                    this.hSpeed = -0.001f;
                    this.vSpeed = -0.003f;
                    QuadLaserBullet b = new QuadLaserBullet(barrel.x, barrel.y, barrelVector);
                    b.killThingType = base.GetType();
                    Level.Add(b);
                }
            }
            
			if (laserTimer <= 0f)
			{
				Level.Remove(this);
			}

            if (!this._pin)
            {
                this._timer -= 0.01f;
            }
            if (this._timer < 0.5f && this.owner == null && !this._didBonus)
            {
                this._didBonus = true;
                if (Recorder.currentRecording != null)
                {
                    Recorder.currentRecording.LogBonus();
                }
            }
            if (!this._localDidExplode && this._timer < 0f)
            {
                if (this._explodeFrames < 0)
                {
                    this.CreateExplosion(this.position);
                    this._explodeFrames = 4;
                }
                else
                {
                    this._explodeFrames--;
                    if (this._explodeFrames == 0 && finsihedExploding == false)
                    {
                        float cx = base.x;
                        float cy = base.y - 2f;
                        Graphics.flashAdd = 1.3f;
                        Layer.Game.darken = 1.3f;
                        if (base.isServerForObject )
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                float dir = (float)i * 18f - 5f + Rando.Float(10f);
                                ATShrapnel shrap = new ATShrapnel();
                                shrap.range = 60f + Rando.Float(18f);
                                Bullet bullet = new Bullet(cx + (float)(System.Math.Cos((double)Maths.DegToRad(dir)) * 6.0), cy - (float)(System.Math.Sin((double)Maths.DegToRad(dir)) * 6.0), shrap, dir, null, false, -1f, false, true);
                                bullet.firedFrom = this;
                                this.firedBullets.Add(bullet);
                                Level.Add(bullet);
                                laserTimer = 2f;

                            }
                            
                            System.Collections.Generic.IEnumerable<Window> windows = Level.CheckCircleAll<Window>(this.position, 40f);
                            foreach (Window w in windows)
                            {
                                if (Level.CheckLine<Block>(this.position, w.position, w) == null)
                                {
                                    w.Destroy(new DTImpact(this));
                                }
                            }
                            this.bulletFireIndex += 20;
                            if (Network.isActive)
                            {
                                   // public NMFireGun(Gun g, List<Bullet> varBullets, byte fIndex, bool rel, byte ownerIndex = 4, bool onlyFireActionVar = false);
                                NMFireGun gunEvent = new NMFireGun(this, this.firedBullets, this.bulletFireIndex, false, 4, false);
                                Send.Message(gunEvent, NetMessagePriority.ReliableOrdered, null);
                                this.firedBullets.Clear();
                            }
                            this.laserTimer = 2f;
                        }
                        // Level.Remove(this);
                        finsihedExploding = true;
                        base._destroyed = true;
                        this._explodeFrames = -1;
                        this.laserTimer = 2f;
                    }
                }
            }
            if (base.prevOwner != null && this._cookThrower == null)
            {
                this._cookThrower = (base.prevOwner as Duck);
                this._cookTimeOnThrow = this._timer;
            }
            this._sprite.frame = (this._pin ? 0 : 1);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.pullOnImpact)
            {
                this.OnPressAction();
            }
            base.OnSolidImpact(with, from);
        }

        public override void OnPressAction()
        {
            if (this._pin)
            {
                this._pin = false;
                Level.Add(new GrenadePin(base.x, base.y)
                {
                    hSpeed = (float)(-(float)this.offDir) * (1.5f + Rando.Float(0.5f)),
                    vSpeed = -2f
                });
                SFX.Play("pullPin", 1f, 0f, 0f, false);
            }
        }
    }
}