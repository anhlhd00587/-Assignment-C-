using System;
using YangYesterday.controller;
using YangYesterday.utility;

namespace YangYesterday.view
{
    public class ApplicationView
    {
        private readonly YYAccountController controller = new YYAccountController();

        // Hiển thị menu chính của chương trình.
        public void GenerateDefaultMenu()
        {
            while (true)
            {
                Console.WriteLine("--------------WELCOME TO YANG_TOMORROW BANK--------------");
                Console.WriteLine("1. Register for free.");
                Console.WriteLine("2. Login.");
                Console.WriteLine("3. Exit.");
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine("Please enter you choice (1|2|3): ");
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        Console.WriteLine(controller.Register()
                            ? "Register success!"
                            : "Register fails. Please try again later.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        break;
                    case 2:
                        Console.WriteLine(controller.Login()
                            ? "Login success! Welcome back " + Program.currentLoggedInYyAccount.FullName + "!"
                            : "Login fails. Please try again later.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine("See you later.");
                        Environment.Exit(1);
                        break;
                }

                if (Program.currentLoggedInYyAccount != null)
                {
                    break;
                }
            }
        }
        
        // Hiển thị menu chính của chương trình.
        public void GenerateCustomerMenu()
        {
            while (true)
            {
                Console.WriteLine("--------------YANG_TOMORROW BANK CUSTOMER MENU--------------");
                Console.WriteLine("Welcome back " + Program.currentLoggedInYyAccount.FullName);
                Console.WriteLine("1. Check information.");
                Console.WriteLine("2. Withdraw.");
                Console.WriteLine("3. Deposit.");
                Console.WriteLine("4. Transfer.");
                Console.WriteLine("5. Transaction history.");
                Console.WriteLine("6. Logout.");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Please enter you choice (1|2|3|4|5|6): ");
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        controller.ShowAccountInformation();
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        break;
                    case 2:                       
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        break;
                    case 3:
                       
                    case 4:
                        controller.Transfer();
                        break;
                    case 5:
                        break;
                    case 6:
                        Console.WriteLine("See you later.");
                        Environment.Exit(1);
                        break;
                }

                if (Program.currentLoggedInYyAccount == null)
                {
                    break;
                }
            }
        }
        
        
    }
}