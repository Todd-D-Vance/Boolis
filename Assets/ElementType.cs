using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Assets
{
    public class ElementType
    {

        private HashSet<Element> elements = new HashSet<Element>();

        public readonly string name;

        public SuperBool[] terminals = { SuperBool.NoValue, SuperBool.NoValue, SuperBool.NoValue, SuperBool.NoValue };

        public SuperBool top
        {
            get
            {
                return terminals[0];
            }
            set
            {
                if (terminals[0] == SuperBool.NoValue || terminals[0] == value || value == SuperBool.Poison)
                {
                    terminals[0] = value;
                }
                else
                {
                    terminals[0] = SuperBool.Poison;
                }
                Propagate();
            }
        }

        public SuperBool right
        {
            get
            {
                return terminals[1];
            }
            set
            {
                if (terminals[1] == SuperBool.NoValue || terminals[1] == value || value == SuperBool.Poison)
                {
                    terminals[1] = value;
                }
                else
                {
                    terminals[1] = SuperBool.Poison;
                }
                Propagate();
            }
        }

        public SuperBool bottom
        {
            get
            {
                return terminals[2];
            }
            set
            {
                if (terminals[2] == SuperBool.NoValue || terminals[2] == value || value == SuperBool.Poison)
                {
                    terminals[2] = value;
                }
                else
                {
                    terminals[2] = SuperBool.Poison;
                }
                Propagate();
            }
        }

        public SuperBool left
        {
            get
            {
                return terminals[3];
            }
            set
            {
                if (terminals[3] == SuperBool.NoValue || terminals[3] == value || value == SuperBool.Poison)
                {
                    terminals[3] = value;
                }
                else
                {
                    terminals[3] = SuperBool.Poison;
                }
                Propagate();
            }
        }

        public ElementType(string name)
        {
            this.name = name;
        }

        public void Register(Element e)
        {
            elements.Add(e);
        }

        public void Deregister(Element e)
        {
            elements.Remove(e);
        }

        private void Notify()
        {
            foreach(Element e in elements)
            {
                e.SetSprite(this);
            }
        }

        public void Propagate()
        {
            switch (name)
            {

                case "straight":
                    top = SuperBool.False;
                    bottom = SuperBool.True;
                    if (left != SuperBool.NoValue)
                    {
                        right = left;
                    }
                    else if (right != SuperBool.NoValue)
                    {
                        left = right;
                    }
                    break;

                case "bend":
                    bottom = SuperBool.False;
                    right = SuperBool.True;
                    if (left != SuperBool.NoValue)
                    {
                        top = left;
                    }
                    else if (top != SuperBool.NoValue)
                    {
                        left = top;
                    }
                    break;

                case "and":
                    right = SuperBool.True;
                    if (left == SuperBool.True || left == SuperBool.Poison)
                    {
                        top = left;
                        bottom = left;
                    }
                    else if (top == SuperBool.False || bottom == SuperBool.False)
                    {
                        left = SuperBool.False;
                    }
                    else if (top == SuperBool.True && bottom == SuperBool.True)
                    {
                        left = top;
                    }
                    else if (top == SuperBool.Poison || bottom == SuperBool.Poison)
                    {
                        left = SuperBool.Poison;
                    }
                    else if (left == SuperBool.False && top == SuperBool.True)
                    {
                        bottom = left;
                    }
                    else if (left == SuperBool.False && bottom == SuperBool.True)
                    {
                        top = left;
                    }
                    break;

                case "not":
                    top = SuperBool.True;
                    bottom = SuperBool.False;
                    if (left != SuperBool.NoValue)
                    {
                        if (right == SuperBool.True)
                        {
                            left = SuperBool.False;
                        }
                        else if (right == SuperBool.False)
                        {
                            left = SuperBool.True;
                        }
                        else
                        {
                            left = right;
                        }
                    }
                    else if (right != SuperBool.NoValue)
                    {
                        if (left == SuperBool.True)
                        {
                            right = SuperBool.False;
                        }
                        else if (left == SuperBool.False)
                        {
                            right = SuperBool.True;
                        }
                        else
                        {
                            right = left;
                        }
                    }
                    break;

                case "or":
                    left = SuperBool.True;
                    if (right == SuperBool.False || right == SuperBool.Poison)
                    {
                        top = right;
                        bottom = right;
                    }
                    else if (top == SuperBool.False && bottom == SuperBool.False)
                    {
                        right = top;
                    }
                    else if (top == SuperBool.True || bottom == SuperBool.True)
                    {
                        right = SuperBool.True;
                    }
                    else if (top == SuperBool.Poison || bottom == SuperBool.Poison)
                    {
                        right = SuperBool.Poison;
                    }
                    else if (right == SuperBool.True && top == SuperBool.False)
                    {
                        bottom = right;
                    }
                    else if (right == SuperBool.True && bottom == SuperBool.False)
                    {
                        top = right;
                    }
                    break;

                case "notbend":
                    right = SuperBool.True;
                    bottom = SuperBool.False;
                    if (left != SuperBool.NoValue)
                    {
                        if (left == SuperBool.True)
                        {
                            top = SuperBool.False;
                        }
                        else if (left == SuperBool.False)
                        {
                            top = SuperBool.True;
                        }
                        else
                        {
                            top = left;
                        }
                    }
                    else if (top != SuperBool.NoValue)
                    {
                        if (top == SuperBool.True)
                        {
                            left = SuperBool.False;
                        }
                        else if (top == SuperBool.False)
                        {
                            left = SuperBool.True;
                        }
                        else
                        {
                            left = top;
                        }
                    }
                    break;

                case "constant":
                    top = bottom = SuperBool.False;
                    left = right = SuperBool.True;
                    break;

                case "poison":
                    top = bottom = left = right = SuperBool.Poison;
                    break;


                default:
                    throw new AssertionException("Case \"" + name + "\" not handled", "Case \"" + name + "\" not handled");
            }
            Notify();
        }

    }
}