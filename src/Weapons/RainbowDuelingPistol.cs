using System;

namespace DuckGame
{
    [BaggedProperty("isInDemo", true), BaggedProperty("canSpawn", true), BaggedProperty("isOnlineCapable", true), EditorGroup("guns")]


    public class RainbowDuelingPistol : Gun
    {
        Boolean isInvisible = false;
        private SpriteMap sprite;
        public RainbowDuelingPistol(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Rainbow Dueling Pistol";
            this.ammo = 25;
            this._ammoType = new AT9mm();
            this._ammoType.range = 300f;
            this._ammoType.accuracy = 0.7f;
            this._ammoType.penetration = 1f;
            this._type = "gun";
            sprite = new SpriteMap(GetPath("weapons/RainbowDuelingPistol"), 12, 9);
            graphic = sprite;
            sprite.AddAnimation("animation", 0.9f, true, 0, 1, 2, 3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69);
            

            this.center = new Vec2(8f, 4.5f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._barrelOffsetTL = new Vec2(15f, 3f);

            this._fireSound = GetPath("/sfx/yee");
            this._kickForce = 3f;
        }
        public override void Initialize()
        {
            base.Initialize();
            sprite.SetAnimation("animation");
        }
        public void switchAmmo(int randomNum) {
            
            switch (randomNum)
            {
                case 1:
                    this._ammoType = new ATPlasmaBlaster();
                    break;
                case 2:
                    this._ammoType = new ATMissile();
                    break;
                case 3:
                    this._ammoType = new ATSniper();
                    break;
                case 4:
                    this._ammoType = new ATReboundLaser();
                    break;
                case 5:
                    this._ammoType = new ATLaser();
                    break;
                default:
                    
                    break;
            }
        }
        public override void Update()
        {
            base.Update();
            
            if (duck != null && duck.inputProfile.Pressed(Triggers.Quack))
            {
                if (isInvisible)
                {
                    this.duck.visible = true;
                    isInvisible = false;
                    
                }
                else {
                    this.duck.visible = false;
                    isInvisible = true;
                }
            }
            

        }
        
        public override void OnPressAction()
        {
            Random r = new Random();
            if (ammo > 1)
            {
                switchAmmo(r.Next(1, 6));
                this.Fire();

            }
            else
            {
                this.Destroy();
            }
        }
    }

}