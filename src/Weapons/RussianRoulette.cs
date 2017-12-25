using System;

namespace DuckGame.MyMod
{
    [BaggedProperty("isInDemo", true), BaggedProperty("canSpawn", true), BaggedProperty("isOnlineCapable", true), EditorGroup("guns|misc")]
    

    public class RussianRoulette : Gun
    {
        Boolean reloaded = true;
        SpriteMap sprite;
        public RussianRoulette(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Last Spin";
            this.ammo = 25;
            this._ammoType = new AT9mm();
            this._ammoType.range = 70f;
            this._ammoType.accuracy = 1f;
            this._ammoType.penetration = 0.4f;
            this._ammoType.immediatelyDeadly = true;
            this._ammoType.barrelAngleDegrees = 135f;
            this._type = "gun";
            
            sprite = new SpriteMap(GetPath("/weapons/Sadgun.png"), 32, 32);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._barrelOffsetTL = new Vec2(20f, 15f);
            this._fireSound = GetPath("/sfx/revolverspin");
            this._kickForce = 3;
            base.graphic = sprite;
        }

        public override void OnPressAction()
        {
            Random r = new Random();
            if (ammo > 1)
            {
                if (reloaded)
                {
                    _hasTrigger = true;
                    if (r.Next(1, 101) <= 17)
                    {
                        this._fireSound = "tinyGun";
                        this.Fire();
                    }
                    else
                    {
                        SFX.Play(GetPath("/sfx/click"), 1f, 0f, 0f, false);
                    }
                    reloaded = false;
                    base.angle = base.angle + -Maths.DegToRad(145f);
                    sprite.frame = 0;
                }
                else
                {
                    _hasTrigger = false;
                    SFX.Play(GetPath("/sfx/revolverspin"), 1f, 0f, 0f, false);
                    reloaded = true;
                    sprite.frame = 1;
                }

            }
            else
            {
                this.Destroy();
            }
        }
    }

}