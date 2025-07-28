/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Game.Sample.DynamicInvokeGenerator
{
    /// <summary>
    /// 函数生成方式对比测试案例
    /// </summary>
    public static class FunctionGenerationComparison
    {
        private abstract class Animal
        {
            public static string AnimalTag = "Animal";
            public int uid;
            protected string name;
            private int age = 0;

            public int Uid => uid;
            public string Name => name;
            public int Age { get { return age; } protected set { age = value; } }

            public abstract void SayHello(Animal target);
            public abstract void SayGoodbye(Animal target);
        }

        private class Dog : Animal
        {
            public static string DogName = "people's friend";
            public int speed;
            protected string luckName;
            private int sex;
            protected int health;

            public int Speed => speed;
            public string LuckName => luckName;
            public int Sex { get { return sex; } protected set { sex = value; } }
            public int Health { get { return health; } set { health = value; } }

            public override void SayHello(Animal target)
            {
                Debugger.Warn("汪汪!");
            }

            public override void SayGoodbye(Animal target)
            {
                Debugger.Warn("汪~~");
            }
        }

        private class Hashiqi : Dog
        {
            public static string DogTag = "no anything";
            public int attack;
            private int def;

            public int Attack => attack;
            public int Def => def;

            public Hashiqi()
            {
                this.uid = 1001;
                this.name = "哈士奇";
                this.Age = 2;

                this.speed = 5;
                this.luckName = "拆家小能手";
                this.Sex = 1;
                this.health = 100;

                this.attack = 10;
                this.def = 2;
            }

            public override void SayHello(Animal target)
            {
                //Debugger.Warn("呜呜!");

                health += 1;
                ((Dog) target).Health += 2;
            }

            public override void SayGoodbye(Animal target)
            {
                //Debugger.Warn("呜~~");
                health += 1;
                ((Dog) target).Health += 2;
            }
        }

        private class Alasijia : Dog
        {
            public static string DogTag = "no anything";
            public int attack;
            private int def;

            public int Attack => attack;
            public int Def => def;

            public Alasijia()
            {
                this.uid = 1001;
                this.name = "阿拉斯加";
                this.Age = 2;

                this.speed = 5;
                this.luckName = "雪橇小王子";
                this.Sex = 1;
                this.health = 150;

                this.attack = 8;
                this.def = 4;
            }

            public override void SayHello(Animal target)
            {
                Debugger.Warn("嗷嗷!");
            }

            public override void SayGoodbye(Animal target)
            {
                Debugger.Warn("嗷~~");
            }
        }

        public static void Run()
        {
            Debugger.Info("_________________________________________________");
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            Type targetType = typeof(Hashiqi);

            Hashiqi obj = new Hashiqi();
            Alasijia obj2 = new Alasijia();

            stopwatch.Start();
            MethodInfo m1 = targetType.GetMethod("SayHello", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo m2 = targetType.GetMethod("SayGoodbye", BindingFlags.Public | BindingFlags.Instance);

            Delegate d1 = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, m1);
            Delegate d2 = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, m2);
            Delegate d3 = Delegate.Combine(d1, d2);
            stopwatch.Stop();
            Debugger.Warn($"build delegatte using elapsed time was {stopwatch.ElapsedMilliseconds}.");

            int count = 1000000;

            stopwatch.Reset();
            obj.Health = 100;
            obj2.Health = 150;
            stopwatch.Start();
            for (int n = 0; n < count; ++n)
            {
                d3.DynamicInvoke(obj2);
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}. obj's health = {obj.Health}, obj2's health = {obj2.Health}.");

            stopwatch.Reset();
            stopwatch.Start();
            ParameterExpression pe = Expression.Parameter(typeof(object));//, "self");
            UnaryExpression ue = Expression.Convert(pe, typeof(Alasijia));
            MethodCallExpression mce = Expression.Call(Expression.Constant(obj), m1, ue);
            Expression<Action<object>> e = Expression.Lambda<Action<object>>(mce, pe);
            Action<object> action1 = e.Compile();

            mce = Expression.Call(Expression.Constant(obj), m2, ue);
            e = Expression.Lambda<Action<object>>(mce, pe);
            Action<object> action2 = e.Compile();

            Action<object> action3 = (Action<object>) Delegate.Combine(action1, action2);
            stopwatch.Stop();
            Debugger.Warn($"build action using elapsed time was {stopwatch.ElapsedMilliseconds}.");

            stopwatch.Reset();
            obj.Health = 100;
            obj2.Health = 150;
            stopwatch.Start();
            for (int n = 0; n < count; ++n)
            {
                //action1.Invoke(obj2);
                //action2.Invoke(obj2);
                action3.Invoke(obj2);
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}. obj's health = {obj.Health}, obj2's health = {obj2.Health}.");

            stopwatch.Reset();
            obj.Health = 100;
            obj2.Health = 150;
            stopwatch.Start();
            for (int n = 0; n < count; ++n)
            {
                m1.Invoke(obj, new object[] { obj2 });
                m2.Invoke(obj, new object[] { obj2 });
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}. obj's health = {obj.Health}, obj2's health = {obj2.Health}.");
        }
    }
}
