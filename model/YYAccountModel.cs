using System;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;
using MySql.Data.MySqlClient;
using YangYesterday.entity;

namespace YangYesterday.model
{
    public class YYAccountModel
    {
        public Boolean Save(YYAccount account)
        {
            DbConnection.Instance().OpenConnection();
            string queryString = "insert into `accounts` " +
                                 "(accountNumber, username, password, balance, identityCar, fullName, " +
                                 "email, phoneNumber, address, dob, gender, salt)" +
                                 " values " +
                                 "(@accountNumber, @username, @password, @balance, @identityCar, @fullName, " +
                                 "@email, @phoneNumber, @address, @dob, @gender, @salt)";
            MySqlCommand cmd = new MySqlCommand(queryString, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@accountNumber", account.AccountNumber);
            cmd.Parameters.AddWithValue("@username", account.Username);
            cmd.Parameters.AddWithValue("@password", account.Password);
            cmd.Parameters.AddWithValue("@balance", account.Balance);
            cmd.Parameters.AddWithValue("@identityCar", account.IdentityCar);
            cmd.Parameters.AddWithValue("@fullName", account.FullName);
            cmd.Parameters.AddWithValue("@email", account.Email);
            cmd.Parameters.AddWithValue("@phoneNumber", account.PhoneNumber);
            cmd.Parameters.AddWithValue("@address", account.Address);
            cmd.Parameters.AddWithValue("@dob", account.Dob);
            cmd.Parameters.AddWithValue("@gender", account.Gender);
            cmd.Parameters.AddWithValue("@salt", account.Salt);
            cmd.ExecuteNonQuery();
            DbConnection.Instance().CloseConnection();
            return true;
        }

        public Boolean CheckExistUsername(string username)
        {
            DbConnection.Instance().OpenConnection();
            var queryString = "select * from `accounts` where username = @username and status = 1" +
                              "";
            var cmd = new MySqlCommand(queryString, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@username", username);
            var reader = cmd.ExecuteReader();
            var isExist = reader.Read();
            DbConnection.Instance().CloseConnection();
            return isExist;
        }

        public YYAccount GetByUsername(string username)
        {
            YYAccount account = null;
            DbConnection.Instance().OpenConnection();
            var queryString = "select * from `accounts` where username = @username";
            var cmd = new MySqlCommand(queryString, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@username", username);
            var reader = cmd.ExecuteReader();
            var isExist = reader.Read();
            if (isExist)
            {
                account = new YYAccount
                {
                    AccountNumber = reader.GetString("accountNumber"),
                    Username = reader.GetString("username"),
                    Password = reader.GetString("password"),
                    Salt = reader.GetString("salt"),
                    FullName = reader.GetString("fullName"),
                    Balance = reader.GetInt32("balance")
                };
            }

            DbConnection.Instance().CloseConnection();
            return account;
        }


        public YYAccount GetByAccountNumber(string accountNumber)
        {
            YYAccount account = null;
            DbConnection.Instance().OpenConnection();
            var queryString = "select * from `accounts` where username = @accountnumber and status = 1";
            var cmd = new MySqlCommand(queryString, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@accountnumber", accountNumber);
            var reader = cmd.ExecuteReader();
            var isExist = reader.Read();
            if (isExist)
            {
                account = new YYAccount
                {
                    AccountNumber = reader.GetString("accountNumber"),
                    Username = reader.GetString("username"),
                    Password = reader.GetString("password"),
                    Salt = reader.GetString("salt"),
                    FullName = reader.GetString("fullName"),
                    Balance = reader.GetInt32("balance")
                };
            }

            DbConnection.Instance().CloseConnection();
            return account;
        }

        public bool TransferAmount(YYAccount account, YYTransaction historyTransaction)
        {
            DbConnection.Instance().OpenConnection();
            var transaction = DbConnection.Instance().Connection.BeginTransaction();

            try
            {
                // Kiểm tra số tài khoản mới nhất
                var queryBalance = "select `balance` from `account` where username = @username and status = 1";
                MySqlCommand queryBalanceCommand = new MySqlCommand(queryBalance, DbConnection.Instance().Connection);
                queryBalanceCommand.Parameters.AddWithValue("@username", account.Username);
                var balanceReader = queryBalanceCommand.ExecuteReader();
                // Không tìm thấy tài khoản tương ứng, throw lỗi.
                if (!balanceReader.Read())
                {
                    throw new TransactionException("Invalid username");
                }

                var currentBalance = balanceReader.GetDecimal("balance");
                currentBalance -= historyTransaction.Amount;
                balanceReader.Close();

                // Update số dư vào database.
                var updateAccountResult = 0;
                var queryUpdateAccountBalance =
                    "update `accounts` set balance = @balance where username = @username and status = 1";
                var cmdUpdateAccountBalance =
                    new MySqlCommand(queryUpdateAccountBalance, DbConnection.Instance().Connection);
                cmdUpdateAccountBalance.Parameters.AddWithValue("@username", account.Username);
                cmdUpdateAccountBalance.Parameters.AddWithValue("@balance", currentBalance);
                updateAccountResult = cmdUpdateAccountBalance.ExecuteNonQuery();

                // Lưu thông tin transaction vào bảng transaction.
                var insertTransactionResult = 0;
                var queryInsertTransaction = "insert into `transaction` " +
                                             "(id, fromAccountNumber, amount, content, toAccountNumber, type, status) " +
                                             "values (@id, @fromAccountNumber, @amount, @content, @toAccountNumber, @type, @status)";
                var cmdInsertTransaction =
                    new MySqlCommand(queryInsertTransaction, DbConnection.Instance().Connection);
                cmdInsertTransaction.Parameters.AddWithValue("@id", historyTransaction.Id);
                cmdInsertTransaction.Parameters.AddWithValue("@fromAccountNumber",
                    historyTransaction.SenderAccountNumber);
                cmdInsertTransaction.Parameters.AddWithValue("@amount", historyTransaction.Amount);
                cmdInsertTransaction.Parameters.AddWithValue("@content", historyTransaction.Content);
                cmdInsertTransaction.Parameters.AddWithValue("@toAccountNumber",
                    historyTransaction.ReceiverAccountNumber);
                cmdInsertTransaction.Parameters.AddWithValue("@type", historyTransaction.Type);
                cmdInsertTransaction.Parameters.AddWithValue("@status", historyTransaction.Status);
                insertTransactionResult = cmdInsertTransaction.ExecuteNonQuery();

                // Kiểm tra lại câu lệnh
                if (updateAccountResult == 1 && insertTransactionResult == 1)
                {
                    transaction.Commit();
                    return true;
                }
            }
            catch (TransactionException e)
            {
                transaction.Rollback();
                return false;
            }

            DbConnection.Instance().CloseConnection();
            return false;
        }
    }
}