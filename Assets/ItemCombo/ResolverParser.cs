using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ItemCombo.Resolver;
using UnityEditor;

namespace Assets.ItemCombo
{
    public static class ResolverParser
    {
        public static IResolver FromRPN(string original)
        {
            var stack = new Stack<IResolver>();
            var originalArray = original.Split(' ');

            foreach (var word in originalArray)
            {
                if (string.IsNullOrEmpty(word))
                {
                    throw new ArgumentException("入力文字列が空です");
                }

                var first = word[0];

                switch (first)
                {
                    case '&': // AND
                        {
                            var p1 = stack.Pop();
                            var p2 = stack.Pop();
                            stack.Push(new AndResolver(p1, p2));
                        }
                        break;
                    case '|': // OR
                        {
                            var p1 = stack.Pop();
                            var p2 = stack.Pop();
                            stack.Push(new OrResolver(p1, p2));
                        }
                        break;
                    case '!': // NOT
                        {
                            var p1 = stack.Pop();
                            stack.Push(new NotResolver(p1));
                        }
                        break;
                    case '+': //Duplicate
                        var num2 = int.Parse(word.Substring(1));
                        stack.Push(new CountResolver(num2, stack.Pop()));
                        break;

                    default:
                        stack.Push(new SingleResolver(word));
                        break;
                }

            }

            //最後はStackの長さが1になるはず
            if (stack.Count != 1)
            {
                throw new ArgumentException(string.Format("Resolverの生成に失敗しました stackのあまり:{0}, {1}", stack.Count, originalArray));
            }

            return stack.Pop();
        }



        public static IResolver From(string[] originals, int start, int end)
        {
//            var s = "";
//            for (int i = start; i < end; i++)
//            {
//                s += (originals[i] + " ");
//            }
//            UnityEngine.Debug.Log(s);


            List<IResolvable> _resolvables = new List<IResolvable>();

            for (int i = start; i < end; i++)
            {
                var current = originals[i];
                switch (current[0])
                {
                    case '+': //OR
                        _resolvables.Add(new OrResolvable());
                        break;
                    case '*': //AND
                        _resolvables.Add(new AndResolvable());
                        break;
                    case '(': //カッコ開き

                        int first = i;
                        int last = i;

                        int count = 0;
                        //かっこの終わりを探す
                        for (int j = first; j < end; j++)
                        {
                            var c = originals[j][0];
                            if (c == '(')
                            {
                                count++;
                                continue;
                            }
                            if (c == ')')
                            {
                                count--;
                                if (count == 0)
                                {
                                    last = j;
                                    break;
                                }
                            }
                        }

                        if (last > first)
                        {
                            _resolvables.Add(new FromStringResolvable(originals, first + 1, last));
                            i = last;
                        }

                        continue;
                    default:
                        _resolvables.Add(new FixedResolvable(new SingleResolver(current)));
                        continue;
                }
            }
//
//            foreach (var resolvable in _resolvables)
//            {
//                UnityEngine.Debug.Log(resolvable);
//            }

            int n = 0;
            while (_resolvables.Count > 1)
            {
                n++;
                if (n > 100) { throw new Exception("Failed resolve"); }

                for (var i = 0; i < _resolvables.Count; i++)
                {
                    var current = _resolvables[i];

                    if (current is FixedResolvable)
                    {
                        continue;
                    }

                    if (current is OrResolvable)
                    {
                        var o = (OrResolvable)current;
                        o.Register(_resolvables[i - 1], _resolvables[i + 1]);
                        _resolvables.RemoveRange(i - 1, 3);
                        _resolvables.Insert(i - 1, new FixedResolvable(o.Create()));
                        i--;
                        continue;
                    }
                    if (current is AndResolvable)
                    {
                        var o = (AndResolvable)current;
                        o.Register(_resolvables[i - 1], _resolvables[i + 1]);
                        _resolvables.RemoveRange(i - 1, 3);
                        _resolvables.Insert(i - 1, new FixedResolvable(o.Create()));
                        i--;
                        continue;
                    }

                    if (current is FromStringResolvable)
                    {
                        _resolvables.RemoveAt(i);
                        _resolvables.Insert(i, new FixedResolvable(current.Create()));
                    }
                }
            }

            return _resolvables[0].Create();
        }

        public interface IResolvable
        {
            IResolver Create();
        }

        public struct FixedResolvable : IResolvable
        {
            private readonly IResolver _resolver;

            public FixedResolvable(IResolver resolver)
            {
                _resolver = resolver;
            }

            public IResolver Create()
            {
                return _resolver;
            }
        }

        public struct OrResolvable : IResolvable
        {
            private IResolvable _a;
            private IResolvable _b;

            public void Register(IResolvable a, IResolvable b)
            {
                _a = a;
                _b = b;
            }

            public IResolver Create()
            {
                return new OrResolver(_a.Create(), _b.Create());
            }
        }

        public struct AndResolvable : IResolvable
        {
            private IResolvable _a;
            private IResolvable _b;

            public void Register(IResolvable a, IResolvable b)
            {
                _a = a;
                _b = b;
            }

            public IResolver Create()
            {
                return new AndResolver(_a.Create(), _b.Create());
            }
        }

        public struct FromStringResolvable : IResolvable
        {
            private readonly string[] _array;
            private int _start;
            private int _end;

            public FromStringResolvable(string[] array, int start, int end)
            {
                _array = array;
                _start = start;
                _end = end;
            }

            public IResolver Create()
            {
                return ResolverParser.From(_array, _start, _end);
            }

        }
    }
}