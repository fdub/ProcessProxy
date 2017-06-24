using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClass
{
    public class BadLib : IBadLib
    {
        public string TypeName => this.GetType().Name;

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Info from BadLib");
        }

        public int Add(int a, int b) => a + b;

        public string GetGreeting()
        {
            return "Hey dude!";
        }

        public void Dispose()
        {
            Console.WriteLine("Shutting down!");
        }
    }
}
