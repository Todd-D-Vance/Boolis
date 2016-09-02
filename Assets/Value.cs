using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace Assets
{
    public class Value
    {
        public static Value Empty = new Value("Empty", -1);
        public static Value True = new Value("Empty", 1);
        public static Value False = new Value("Empty", 0);
        public static Value Poison = new Value("Empty", -2);

        public static Value Get(int? index)
        {
            if (index > 0)
            {
                return Value.True;
            }
            else if (index == 0)
            {
                return Value.False;
            }
            else if (index == -1 || index == null)
            {
                return Value.Empty;
            }
            else
            {
                return Value.Poison;
            }
        }

        private readonly string name;
        private readonly int index;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        private Value(string name, int index)
        {
            this.name = name;
            this.index = index;
        }

        public Value Connect(Value other)
        {
            if (this == other || other == Empty || this == Poison)
            {
                return this;
            }
            else if (this == Empty || other == Poison)
            {
                return other;
            }
            else
            {
                return Poison;
            }
        }

        public Value And(Value other)
        {
            if (this == other || (other == Empty && this == False) || this == Poison)
            {
                return this;
            }
            else if ((this == Empty && other == False) || other == Poison)
            {
                return other;
            }
            else if (this == Empty || other == Empty)
            {
                return Empty;
            }
            else
            {
                return False;
            }
        }

        public Value Or(Value other)
        {
            if (this == other || (other == Empty && this == True) || this == Poison)
            {
                return this;
            }
            else if ((this == Empty && other == True) || other == Poison)
            {
                return other;
            }
            else if (this == Empty || other == Empty)
            {
                return Empty;
            }
            else
            {
                return True;
            }
        }

        public Value Not()
        {
            if (this == True)
            {
                return False;
            }
            else if (this == False)
            {
                return True;
            }
            else
            {
                return this;
            }
        }

        public override bool Equals(object obj)
        {
            return ((Value)obj).index == this.index;
        }

        public override int GetHashCode()
        {
            return index;
        }

        public override string ToString()
        {
            return name;
        }

        #region tests
        private static void ae(Value a, Value b)
        {
            if (!a.Equals(b))
            {
                throw new AssertionException("Unequal values: " + a + ", " + b, "Unequal values: " + a + ", " + b);
            }
        }
        public static void test()
        {
            ae(Empty.Connect(Empty), Empty);
            ae(Empty.Connect(True), True);
            ae(Empty.Connect(False), False);
            ae(Empty.Connect(Poison), Poison);
            ae(True.Connect(Empty), True);
            ae(True.Connect(True), True);
            ae(True.Connect(False), Poison);
            ae(True.Connect(Poison), Poison);
            ae(False.Connect(Empty), False);
            ae(False.Connect(True), Poison);
            ae(False.Connect(False), False);
            ae(False.Connect(Poison), Poison);
            ae(Poison.Connect(Empty), Poison);
            ae(Poison.Connect(True), Poison);
            ae(Poison.Connect(False), Poison);
            ae(Poison.Connect(Poison), Poison);

            ae(Empty.And(Empty), Empty);
            ae(Empty.And(True), Empty);
            ae(Empty.And(False), False);
            ae(Empty.And(Poison), Poison);
            ae(True.And(Empty), Empty);
            ae(True.And(True), True);
            ae(True.And(False), False);
            ae(True.And(Poison), Poison);
            ae(False.And(Empty), False);
            ae(False.And(True), False);
            ae(False.And(False), False);
            ae(False.And(Poison), Poison);
            ae(Poison.And(Empty), Poison);
            ae(Poison.And(True), Poison);
            ae(Poison.And(False), Poison);
            ae(Poison.And(Poison), Poison);

            ae(Empty.Or(Empty), Empty);
            ae(Empty.Or(True), True);
            ae(Empty.Or(False), Empty);
            ae(Empty.Or(Poison), Poison);
            ae(True.Or(Empty), True);
            ae(True.Or(True), True);
            ae(True.Or(False), True);
            ae(True.Or(Poison), Poison);
            ae(False.Or(Empty), Empty);
            ae(False.Or(True), True);
            ae(False.Or(False), False);
            ae(False.Or(Poison), Poison);
            ae(Poison.Or(Empty), Poison);
            ae(Poison.Or(True), Poison);
            ae(Poison.Or(False), Poison);
            ae(Poison.Or(Poison), Poison);

            ae(Empty.Not(), Empty);
            ae(True.Not(), False);
            ae(False.Not(), True);
            ae(Poison.Not(), Poison);

        }
        #endregion
    }
}