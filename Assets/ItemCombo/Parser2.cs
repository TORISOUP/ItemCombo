using System;
using System.Text;
using Assets.ItemCombo.Resolver;

namespace Assets.ItemCombo
{
    public class Source
    {
        private string str;
        private int pos;

        public Source(string str)
        {
            this.str = str;
        }

        public int Peek()
        {
            if (pos < str.Length)
            {
                return str[pos];
            }
            return -1;
        }

        public void Next()
        {
            ++pos;
        }
    }

    public class Parser : Source
    {
        public Parser(string str) : base(str)
        {
        }

        private bool IsNormalChar(char c)
        {
            return !(c == '+' || c == '*' || c == '!' || c == ' ');
        }

        public IResolver GetSingle()
        {
            var sb = new StringBuilder();
            int ch = 0;
            while ((ch = Peek()) >= 0 && IsNormalChar((char)ch))
            {
                sb.Append((char)ch);
                Next();
            }
            return new SingleResolver(sb.ToString());
        }

        public int GetNumber()
        {
            var sb = new StringBuilder();
            int ch = 0;
            while ((ch = Peek()) >= 0 && char.IsDigit((char)ch))
            {
                sb.Append((char)ch);
                Next();
            }
            return int.Parse(sb.ToString());
        }

        public IResolver Expr()
        {
            var x = Term();
            var c = x;

            while (true)
            {
                switch (Peek())
                {
                    case '+':
                        Next();
                        c = new OrResolver(c, Term());
                        continue;
                }
                break;
            }
            return c;
        }

        protected IResolver Term()
        {
            var c = Word();

            while (true)
            {
                switch (Peek())
                {
                    case '*':
                        Next();
                        c = new AndResolver(c, Count());
                        continue;
                }
                break;
            }
            return c;
        }

        protected IResolver Count()
        {
            var c = Word();
            if (Peek() == '^')
            {
                Next();
                Spaces();
                return new CountResolver(GetNumber(), c);
            }
            return c;
        }

        protected IResolver Word()
        {
            Spaces();
            if (Peek() == '!')
            {
                Next();
                return new NotResolver(Word());
            }
            return Factor();
        }

        protected IResolver Factor()
        {
            IResolver ret;
            Spaces();
            var p = Peek();
            if (p == '(')
            {
                Next();
                ret = Expr();
                if (Peek() == ')')
                {
                    Next();
                }
            }
            else
            {
                ret = GetSingle();
            }
            Spaces();
            return ret;
        }


        protected void Spaces()
        {
            while (Peek() == ' ')
            {
                Next();
            }
        }
    }
}