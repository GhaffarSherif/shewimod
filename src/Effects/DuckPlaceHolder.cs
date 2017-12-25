using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.MyMod
{
    public class DuckPlaceHolder : PhysicsParticle
    {
        private SpriteMap _sprite;

        public override void Initialize()
        {
            base.Initialize();
            _sprite.SetAnimation("animation");
            
            _visibleInGame = true;
            visible = true;
        }
        public DuckPlaceHolder(float xpos, float ypos) : base(xpos, ypos)
        {
            _gravMult = 0;
            _sprite = new SpriteMap(GetPath("/weapons/placeholder"), 32, 32);
            base.graphic = _sprite;
            _sprite.AddAnimation("animation", 1.5f, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27);
            this.center = new Vec2(16f, 16f);
            
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
