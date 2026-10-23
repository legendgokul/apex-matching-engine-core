namespace apexLedger.BuisnessLogic
{
    public interface ILedgerService
    {
        public bool sellStock();
        public bool buyStock();
    }

    public class LedgerService : ILedgerService
    {
        public bool sellStock()
        {
            return true;
        }

        public bool buyStock()
        {
            return false;
        }

    }
}
