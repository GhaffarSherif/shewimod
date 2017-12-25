using System;

namespace DuckGame.MyMod
{
    [BaggedProperty("isInDemo", true), BaggedProperty("canSpawn", true), BaggedProperty("isOnlineCapable", true), EditorGroup("Shewi|guns")]


    public class RedDuelingPistol : Gun
    {
        public RedDuelingPistol(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Red Dueling Pistol";
            this.ammo = 25;
            this._ammoType = new AT9mm();
            this._ammoType.range = 700f;
            this._ammoType.accuracy = 0.4f;
            this._ammoType.penetration = 1f;
            this._type = "gun";
            base.graphic = new SpriteMap(GetPath("/weapons/RedDuelingPistol.png"), 32, 32, false);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._barrelOffsetTL = new Vec2(20f, 15f);

            this._fireSound = GetPath("/sfx/yee");
            this._kickForce = 3f;
        }
        
        public override void OnPressAction()
        {
            
            Random r = new Random();
            if (ammo > 1)
            {
                if (r.Next(1, 101) <= 25)
                {
                    this._ammoType.barrelAngleDegrees = 180f;
                    this._ammoType.immediatelyDeadly = true;
                    this.Fire();
                }
                else
                {
                    this.Fire();
                }
                    
            }
            else
            {
                this.Destroy();
            }
        }
    }
}