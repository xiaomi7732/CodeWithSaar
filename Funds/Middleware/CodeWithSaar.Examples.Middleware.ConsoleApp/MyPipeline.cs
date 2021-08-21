using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeWithSaar.Examples.Middleware
{
    public class MyPipeline
    {
        private readonly List<Func<Func<Task>, Func<Task>>> _components = new();

        [Obsolete("Consider the easier way by using 'Use' instead.", error: false)]
        public MyPipeline Add(Func<Func<Task>, Func<Task>> component)
        {
            _components.Add(component);
            return this;
        }

        public MyPipeline Use(Func<Func<Task>, Task> current)
        {
            Func<Func<Task>, Func<Task>> component = (next) =>
            {
                return () => current.Invoke(next);
            };
            _components.Add(component);
            return this;
        }

        public Func<Task> Run(Func<Task> last)
        {
            for (int i = _components.Count - 1; i >= 0; i--)
            {
                last = _components[i].Invoke(last);
            }
            return last;
        }
    }
}