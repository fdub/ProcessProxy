using System;

namespace TestClass
{
    public interface IBadLib : IDisposable
    {
        string TypeName { get; }

        void ShowMessage(string message);
        int Add(int a, int b);
        string GetGreeting();
    }
}