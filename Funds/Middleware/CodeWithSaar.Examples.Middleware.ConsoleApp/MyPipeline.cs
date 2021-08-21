using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeWithSaar.Examples.Middleware
{
    public class MyPipeline
    {
        private readonly List<Func<Func<Task>, Func<Task>>> _components = new();

        public Func<Task> Build()
        {
            Func<Task> app = () =>
            {
                Console.WriteLine("I am the last to execute");
                return Task.CompletedTask;
            };

            for (int i = _components.Count - 1; i >= 0; i--)
            {
                app = _components[i].Invoke(app);
            }

            return app;
        }

        public MyPipeline Use(Func<Func<Task>, Task> current)
        {
            Func<Func<Task>, Func<Task>> component = (n) => {
                return ()=> current.Invoke(n);
            };
            _components.Add(component);
            return this;
        }

        public Func<Task> Run(Func<Task> last)
        {
            for(int i=_components.Count-1;i>=0;i--)
            {
                last = _components[i].Invoke(last);
            }
            return last;
        }
    }
}