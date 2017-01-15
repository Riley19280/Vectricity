using System;
using System.Collections.Generic;
using System.Linq;


namespace MyDataTypes
{

        public class Wave
        {
            public int id;

            public Spawn[] Spawns;

            public struct Spawn
            {
                public int time;
                public int a;
                public int b;
                public int c;
                public int d;
            }

            public int waitTime;
        }

        public class HighScore 
        {
           public int score;
        }

        public class KEYBOARDENABLED 
        {
            public bool enabled = false;
        }
}
