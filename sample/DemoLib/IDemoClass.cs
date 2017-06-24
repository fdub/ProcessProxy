using System;

namespace DemoLib
{
    public interface IDemoClass : IDisposable
    {
        string TypeName { get; }

        string GetGreeting();
        void ShowMessage(string message);
        int Add(int a, int b);
    }
}