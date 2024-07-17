namespace Sadna_17_B_API.Models
{
    public class PurchaseDTO
    {
       public Dictionary<string, string> Supply{ get; set; }
       public Dictionary<string,string> Payment { get; set; }
       public string AccessToken { get; set; }
    }
}
