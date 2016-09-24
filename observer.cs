/// <summary>
    /// 抽象主题
    /// </summary>
    public interface ISubject
    {
        void Notify();
    }

    /// <summary>
    /// 声明委托
    /// </summary>
    public delegate void CustomerEventHandler();

    /// <summary>
    /// 具体主题
    /// </summary>
    public class Customer : ISubject
    {
        private string customerState;

        // 声明一个委托事件，类型为 CustomerEventHandler
        public event CustomerEventHandler Update;

        public void Notify()
        {
            if (Update != null)
            {
                // 使用事件来通知给订阅者
                Update();
            }
        }

        public string CustomerState
        {
            get { return customerState; }
            set { customerState = value; }
        }
    }

    /// <summary>
    /// 财务，已经不需要实现抽象的观察者类，并且不用引用具体的主题
    /// </summary>
    public class Accountant
    {
        private string accountantState;

        public Accountant()
        { }

        /// <summary>
        /// 开发票
        /// </summary>
        public void GiveInvoice()
        {
            Console.WriteLine("我是会计，我来开具发票。");
            accountantState = "已开发票";
        }
    }

    /// <summary>
    /// 出纳，已经不需要实现抽象的观察者类，并且不用引用具体的主题
    /// </summary>
    public class Cashier
    {
        private string cashierState;

        public void Recoded()
        {
            Console.WriteLine("我是出纳员，我给登记入账。");
            cashierState = "已入账";
        }
    }

    /// <summary>
    /// 配送员，已经不需要实现抽象的观察者类，并且不用引用具体的主题
    /// </summary>
    public class Dilliveryman
    {
        private string dillivierymanState;

        public void Dilliver()
        {
            Console.WriteLine("我是配送员，我来发货。");
            dillivierymanState = "已发货";
        }
    }
    
    
    
    
  // entry
  
    class Program
    {
        static void Main(string[] args)
        {

            Customer subject = new Customer();

            Accountant accountant = new Accountant();
            Cashier cashier = new Cashier();
            Dilliveryman dilliveryman = new Dilliveryman();

            // 注册事件
            subject.Update += accountant.GiveInvoice;
            subject.Update += cashier.Recoded;
            subject.Update += dilliveryman.Dilliver;

            /*
             * 以上写法也可以用下面代码来替换
            subject.Update += new CustomerEventHandler(accountant.GiveInvoice);
            subject.Update += new CustomerEventHandler(cashier.Recoded);
            subject.Update += new CustomerEventHandler(dilliveryman.Dilliver);
             */

            subject.CustomerState = "已付款";
            subject.Notify();

            Console.Read();
        }
    }
