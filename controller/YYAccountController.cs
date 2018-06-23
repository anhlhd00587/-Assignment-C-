using System;
using System.Collections.Generic;
using YangYesterday.entity;
using YangYesterday.model;
using YangYesterday.utility;
using static YangYesterday.Program;

namespace YangYesterday.controller
{
    public class YYAccountController
    {
     private static YYAccountModel model = new YYAccountModel();
        public bool Register()
        {
            YYAccount  account = GetAccountInformation();
            Dictionary<string, string> errors =  account.CheckValidate();
            if (errors.Count > 0)
            {
                Console.WriteLine("Please fix errros below and try again.");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return false;
            }
            else
            {
                // Lưu vào database.
                account.EncryptPassword();
                model.Save(account);
                return true;
            }
        }
        public bool Login()
        {
            Console.WriteLine("----------------LOGIN INFORMATION----------------");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            YYAccount existingAccount = model.GetByUsername(username);
            if (existingAccount == null)
            {                
                return false;
            }

            if (!existingAccount.CheckEncryptedPassword(password))
            {                
                return false;
            }

            Program.currentLoggedInYyAccount = existingAccount;
            return true;    
        }
        private YYAccount GetAccountInformation()
        {
            Console.WriteLine("----------------REGISTER INFORMATION----------------");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Confirm Password: ");
            var cpassword = Console.ReadLine();
            Console.WriteLine("Balance: ");
            var balance = Utility.GetDecimalNumber();
            Console.WriteLine("Identity Car: ");
            var identityCar = Console.ReadLine();
            Console.WriteLine("Full Name: ");
            var fullName = Console.ReadLine();
            Console.WriteLine("Birthday: ");
            var birthday = Console.ReadLine();
            Console.WriteLine("Gender (1. Male |2. Female| 3.Others): ");
            var gender = Utility.GetInt32Number();
            Console.WriteLine("Email: ");
            var email = Console.ReadLine();
            Console.WriteLine("Phone Number: ");
            var phoneNumber = Console.ReadLine();
            Console.WriteLine("Address: ");
            var address = Console.ReadLine();
            var acc = new YYAccount()
            {
                Username = username,
                Password = password,
                Cpassword = cpassword,
                IdentityCar = identityCar,
                Gender = gender,
                Balance = balance,
                Address = address,
                Dob = birthday,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber
            };
            return acc;
        }

        public void ShowAccountInformation()
        {
           var currentAccount = model.GetByUsername(Program.currentLoggedInYyAccount.Username);
            if (currentAccount  == null)
            {
                Program.currentLoggedInYyAccount = null;
                Console.WriteLine("Sai thông tin tài khoản phải bị khóa.");
                return;
            }
            Console.WriteLine("Số tài khoản :");
            Console.WriteLine(Program.currentLoggedInYyAccount.AccountNumber);
            Console.WriteLine("Số dư hiện tại (vnd):");
            Console.WriteLine(Program.currentLoggedInYyAccount.Balance);
        }
        
        /*
         * Tiến hành chuyển khoản, mặc định là trong ngân hàng.
         * 1. Yêu cầu nhập số tài khoản cần chuyển.
         *     1.1. Xác minh thông tin tài khoản và hiển thị tên người cần chuyển.
         * 2. Nhập số tiền cần chuyển.
         *     2.1. Kiểm tra số dư tài khoản.
         * 3. Nhập nội dung chuyển tiền.
         *     3.1 Xác nhận nội dung chuyển tiền.
         * 4. Thực hiện chuyển tiền.
         *     4.1. Mở transaction. Mở block try catch.
         *     4.2. Trừ tiền người gửi.
         *         4.2.1. Lấy thông tin tài khoản gửi tiền một lần nữa. Đảm bảo thông tin là mới nhất.
         *         4.2.2. Kiểm tra lại một lần nữa số dư xem có đủ tiền để chuyển không.
         *             4.2.2.1. Nếu không đủ thì rollback.
         *             4.2.2.2. Nếu đủ thì trừ tiền và update vào bảng `accounts`.
         *     4.3. Cộng tiền người nhận.
         *         4.3.1. Lấy thông tin tài khoản nhận, đảm bảo tài khoản không bị khoá hoặc inactive.
         *         4.3.1.1. Nếu ok thì update số tiền cho người nhận.
         *         4.3.1.2. Nếu không ok thì rollback.
         *     4.4. Lưu lịch sử giao dịch.
         *     4.5. Kiểm tra lại trạng thái của 3 câu lệnh trên.
         *         4.5.1. Nếu cả 3 cùng thành công thì commit transaction.
         *         4.5.2. Nếu bất kỳ một câu lệnh nào bị lỗi thì rollback.
         *     4.x. Đóng, commit transaction.
         */

        public void Transfer()
        {
            Console.WriteLine("------------------Transerf Information-------------------");
            var accountNumber = "8ae77d16-4ef7-4e3b-8620-89a794030923";
            var account = model.GetByAccountNumber(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Invalid account info ");
                return;
            }

            Console.WriteLine("You are doing transaction with account :" +account.FullName);
            Console.WriteLine("enter amout to transfer");
            var amount = Utility.GetDecimalNumber();
            if (amount > account.Balance)
            
            {
                Console.WriteLine("Amout not enough to perfom transaction:");
                return;
            }

            amount += account.Balance;

            Console.WriteLine("Please enter message conten:");
            var content = Console.ReadLine();
            Console.WriteLine("Are you sure you want to make a transaction with your account ? (y/n)");
            var choice = Console.ReadLine();

            if (choice.Equals("n"))
            {
                return;
            }

            var historyTransaction = new YYTransaction()
            {
                Id = Guid.NewGuid().ToString(),
                Type = 4,
                Content = content,
                Amount = amount,
                SenderAccountNumber = Program.currentLoggedInYyAccount.AccountNumber,
                ReceiverAccountNumber = account.AccountNumber,
                Status = 2,
            };
            if (model.TransferAmount(Program.currentLoggedInYyAccount, historyTransaction))
                       {
                           Console.WriteLine("Transaction success!");
                       }
                       else
                       {
                           Console.WriteLine("Transaction fails, please try again!");
                       }
           
           
                       Program.currentLoggedInYyAccount = model.GetByUsername(Program.currentLoggedInYyAccount.Username);
                       Console.WriteLine("Current balance: " + Program.currentLoggedInYyAccount.Balance);
                       Console.WriteLine("Press enter to continue!");
                       Console.ReadLine();
        }
    }
}