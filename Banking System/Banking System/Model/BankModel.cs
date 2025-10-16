namespace Banking_System.Model
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public List<Account> Accounts { get; set; }
    }

    public class Account
    {
        public int AccountID { get; set; }
        public int CustomerID { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Transaction> Transactions { get; set; }
        public Customer Customer { get; set; }  
    }

    public class Transaction
    {
        public int TransactionID { get; set; }
        public int AccountID { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? TargetAccountID { get; set; } // nullable for transfers
    }

    public class Branch
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
    }

}
