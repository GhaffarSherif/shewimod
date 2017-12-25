using System;

namespace DuckGame.MyMod
{
	[BaggedProperty("isInDemo", true), BaggedProperty("canSpawn", true), BaggedProperty("isOnlineCapable", true), EditorGroup("Shewi|explosives")]
	public class PhoneyGernade : Gun
	{
		public StateBinding _timerBinding = new StateBinding("_timer", -1, false);

		public StateBinding _pinBinding = new StateBinding("_pin", -1, false);

		private SpriteMap _sprite;

		public bool _pin = true;

		public float _timer = 1.2f;

		private Duck _cookThrower;

		private float _cookTimeOnThrow;

		public bool pullOnImpact;

		private bool _explosionCreated;

		private bool _localDidExplode;

		private bool _didBonus;

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

		public PhoneyGernade(float xval, float yval) : base(xval, yval)
		{
			
			this.ammo = 1;
			this._ammoType = new ATShrapnel();
			this._ammoType.penetration = 0.4f;
			this._type = "gun";
			this._sprite = new SpriteMap("grenade", 16, 16, false);
			base.graphic = this._sprite;
			this.center = new Vec2(7f, 8f);
			this.collisionOffset = new Vec2(-4f, -5f);
			this.collisionSize = new Vec2(8f, 10f);
			base.bouncy = 0.4f;
			this.friction = 0.05f;
			this._editorName = "Phoney Grenade";
			this._bio = "Me and this Grenade got something in common, were filled with dissapointments.";
		}

		public override void Initialize()
		{
			this.gr = PhoneyGernade.grenade;
			PhoneyGernade.grenade++;
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
				
				this._explosionCreated = true;
                SFX.Play(GetPath("/sfx/womp"), 1f, 0f, 0f, false);
            }
		}

		public override void Update()
		{
			base.Update();
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
					if (this._explodeFrames == 0)
					{
						float cx = base.x;
						float cy = base.y - 2f;
						Graphics.flashAdd = 1.3f;
						Layer.Game.darken = 1.3f;
						if (base.isServerForObject)
						{
							
							System.Collections.Generic.IEnumerable<Window> windows = Level.CheckCircleAll<Window>(this.position, 40f);
							foreach (Window w in windows)
							{
								if (Level.CheckLine<Block>(this.position, w.position, w) == null)
								{
									w.Destroy(new DTImpact(this));
								}
							}
							this.bulletFireIndex += 20;
							
						}
						Level.Remove(this);
						base._destroyed = true;
						this._explodeFrames = -1;
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
