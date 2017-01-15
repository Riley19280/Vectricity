using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Vectricity
{
    public class Enemy
    {
        //refactor and organize theese vars
        #region Vars
        protected bool isBoss = false;
        protected int value;
        protected float radius;
        protected Random _rand = new Random();
        protected Texture2D sprite;
        protected Vector2 location;
        protected Color color = Color.White;
        protected bool IsDead = false;
        protected float difficulty = 1, health = 100, damage = 10;
        public float Rotation, speed = 5;
        protected float scale = 1.0f;
        public Vector2 Direction;
        protected bool isTakingDamage;
        protected float dmgPerTick;
        protected float dmgTime;

        protected bool tarDead = false;
        protected int atkCooldown = 0;
        Vector2 point = GM.basePos;
        int attackIndex;
        Vector2 targetPlayer;
        protected int trackMode = 0;

        #endregion

        #region Properties

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public float Radius
        {
            get { return radius; }
            protected set { radius = value; }
        }


        public float Damage
        {
            get { return damage; }
            protected set { damage = value; }
        }



        #endregion

        #region Constructors
        public Enemy(Vector2 pos)
        {
            Initialize();
            location = pos;
            attackIndex = _rand.Next(PlayerManager.players.Count);
            trackMode = 0;
        }

        public Enemy(Vector2 pos, Vector2 Point)
        {
            Initialize();
            location = pos;
            point = Point;
            trackMode = 1;
        }
        #endregion

        public virtual void Initialize()
        {
            health = 100 * GM.Difficulty;
            Radius = sprite.Width / 2.2f;

        }

        public virtual void Update(GameTime gameTime)
        {
            CheckDead();

            //update meelee timer
            atkCooldown--;
            if (atkCooldown < 0) atkCooldown = 0;

            //update take damage timer


            switch (trackMode)
            {
                case 0:
                    FollowPlayer();
                    break;

                case 1:
                    gotoPoint(point);
                    break;
            }

            dmgTime--;
            if (dmgTime <= 0) { dmgTime = 0; isTakingDamage = false; }

            //update take damage
            if (isTakingDamage == true)
            {
                health -= dmgPerTick;

            }
            else { color = Color.White; }



            location = Vector2.Clamp(location, Vector2.Zero + new Vector2(sprite.Height / 2, sprite.Width / 2), GM.ScreenDims - new Vector2(sprite.Height / 2, sprite.Width / 2));
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!isBoss)
                spriteBatch.Draw(sprite, location, null, color, Rotation, new Vector2(sprite.Width / 2, sprite.Height / 2), scale, SpriteEffects.None, 0.0f);
            else
                spriteBatch.Draw(sprite, location, null, Color.White, Rotation, new Vector2(sprite.Width / 2, sprite.Height / 2), scale, SpriteEffects.None, 0.0f);
        }

        #region behavours

        protected void gotoPoint(Vector2 pos)
        {
            trackMode = 1;
            point = pos;
            Rotation = (float)Math.Atan2(((double)point.Y - location.Y), ((double)point.X - location.X));
            Direction = point - location;
            Direction.Normalize();

            if (Vector2.DistanceSquared(location, point) >= 25)
                location += Direction * speed;

            if (Vector2.DistanceSquared(location, point) <= 25) { trackMode = 0; }

        }

        protected void FollowPlayer()
        {

            targetPlayer = PlayerManager.players.ElementAt(attackIndex).Location;
            try
            {
                tarDead = PlayerManager.players.ElementAt(attackIndex).IsDead;
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.StackTrace); }


            if (!tarDead)
            {
                Rotation = (float)Math.Atan2(((double)targetPlayer.Y - location.Y), ((double)targetPlayer.X - location.X));


                Direction = targetPlayer - location;
                Direction.Normalize();

                location += Direction * speed;
            }
            else
            {
                attackIndex = _rand.Next(PlayerManager.players.Count);
            }
        }

        //needs overload to attack player base
        public void MeeleeAttack(PlayerShip pl)
        {

            if (atkCooldown <= 0)
            {
                pl.TakeDamage(Damage);
                atkCooldown = 20;
            }

        }

        public void ChaseOnHit()
        {

            trackMode = 0;
        }
        #endregion

        private void CheckDead()
        {
            if (health <= 0) { Die(); }
        }

        public void Die()
        {
            EnemyManager.DelEnemy(this);
            GM.Points += value;
            GM.maxPoints += value;

            ParticleManager.addParticle(Location, 3, Art.Line);

        }

        public void TakeDamage(float dmg)
        {
            health -= dmg;
            ChaseOnHit();
        }

        public void TakeDamage(float dmg, float time)
        {
            isTakingDamage = true;
            dmgPerTick = dmg / time;
            dmgTime = time;
            color = Color.Red;

        }

    }

    public class Triangle : Enemy
    {

        public Triangle(Vector2 pos) : base(pos)
        {
            Location = pos;
            Initialize();
            value = 3;
        }

        public override void Initialize()
        {
            sprite = Art.Triangle;
            base.Initialize();
            scale = 0.5f;
            Radius = (sprite.Width / 2.1f) * scale;
            //gotoPoint(new Vector2(300));

        }
    }

    public class Splitter : Enemy
    {
        public Splitter(Vector2 pos) : base(pos)
        {

            Initialize();
        }
        public override void Initialize()
        {
            sprite = Art.Splitter;
            base.Initialize();
            scale = 0.5f;
            Radius = (sprite.Width / 2.1f) * scale;
            value = 7;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (health <= 0)
            {
                int offset = 300;

                EnemyManager.AddEnemy(new Triangle(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));
                EnemyManager.AddEnemy(new Triangle(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));
                EnemyManager.AddEnemy(new Triangle(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));
                EnemyManager.AddEnemy(new Triangle(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));
                EnemyManager.AddEnemy(new Triangle(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));
                EnemyManager.AddEnemy(new Triangle(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));


            }

        }



    }

    public class Splitter2 : Enemy
    {
        public Splitter2(Vector2 pos)
            : base(pos)
        {

            Initialize();
        }
        public override void Initialize()
        {
            sprite = Art.Splitter;
            base.Initialize();
            scale = 0.5f;
            Radius = (sprite.Width / 2.1f) * scale;
            value = 8;
            health = 300 * difficulty;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (health <= 0)
            {
                int offset = 300;

                EnemyManager.AddEnemy(new Splitter(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));
                EnemyManager.AddEnemy(new Splitter(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));
                EnemyManager.AddEnemy(new Splitter(location + new Vector2(_rand.Next(-offset, offset), _rand.Next(-offset, offset))));


            }

        }



    }

    public class Shooter : Enemy
    {
        public Shooter(Vector2 pos)
            : base(pos)
        {

            Initialize();
        }
        public override void Initialize()
        {
            sprite = Art.Square;
            base.Initialize();
            scale = 0.5f;
            Radius = (sprite.Width / 2.1f) * scale;
            Damage = 10 * (difficulty / 2);
            health = 200 * difficulty / 1.5f;
            value = 10;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (atkCooldown <= 0 && !tarDead)
            {
                if (WaveManager.waveNum < 10)
                {

                    baseBullet b = new bullet_T2(location, Rotation + (float)(Math.PI / 2), false);

                    b.color = Color.Blue;
                    Bullets.Add(b);
                }
                else if (WaveManager.waveNum >= 10 && WaveManager.waveNum <= 20)
                {

                    baseBullet b = new laser_T4(location, Rotation + (float)(Math.PI / 2), false);


                    Bullets.Add(b);

                }
                else
                {

                    baseBullet b = new missile_T4(location, Rotation + (float)(Math.PI / 2), false);


                    Bullets.Add(b);
                }


                atkCooldown = 75;
            }
        }

    }

    public class CloudBoss : Enemy
    {
        int attTime = 15;
        PlayerShip target;
        List<bolt> bolts = new List<bolt>();
        List<bolt> delBolts = new List<bolt>();


        public CloudBoss(Vector2 pos) : base(pos)
        {
            Location = pos;
            Initialize();
            damage = 0;
            value = 500;
            speed = 10;
        }

        public override void Initialize()
        {
            sprite = Art.CloudBoss;
            base.Initialize();
            health = 15000 * difficulty;
            scale = 1.0f;
            Radius = (sprite.Width / 2.5f) * scale;
            speed = 4;
            damage = 75;
            isBoss = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (health <= 0) Die();

            if (attTime > 0) attTime--;
            if (attTime == 0) { Attack(); attTime = 60; }

            foreach (bolt b in bolts)
            {
                if (b.dead == true)
                    delBolts.Add(b);
                else
                    b.Update(gameTime);

            }

            foreach (bolt b in delBolts)
            {
                bolts.Remove(b);
            }
            delBolts.Clear();

            dmgTime--;
            if (dmgTime <= 0) { dmgTime = 0; isTakingDamage = false; }

            //update take damage
            if (isTakingDamage == true)
            {
                health -= dmgPerTick;

            }
            else { color = Color.White; }


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach (bolt b in bolts)
            {
                b.Draw(spriteBatch);
            }

        }

        private void Attack()
        {
            target = findTarget();
            if (Vector2.Distance(target.Location, location) < 400)
                Shoot(target);


        }

        private void Shoot(PlayerShip target)
        {
            bolts.Add(new bolt(location, target));

        }

        private PlayerShip findTarget()
        {
            return PlayerManager.player(_rand.Next(1, PlayerManager.players.Count));
        }

        class bolt
        {
            public bool dead = false;

            float textureWidth = Art.Lightning.Width;

            Rectangle lightning;

            float rotation, damage = 50;

            int tmr = 4;

            Vector2 midLoc, moveVec, playerLoc;

            public Vector2 Location { get; protected set; }

            public bolt(Vector2 stLoc, PlayerShip pl)
            {
                pl.TakeDamage(damage);
                Location = stLoc;
                midLoc = (pl.Location + Location) / 2;
                playerLoc = pl.Location;

                moveVec = (pl.Location - Location);
                moveVec.Normalize();

                rotation = (float)Math.Atan2(((double)playerLoc.Y - Location.Y), ((double)playerLoc.X - Location.X)) + MathHelper.PiOver2;
                Art.Thunder.Play();
            }

            public void Update(GameTime gameTime)
            {
                Location = Vector2.Clamp(Location, Vector2.Zero + new Vector2(Art.Lightning.Height / 2, Art.Lightning.Width / 2), GM.ScreenDims - new Vector2(Art.Lightning.Height / 2, Art.Lightning.Width / 2));

                doAnim();


            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (dead == false)
                    spriteBatch.Draw(Art.Lightning, lightning, null, Color.White, rotation, new Vector2(Art.Lightning.Width / 2, Art.Lightning.Height / 2), SpriteEffects.None, 0f);

            }

            public void doAnim()
            {
                if (tmr > 0) tmr--;
                if (tmr <= 0) dead = true;

                lightning = new Rectangle((int)midLoc.X, (int)midLoc.Y, Art.Lightning.Width, (int)Vector2.Distance(Location, playerLoc));
            }




        }

    }
}


