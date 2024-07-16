using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B_API.Controllers
{
    public class RespondOfferDTO
    {
        public string accessToken { get; set; }
        public int storeId { get; set; }
        public bool decision { get; set; }

        public RespondOfferDTO()
        {
        }
    }
}