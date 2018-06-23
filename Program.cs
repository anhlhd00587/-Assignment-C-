using System;
using YangYesterday.entity;
using YangYesterday.view;

namespace YangYesterday
{
    class Program
    {
        public static YYAccount currentLoggedInYyAccount;
        
        
        

        static void Main(string[] args)
        {
            ApplicationView view = new ApplicationView();
            while (true)
            {
                if (Program.currentLoggedInYyAccount != null)
                {
                    view.GenerateCustomerMenu();
                }
                else
                {
                    view.GenerateDefaultMenu();
                }
            }
        }
    }
}