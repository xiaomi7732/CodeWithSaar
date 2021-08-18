using System;
using System.Threading.Tasks;

namespace CodeWithSaar.Examples.Middleware.DecoratorPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Middleware m1 = new Middleware(() =>
            // {
            //     Console.WriteLine("1st middleware.");
            //     return Task.CompletedTask;
            // },
            // new Middleware(() =>
            // {
            //     Console.WriteLine("2nd middleware.");
            //     return Task.CompletedTask;
            // }, new Middleware(() =>
            // {
            //     Console.WriteLine("3rd middleware.");
            //     return Task.CompletedTask;
            // }, null)));

            // await m1.InvokeAsync().ConfigureAwait(false);

            // new MiddlewareBuilder().Use(new Middleware(async ()=>{}))
            MyPipeline pipeline = new MyPipeline();
            pipeline.Use(async next =>
            {
                Console.WriteLine("Incoming 1");
                await next();
                Console.WriteLine("Outgoing 1");
            });

            pipeline.Use(async next =>
            {
                Console.WriteLine("Incoming 2");
                await next();
                Console.WriteLine("Outgoing 2");
            });

            Func<Task> lastTask = pipeline.Run(() => {
                Console.WriteLine("This is the last one. There is no next.");
                return Task.CompletedTask;
            });

            await lastTask();
        }
    }

    interface IDecorator
    {
        IDecorator Next { get; }

        Task InvokeAsync();
    }

    class Middleware : IDecorator
    {
        private readonly Func<Task> _thisTask;

        public Middleware(Func<Task> thisTask, IDecorator next)
        {
            this._thisTask = thisTask;
            Next = next;
        }

        public IDecorator Next { get; set; }

        public async Task InvokeAsync()
        {
            await _thisTask();
            Task nextTask = Next?.InvokeAsync();
            if (nextTask != null)
            {
                await nextTask;
            }
        }
    }

    class MiddlewareBuilder
    {
        private Middleware _header;

        public Middleware Build()
        {
            return _header;
        }

        public void Use(Middleware newInstance)
        {
            newInstance.Next = _header;
            _header = newInstance;
        }

    }
}
