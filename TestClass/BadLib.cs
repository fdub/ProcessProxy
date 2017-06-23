using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClass
{
    public class BadLib : IBadLib
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Info from BadLib");
        }

        public string GreetMe()
        {
            return "Hey dude!";
        }
    }
}
