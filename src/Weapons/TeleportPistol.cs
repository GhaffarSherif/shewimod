using System;

namespace DuckGame.MyMod
{
    [BaggedProperty("isInDemo", true), BaggedProperty("canSpawn", true), BaggedProperty("isOnlineCapable", true), EditorGroup("guns")]


    public class TeleportPistol : Gun
    {
        // Animations
        SpriteMap sprite;

		// Gernade to be fired
		TunnelGrenade g;
		float tempX=0f, tempY=0f, teleTimer=0.06f;
		Boolean teleported = false;
        DuckPlaceHolder d;


        // The following is taken from the Bow weapon

        public StateBinding _fireAngleState = new StateBinding("_fireAngle");
        public StateBinding _aimAngleState = new StateBinding("_aimAngle");
        public StateBinding _aimWaitState = new StateBinding("_aimWait");
        public StateBinding _aimingState = new StateBinding("_aiming");
        public StateBinding _firingState = new StateBinding("_firing");
        public StateBinding _cooldownState = new StateBinding("_cooldown");
        public float _fireAngle;
        public float _aimAngle;
        public float _aimWait;
        public bool _aiming;
        public bool _firing;
        public float _cooldown;

        public override float angle
        {
            get
            {
                return base.angle + _aimAngle;
            }
            set
            {
                _angle = value;
            }
        }


        public TeleportPistol(float xval, float yval) : base(xval, yval)
        {
            _ammoType = new ATLaser();
            _editorName = "Telport Pistol";
			_bio = "Pew pew";
            this.ammo = 3;
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._barrelOffsetTL = new Vec2(20f, 15f);
            _kickForce = 0.2f;
            this._fireSound = GetPath("/sfx/pewsound");
            physicsMaterial = PhysicsMaterial.Wood;

            

            sprite = new SpriteMap(GetPath("/weapons/TeleportPistol.png"), 32, 32);
            base.graphic = sprite;
            sprite.AddAnimation("3", 0.4f, true, 0, 1, 2, 3);
            sprite.AddAnimation("2", 0.4f, true, 4, 5, 6, 7);
            sprite.AddAnimation("1", 0.4f, true, 8, 9, 10, 11);
            sprite.AddAnimation("0", 0.4f, true, 12);
            
        }
        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Duck && Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed) >= 2f)
            {
                if (isServerForObject)
                {
                    if (with.isServerForObject)
                    {
                        this.EquipGun(with as Duck);

                    }
                }
                
                this.y = 9999f;
                
            }
        }
        public void EquipGun(Duck d)
        {
            d.ThrowItem(false);
            d.GiveHoldable(this);
           
           // d.Equip(this);
        }


        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSoftImpact(with, from);
        }
        public override void Initialize()
        {
            base.Initialize();
            if (ammo >= 3) 
                sprite.SetAnimation("3");
            else
                sprite.SetAnimation(ammo.ToString());
        }
        public override void OnPressAction()
        {
			
			if (teleTimer != 0.06f)
			{
				DoAmmoClick();
			}
			if (teleTimer == 0.06f)
			{
				if (owner == null)
					return;
				if (_cooldown != 0f)
					return;
				if (ammo > 0)
				{
					_aiming = true;
					_aimWait = 1f;
					_ammoType.bulletSpeed = 6f;
				}
			}
        }

        public override void OnReleaseAction()
        {
			if (teleTimer == 0.06f)
			{
				
				if (_cooldown != 0 || ammo <= 0)
					return;
				_aiming = false;
				Fire();
				_cooldown = 1f;
				angle = 0f;
				_fireAngle = 0f;
			}          
        }
	
        public override void Update()
        {
            base.Update();
            
			//Animations
            if (ammo >= 3)
                sprite.SetAnimation("3");
            else if (ammo == 2)
                sprite.SetAnimation("2");
            else if (ammo == 1)
                sprite.SetAnimation("1");
            else
                sprite.SetAnimation("0");


            if (tempX != 0f && tempY != 0f)
            {
                d = new DuckPlaceHolder(tempX, tempY);
                Level.Add(new DuckPlaceHolder(tempX, tempY));
            }

            if (g != null && g._timer <= 0.1f && teleported == false)
            {
                tempX = g.position.x;
                tempY = g.position.y - 10f;
                teleported = true;

            }

            if (duck != null)
			{
				if (g != null && teleported == false && duck != null )
				{
                    
					if (g._timer <= 0.1f)
					{
						tempX = g.position.x;
						tempY = g.position.y -10f;
						teleported = true;
                       
                    }
				}

				if (g != null && tempX != 0f && tempY != 0f && teleported == true)
				{
					if (g._destroyed == true)
					{
						teleTimer = teleTimer - 0.01f;
						if (teleTimer <= 0.03f)
						{
                            Level.Add(SmallSmoke.New(duck.position.x,duck.position.y));
                            if (duck.ragdoll != null)
                            {
                                duck.ragdoll.position.x = tempX;
                                duck.ragdoll.position.y = tempY;
                            }
                            else {
                                duck.position.x = tempX;
                                duck.position.y = tempY;
                            }
                            Level.Add(SmallSmoke.New(duck.position.x, duck.position.y));
                            duck.vSpeed = -0.5f;

							g = null;
							teleported = false;
							tempX = 0f;
							tempY = 0f;
							teleTimer = 0.06f;
                            SFX.Play(GetPath("/sfx/teleport"), 1f, 0f, 0f, false);
                            Level.Remove(d);
                            level.RemoveThing(d);
                            d = null;
                        }
					}					
				}
			}
			
			

            if (_aiming && _aimWait <= 0f && _fireAngle < 80f)
            {
                _ammoType.bulletSpeed += 0.3f;
                _fireAngle += 7f;
            }
            if (_aimWait > 0.0)
                _aimWait -= 0.9f;
            if (_cooldown > 0.0)
                _cooldown -= 0.1f;
            else
                _cooldown = 0f;
            if (owner != null)
            {
                _aimAngle = -Maths.DegToRad(_fireAngle);
                if (offDir < 0)
                    _aimAngle = -_aimAngle;
            }
            else
            {
                _aimWait = 0f;
                _aiming = false;
                _aimAngle = 0f;
                _fireAngle = 0f;
            }
            if (!_raised)
                return;
            _aimAngle = 0.0f;
        }

        
        public override void Fire()
        {
            if(!loaded)
                return;
            firedBullets.Clear();
            if (ammo > 0 && _wait == 0)
            {
                ApplyKick();
                for (int index = 0; index < _numBulletsPerFire; ++index)
                {
                    float num = _ammoType.accuracy;
                    _ammoType.accuracy *= 1f - _accuracyLost;
                    _ammoType.bulletColor = _bulletColor;
                    float angle = offDir >= 0 ? angleDegrees + _ammoType.barrelAngleDegrees : angleDegrees + 180f - _ammoType.barrelAngleDegrees;
                    if (!receivingPress)
                    {
						Vec2 vec = barrelVector * Rando.Float(1f, 3f);
						
						if (barrelVector.x > 0f)
						{
							
							vec.y += _aimAngle - 4f;
							vec.x += _ammoType.bulletSpeed ;
						}
						else
						{
							vec.y += -(_aimAngle + 4f);
							vec.x += -(_ammoType.bulletSpeed );
						}

						//addded the following code
						g = new TunnelGrenade(this.x, this.y);
						Level.Add(g);

                        g.bouncy = 0.6f;
						g._pin = false;
						teleported = false;

                       // Bullet bullet = _ammoType.FireBullet(Offset(barrelOffset), owner, angle, this);
						g.ApplyForce(vec);

						/*if (Network.isActive && isServerForObject)
                        {
                            firedBullets.Add(bullet);
                            if (duck != null && duck.profile.connection != null)
                                bullet.connection = duck.profile.connection;
                        }*/
					}
					++bulletFireIndex;
                    _ammoType.accuracy = num;
                }
                loaded = false;
                if (!_manualLoad)
                    Reload(true);
                firing = true;
                _wait = _fireWait;
                PlayFireSound();
                if (owner == null)
                {
                    Vec2 vec2 = barrelVector * Rando.Float(1f, 3f);
                    vec2.y += Rando.Float(2f);
                    hSpeed -= vec2.x;
                    vSpeed -= vec2.y;
                }
                _accuracyLost += loseAccuracy;
                if (_accuracyLost <= maxAccuracyLost)
                    return;
                _accuracyLost = maxAccuracyLost;
            }
            else
            {
                if (ammo > 0 || _wait != 0f)
                    return;
                _wait = _fireWait;
            }

        }
    }
}