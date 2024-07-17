namespace Sadna_17_B_API.Controllers
{
    public class DiscountPolicyDTOAPI
    {
        public string EditType { get; set; }
        public string StoreId { get; set; }
        public string AncestorId { get; set; }
        public string DiscountId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Strategy { get; set; }
        public string Flat { get; set; }
        public string Percentage { get; set; }
        public string RelevantType { get; set; }
        public string RelevantFactors { get; set; }
        public string CondType { get; set; }
        public string CondProduct { get; set; }
        public string CondCategory { get; set; }
        public string CondOp { get; set; }
        public string CondPrice { get; set; }
        public string CondAmount { get; set; }
        public string CondDate { get; set; }

        public DiscountPolicyDTOAPI() { }
    }
}