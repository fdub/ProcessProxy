using System;
using System.Windows.Forms;

namespace DemoLib
{
    public class DemoClass : IDemoClass
    {
        public string TypeName => GetType().Name;

        public string GetGreeting() => "Hey dude!";
        public void ShowMessage(string message) => MessageBox.Show(message, "Info from DemoClass");
        public int Add(int a, int b) => a + b;

        public void Dispose()
        {
            Console.WriteLine("Shutting down!");
        }
    }
}
