using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using MyDataTypes;

namespace Vectricity
{
    public class GameRoot : Microsoft.Xna.Framework.Game
    {
        //this is the main class where everything else originates form
        #region VARIABLES
        public static GameRoot Instance { get; private set; }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;




        #endregion

        public GameRoot()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Instance = this;
            if (GM.REALMODE == true)
            {
                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferWidth = 1200;
                graphics.PreferredBackBufferHeight = 800;
                graphics.ApplyChanges();

            }

            KEYBOARDENABLED ke;
            ke = Content.Load<KEYBOARDENABLED>("enableKeyboard");
            if (ke.enabled == true)
                GM.keyboardEnabled = true;
            else
                GM.keyboardEnabled = false;
        }


        protected override void Initialize()
        {
            //setting up crucial parts
            InputManager.addPlayer1();

            GM.ScreenDims = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.LoadContent(Content);
            Art.LoadContent(Content);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        protected override void Update(GameTime gameTime)
        {

            //update screen dims
            GM.ScreenDims = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            GM.Update(gameTime);
            InputManager.Update(gameTime);
            ScreenManager.Update(gameTime);

            //id the game is running
            if (GM.InGame)
            {
                WaveManager.Update(gameTime);
                CollisionManager.Update(gameTime);
                Bullets.Update(gameTime);
                EnemyManager.Update(gameTime);
                PlayerManager.Update(gameTime);
                ItemManager.Update(gameTime);
                ParticleManager.Update(gameTime);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //telling everything else to draw its things

            if (!GM.InGame)
                spriteBatch.Begin();

            if (GM.InGame)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
                ParticleManager.Draw(spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin();
                ItemManager.Draw(spriteBatch);
                Bullets.Draw(spriteBatch);
                EnemyManager.Draw(spriteBatch);
                PlayerManager.Draw(spriteBatch);
            }

            ScreenManager.Draw(spriteBatch);

            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static class GM
    {
        //global vars
        public static Vector2 ScreenDims;

        public static int maxPoints = 0;
        public static int Points = 0;
        public static bool InGame = false;
        public static Vector2 basePos = GM.ScreenDims / 2;
        public static bool REALMODE = true;
        public static int nukeRefund = 0;
        public static int firewallTime = 1800;
        public static int freezeTime = 1800;
        public static int towerTime = 1800;

        public static int Difficulty = 1;

        public static int regenerationAmt = 5;
        public static int maxHealth = 100;

        static int chkTmr = 120;

        public static bool keyboardEnabled;

        public static void Update(GameTime gameTime)
        {
            //checking for dead players
            int dead = 0;
            foreach (PlayerShip pl in PlayerManager.players)
            {
                if (pl.IsDead == true) { dead += 1; }
            }

            if (chkTmr != 0) chkTmr--;

            //checking for game over
            if (InGame == true && (dead == PlayerManager.players.Count) && chkTmr == 0)
            {
                InGame = false;
                ScreenManager.AddScreen(new highScore(PlayerIndex.One));

            }
            //cheat code
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.U) && ks.IsKeyDown(Keys.I) && ks.IsKeyDown(Keys.O) && ks.IsKeyDown(Keys.P))
            {
                Points += 10;

            }


        }

        public static void NEWGAME()
        {
            //reseting all values necessary for a new game

            GM.InGame = true;
            ScreenManager.AddScreen(new GameGUI());

            WaveManager.Spawning = true;

            foreach (PlayerShip pl in PlayerManager.players)
            {
                pl.Initialize();
                pl.Respawn();
            }
            EnemyManager.CLEARALL();
            Bullets.CLEARALL();
            ItemManager.CLEARALL();
            ParticleManager.CLEARALL();
            Points = 0;
            WaveManager.waveNum = 1;
            WaveManager.Spawning = false;
            WaveManager.gotInfo = false;
            WaveManager.sentMessage = false;

            chkTmr = 120;
            regenerationAmt = 5;
            maxHealth = 100;
            nukeRefund = 0;
            firewallTime = 1800;
            freezeTime = 1800;
            towerTime = 1800;
            maxPoints = 0;

        }
    }

    public static class InputManager
    {
        static InputMethod p1Input, p2Input, p3Input, p4Input;

        public static void Update(GameTime gameTime)
        {
            //update players inputs
            if (p1Input != null) p1Input.Update(gameTime);
            if (p2Input != null) p2Input.Update(gameTime);
            if (p3Input != null) p3Input.Update(gameTime);
            if (p4Input != null) p4Input.Update(gameTime);
        }

        public static InputMethod PlayerInput(PlayerIndex player)
        {
            switch (player)
            {
                case PlayerIndex.One:
                    return p1Input;

                case PlayerIndex.Two:
                    return p2Input;

                case PlayerIndex.Three:
                    return p3Input;

                case PlayerIndex.Four:
                    return p4Input;

            }

            return null;

        }

        /// <summary>
        /// get a player from a number
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static InputMethod PlayerInput(int player)
        {
            switch (player)
            {
                case 1:
                    return p1Input;

                case 2:
                    return p2Input;

                case 3:
                    return p3Input;

                case 4:
                    return p4Input;

            }

            return null;

        }

        #region Add Player Inputs

        public static void addPlayer1()
        {
            if (GM.keyboardEnabled == false)
            {
                Console.WriteLine("ControllerInput");
                p1Input = new ControllerInput();
            }
            else
            {
                Console.WriteLine("Keyboard");
                p1Input = new KeyboardInput();
            }
            p1Input.SetPlayer = PlayerIndex.One;

        }
        public static void addPlayer2()
        {
            p2Input = new ControllerInput();
            p2Input.SetPlayer = PlayerIndex.Two;

        }
        public static void addPlayer3()
        {
            p3Input = new ControllerInput();
            p3Input.SetPlayer = PlayerIndex.Three;

        }
        public static void addPlayer4()
        {
            p4Input = new ControllerInput();
            p4Input.SetPlayer = PlayerIndex.Four;

        }
        #endregion

    }

    public static class PlayerManager
    {

        public static List<PlayerShip> players = new List<PlayerShip>();
        static PlayerShip player1;
        static PlayerShip player2;
        static PlayerShip player3;
        static PlayerShip player4;

        public static void Update(GameTime gameTime)
        {
            //players.Sort();
            CheckPlaying();
            foreach (PlayerShip player in players) { player.Update(gameTime); }



        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (GM.InGame == true)
                foreach (PlayerShip player in players) { player.Draw(spriteBatch); }


        }

        public static void RespawnPlayers()
        {
            foreach (PlayerShip player in players)
            {
                player.Respawn();
            }

        }

        static void CheckPlaying()
        {

            #region Player1
            //game pad is a controller, playerindex is the number of the controller
            if (GamePad.GetState(PlayerIndex.One).IsConnected || GM.keyboardEnabled)
            {
                if (player1 == null)
                {

                    Console.WriteLine("new player 1");
                    player1 = new PlayerShip(PlayerIndex.One);
                    player1.IsDead = false;
                }
                //add player to game
                if (!players.Contains(player1))
                {

                    players.Add(player1);
                    Console.WriteLine("p1 added");

                }
            }
            else
            {
                //remove from game
                players.Remove(player1);
            }
            #endregion

            #region Player2
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                if (player2 == null)
                {
                    Console.WriteLine("new player 2");
                    player2 = new PlayerShip(PlayerIndex.Two);

                    InputManager.addPlayer2();
                    player2.IsDead = false;
                }
                //add player to game
                if (!players.Contains(player2))
                {
                    players.Add(player2);
                    Console.WriteLine("p2 added");

                }
            }
            else
            {
                //remove from game
                players.Remove(player2);
            }
            #endregion

            #region Player3
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
            {
                if (player3 == null)
                {
                    Console.WriteLine("new player 3");
                    player3 = new PlayerShip(PlayerIndex.Three);

                    InputManager.addPlayer3();
                    player3.IsDead = false;
                }
                //add player to game
                if (!players.Contains(player3))
                {
                    players.Add(player3);
                    Console.WriteLine("p3 added");
                }
            }
            else
            {
                //remove from game
                players.Remove(player3);
            }
            #endregion

            #region Player4
            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
            {
                if (player4 == null)
                {
                    Console.WriteLine("new player 4");
                    player4 = new PlayerShip(PlayerIndex.Four);


                    InputManager.addPlayer4();
                    player4.IsDead = false;
                }
                //add player to game
                if (!players.Contains(player4))
                {
                    players.Add(player4);
                    Console.WriteLine("p4 added");
                }
            }
            else
            {
                //remove from game
                players.Remove(player4);
            }
            #endregion



        }

        public static PlayerShip player(PlayerIndex pl)
        {
            switch (pl)
            {
                case PlayerIndex.One:
                    return player1;
                case PlayerIndex.Two:
                    return player2;
                case PlayerIndex.Three:
                    return player3;
                case PlayerIndex.Four:
                    return player4;
                default:
                    return null;
            }



        }

        /// <summary>
        /// returns player index from 1 to 4
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        public static PlayerShip player(int pl)
        {
            switch (pl)
            {
                case 1:
                    return player1;
                case 2:
                    return player2;
                case 3:
                    return player3;
                case 4:
                    return player4;
                default:
                    return null;
            }


        }
    }

    public static class CollisionManager
    {
        static Random rand = new Random();

        //detects colissions between player and bullet
        static bool DetectCollision(PlayerShip p, baseBullet b)
        {
            float x1 = p.Location.X;
            float y1 = p.Location.Y;

            float x2 = b.Location.X;
            float y2 = b.Location.Y;

            float r1 = p.Radius;
            float r2 = b.Radius;


            if ((float)Math.Pow((x2 - x1), 2) + (float)Math.Pow((y1 - y2), 2) <= Math.Pow((r1 + r2), 2))
            {

                return true;
            }
            else
            {

                return false;
            }


        }
        //detects colissions between enemy and player
        static bool DetectCollision(Enemy e1, PlayerShip e2)
        {
            float x1 = e1.Location.X;
            float y1 = e1.Location.Y;

            float x2 = e2.Location.X;
            float y2 = e2.Location.Y;

            float r1 = e1.Radius;
            float r2 = e2.Radius;


            if ((float)Math.Pow((x2 - x1), 2) + (float)Math.Pow((y1 - y2), 2) <= Math.Pow((r1 + r2), 2))
            {

                return true;
            }
            else
            {

                return false;
            }


        }
        //detects colissions between enemy and bullet
        static bool DetectCollision(Enemy e1, baseBullet b2)
        {
            float x1 = e1.Location.X;
            float y1 = e1.Location.Y;

            float x2 = b2.Location.X;
            float y2 = b2.Location.Y;

            float r1 = e1.Radius;
            float r2 = b2.Radius;


            if ((float)Math.Pow((x2 - x1), 2) + (float)Math.Pow((y1 - y2), 2) <= Math.Pow((r1 + r2), 2))
            {

                return true;
            }
            else
            {

                return false;
            }


        }


        //detects colissions between items and enemies
        static bool DetectCollision(Items i1, Enemy e1)
        {
            float x1 = i1.Location.X;
            float y1 = i1.Location.Y;

            float x2 = e1.Location.X;
            float y2 = e1.Location.Y;

            float r1 = i1.Radius;
            float r2 = e1.Radius;


            if ((float)Math.Pow((x2 - x1), 2) + (float)Math.Pow((y1 - y2), 2) <= Math.Pow((r1 + r2), 2))
            {
                // Console.WriteLine(((float)Math.Pow((x2 - x1), 2) + (float)Math.Pow((y1 - y2), 2)).ToString() +"  "+  (r1 + r2).ToString());
                return true;
            }
            else
            {

                return false;
            }


        }

        public static void Update(GameTime gameTime)
        {

            //checking for the collisions

            #region player and enemy
            foreach (Enemy e in EnemyManager.enemies)
            {
                foreach (PlayerShip pl in PlayerManager.players)
                {
                    if (DetectCollision(e, pl))
                    {
                        e.MeeleeAttack(pl);
                        break;
                    }
                }
            }
            #endregion

            #region enemy and bullet
            List<baseBullet> toDie = new List<baseBullet>();

            if (!EnemyManager.isUpdating)
                foreach (Enemy e in EnemyManager.enemies)
                {
                    if (!Bullets.updating)
                        foreach (baseBullet b in Bullets.bullets)
                        {

                            if (DetectCollision(e, b) && b.IsPlayer)
                            {

                                bool needsDestroy = false;


                                if (b.IsFire && !needsDestroy)
                                {
                                    e.TakeDamage(b.Damage);
                                    e.TakeDamage(50, 180);
                                    if (!b.Penetrates)
                                        needsDestroy = true;
                                }

                                if (b.IsAOE && !needsDestroy)
                                {
                                    b.AOE(b.Damage);
                                    if (!b.Penetrates)
                                        needsDestroy = true;
                                }

                                if (!needsDestroy && b.IsPlayer)
                                {
                                    e.TakeDamage(b.Damage);
                                    if (!b.Penetrates)
                                        needsDestroy = true;
                                }

                                if (needsDestroy)
                                    toDie.Add(b);

                                break;

                            }
                        }
                }

            foreach (baseBullet b in toDie)
            {
                b.Die();
            }

            toDie.Clear();
            #endregion

            #region Enemy and Enemy


            //if (!EnemyManager.isUpdating)
            //    foreach (Enemy e in EnemyManager.enemies)
            //    {
            //        if (!EnemyManager.isUpdating)
            //            foreach (Enemy e2 in EnemyManager.enemies)
            //            {
            //                if (e != e2)
            //                {
            //                    if (DetectCollision(e, e2))
            //                    {
            //                        //   e.Location += new Vector2(rand.Next((int)b.Radius + 1), rand.Next((int)e.Radius + 1));
            //                        Vector2 offset = new Vector2((float)Math.Cos(e2.Rotation), (float)Math.Sin(e2.Rotation));
            //                        offset.Normalize();
            //                        Vector2.Lerp(e.Location, offset * 1000, 1f);


            //                        //e.Location +=offset* Vector2.Distance(e.Location, e2.Location);

            //                    }
            //                }
            //            }
            //    }
            #endregion

            #region player and bullet

            if (!EnemyManager.isUpdating)
                foreach (PlayerShip p in PlayerManager.players)
                {
                    if (!Bullets.updating)
                        foreach (baseBullet b in Bullets.bullets)
                        {
                            if (DetectCollision(p, b) && !b.IsPlayer)
                            {
                                if (b.IsFire)
                                {
                                    p.TakeDamage(b.Damage);
                                }
                                else
                                    p.TakeDamage(b.Damage);

                                toDie.Add(b);

                                break;

                            }
                        }
                }

            foreach (baseBullet b in toDie)
            {
                b.Die();
            }

            toDie.Clear();
            #endregion

            #region Item and Enemy


            if (ItemManager.updating == false)
                foreach (Items i in ItemManager.powerups)
                {

                    foreach (Enemy e in EnemyManager.enemies)
                    {
                        if (DetectCollision(i, e))
                        {

                            if (i.type == "firewall")
                            {
                                e.TakeDamage(30, 180);

                            }

                            if (i.type == "freeze")
                            {
                                e.speed = 3;

                            }
                        }
                    }
                }
            #endregion




        }
    }

    public static class WaveManager
    {
        public static int maxWaves;
        public static Wave[] Waves;
        public static int waveNum = 10;
        public static bool Spawning = false;


        static int countdown;

        static Wave currWave;
        static int numOfSpawns;
        static int currSpawn;

        public static bool gotInfo = false;
        public static bool sentMessage = false;
        public static void Update(GameTime gameTime)
        {
            countdown--;

            if (countdown <= 0) countdown = 0;

            if (gotInfo == false)
            {
                GetSpawnInfo(waveNum);
                gotInfo = true;

            }

            // while wave is Not over
            if (currSpawn == numOfSpawns)
            {
                Spawning = false;
                gotInfo = false;
                sentMessage = false;
                waveNum++;
            }

            if (Spawning == true)
            {
                if (currSpawn == 0 && sentMessage == false && countdown <= 0)
                {
                    ScreenManager.AddOverlay(new waveAlret(waveNum));
                    sentMessage = true;

                    GM.Difficulty += (int)(waveNum * .1f);

                    if (waveNum % 10 == 0) EnemyManager.AddEnemy(new CloudBoss(GM.basePos));

                    //handle respawn
                    foreach (PlayerShip pl in PlayerManager.players)
                    {
                        if (pl.IsDead == true && PlayerManager.players.Contains(pl))
                        {
                            pl.Respawn();
                        }

                    }
                }
                //get spawn num

                //wait countdown 
                if (countdown > 0) return;
                else
                {

                    //spawn things
                    Spawn(currWave.Spawns[currSpawn].a);
                    Spawn(currWave.Spawns[currSpawn].b);
                    Spawn(currWave.Spawns[currSpawn].c);
                    Spawn(currWave.Spawns[currSpawn].d);
                    countdown = currWave.Spawns[currSpawn].time;
                }
                //move to next spawn
                currSpawn += 1;

            }
        }

        //converts a number into the enemy it is associated with
        private static void Spawn(int id)
        {


            switch (id)
            {
                case 1:
                    EnemyManager.AddEnemy(new Triangle(GetPos()));
                    break;
                case 2:
                    EnemyManager.AddEnemy(new Splitter(GetPos()));
                    break;
                case 3:
                    EnemyManager.AddEnemy(new Splitter2(GetPos()));
                    break;
                case 4:
                    EnemyManager.AddEnemy(new Shooter(GetPos()));
                    break;

            }
        }

        static int posCtr = 0;
        //depending on the order loaded the enemies are put into the approiate corrners
        private static Vector2 GetPos()
        {

            Vector2 topLeft = new Vector2(100, 100),
                   topRight = new Vector2(GM.ScreenDims.X - 100, 100),
                   btmLeft = new Vector2(100, GM.ScreenDims.Y - 100),
                   btmRight = new Vector2(GM.ScreenDims.X - 100, GM.ScreenDims.Y - 100);
            switch (posCtr)
            {
                case 0:
                    posCtr++;
                    return topLeft;
                case 1:
                    posCtr++;
                    return topRight;
                case 2:
                    posCtr++;
                    return btmLeft;
                case 3:
                    posCtr++;
                    posCtr = 0;
                    return btmRight;
                default:
                    return GM.ScreenDims / 2;
            }


        }

        //reads the spawn info from the wave
        public static void GetSpawnInfo(int num)
        {
            if (num > maxWaves)
            {
                Console.WriteLine("max waves exceeded");
                currWave = Waves[maxWaves];
                numOfSpawns = currWave.Spawns.Length;
                currSpawn = 0;
                countdown = currWave.waitTime;
                Spawning = true;
            }
            else
            {

                currWave = Waves[num - 1];
                numOfSpawns = currWave.Spawns.Length;
                currSpawn = 0;
                countdown = currWave.waitTime;
                Spawning = true;
            }
        }

    }

    public static class EnemyManager
    {
        #region Vars
        public static List<Enemy> enemies = new List<Enemy>();
        static List<Enemy> addEnemies = new List<Enemy>();
        static List<Enemy> delEnemies = new List<Enemy>();
        public static bool isUpdating = false;

        #endregion

        public static void Initalize()
        {
            foreach (Enemy enemy in enemies) { enemy.Initialize(); }
        }


        public static void Update(GameTime gameTime)
        {

            isUpdating = true;
            foreach (Enemy enemy in enemies) { enemy.Update(gameTime); }
            isUpdating = false;

            foreach (Enemy e in addEnemies) { enemies.Add(e); }

            foreach (Enemy e in delEnemies) { enemies.Remove(e); }

            addEnemies.Clear();
            delEnemies.Clear();
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemies) { enemy.Draw(spriteBatch); }
        }

        //these is used if the main list of enemies is currently updating to avoid an iteration exception
        //this is the same methodology used for all the lists of things
        public static void AddEnemy(Enemy e)
        {
            if (!isUpdating)
                enemies.Add(e);
            else
                addEnemies.Add(e);
        }

        public static void DelEnemy(Enemy e)
        {
            if (!isUpdating)
                enemies.Remove(e);
            else
                delEnemies.Add(e);

        }

        //clears everything
        public static void CLEARALL()
        {
            enemies.Clear();
            addEnemies.Clear();
            delEnemies.Clear();
        }
    }

    public static class ItemManager
    {
        public static bool updating = false;
        public static List<Items> powerups = new List<Items>();
        static List<Items> addPowerups = new List<Items>();
        static List<Items> delPowerups = new List<Items>();

        public static void Update(GameTime gameTime)
        {

            updating = true;
            foreach (Items p in powerups)
            {
                p.Update(gameTime);
            }
            updating = false;

            foreach (Items i in addPowerups)
            {
                powerups.Add(i);
            }

            foreach (Items i in delPowerups)
            {
                powerups.Remove(i);
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Items p in powerups)
            {
                p.Draw(spriteBatch);
            }
        }
        public static void addItem(Items i)
        {
            if (updating)
                addPowerups.Add(i);
            else
                powerups.Add(i);

        }
        public static void delItem(Items i)
        {
            if (updating)
                delPowerups.Add(i);
            else
                powerups.Remove(i);
        }
        public static void CLEARALL()
        {
            powerups.Clear();
            addPowerups.Clear();
            delPowerups.Clear();

        }

    }

    public static class Bullets
    {
        #region Vars

        public static List<baseBullet> bullets = new List<baseBullet>(500);
        static List<baseBullet> addedBullets = new List<baseBullet>();
        static List<baseBullet> removeBullets = new List<baseBullet>();
        public static bool updating;

        #endregion

        public static void Add(baseBullet bullet)
        {
            if (!updating)
                bullets.Add(bullet);
            else
                addedBullets.Add(bullet);

        }

        public static void Remove(baseBullet bullet)
        {
            if (!updating)
                bullets.Remove(bullet);
            else
                removeBullets.Add(bullet);
        }

        public static void Initialize()
        {

        }


        public static void Update(GameTime gameTime)
        {
            updating = true;

            foreach (baseBullet bullet in bullets)
                bullet.Update(gameTime);



            foreach (baseBullet entity in addedBullets)
                bullets.Add(entity);

            foreach (baseBullet entity in removeBullets)
                bullets.Remove(entity);
            updating = false;

            addedBullets.Clear();
            removeBullets.Clear();
        }

        public static void Draw(SpriteBatch sprtieBatch)
        {
            foreach (baseBullet bullet in bullets)
                bullet.Draw(sprtieBatch);


        }

        public static void CLEARALL()
        {
            addedBullets.Clear();
            removeBullets.Clear();
            bullets.Clear();
        }
    }

    public static class Art
    {
        //all the assets used for the game
        public static SpriteFont Font;

        public static Texture2D Player;
        public static Texture2D Bullet;
        public static Texture2D BouncyBullet;
        public static Texture2D Missile;
        public static Texture2D Missile2;
        public static Texture2D Laser;
        public static Texture2D Laser2;
        public static Texture2D MiniMissile;
        public static Texture2D Triangle;
        public static Texture2D Square;
        public static Texture2D Splitter;
        public static Texture2D[] MenuItems = new Texture2D[9];
        public static Texture2D HealthBar;
        public static Texture2D Freeze;
        public static Texture2D Firewall;
        public static Texture2D Line;
        public static Texture2D Cursor;
        public static Texture2D Tower;
        public static Texture2D CloudBoss;
        public static Texture2D Lightning;

        public static SoundEffect Cannon;
        public static SoundEffect Click;
        public static SoundEffect Pew;
        public static SoundEffect Error;
        public static SoundEffect Thunder;

        //this loads them into memory
        public static void LoadContent(ContentManager Content)
        {
            Font = Content.Load<SpriteFont>("DefaultFont");

            //wave list
            WaveManager.Waves = Content.Load<Wave[]>("Waves");
            WaveManager.maxWaves = WaveManager.Waves.Length;
            //sprites
            Player = Content.Load<Texture2D>("player");
            Cursor = Content.Load<Texture2D>("cursor2");
            Lightning = Content.Load<Texture2D>("lightningBolt");

            //bullets
            Bullet = Content.Load<Texture2D>("Bullets/baseBullet");
            BouncyBullet = Content.Load<Texture2D>("Bullets/BouncyBullet");
            Missile = Content.Load<Texture2D>("Bullets/missile");
            Missile2 = Content.Load<Texture2D>("Bullets/missile2");
            Laser = Content.Load<Texture2D>("Bullets/laser");
            Laser2 = Content.Load<Texture2D>("Bullets/laser1");
            MiniMissile = Content.Load<Texture2D>("Bullets/miniRocket");

            Freeze = Content.Load<Texture2D>("freeze");
            Firewall = Content.Load<Texture2D>("firewall");
            Tower = Content.Load<Texture2D>("tower");
            CloudBoss = Content.Load<Texture2D>("cloud");

            Line = Content.Load<Texture2D>("line");

            for (int i = 0; i <= 8; i++)
            {
                MenuItems[i] = Content.Load<Texture2D>("Menu/menu" + i.ToString());

            }

            //menu items
            //MenuItems[0] = Content.Load<Texture2D>("menu0");
            //MenuItems[1] = Content.Load<Texture2D>("menu0");
            //MenuItems[2] = Content.Load<Texture2D>("menu0");
            //MenuItems[3] = Content.Load<Texture2D>("menu0");
            //MenuItems[4] = Content.Load<Texture2D>("menu0");
            //MenuItems[5] = Content.Load<Texture2D>("menu0");
            //MenuItems[6] = Content.Load<Texture2D>("menu0");
            //MenuItems[7] = Content.Load<Texture2D>("menu0");
            //MenuItems[8] = Content.Load<Texture2D>("menu0");

            HealthBar = Content.Load<Texture2D>("healthbar");

            //enemies
            Triangle = Content.Load<Texture2D>("Enemies/triangle");
            Square = Content.Load<Texture2D>("Enemies/square");
            Splitter = Content.Load<Texture2D>("Enemies/splitter");


            //sound effects
            Cannon = Content.Load<SoundEffect>("Sound/cannon");
            Click = Content.Load<SoundEffect>("Sound/click");
            Pew = Content.Load<SoundEffect>("Sound/pew");
            Error = Content.Load<SoundEffect>("Sound/Windows_Error");
            Thunder = Content.Load<SoundEffect>("Sound/thunder_strike");

        }



    }

    public class Items
    {
        //these are the large circles that set enemies on fire of slow them down

        public Vector2 Location { get; protected set; }
        public int Radius { get; protected set; }
        public string type { get; protected set; }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }

    public class PlayerShip
    {

        #region Vars
        Random rand = new Random();
        Vector2 location;
        public float Rotation { get; set; }
        float radius;
        public Color Color { get; protected set; }
        protected PlayerIndex playerIndex;
        protected Texture2D sprite;

        bool isDead = true;


        public float Health { get; protected set; }
        protected int speed,
            baseSpeed = 10,
            sprintSpeed = 40,
            baseSprintSpeed = 40;


        protected int bulletSprayCooldown = 0, sprintCooldown = 0;

        public int fireRate = 5;
        int shootTmr = 0;
        int secTmr = 60;
        public enum Ammo { B1, B2, B3, B4, B5, L1, L2, L3, L4, L5, M1, M2, M3, M4, M5 }
        public Ammo ammo;
        public int BulletTier { get; set; }
        public int LaserTier { get; set; }
        public int MissileTier { get; set; }



        #region Properties

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        public float Radius
        {
            get { return radius; }
            private set { radius = value; }
        }


        public Vector2 Location
        {
            get { return location; }
            private set { location = value; }
        }


        #endregion


        public PlayerShip(PlayerIndex pl)
        {
            playerIndex = pl;


            Initialize();

        }
        #endregion


        public void Initialize()
        {
            sprite = Art.Player;
            speed = baseSpeed;
            location = new Vector2(GM.ScreenDims.X / 2, GM.ScreenDims.Y / 2);

            radius = sprite.Width * 0.5f;
            Health = GM.maxHealth;
            baseSpeed = 10;
            sprintSpeed = 40;
            baseSprintSpeed = 40;
            fireRate = 5;

            //set the player color
            #region pick color
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    Color = Color.Red;
                    break;
                case PlayerIndex.Two:
                    Color = Color.Green;
                    break;

                case PlayerIndex.Three:
                    Color = Color.Blue;
                    break;
                case PlayerIndex.Four:
                    Color = Color.Yellow;
                    break;
            }
            #endregion

            BulletTier = 1;
            LaserTier = 1;
            MissileTier = 1;
            ammo = Ammo.B1;
        }

        public void Update(GameTime gameTime)
        {
            if (secTmr == 0) { secTmr = 60; }
            secTmr--;

            if (!isDead)
            {
                RegenAmt();
                UpdateHealth();
                ControlPlayer();
                ShootControls();
                BulletSpray();
                Sprint();
                CheckBuyMenu();
                DoPowerups();



                //keep player in view
                location = Vector2.Clamp(location, Vector2.Zero + new Vector2(sprite.Height / 2, sprite.Width / 2), GM.ScreenDims - new Vector2(sprite.Height / 2, sprite.Width / 2));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {


            if (!isDead)
            {
                Rectangle rect = new Rectangle(Convert.ToInt16(location.X), Convert.ToInt16(location.Y), sprite.Width, sprite.Height);
                spriteBatch.Draw(sprite, rect, null, Color, Rotation, new Vector2(sprite.Width / 2, sprite.Height / 2), SpriteEffects.None, 0.0f);

            }
        }

        #region Methods

        private void DoPowerups()
        {
            //nuke
            if (InputManager.PlayerInput(playerIndex).BtnDpadDown)
            {
                if (GM.Points >= 400)
                {
                    GM.Points = 0;
                    EnemyManager.CLEARALL();
                    GM.Points += GM.nukeRefund;
                }
            }
            //freeze
            if (InputManager.PlayerInput(playerIndex).BtnDpadLeft)
            {
                if (GM.Points >= 25)
                {
                    new Freeze(Location);
                    GM.Points -= 25;
                }
            }
            //firewall
            if (InputManager.PlayerInput(playerIndex).BtnDpadRight)
            {
                if (GM.Points >= 50)
                {
                    new Firewall(Location);
                    GM.Points -= 50;
                }
            }

            //tower
            if (InputManager.PlayerInput(playerIndex).BtnDpadUp)
            {
                if (GM.Points >= 75)
                {
                    new Tower(Location);
                    GM.Points -= 75;
                }
            }
        }

        public PlayerIndex SetIndex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }

        private void RegenAmt()
        {
            if (secTmr == 0)
            {
                Health += GM.regenerationAmt;

            }
        }

        private void Sprint()
        {

            //sprint
            if (InputManager.PlayerInput(playerIndex).BtnLeftStick)
            {
                if (sprintCooldown <= 0)
                {
                    sprintSpeed = baseSprintSpeed;
                    sprintCooldown = 500;
                }
            }

            sprintCooldown--;
            if (sprintCooldown < 0) { sprintCooldown = 0; }

            if (sprintSpeed != baseSpeed)
            {
                sprintSpeed--;
                speed = sprintSpeed;
            }

        }

        private void CheckBuyMenu()
        {
            //buymenu
            if (GM.InGame == true)
                if (InputManager.PlayerInput(playerIndex).BtnX)
                {
                    ScreenManager.AddScreen(new buyScreen(playerIndex));
                }
        }

        private void BulletSpray()
        {
            //bullet spray
            if (InputManager.PlayerInput(playerIndex).BtnRightStick)
            {
                if (bulletSprayCooldown == 0)
                {
                    for (int i = 1; i <= 360; i += 15)
                    {


                        switch (ammo)
                        {
                            case Ammo.B1:
                                Bullets.Add(new bullet_T1(location, i));
                                break;
                            case Ammo.B2:
                                Bullets.Add(new bullet_T2(location, i));
                                break;
                            case Ammo.B3:
                                Bullets.Add(new bullet_T3(location, i));
                                break;
                            case Ammo.B4:
                                Bullets.Add(new bullet_T4(location, i));
                                break;
                            case Ammo.B5:
                                Bullets.Add(new bullet_T5(location, i));
                                break;
                            case Ammo.L1:
                                Bullets.Add(new laser_T1(location, i));
                                break;
                            case Ammo.L2:
                                Bullets.Add(new laser_T2(location, i));
                                break;
                            case Ammo.L3:
                                Bullets.Add(new laser_T3(location, i));
                                break;
                            case Ammo.L4:
                                Bullets.Add(new laser_T4(location, i));
                                break;
                            case Ammo.L5:
                                Bullets.Add(new laser_T5(location, i));
                                break;
                            case Ammo.M1:
                                Bullets.Add(new missile_T1(location, i));
                                break;
                            case Ammo.M2:
                                Bullets.Add(new missile_T2(location, i));
                                break;
                            case Ammo.M3:
                                Bullets.Add(new missile_T3(location, i));
                                break;
                            case Ammo.M4:
                                Bullets.Add(new missile_T4(location, i));
                                break;
                            case Ammo.M5:
                                Bullets.Add(new missile_T5(location, i));
                                break;

                        }

                        Art.Cannon.Play(0.02f, 0.0f, 0.0f);
                    }
                }

                bulletSprayCooldown = 600;
            }
            if (bulletSprayCooldown != 0) { bulletSprayCooldown--; }
        }

        private void ShootControls()
        {
            //shoot controls
            if (InputManager.PlayerInput(playerIndex).RightTrigger > 0f || (GM.keyboardEnabled && (Mouse.GetState().LeftButton == ButtonState.Pressed) && playerIndex == PlayerIndex.One))
            {

                shootTmr--;

                if (shootTmr <= 0)
                {
                    //shoot here
                    //Bullets.Add(new bullet_T1(location, RotationAngle));
                    //baseBullet b = new bullet_T5(Location, Rotation);
                    //b.IsPlayer = false;

                    //Bullets.Add(b);
                    switch (ammo)
                    {
                        case Ammo.B1:
                            Bullets.Add(new bullet_T1(Location, Rotation));
                            break;
                        case Ammo.B2:
                            Bullets.Add(new bullet_T2(Location, Rotation));
                            break;
                        case Ammo.B3:
                            Bullets.Add(new bullet_T3(Location, Rotation));
                            break;
                        case Ammo.B4:
                            Bullets.Add(new bullet_T4(Location, Rotation));
                            break;
                        case Ammo.B5:
                            Bullets.Add(new bullet_T5(Location, Rotation));
                            break;
                        case Ammo.L1:
                            Bullets.Add(new laser_T1(Location, Rotation));
                            break;
                        case Ammo.L2:
                            Bullets.Add(new laser_T2(Location, Rotation));
                            break;
                        case Ammo.L3:
                            Bullets.Add(new laser_T3(Location, Rotation));
                            break;
                        case Ammo.L4:
                            Bullets.Add(new laser_T4(Location, Rotation));
                            break;
                        case Ammo.L5:
                            Bullets.Add(new laser_T5(Location, Rotation));
                            break;
                        case Ammo.M1:
                            Bullets.Add(new missile_T1(Location, Rotation));
                            break;
                        case Ammo.M2:
                            Bullets.Add(new missile_T2(Location, Rotation));
                            break;
                        case Ammo.M3:
                            Bullets.Add(new missile_T3(Location, Rotation));
                            break;
                        case Ammo.M4:
                            Bullets.Add(new missile_T4(Location, Rotation));
                            break;
                        case Ammo.M5:
                            Bullets.Add(new missile_T5(Location, Rotation));
                            break;

                    }


                    //got annoying     Art.Pew.Play(0.1f, 0.0f, 0.0f);

                    shootTmr = fireRate;
                }


            }
        }

        private void ControlPlayer()
        {
            if (!GM.keyboardEnabled)
            {
                //movement
                location.X += InputManager.PlayerInput(playerIndex).ThumbStickLeftX * speed;
                location.Y += -InputManager.PlayerInput(playerIndex).ThumbStickLeftY * speed;

                //rotation manager
                if (InputManager.PlayerInput(playerIndex).ThumbStickRightX != 0 || InputManager.PlayerInput(playerIndex).ThumbStickRightY != 0)
                {
                    Rotation = InputManager.PlayerInput(playerIndex).ThumbStickRightDeg;

                }
                else
                {
                    Rotation = InputManager.PlayerInput(playerIndex).ThumbStickLeftDeg;
                }
            }
            else
            {
                if (InputManager.PlayerInput(playerIndex).isDown(Keys.W)) { location.Y -= speed; }
                if (InputManager.PlayerInput(playerIndex).isDown(Keys.S)) { location.Y += speed; }
                if (InputManager.PlayerInput(playerIndex).isDown(Keys.A)) { location.X -= speed; }
                if (InputManager.PlayerInput(playerIndex).isDown(Keys.D)) { location.X += speed; }

                //rotation manager
                Point point = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                Rotation = (float)Math.Atan2(((double)point.Y - location.Y), ((double)point.X - location.X)) + (float)Math.PI / 2;



            }

        }

        public void Die()
        {
            isDead = true;
        }

        public void TakeDamage(float dmg)
        {
            Health -= dmg;
        }

        public void Respawn()
        {
            isDead = false;
            Health = GM.maxHealth;
            Location = GM.ScreenDims / 2;
        }

        private void UpdateHealth()
        {
            if (Health > GM.maxHealth)
            {
                Health = GM.maxHealth;

            }


            if (Health <= 0) Die();


        }

        #endregion

    }

    public class InputMethod
    {
        //all the vars that the controller can have so that they can be accessed easily

        protected PlayerIndex player;

        protected bool btnA, btnB, btnBack, btnBigButton, btnDPadDown, btnDPadLeft, btnDPadRight, btnDPadUp, btnLeftShoulder, btnLeftStick, btnLeftThumbDown, btnLeftThumbLeft, btnLeftThumbRight, btnLeftThumbUp, btnLeftTrigger, btnRightShoulder, btnRightStick, btnRightThumbDown, btnRightThumbLeft, btnRightThumbRight, btnRightThumbUp, btnRightTrigger, btnStart, btnX, btnY;
        protected float thumbStickLeftX, thumbStickLeftY, thumbStickRightX, thumbStickRightY, thumbStickLeftDeg, thumbStickRightDeg;

        #region Properties

        public PlayerIndex SetPlayer
        {
            get { return player; }
            set { player = value; }
        }

        #region Triggers
        public float LeftTrigger { get; set; }
        public float RightTrigger { get; set; }
        #endregion

        #region Thumb stick value properties
        public float ThumbStickLeftX { get { return thumbStickLeftX; } }
        public float ThumbStickLeftY { get { return thumbStickLeftY; } }
        public float ThumbStickRightX { get { return thumbStickRightX; } }
        public float ThumbStickRightY { get { return thumbStickRightY; } }
        public float ThumbStickLeftDeg { get { return thumbStickLeftDeg; } }
        public float ThumbStickRightDeg { get { return thumbStickRightDeg; } }
        #endregion

        #region button properties
        public bool BtnA { get { return btnA; } }
        public bool BtnB { get { return btnB; } }
        public bool BtnX { get { return btnX; } }
        public bool BtnY { get { return btnY; } }
        public bool BtnBack { get { return btnBack; } }
        public bool BtnStart { get { return btnStart; } }
        public bool BtnBigButton { get { return btnBigButton; } }

        //dpad
        public bool BtnDpadUp { get { return btnDPadUp; } }
        public bool BtnDpadDown { get { return btnDPadDown; } }
        public bool BtnDpadLeft { get { return btnDPadLeft; } }
        public bool BtnDpadRight { get { return btnDPadRight; } }

        //left stick
        public bool BtnLeftThumbUp { get { return btnLeftThumbUp; } }
        public bool BtnLeftThumbDown { get { return btnLeftThumbDown; } }
        public bool BtnLeftThumbLeft { get { return btnLeftThumbLeft; } }
        public bool BtnLeftThumbRight { get { return btnLeftThumbRight; } }
        public bool BtnLeftShoulder { get { return btnLeftShoulder; } }
        public bool BtnLeftStick { get { return btnLeftStick; } }
        public bool BtnLeftTrigger { get { return btnLeftTrigger; } }

        //Right stick
        public bool BtnRightThumbUp { get { return btnRightThumbUp; } }
        public bool BtnRightThumbDown { get { return btnRightThumbDown; } }
        public bool BtnRightThumbLeft { get { return btnRightThumbLeft; } }
        public bool BtnRightThumbRight { get { return btnRightThumbRight; } }
        public bool BtnRightShoulder { get { return btnRightShoulder; } }
        public bool BtnRightStick { get { return btnRightStick; } }
        public bool BtnRightTrigger { get { return btnRightTrigger; } }

        #endregion

        #endregion

        public virtual void Update(GameTime gameTime) { }

        public virtual bool isDown(Buttons b)
        {
            return false;
        }

        public virtual bool isUp(Buttons b)
        {
            return false;
        }

        public virtual bool isDown(Keys k)
        {
            return false;
        }

        public virtual bool isUp(Keys k)
        {
            return false;
        }
    }

    public class ControllerInput : InputMethod
    {
        GamePadState pad;
        GamePadState oldPad;

        //polls the controller for its values and sets them in the main class
        public override void Update(GameTime gameTime)
        {
            pad = GamePad.GetState(player);

            #region Button Presses
            ///for anyone reading this, this is the worst way to do this but the only way i could find after 7 hours

            //other
            if (pad.IsButtonDown(Buttons.A) && oldPad.IsButtonUp(Buttons.A)) { btnA = true; } else { btnA = false; }

            if (pad.IsButtonDown(Buttons.B) && !oldPad.IsButtonDown(Buttons.B)) { btnB = true; } else { btnB = false; }

            if (pad.IsButtonDown(Buttons.X) && !oldPad.IsButtonDown(Buttons.X)) { btnX = true; } else { btnX = false; }

            if (pad.IsButtonDown(Buttons.Y) && !oldPad.IsButtonDown(Buttons.Y)) { btnY = true; } else { btnY = false; }

            if (pad.IsButtonDown(Buttons.Back) && !oldPad.IsButtonDown(Buttons.Back)) { btnBack = true; } else { btnBack = false; }

            if (pad.IsButtonDown(Buttons.Start) && !oldPad.IsButtonDown(Buttons.Start)) { btnStart = true; } else { btnStart = false; }

            if (pad.IsButtonDown(Buttons.BigButton) && !oldPad.IsButtonDown(Buttons.BigButton)) { btnBigButton = true; } else { btnBigButton = false; }

            if (pad.IsButtonDown(Buttons.DPadDown) && !oldPad.IsButtonDown(Buttons.DPadDown)) { btnDPadDown = true; } else { btnDPadDown = false; }

            if (pad.IsButtonDown(Buttons.DPadLeft) && !oldPad.IsButtonDown(Buttons.DPadLeft)) { btnDPadLeft = true; } else { btnDPadLeft = false; }

            if (pad.IsButtonDown(Buttons.DPadRight) && !oldPad.IsButtonDown(Buttons.DPadRight)) { btnDPadRight = true; } else { btnDPadRight = false; }

            if (pad.IsButtonDown(Buttons.DPadUp) && !oldPad.IsButtonDown(Buttons.DPadUp)) { btnDPadUp = true; } else { btnDPadUp = false; }




            //left
            if (pad.IsButtonDown(Buttons.LeftShoulder) && !oldPad.IsButtonDown(Buttons.LeftShoulder)) { btnLeftShoulder = true; } else { btnLeftShoulder = false; }

            if (pad.IsButtonDown(Buttons.LeftStick) && !oldPad.IsButtonDown(Buttons.LeftStick)) { btnLeftStick = true; } else { btnLeftStick = false; }

            if (pad.IsButtonDown(Buttons.LeftThumbstickDown) && !oldPad.IsButtonDown(Buttons.LeftThumbstickDown)) { btnLeftThumbDown = true; } else { btnLeftThumbDown = false; }

            if (pad.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldPad.IsButtonDown(Buttons.LeftThumbstickLeft)) { btnLeftThumbLeft = true; } else { btnLeftThumbLeft = false; }

            if (pad.IsButtonDown(Buttons.LeftThumbstickRight) && !oldPad.IsButtonDown(Buttons.LeftThumbstickRight)) { btnLeftThumbRight = true; } else { btnLeftThumbRight = false; }

            if (pad.IsButtonDown(Buttons.LeftThumbstickUp) && !oldPad.IsButtonDown(Buttons.LeftThumbstickUp)) { btnLeftThumbUp = true; } else { btnLeftThumbUp = false; }

            if (pad.IsButtonDown(Buttons.LeftTrigger) && !oldPad.IsButtonDown(Buttons.LeftTrigger)) { btnLeftTrigger = true; } else { btnLeftTrigger = false; }



            //right
            if (pad.IsButtonDown(Buttons.RightShoulder) && !oldPad.IsButtonDown(Buttons.RightShoulder)) { btnRightShoulder = true; } else { btnRightShoulder = false; }

            if (pad.IsButtonDown(Buttons.RightStick) && !oldPad.IsButtonDown(Buttons.RightStick)) { btnRightStick = true; } else { btnRightStick = false; }

            if (pad.IsButtonDown(Buttons.RightThumbstickDown) && !oldPad.IsButtonDown(Buttons.RightThumbstickDown)) { btnRightThumbDown = true; } else { btnRightThumbDown = false; }

            if (pad.IsButtonDown(Buttons.RightThumbstickLeft) && !oldPad.IsButtonDown(Buttons.RightThumbstickLeft)) { btnRightThumbLeft = true; } else { btnRightThumbLeft = false; }

            if (pad.IsButtonDown(Buttons.RightThumbstickRight) && !oldPad.IsButtonDown(Buttons.RightThumbstickRight)) { btnRightThumbRight = true; } else { btnRightThumbRight = false; }

            if (pad.IsButtonDown(Buttons.RightThumbstickUp) && !oldPad.IsButtonDown(Buttons.RightThumbstickUp)) { btnRightThumbUp = true; } else { btnRightThumbUp = false; }

            if (pad.IsButtonDown(Buttons.RightTrigger) && !oldPad.IsButtonDown(Buttons.RightTrigger)) { btnRightTrigger = true; } else { btnRightTrigger = false; }


            #endregion
            thumbStickLeftX = GamePad.GetState(player).ThumbSticks.Left.X;
            thumbStickLeftY = GamePad.GetState(player).ThumbSticks.Left.Y;
            thumbStickRightX = GamePad.GetState(player).ThumbSticks.Right.X;
            thumbStickRightY = GamePad.GetState(player).ThumbSticks.Right.Y;
            thumbStickLeftDeg = (float)Math.Atan2(thumbStickLeftX, thumbStickLeftY);
            thumbStickRightDeg = (float)Math.Atan2(thumbStickRightX, thumbStickRightY);
            LeftTrigger = GamePad.GetState(player).Triggers.Left;
            RightTrigger = GamePad.GetState(player).Triggers.Right;

            oldPad = pad;



        }

        public override bool isDown(Buttons button)
        {
            if (pad.IsButtonDown(button)) return true;

            return false;
        }

        public override bool isUp(Buttons button)
        {
            if (pad.IsButtonUp(button)) return true;

            return false;
        }
    }

    public class KeyboardInput : InputMethod
    {

        KeyboardState ks;
        KeyboardState oldKs;

        //translates the key presses into the gamepad variables that the controler uses
        public override void Update(GameTime gameTime)
        {
            #region Keyboard Stuff
            ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Enter) && oldKs.IsKeyUp(Keys.Enter)) { btnA = true; } else { btnA = false; }
            if (ks.IsKeyDown(Keys.Escape) && oldKs.IsKeyUp(Keys.Escape)) { btnB = true; } else { btnB = false; }

            if (ks.IsKeyDown(Keys.W) && oldKs.IsKeyUp(Keys.W) || (ks.IsKeyDown(Keys.Up) && oldKs.IsKeyUp(Keys.Up))) { btnLeftThumbUp = true; } else { btnLeftThumbUp = false; }
            if (ks.IsKeyDown(Keys.S) && oldKs.IsKeyUp(Keys.S) || (ks.IsKeyDown(Keys.Down) && oldKs.IsKeyUp(Keys.Down))) { btnLeftThumbDown = true; } else { btnLeftThumbDown = false; }
            if (ks.IsKeyDown(Keys.A) && oldKs.IsKeyUp(Keys.A) || (ks.IsKeyDown(Keys.Left) && oldKs.IsKeyUp(Keys.Left))) { btnLeftThumbLeft = true; } else { btnLeftThumbLeft = false; }
            if (ks.IsKeyDown(Keys.D) && oldKs.IsKeyUp(Keys.D) || (ks.IsKeyDown(Keys.Right) && oldKs.IsKeyUp(Keys.Right))) { btnLeftThumbRight = true; } else { btnLeftThumbRight = false; }

            if (ks.IsKeyDown(Keys.Escape) && oldKs.IsKeyUp(Keys.Escape)) { btnStart = true; } else { btnStart = false; }
            if (ks.IsKeyDown(Keys.Escape) && oldKs.IsKeyUp(Keys.Escape)) { btnBack = true; } else { btnBack = false; }


            //bullet spray
            if (ks.IsKeyDown(Keys.Q) && oldKs.IsKeyUp(Keys.Q)) { btnRightStick = true; } else { btnRightStick = false; }
            //inventory
            if (ks.IsKeyDown(Keys.E) && oldKs.IsKeyUp(Keys.E)) { btnX = true; } else { btnX = false; }
            //sprint
            if (ks.IsKeyDown(Keys.Space) && oldKs.IsKeyUp(Keys.Space)) { btnLeftStick = true; } else { btnLeftStick = false; }
            //unused / towers
            if (ks.IsKeyDown(Keys.Z) && oldKs.IsKeyUp(Keys.Z)) { btnDPadUp = true; } else { btnDPadUp = false; }
            //freeze
            if (ks.IsKeyDown(Keys.X) && oldKs.IsKeyUp(Keys.X)) { btnDPadLeft = true; } else { btnDPadLeft = false; }
            //fire
            if (ks.IsKeyDown(Keys.C) && oldKs.IsKeyUp(Keys.C)) { btnDPadRight = true; } else { btnDPadRight = false; }
            //nuke
            if (ks.IsKeyDown(Keys.V) && oldKs.IsKeyUp(Keys.V)) { btnDPadDown = true; } else { btnDPadDown = false; }

            oldKs = ks;
            #endregion

        }
        public override bool isUp(Keys k)
        {
            if (ks.IsKeyUp(k))
                return true;
            else
                return false;
        }

        public override bool isDown(Keys k)
        {
            if (ks.IsKeyDown(k))
                return true;
            else
                return false;
        }
    }

    public class Freeze : Items
    {
        Texture2D sprite = Art.Freeze;
        int alpha = 255;
        float scale = 1f;
        int destructTimer = GM.freezeTime;

        public Freeze(Vector2 Loc)
        {
            Location = Loc;

            ItemManager.addItem(this);
            Radius = (int)((Art.Freeze.Width / 2.2f) * scale);
            type = "freeze";
        }

        public override void Update(GameTime gameTime)
        {
            destructTimer--;

            if (destructTimer <= 255) alpha--;
            if (alpha <= 0) alpha = 0;
            if (destructTimer <= 0) { ItemManager.delItem(this); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Art.Freeze, Location - new Vector2(sprite.Width / 2, sprite.Height / 2), new Color(255, 255, 255, alpha));


        }

    }

    public class Firewall : Items
    {
        Texture2D sprite = Art.Firewall;
        int alpha = 255;
        float scale = 1;
        int destructTimer = GM.firewallTime;
        public Firewall(Vector2 Loc)
        {
            Location = Loc;
            Radius = (int)((Art.Firewall.Width / 2.2f) * scale);
            ItemManager.addItem(this);
            type = "firewall";
        }

        public override void Update(GameTime gameTime)
        {
            destructTimer--;

            if (destructTimer <= 255) alpha--;
            if (alpha <= 0) alpha = 0;

            if (destructTimer <= 0) { ItemManager.delItem(this); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Location - new Vector2(sprite.Width / 2, sprite.Height / 2), new Color(255, 255, 255, alpha));

        }


    }

    public class Tower : Items
    {
        Texture2D sprite = Art.Tower;
        int alpha = 255;
        float scale = 1;
        int destructTimer = GM.towerTime;
        int shootTmr = 15;
        float rotation;

        Vector2 point = Vector2.Zero;

        public Tower(Vector2 Loc)
        {
            Location = Loc;
            Radius = (int)((Art.Tower.Width / 2.2f) * scale);
            ItemManager.addItem(this);
        }

        public override void Update(GameTime gameTime)
        {
            destructTimer--;

            if (destructTimer <= 255) alpha--;
            if (alpha <= 0) alpha = 0;

            if (destructTimer <= 0) { ItemManager.delItem(this); }

            if (shootTmr > 0) shootTmr--;
            if (shootTmr == 0 && EnemyManager.enemies.Count > 0) { shootTmr = 15; shoot(); }

            if (!(EnemyManager.enemies.Count == 0))
                point = FindNearestEnemy();

            rotation = MathHelper.Lerp(rotation, (float)Math.Atan2(((double)point.Y - Location.Y), ((double)point.X - Location.X)) + MathHelper.PiOver2, .1f);

        }

        public Vector2 FindNearestEnemy()
        {

            Vector2 pos = Vector2.Zero;

            if (!EnemyManager.isUpdating)
                foreach (Enemy e in EnemyManager.enemies)
                {
                    if (Vector2.DistanceSquared(Location, e.Location) < Vector2.DistanceSquared(Location, pos))
                    {
                        pos = e.Location;
                    }
                }
            return pos;

        }

        private void shoot()
        {
            Bullets.Add(new missile_track(Location, 0, Art.Laser));
            //- new Vector2(sprite.Width / 2, sprite.Height / 2)
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Location, null, new Color(255, 255, 255, alpha), rotation, new Vector2(Art.Tower.Width / 2, Art.Tower.Height / 2) * scale, scale, SpriteEffects.None, 0f);

        }
    }

    public static class ParticleManager
    {
        public static bool updating = false;
        public static List<Particles> particles = new List<Particles>();
        static List<Particles> addParticles = new List<Particles>();
        static List<Particles> delParticles = new List<Particles>();

        public static void Update(GameTime gameTime)
        {

            updating = true;
            foreach (Particles p in particles)
            {
                p.Update(gameTime);
            }
            updating = false;

            foreach (Particles p in addParticles)
            {
                particles.Add(p);
            }

            foreach (Particles p in delParticles)
            {
                particles.Remove(p);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particles p in particles)
            {
                p.Draw(spriteBatch);
            }
        }

        public static void addParticle(Vector2 loc, int num, Texture2D sprite)
        {
            if (particles.Count < 100)
            {

                if (updating)
                    for (int i = 0; i < num; i++)
                    {

                        addParticles.Add(new Particles(loc, sprite, i));
                    }
                else
                    for (int i = 0; i < num; i++)
                    {

                        particles.Add(new Particles(loc, sprite, i));
                    }
            }

        }

        public static void delParticle(Particles p)
        {
            if (updating)
                delParticles.Add(p);
            else
                particles.Remove(p);
        }

        public static void CLEARALL()
        {
            particles.Clear();
            addParticles.Clear();
            delParticles.Clear();

        }

    }

    public class Particles
    {
        int destructTimer = 120;
        public Vector2 Location { get; protected set; }
        Texture2D sprite;

        Vector2 velocityT;
        float rotationT;
        float currRot;

        int alpha = 255;


        public Particles(Vector2 pos, Texture2D tex, int seed)
        {
            Random _rand = new Random((int)DateTime.Now.Ticks + seed);



            sprite = tex;
            Location = pos;

            //max units/sec
            velocityT = new Vector2(_rand.Next(-10, 10), _rand.Next(-10, 10));
            //max deg/sec
            rotationT = _rand.Next(-10, 10);

        }
        public void Update(GameTime gameTime)
        {

            if (destructTimer > 0) destructTimer--;
            else
                ParticleManager.delParticle(this);

            if (alpha > 0) alpha -= 5;
            MathHelper.Clamp(alpha, 0, 255);
            Location += velocityT;
            currRot += rotationT;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Location, null, new Color(255, 255, 255, alpha), currRot, new Vector2(sprite.Width / 2, sprite.Height / 2), 1f, SpriteEffects.None, 0.0f);
        }
    }

    #region Screens
    public static class ScreenManager
    {
        static ContentManager content;
        static GameScreen currScreen;
        static GameScreen overlay;



        public static Stack<GameScreen> screenStack = new Stack<GameScreen>();



        public static void Initialize()
        {

        }

        public static void LoadContent(ContentManager Content)
        {
            content = Content;

            AddScreen(new TitleScreen());

            if (currScreen != null)
                currScreen.LoadContent(content);
        }

        public static void Update(GameTime gameTime)
        {
            if (currScreen != null)
            {
                currScreen.Update(gameTime);
                currScreen = screenStack.Peek();

            }
            if (overlay != null) overlay.Update(gameTime);

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (currScreen != null)
                currScreen.Draw(spriteBatch);
            if (overlay != null)
                overlay.Draw(spriteBatch);
        }



        public static void AddScreen(GameScreen screen)
        {
            if (screenStack.Count == 0)
            {
                screenStack.Push(screen);
                currScreen = screenStack.Peek();
                currScreen.Initailize();
                currScreen.LoadContent(content);
                return;
            }

            if (screenStack.Peek() != screen)
            {
                screenStack.Push(screen);
                currScreen = screenStack.Peek();
                currScreen.Initailize();
                currScreen.LoadContent(content);
            }
        }

        public static void DelScreen()
        {
            screenStack.Pop();

            //TODO: set curr screen = to prev screen ??
            currScreen = screenStack.Peek();

        }

        public static void AddOverlay(GameScreen screen) { overlay = screen; }
        public static void DelOverlay() { overlay = null; }

    }

    public class GameScreen
    {
        protected SpriteFont font;

        public GameScreen()
        {

        }


        public virtual void Initailize() { }
        public virtual void LoadContent(ContentManager Content)
        {
            font = Content.Load<SpriteFont>("font");
        }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

    }

    public class TitleScreen : GameScreen
    {
        #region Vars

        protected string titleText;
        protected string option1;
        protected string option2;
        protected string option3;

        Texture2D bg;

        Color option1Color;
        Color option2Color;
        Color option3Color;

        int selectedIndex;
        #endregion

        public TitleScreen()
        {
            Initailize();
        }

        public override void Initailize()
        {
            base.Initailize();
            titleText = "Vectricity!";
            option1 = "PLAY";
            option2 = "OPTIONS";
            option3 = "EXIT";

            selectedIndex = 0;

            option1Color = Color.White;
            option2Color = Color.White;
            option3Color = Color.White;
        }

        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            bg = Content.Load<Texture2D>("bg");
        }

        public override void Update(GameTime gameTime)
        {
            processIndex();

            changeIndex();

        }

        private void changeIndex()
        {
            if (InputManager.PlayerInput(PlayerIndex.One).BtnLeftThumbUp || InputManager.PlayerInput(PlayerIndex.One).BtnDpadUp)
            {

                selectedIndex--;
                if (selectedIndex <= -1) selectedIndex = 2;
            }

            if (InputManager.PlayerInput(PlayerIndex.One).BtnLeftThumbDown || InputManager.PlayerInput(PlayerIndex.One).BtnDpadDown)
            {

                selectedIndex++;
                if (selectedIndex >= 3) selectedIndex = 0;
            }
            updateColor();
        }

        private void updateColor()
        {
            switch (selectedIndex)
            {
                case 0:
                    option1Color = Color.Blue;
                    option2Color = Color.White;
                    option3Color = Color.White;
                    break;
                case 1:
                    option1Color = Color.White;
                    option2Color = Color.Blue;
                    option3Color = Color.White;
                    break;
                case 2:
                    option1Color = Color.White;
                    option2Color = Color.White;
                    option3Color = Color.Blue;
                    break;

            }

        }

        private void processIndex()
        {
            if (InputManager.PlayerInput(PlayerIndex.One).BtnA)
            {


                switch (selectedIndex)
                {
                    case 0:
                        //TODO: Start game here
                        Play();
                        break;
                    case 1:
                        //TODO: Load Options Screen
                        Options();
                        break;
                    case 2:
                        Exit();
                        break;

                }

            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //input screen dimsension
            spriteBatch.Draw(bg, new Rectangle(0, 0, (int)GM.ScreenDims.X, (int)GM.ScreenDims.Y), Color.White);
            spriteBatch.DrawString(font, titleText, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(titleText).X / 2, GM.ScreenDims.Y * 0.05f), Color.White);
            spriteBatch.DrawString(font, option1, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option1).X / 2, GM.ScreenDims.Y * 0.4f), option1Color);
            spriteBatch.DrawString(font, option2, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option2).X / 2, GM.ScreenDims.Y * 0.60f), option2Color);
            spriteBatch.DrawString(font, option3, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option3).X / 2, GM.ScreenDims.Y * 0.80f), option3Color);
            spriteBatch.DrawString(font, "Download at http://triangularcubicle.xyz/", new Vector2((GM.ScreenDims.X / 2) - font.MeasureString("Download at http://triangularcubicle.xyz/").X / 2, GM.ScreenDims.Y * 0.9f), Color.White);

            spriteBatch.DrawString(font, "Developed By: Riley Schoppa", new Vector2((GM.ScreenDims.X * .80f) - font.MeasureString(option3).X / 2, GM.ScreenDims.Y * 0.95f), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);

        }

        public void Play()
        {
            GM.NEWGAME();
        }
        public void Options() { ScreenManager.AddScreen(new options()); }
        public void Exit() { System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow(); }


    }

    public class GameGUI : GameScreen
    {


        public override void Initailize()
        {
            base.Initailize();

        }

        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if (InputManager.PlayerInput(PlayerIndex.One).BtnStart) ScreenManager.AddScreen(new Pause(PlayerIndex.One));

            //clamp mouse
            Vector2 mp = Vector2.Clamp(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Vector2.Zero + new Vector2(Art.Cursor.Height / 2, Art.Cursor.Width / 2), GM.ScreenDims/* - new Vector2(Art.Cursor.Height / 2, Art.Cursor.Width / 2)*/);
            Mouse.SetPosition((int)mp.X, (int)mp.Y);

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            int stringLength = (int)font.MeasureString("SCORE: " + GM.Points.ToString()).X;
            int stringHeight = (int)font.MeasureString("SCORE: " + GM.Points.ToString()).Y;

            int offset = 10;
            int space = (int)(GM.ScreenDims.X - stringLength - (offset * 4)) / 4;

            base.Draw(spriteBatch);
            //drawing score
            spriteBatch.DrawString(font, "SCORE: " + GM.Points.ToString(), new Vector2((GM.ScreenDims.X * .01f), (GM.ScreenDims.Y * .01f)), Color.White);

            //drawing the health bars for multiplayers
            if (PlayerManager.player(1) != null)
            {
                spriteBatch.Draw(Art.HealthBar, new Rectangle((stringLength + offset * 2), offset, (int)(space * ((PlayerManager.player(1).Health / GM.maxHealth))), stringHeight - 5), Color.Red);
            }
            if (PlayerManager.player(2) != null)
            {
                spriteBatch.Draw(Art.HealthBar, new Rectangle((stringLength + offset * 2) + space, offset, (int)(space * ((PlayerManager.player(2).Health / GM.maxHealth))), stringHeight - 5), Color.Green);
            }
            if (PlayerManager.player(3) != null)
            {
                spriteBatch.Draw(Art.HealthBar, new Rectangle((stringLength + offset * 2) + space * 2, offset, (int)(space * ((PlayerManager.player(3).Health / GM.maxHealth))), stringHeight - 5), Color.Blue);
            }
            if (PlayerManager.player(4) != null)
            {
                spriteBatch.Draw(Art.HealthBar, new Rectangle((stringLength + offset * 2) + space * 3, offset, (int)(space * ((PlayerManager.player(4).Health / GM.maxHealth))), stringHeight - 5), Color.Yellow);
            }

            //drawing mouse
            float cursorScale = .5f;
            if (GM.keyboardEnabled)
            {
                Vector2 loc = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - new Vector2(Art.Cursor.Width / 2);
                //keeping mouse in bounds
                loc = Vector2.Clamp(loc, Vector2.Zero, GM.ScreenDims - new Vector2(Art.Cursor.Width, Art.Cursor.Height) * cursorScale);

                spriteBatch.Draw(Art.Cursor, loc + new Vector2(Art.Cursor.Width / 2, Art.Cursor.Height / 2), null, Color.White, 0, new Vector2(Art.Cursor.Width / 2, Art.Cursor.Height / 2) * cursorScale, cursorScale, SpriteEffects.None, 0f);

            }


        }

    }

    public class options : GameScreen
    {
        int selectedIndex;
        Color option1Color;
        Color option2Color;
        string option1;
        string option2;

        public override void Initailize()
        {

            base.Initailize();
            option1Color = Color.White;
            option1 = "Controls: WSAD, bomb-Q,Space-Speed, Z,X,C,V-Powerups";
            option2Color = Color.White;
            option2 = "Back";

        }

        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            checkInput();
            processIndex();

            updateColor();
        }

        private void processIndex()
        {
            if (InputManager.PlayerInput(PlayerIndex.One).BtnA)
            {
                switch (selectedIndex)
                {
                    case 0:
                        //do thing here for show controls
                        break;

                    case 1:
                        ScreenManager.DelScreen();
                        break;
                }
            }
        }

        private void checkInput()
        {
            if (InputManager.PlayerInput(PlayerIndex.One).BtnB) { ScreenManager.DelScreen(); }

            if (InputManager.PlayerInput(PlayerIndex.One).BtnLeftThumbUp || InputManager.PlayerInput(PlayerIndex.One).BtnDpadUp)
            {

                selectedIndex--;
                if (selectedIndex <= -1) selectedIndex = 1;
            }

            if (InputManager.PlayerInput(PlayerIndex.One).BtnLeftThumbDown || InputManager.PlayerInput(PlayerIndex.One).BtnDpadDown)
            {

                selectedIndex++;
                if (selectedIndex >= 2) selectedIndex = 0;
            }
        }

        private void updateColor()
        {
            switch (selectedIndex)
            {
                case 0:
                    option1Color = Color.Blue;
                    option2Color = Color.White;

                    break;
                case 1:
                    option1Color = Color.White;
                    option2Color = Color.Blue;

                    break;
                case 2:
                    option1Color = Color.White;
                    option2Color = Color.White;

                    break;

            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.DrawString(font, option1, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option1).X / 2, GM.ScreenDims.Y * 0.4f), option1Color);
            spriteBatch.DrawString(font, option2, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option2).X / 2, GM.ScreenDims.Y * 0.60f), option2Color);
        }
    }

    public class buyScreen : GameScreen
    {
        int[] bulletCost = new int[4] { 25, 50, 100, 150 };
        int[] laserCost = new int[4] { 50, 75, 125, 200 };


        int[] missileCost = new int[4] { 50, 100, 150, 250 };

        float textScale = .25f;

        int timer = 0;

        bool anim = true;
        PlayerIndex player;
        int selectedIndex = 0;
        int boxWidth, boxHeight;
        Color[] color = new Color[9];
        Rectangle[] rects = new Rectangle[9];

        public buyScreen(PlayerIndex Player)
        {
            GM.InGame = false;
            player = Player;
            /*
                 0  1  2 
                 3  4  5
                 6  7  8
             */

            boxWidth = (int)(GM.ScreenDims.X - 100) / 3;
            boxHeight = (int)(GM.ScreenDims.Y - 100) / 3;
            InitializeRects();
        }

        public override void Update(GameTime gameTime)
        {
            if (anim == true)
            {
                timer++;
            }

            UpdateGraphics();

            ChangeIndex();

            DoActions();

            if (InputManager.PlayerInput(player).BtnB || InputManager.PlayerInput(player).BtnBack)
            {
                ScreenManager.DelScreen();
                //only 1 player
                if (GM.keyboardEnabled)
                    ScreenManager.AddOverlay(new resumeCountdown(3));
                else
                    GM.InGame = true;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //drawing the items
            if (anim == true)
                playAnim(spriteBatch);
            else
            {
                spriteBatch.Draw(Art.MenuItems[0], rects[0], color[0]);
                spriteBatch.Draw(Art.MenuItems[1], rects[1], color[1]);
                spriteBatch.Draw(Art.MenuItems[2], rects[2], color[2]);

                spriteBatch.Draw(Art.MenuItems[3], rects[3], color[3]);
                spriteBatch.Draw(Art.MenuItems[4], rects[4], color[4]);
                spriteBatch.Draw(Art.MenuItems[5], rects[5], color[5]);

                spriteBatch.Draw(Art.MenuItems[6], rects[6], color[6]);
                spriteBatch.Draw(Art.MenuItems[7], rects[7], color[7]);
                spriteBatch.Draw(Art.MenuItems[8], rects[8], color[8]);
            }


            //drawing points

            spriteBatch.DrawString(Art.Font, "SCORE: " + GM.Points, new Vector2(GM.ScreenDims.X / 2 - (Art.Font.MeasureString("SCORE: " + GM.Points).X * textScale) / 2, boxHeight / 10), Color.White, 0f, new Vector2(), textScale, SpriteEffects.None, 0f);


            drawPrices(spriteBatch);
        }

        private void InitializeRects()
        {
            rects[0] = new Rectangle((0 * boxWidth) + 25, (0 * boxHeight) + 25, boxWidth, boxHeight);
            rects[1] = new Rectangle((1 * boxWidth) + 50, (0 * boxHeight) + 25, boxWidth, boxHeight);
            rects[2] = new Rectangle((2 * boxWidth) + 75, (0 * boxHeight) + 25, boxWidth, boxHeight);
            rects[3] = new Rectangle((0 * boxWidth) + 25, (1 * boxHeight) + 50, boxWidth, boxHeight);
            rects[4] = new Rectangle((1 * boxWidth) + 50, (1 * boxHeight) + 50, boxWidth, boxHeight);
            rects[5] = new Rectangle((2 * boxWidth) + 75, (1 * boxHeight) + 50, boxWidth, boxHeight);
            rects[6] = new Rectangle((0 * boxWidth) + 25, (2 * boxHeight) + 75, boxWidth, boxHeight);
            rects[7] = new Rectangle((1 * boxWidth) + 50, (2 * boxHeight) + 75, boxWidth, boxHeight);
            rects[8] = new Rectangle((2 * boxWidth) + 75, (2 * boxHeight) + 75, boxWidth, boxHeight);
        }

        private void playAnim(SpriteBatch spriteBatch)
        {
            int interval = 10;

            if (timer >= 0 * interval)
                spriteBatch.Draw(Art.MenuItems[0], rects[0], color[0]);

            if (timer >= 1 * interval)
            {
                spriteBatch.Draw(Art.MenuItems[1], rects[1], color[1]);
                spriteBatch.Draw(Art.MenuItems[3], rects[3], color[3]);
            }

            if (timer >= 2 * interval)
            {
                spriteBatch.Draw(Art.MenuItems[2], rects[2], color[2]);
                spriteBatch.Draw(Art.MenuItems[4], rects[4], color[4]);
                spriteBatch.Draw(Art.MenuItems[6], rects[6], color[6]);
            }

            if (timer >= 3 * interval)
            {
                spriteBatch.Draw(Art.MenuItems[5], rects[5], color[5]);
                spriteBatch.Draw(Art.MenuItems[7], rects[7], color[7]);
            }

            if (timer >= 4 * interval)
            {
                spriteBatch.Draw(Art.MenuItems[8], rects[8], color[8]);
                anim = false;
            }

        }

        private void ChangeIndex()
        {
            if (InputManager.PlayerInput(player).BtnDpadUp || InputManager.PlayerInput(player).BtnLeftThumbUp)
            {
                selectedIndex -= 3;
            }
            if (InputManager.PlayerInput(player).BtnDpadDown || InputManager.PlayerInput(player).BtnLeftThumbDown)
            {
                selectedIndex += 3;
            }
            if (InputManager.PlayerInput(player).BtnDpadLeft || InputManager.PlayerInput(player).BtnLeftThumbLeft)
            {
                selectedIndex -= 1;
            }
            if (InputManager.PlayerInput(player).BtnDpadRight || InputManager.PlayerInput(player).BtnLeftThumbRight)
            {
                selectedIndex += 1;
            }
        }

        private void UpdateGraphics()
        {
            for (int i = 0; i < 9; i++)
            {
                color[i] = Color.White;

            }



            if (selectedIndex < 0 || selectedIndex > 8)
            {
                if (selectedIndex < 0)
                {
                    selectedIndex += 11;
                }
                if (selectedIndex > 8)
                {
                    selectedIndex -= 11;
                }

            }

            switch (selectedIndex)
            {
                case 0:
                    color[0] = PlayerManager.player(player).Color;
                    break;
                case 1:
                    color[1] = PlayerManager.player(player).Color;
                    break;
                case 2:
                    color[2] = PlayerManager.player(player).Color;
                    break;
                case 3:
                    color[3] = PlayerManager.player(player).Color;
                    break;
                case 4:
                    color[4] = PlayerManager.player(player).Color;
                    break;
                case 5:
                    color[5] = PlayerManager.player(player).Color;
                    break;
                case 6:
                    color[6] = PlayerManager.player(player).Color;
                    break;
                case 7:
                    color[7] = PlayerManager.player(player).Color;
                    break;
                case 8:
                    color[8] = PlayerManager.player(player).Color;
                    break;
            }

        }

        private void drawPrices(SpriteBatch spriteBatch)
        {
            if (!anim)
            {
                if (PlayerManager.player(player).BulletTier != 5)
                {
                    string dispTxt0 = "Price: " + bulletCost[PlayerManager.player(player).BulletTier - 1];
                    spriteBatch.DrawString(Art.Font, dispTxt0, getBottomCorrner(rects[0]) - Art.Font.MeasureString(dispTxt0) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }
                if (PlayerManager.player(player).LaserTier != 5)
                {
                    string dispTxt1 = "Price: " + laserCost[PlayerManager.player(player).LaserTier - 1];
                    spriteBatch.DrawString(Art.Font, dispTxt1, getBottomCorrner(rects[1]) - Art.Font.MeasureString(dispTxt1) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }
                if (PlayerManager.player(player).MissileTier != 5)
                {
                    string dispTxt2 = "Price: " + missileCost[PlayerManager.player(player).MissileTier - 1];
                    spriteBatch.DrawString(Art.Font, dispTxt2, getBottomCorrner(rects[2]) - Art.Font.MeasureString(dispTxt2) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }

                string dispTxt3 = "Price: " + 100;
                spriteBatch.DrawString(Art.Font, dispTxt3, getBottomCorrner(rects[3]) - Art.Font.MeasureString(dispTxt3) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                string dispTxt4 = "Price: " + 100;
                spriteBatch.DrawString(Art.Font, dispTxt4, getBottomCorrner(rects[4]) - Art.Font.MeasureString(dispTxt4) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                string dispTxt5 = "Price: " + 125;
                spriteBatch.DrawString(Art.Font, dispTxt5, getBottomCorrner(rects[5]) - Art.Font.MeasureString(dispTxt5) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

                string dispTxt6 = "Price: " + 100;
                spriteBatch.DrawString(Art.Font, dispTxt6, getBottomCorrner(rects[6]) - Art.Font.MeasureString(dispTxt6) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                string dispTxt7 = "Price: " + 75;
                spriteBatch.DrawString(Art.Font, dispTxt7, getBottomCorrner(rects[7]) - Art.Font.MeasureString(dispTxt7) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                string dispTxt8 = "Price: " + 100;
                spriteBatch.DrawString(Art.Font, dispTxt8, getBottomCorrner(rects[8]) - Art.Font.MeasureString(dispTxt8) * textScale, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

            }
        }

        public Vector2 getBottomCorrner(Rectangle v)
        {
            return new Vector2(v.Right, v.Bottom);

        }

        //functions for each button press
        private void DoActions()
        {
            if (InputManager.PlayerInput(player).BtnA || InputManager.PlayerInput(player).BtnRightTrigger)
            {
                switch (selectedIndex)
                {
                    #region Upgrade bullets
                    case 0:
                        PlayerManager.player(player).fireRate = 5;
                        PickBullet();

                        if (PlayerManager.player(player).BulletTier != 5)
                        {

                            if (GM.Points > bulletCost[PlayerManager.player(player).BulletTier - 1])
                            {
                                //play buy sound here
                                Art.Click.Play();


                                GM.Points -= bulletCost[PlayerManager.player(player).BulletTier - 1];
                                PlayerManager.player(player).BulletTier += 1;
                                PickBullet();
                            }
                            else { Art.Error.Play(); }

                        }
                        else
                        {
                            //play sound here
                            notEnoughSound();
                            PickBullet();
                        }
                        break;


                    #endregion

                    #region Upgrade Lasers

                    case 1:
                        PlayerManager.player(player).fireRate = 5;
                        PickLaser();
                        //upgrade lasers
                        if (PlayerManager.player(player).LaserTier != 5)
                        {
                            if (GM.Points > laserCost[PlayerManager.player(player).LaserTier - 1])
                            {
                                //play buy sound here
                                Art.Click.Play();


                                GM.Points -= laserCost[PlayerManager.player(player).LaserTier - 1];
                                PlayerManager.player(player).LaserTier += 1;
                                PickLaser();
                            }
                            else { Art.Error.Play(); }

                        }
                        else
                        {
                            PickLaser();
                            //play sound here
                            notEnoughSound();
                        }
                        break;

                    #endregion

                    #region Upgrade Missiles
                    case 2:
                        PlayerManager.player(player).fireRate = 8;
                        PickMissile();
                        //upgrade missiles                 
                        if (PlayerManager.player(player).MissileTier != 5)
                        {

                            if (GM.Points > missileCost[PlayerManager.player(player).MissileTier - 1])
                            {
                                //play buy sound here
                                Art.Click.Play();


                                GM.Points -= missileCost[PlayerManager.player(player).MissileTier - 1];
                                PlayerManager.player(player).MissileTier += 1;
                                PickMissile();
                            }
                            else { Art.Error.Play(); }

                        }
                        else
                        {
                            //play sound here
                            PickMissile();
                            notEnoughSound();
                        }
                        break;
                    #endregion

                    case 3:
                        //upgrade regen
                        if (GM.Points >= 100)
                        {
                            GM.regenerationAmt += 5;
                            GM.Points -= 100;
                            Art.Click.Play();
                        }
                        else { Art.Error.Play(); }
                        break;
                    case 4:
                        //upgrade max health
                        if (GM.Points >= 100)
                        {
                            GM.maxHealth += 25;
                            GM.Points -= 100;
                            Art.Click.Play();
                        }
                        else { Art.Error.Play(); }
                        break;
                    case 5:
                        //upgrade base defense
                        if (GM.Points >= 125)
                        {
                            GM.towerTime += 900;
                            GM.Points -= 125;
                            Art.Click.Play();
                        }
                        else { Art.Error.Play(); }
                        break;
                    case 6:
                        // upgrade firewall
                        if (GM.Points >= 100)
                        {
                            GM.firewallTime += 900;
                            GM.Points -= 100;
                            Art.Click.Play();
                        }
                        else { Art.Error.Play(); }
                        break;
                    case 7:
                        //upgrade freeze
                        if (GM.Points >= 75)
                        {
                            GM.freezeTime += 900;
                            GM.Points -= 75;
                            Art.Click.Play();
                        }
                        else { Art.Error.Play(); }
                        break;
                    case 8:
                        //upgrade nuke
                        if (GM.Points >= 100 && GM.nukeRefund < 250)
                        {
                            GM.nukeRefund += 25;
                            GM.Points -= 100;
                            Art.Click.Play();
                        }
                        else { Art.Error.Play(); }
                        break;
                }
            }
        }

        private void PickBullet()
        {
            switch (PlayerManager.player(player).BulletTier)
            {
                case 1:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.B1;
                    break;
                case 2:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.B2;
                    break;
                case 3:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.B3;
                    break;
                case 4:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.B4;
                    break;
                case 5:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.B5;
                    break;
            }
        }

        private void PickLaser()
        {
            switch (PlayerManager.player(player).LaserTier)
            {
                case 1:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.L1;
                    break;
                case 2:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.L2;
                    break;
                case 3:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.L3;
                    break;
                case 4:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.L4;
                    break;
                case 5:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.L5;
                    break;
            }
        }

        private void PickMissile()
        {
            switch (PlayerManager.player(player).MissileTier)
            {
                case 1:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.M1;
                    break;
                case 2:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.M2;
                    break;
                case 3:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.M3;
                    break;
                case 4:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.M4;
                    break;
                case 5:
                    PlayerManager.player(player).ammo = PlayerShip.Ammo.M5;
                    break;
            }
        }

        private void notEnoughSound()
        {
            //TODO: play sound here
            Art.Error.Play();

        }
    }

    public class highScore : GameScreen
    {
        PlayerIndex player;
        string titleText = "YOUR SCORE WAS " + GM.maxPoints.ToString();
        int old_highscore;
        HighScore hs;
        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            hs = Content.Load<HighScore>("highscore");
            old_highscore = hs.score;
        }

        public highScore(PlayerIndex pl)
        {
            player = pl;
            if (GM.maxPoints > old_highscore)
            {
                //TODO: WRITE HS TO FILE HERE
            }

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            EnemyManager.CLEARALL();

            if (InputManager.PlayerInput(player).BtnStart || InputManager.PlayerInput(player).BtnA)
            {
                ScreenManager.AddScreen(new TitleScreen());
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(font, titleText, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(titleText).X / 2, GM.ScreenDims.Y * 0.4f), Color.White);
            if (GM.maxPoints > old_highscore)
            {
                spriteBatch.DrawString(font, "YOU MADE A NEW HIGH SCORE!!!", new Vector2((GM.ScreenDims.X / 2) - font.MeasureString("YOU MADE A NEW HIGH SCORE!!!").X / 2, GM.ScreenDims.Y * 0.6f), Color.White);
            }
            spriteBatch.DrawString(font, "Download at http://triangularcubicle.xyz/", new Vector2((GM.ScreenDims.X / 2) - font.MeasureString("Download at http://triangularcubicle.xyz/").X / 2, GM.ScreenDims.Y * 0.9f), Color.White);

        }


    }

    public class waveAlret : GameScreen
    {

        int alpha = 255;
        string alertText = "";
        public waveAlret(int num)
        {
            alertText = num.ToString();
        }
        public override void Update(GameTime gameTime)
        {
            alpha -= 2;
            if (alpha < 0)
            {
                alpha = 0;
                ScreenManager.DelOverlay();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {

            //spriteBatch.DrawString(
            spriteBatch.DrawString(Art.Font, alertText, new Vector2((GM.ScreenDims.X / 2) - Art.Font.MeasureString(alertText).X / 2, GM.ScreenDims.Y / 2), new Color(alpha, alpha, alpha, alpha), 0.0f, new Vector2(Art.Font.MeasureString(alertText).X / 2, Art.Font.MeasureString(alertText).Y / 2), 5f, SpriteEffects.None, 1.0f);

        }


    }

    public class resumeCountdown : GameScreen
    {

        int alpha = 255;
        string alertText = "";
        int secs = 0;
        int ticks = 0;
        public resumeCountdown(int num)
        {
            GM.InGame = false;
            alertText = num.ToString();
            ticks = 60 * num;
            int secs = num;
        }


        public override void Update(GameTime gameTime)
        {



            alertText = ((ticks / 60) + 1).ToString();




            if (alpha < 0)
            {
                alpha = 0;
                alpha = 255;
            }
            if (ticks <= 0)
            {
                GM.InGame = true;
                ScreenManager.DelOverlay();
            }
            ticks--;
            //alpha -= 3;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {

            //spriteBatch.DrawString(
            spriteBatch.DrawString(Art.Font, alertText, new Vector2((GM.ScreenDims.X / 2) - Art.Font.MeasureString(alertText).X / 2, GM.ScreenDims.Y / 2), new Color(alpha, alpha, alpha, alpha), 0.0f, new Vector2(Art.Font.MeasureString(alertText).X / 2, Art.Font.MeasureString(alertText).Y / 2), 5f, SpriteEffects.None, 1.0f);

        }


    }

    public class Pause : GameScreen
    {
        PlayerIndex player;
        int selectedIndex;
        Color option1Color;
        Color option2Color;
        Color option3Color;
        string titleText = "Options";
        string option1 = "Cancel";
        string option2 = "New Game";
        string option3 = "Main Menu";
        public Pause(PlayerIndex pl) { player = pl; GM.InGame = false; }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            changeIndex();
            if (InputManager.PlayerInput(player).BtnA || InputManager.PlayerInput(player).BtnRightTrigger)
            {
                if (selectedIndex == 0)
                {
                    ScreenManager.DelScreen();
                    ScreenManager.AddOverlay(new resumeCountdown(3));
                }
                if (selectedIndex == 1)
                {
                    GM.NEWGAME();
                    //ScreenManager.DelScreen();

                }
                if (selectedIndex == 2)
                {
                    ScreenManager.AddScreen(new TitleScreen());

                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, titleText, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(titleText).X / 2, GM.ScreenDims.Y * 0.05f), Color.White);
            spriteBatch.DrawString(font, option1, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option1).X / 2, GM.ScreenDims.Y * 0.4f), option1Color);
            spriteBatch.DrawString(font, option2, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option2).X / 2, GM.ScreenDims.Y * 0.60f), option2Color);
            spriteBatch.DrawString(font, option3, new Vector2((GM.ScreenDims.X / 2) - font.MeasureString(option3).X / 2, GM.ScreenDims.Y * 0.80f), option3Color);


            if (selectedIndex == 0)
            { option1Color = Color.Blue; option2Color = Color.White; }
            if (selectedIndex == 1) { option1Color = Color.White; option2Color = Color.Blue; }
        }


        private void changeIndex()
        {
            if (InputManager.PlayerInput(PlayerIndex.One).BtnLeftThumbUp || InputManager.PlayerInput(PlayerIndex.One).BtnDpadUp)
            {

                selectedIndex--;
                if (selectedIndex <= -1) selectedIndex = 2;
            }

            if (InputManager.PlayerInput(PlayerIndex.One).BtnLeftThumbDown || InputManager.PlayerInput(PlayerIndex.One).BtnDpadDown)
            {

                selectedIndex++;
                if (selectedIndex >= 3) selectedIndex = 0;
            }
            updateColor();
        }

        private void updateColor()
        {
            switch (selectedIndex)
            {
                case 0:
                    option1Color = Color.Blue;
                    option2Color = Color.White;
                    option3Color = Color.White;
                    break;
                case 1:
                    option1Color = Color.White;
                    option2Color = Color.Blue;
                    option3Color = Color.White;
                    break;
                case 2:
                    option1Color = Color.White;
                    option2Color = Color.White;
                    option3Color = Color.Blue;
                    break;

            }

        }
    }
    #endregion

    #region BULLET CLASSES

    public class baseBullet
    {
        protected int AOERADIUS = 75;
        #region Vars
        public Color color = Color.White;
        protected Texture2D Sprite;
        protected Vector2 location { get; set; }
        protected float Rotation { get; set; }
        protected float speed = 15f;
        protected bool exists = true;
        protected float damage = 20, radius, scale = 0.25f;
        protected float Offset = 0;
        bool isFire = false;
        bool isAOE = false;
        bool isPlayer = true;
        bool penetrates = false;
        private bool player;
        #endregion

        #region properties


        public bool IsPlayer
        {
            get { return isPlayer; }
            protected set { isPlayer = value; }
        }


        public bool IsFire
        {
            get { return isFire; }
            protected set { isFire = value; }
        }

        public bool Penetrates
        {
            get { return penetrates; }
            protected set { penetrates = value; }
        }

        public bool IsAOE { get; protected set; }

        public Vector2 Location
        {
            get { return location; }
            protected set { location = value; }
        }


        public float Damage
        {
            get { return damage; }
            protected set { damage = value; }
        }

        public float Radius
        {
            get { return radius; }
            protected set { radius = value; }
        }


        #endregion

        public baseBullet(Vector2 loc, float rot)
        {
            //constructor

            Initialize();
            Location = loc;
            Rotation = rot;

            //radius = (Sprite.Width / 2.2f) * scale;
        }

        public baseBullet(Vector2 loc, float rot, bool player)
        {
            this.player = player;
        }

        public virtual void Initialize()
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            if (!GameRoot.Instance.GraphicsDevice.Viewport.Bounds.Contains(new Point((int)Location.X, (int)Location.Y)))
            {
                exists = false;
                Die();
                return;
            }
            if (exists)
            {
                Vector2 velocity = new Vector2((float)Math.Cos(Rotation - (Math.PI / 2)), (float)Math.Sin(Rotation - (Math.PI / 2)));

                Location += velocity * speed;

            }
        }

        public virtual void Draw(SpriteBatch sprtieBatch)
        {
            if (exists)
            {
                sprtieBatch.Draw(Sprite, Location, null, color, Rotation, new Vector2(Sprite.Width / 2, Sprite.Height / 2), scale, SpriteEffects.None, 0.0f);

            }
        }

        public void Die() { exists = false; Bullets.Remove(this); }

        public void AOE(float dmg)
        {
            foreach (Enemy e in NearestEnemys(AOERADIUS))
            {

                e.TakeDamage(0, 10);
                e.TakeDamage(dmg);

            }

        }

        public List<Enemy> NearestEnemys(int rad)
        {
            List<Enemy> toAtt = new List<Enemy>();

            if (!EnemyManager.isUpdating)
                foreach (Enemy e in EnemyManager.enemies)
                {
                    if (Vector2.DistanceSquared(Location, e.Location) <= Math.Pow(rad, 2))
                    {
                        toAtt.Add(e);
                    }
                }
            return toAtt;

        }

    }


    #region Bullets

    public class bullet_T1 : baseBullet
    {
        public bullet_T1(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Initialize();

        }

        public bullet_T1(Vector2 initPos, float initRot, bool player)
             : base(initPos, initRot)
        {
            IsPlayer = player;
            Initialize();

        }

        public override void Initialize()
        {
            Sprite = Art.Bullet;
            color = Color.Yellow;

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class bullet_T1_Bounce : baseBullet
    {
        int destructTimer = 240;
        bool changed = false;
        public bullet_T1_Bounce(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Initialize();

        }

        public bullet_T1_Bounce(Vector2 initPos, float initRot, bool player)
           : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Initialize();

        }

        public override void Initialize()
        {
            Sprite = Art.Bullet;
            color = Color.Green;
            //scale = .5f;
            //radius = (Sprite.Width / 2.2f) * scale;
        }


        public override void Update(GameTime gameTime)
        {
            Vector2 velocity = new Vector2((float)Math.Cos(Rotation - (Math.PI / 2)), (float)Math.Sin(Rotation - (Math.PI / 2)));

            if (!GameRoot.Instance.GraphicsDevice.Viewport.Bounds.Contains(new Point((int)Location.X, (int)Location.Y)))
            {

                //TODO Proper bounce physics

                speed *= -1;


            }

            if (exists)
            {
                //KeyboardState ks = Keyboard.GetState();
                //if (ks.IsKeyDown(Keys.U)) {
                //    velocity.X *= -1;

                //}
                Location += velocity * speed;


            }

            if (destructTimer <= 0)
            {
                destructTimer = 0;
                exists = false;
                Die();
                return;
            }


            destructTimer--;
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class bullet_T2 : bullet_T1
    {
        public bullet_T2(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;

        }

        public bullet_T2(Vector2 initPos, float initRot, bool player)
           : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class bullet_T3 : bullet_T2
    {
        public bullet_T3(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new bullet_T1(initPos + offset * Offset, initRot + .05f);
            Bullets.Add(i);

            baseBullet j = new bullet_T1(initPos - offset * Offset, initRot - .05f);
            Bullets.Add(j);
        }

        public bullet_T3(Vector2 initPos, float initRot, bool player)
            : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new bullet_T1(initPos + offset * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(i);

            baseBullet j = new bullet_T1(initPos - offset * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(j);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class bullet_T4 : bullet_T3
    {
        public bullet_T4(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new bullet_T1_Bounce(initPos + offset * Offset, initRot + .05f);
            Bullets.Add(i);

            baseBullet j = new bullet_T1_Bounce(initPos - offset * Offset, initRot - .05f);
            Bullets.Add(j);
        }

        public bullet_T4(Vector2 initPos, float initRot, bool player)
   : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new bullet_T1_Bounce(initPos + offset * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(i);

            baseBullet j = new bullet_T1_Bounce(initPos - offset * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(j);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class bullet_T5 : bullet_T4
    {
        public bullet_T5(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new bullet_T1_Bounce(initPos + offset * 2 * Offset, initRot + .05f);
            Bullets.Add(i);

            baseBullet j = new bullet_T1_Bounce(initPos - offset * 2 * Offset, initRot - .05f);
            Bullets.Add(j);

            baseBullet k = new bullet_T1_Bounce(initPos + offset * Offset, initRot + .05f);
            Bullets.Add(k);

            baseBullet l = new bullet_T1_Bounce(initPos - offset * Offset, initRot - .05f);
            Bullets.Add(l);
        }

        public bullet_T5(Vector2 initPos, float initRot, bool player)
          : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Bullet;
            color = Color.Yellow;
            scale = 0.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new bullet_T1_Bounce(initPos + offset * 2 * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(i);

            baseBullet j = new bullet_T1_Bounce(initPos - offset * 2 * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(j);

            baseBullet k = new bullet_T1_Bounce(initPos + offset * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(k);

            baseBullet l = new bullet_T1_Bounce(initPos - offset * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(l);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    #endregion

    #region lasers

    public class laser_T1 : baseBullet
    {
        public laser_T1(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            IsFire = true;
            Initialize();

        }

        public laser_T1(Vector2 initPos, float initRot, bool player)
           : base(initPos, initRot)
        {
            IsPlayer = player;
            IsFire = true;
            Initialize();

        }

        public override void Initialize()
        {
            Sprite = Art.Laser2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            speed += 3;
            Penetrates = true;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }

    }

    public class laser_T2 : laser_T1
    {
        public laser_T2(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Initialize();
            Sprite = Art.Laser;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
        }

        public laser_T2(Vector2 initPos, float initRot, bool player)
             : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Initialize();
            Sprite = Art.Laser;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
        }

        public override void Initialize()
        {
            Penetrates = true;
            Sprite = Art.Laser;

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class laser_T3 : laser_T2
    {
        public laser_T3(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Initialize();
            Sprite = Art.Laser;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new laser_T1(initPos + offset * Offset, initRot + .05f);
            Bullets.Add(i);

            baseBullet j = new laser_T1(initPos - offset * Offset, initRot - .05f);
            Bullets.Add(j);
        }

        public laser_T3(Vector2 initPos, float initRot, bool player)
            : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Initialize();
            Sprite = Art.Laser;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));




            baseBullet i = new laser_T1(initPos + offset * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(i);


            baseBullet j = new laser_T1(initPos - offset * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(j);
        }

        public override void Initialize()
        {
            Sprite = Art.Laser;
            Penetrates = true;

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class laser_T4 : laser_T3
    {
        public laser_T4(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Initialize();

            Sprite = Art.Laser;
            scale = 1.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new laser_T1(initPos + offset * Offset * 2, initRot + .05f);
            Bullets.Add(i);

            baseBullet j = new laser_T1(initPos - offset * Offset * 2, initRot - .05f);
            Bullets.Add(j);

            baseBullet k = new laser_T1(initPos + offset * Offset, initRot + .05f);
            Bullets.Add(k);

            baseBullet l = new laser_T1(initPos - offset * Offset, initRot - .05f);
            Bullets.Add(l);
        }

        public laser_T4(Vector2 initPos, float initRot, bool player)
          : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Initialize();

            Sprite = Art.Laser;
            scale = 1.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new laser_T1(initPos + offset * Offset * 2, initRot + .05f, IsPlayer);
            Bullets.Add(i);

            baseBullet j = new laser_T1(initPos - offset * Offset * 2, initRot - .05f, IsPlayer);
            Bullets.Add(j);


            baseBullet k = new laser_T1(initPos + offset * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(k);

            baseBullet l = new laser_T1(initPos - offset * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(l);
        }

        public override void Initialize()
        {
            Sprite = Art.Laser;
            Penetrates = true;

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }

    public class laser_T5 : laser_T4
    {
        public laser_T5(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Initialize();


            Sprite = Art.Laser;
            scale = 1.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new laser_T2(initPos + offset * Offset, initRot + .05f);
            Bullets.Add(i);

            baseBullet j = new laser_T2(initPos - offset * Offset, initRot - .05f);
            Bullets.Add(j);


            baseBullet k = new laser_T1(initPos + offset * Offset * 2, initRot + .05f);
            Bullets.Add(k);

            baseBullet l = new laser_T1(initPos - offset * Offset * 2, initRot - .05f);
            Bullets.Add(l);

        }

        public laser_T5(Vector2 initPos, float initRot, bool player)
           : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Initialize();


            Sprite = Art.Laser;
            scale = 1.5f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Damage = 25f;
            Offset = 12;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            baseBullet i = new laser_T2(initPos + offset * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(i);

            baseBullet j = new laser_T2(initPos - offset * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(j);


            baseBullet k = new laser_T1(initPos + offset * Offset * 2, initRot + .05f, IsPlayer);
            Bullets.Add(k);

            baseBullet l = new laser_T1(initPos - offset * Offset * 2, initRot - .05f, IsPlayer);
            Bullets.Add(l);

        }

        public override void Initialize()
        {
            Sprite = Art.Laser;
            Penetrates = true;

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprtieBatch)
        {
            base.Draw(sprtieBatch);
        }
    }
    #endregion

    #region Missiles

    public class missile_T1 : baseBullet
    {
        public missile_T1(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Missile;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 20f;
            speed = 7;
        }

        public missile_T1(Vector2 initPos, float initRot, bool player)
         : base(initPos, initRot)
        {
            IsPlayer = player;
            Sprite = Art.Missile;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 20f;
            speed = 7;
        }

    }

    public class missile_T2 : missile_T1
    {
        public missile_T2(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            speed = 7;
        }

        public missile_T2(Vector2 initPos, float initRot, bool player)
           : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            speed = 7;
        }

    }

    public class missile_T3 : missile_T2
    {
        public missile_T3(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            Offset = 12;
            speed = 7;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            Bullets.Add(new missile_T1(initPos + offset * Offset, initRot + .05f));
            Bullets.Add(new missile_T1(initPos - offset * Offset, initRot - .05f));

        }

        public missile_T3(Vector2 initPos, float initRot, bool player)
        : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            Offset = 12;
            speed = 7;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));


            Bullets.Add(new missile_T1(initPos + offset * Offset, initRot + .05f, IsPlayer));
            Bullets.Add(new missile_T1(initPos - offset * Offset, initRot - .05f, IsPlayer));

        }
    }

    public class missile_T4 : missile_T3
    {
        public missile_T4(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            Offset = 12;
            speed = 7;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

            Bullets.Add(new missile_T1(initPos + offset * Offset, initRot + .05f));
            Bullets.Add(new missile_T1(initPos - offset * Offset, initRot - .05f));

            Bullets.Add(new missile_mini(initPos + offset * Offset * 2, initRot + .05f));
            Bullets.Add(new missile_mini(initPos - offset * Offset * 2, initRot - .05f));

        }

        public missile_T4(Vector2 initPos, float initRot, bool player)
         : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            Offset = 12;
            speed = 7;

            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

            Bullets.Add(new missile_T1(initPos + offset * Offset, initRot + .05f, IsPlayer));
            Bullets.Add(new missile_T1(initPos - offset * Offset, initRot - .05f, IsPlayer));

            Bullets.Add(new missile_mini(initPos + offset * Offset * 2, initRot + .05f, IsPlayer));
            Bullets.Add(new missile_mini(initPos - offset * Offset * 2, initRot - .05f, IsPlayer));

        }

    }

    public class missile_T5 : missile_T3
    {
        public missile_T5(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            Offset = 12;
            speed = 7;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));



            baseBullet i = new missile_T1(initPos + offset * Offset, initRot + .05f);
            Bullets.Add(i);


            baseBullet j = new missile_T1(initPos - offset * Offset, initRot - .05f);
            Bullets.Add(j);



            baseBullet k = new missile_track(initPos + offset * Offset * 2, initRot + .05f);
            Bullets.Add(k);


            baseBullet l = new missile_track(initPos - offset * Offset * 2, initRot - .05f);
            Bullets.Add(l);


        }

        public missile_T5(Vector2 initPos, float initRot, bool player)
           : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.Missile2;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 30f;
            Offset = 12;
            speed = 7;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));



            baseBullet i = new missile_track(initPos + offset * Offset, initRot + .05f, IsPlayer);
            Bullets.Add(i);


            baseBullet j = new missile_track(initPos - offset * Offset, initRot - .05f, IsPlayer);
            Bullets.Add(j);



            baseBullet k = new missile_track(initPos + offset * Offset * 2, initRot + .05f, IsPlayer);
            Bullets.Add(k);


            baseBullet l = new missile_track(initPos - offset * Offset * 2, initRot - .05f, IsPlayer);
            Bullets.Add(l);


        }
    }

    public class missile_mini : baseBullet
    {
        public missile_mini(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.MiniMissile;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 15f;

        }

        public missile_mini(Vector2 initPos, float initRot, bool player)
           : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.MiniMissile;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 15f;

        }
    }

    public class missile_track : baseBullet
    {
        int destructTimer = 60;

        Random _rand = new Random();

        public Vector2 Direction;

        public missile_track(Vector2 initPos, float initRot)
            : base(initPos, initRot)
        {
            Sprite = Art.MiniMissile;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;
            damage = 15f;

        }


        public missile_track(Vector2 initPos, float initRot, bool player)
          : base(initPos, initRot, player)
        {
            IsPlayer = player;
            Sprite = Art.MiniMissile;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            IsAOE = true;

            damage = 20f;

        }

        // for towers
        public missile_track(Vector2 initPos, float initRot, Texture2D s)
            : base(initPos, initRot)
        {
            Sprite = s;
            scale = 1f;
            Radius = (Sprite.Width / 2.2f) * scale;
            Penetrates = false;
            damage = 15f;
            IsFire = true;
        }


        public Vector2 FindNearestEnemy()
        {

            Vector2 pos = Vector2.Zero;

            if (!EnemyManager.isUpdating)
                foreach (Enemy e in EnemyManager.enemies)
                {
                    if (Vector2.DistanceSquared(Location, e.Location) < Vector2.DistanceSquared(Location, pos))
                    {
                        pos = e.Location;
                    }
                }
            return pos;

        }

        public override void Update(GameTime gameTime)
        {
            if (destructTimer > 0)
                destructTimer--;
            else
                Bullets.Remove(this);

            if (!GameRoot.Instance.GraphicsDevice.Viewport.Bounds.Contains(new Point((int)Location.X, (int)Location.Y)))
            {
                exists = false;
                Die();
                return;
            }



            if (EnemyManager.enemies.Count > 0)
            {
                Rotation = (float)Math.Atan2(((double)FindNearestEnemy().Y - location.Y), ((double)FindNearestEnemy().X - location.X)) - (float)(Math.PI / 2);

                Direction = FindNearestEnemy() - location;

                Direction.Normalize();

                location += Direction * speed;
            }
            else
            {
                Vector2 velocity = new Vector2((float)Math.Cos(Rotation - (Math.PI / 2)), (float)Math.Sin(Rotation - (Math.PI / 2)));

                Location += velocity * speed;

            }

        }
    }

    #endregion

    #endregion
}

