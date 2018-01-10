using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ItemCombo.Resolver;
using UnityEditor;

namespace Assets.ItemCombo
{
    public static class ResolverParser
    {
        public static IResolver Parse(string original)
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
                    case '+': // Unique
                        var num1 = int.Parse(word.Substring(1));
                        var a3 = stack.ToArray();
                        stack.Clear();
                        stack.Push(new UniqueSelectResolver(num1, a3.Cast<SingleResolver>().ToArray()));
                        break;
                    case '~': //Duplicate
                        var num2 = int.Parse(word.Substring(1));
                        stack.Push(new DuplicateResolver(num2, stack.Pop() as SingleResolver));
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
    }
}