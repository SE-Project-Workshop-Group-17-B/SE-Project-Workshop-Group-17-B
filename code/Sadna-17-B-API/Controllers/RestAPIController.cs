using Microsoft.AspNetCore.Mvc;
using Sadna_17_B_API.Models;
using Sadna_17_B;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;

namespace Sadna_17_B_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestAPIController : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory;
        private readonly UserService _userService;
        private readonly StoreService _storeService;

        public RestAPIController(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _userService = serviceFactory.UserService;
            _storeService = serviceFactory.StoreService;
        }

        [HttpGet("entry")]
        public Response Entry()
        {
            var response = _userService.entry_guest();
            return response;
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] UIuserDTOAPI userDto)
        {
            // Validate userDto (consider adding data annotations to your UserDto)

            var response = _userService.upgrade_subscriber(userDto.Username, userDto.Password);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] UIuserDTOAPI userDto)
        {
            // Validate userDto

            var response = _userService.entry_subscriber(userDto.Username, userDto.Password);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost("logout")]
        public Response Logout([FromBody] UIuserDTOAPI user)
        {
            var response = _userService.exit_subscriber(user.AccessToken);
            return response;
        }

        [HttpGet("get_stores")]
        public Response get_stores()
        {
            var response = _storeService.all_stores();
            return response;
        }
        [HttpPost("isFounder")]
        public Response IsFounder([FromBody] RoleCheckRequest request)
        {
            var response = _userService.founder(request.AccessToken, request.StoreId);
            return response;
        }

        [HttpPost("isOwner")]
        public Response IsOwner([FromBody] RoleCheckRequest request)
        {
            var response = _userService.owner(request.AccessToken, request.StoreId);
            return response;
        }

        [HttpPost("isManager")]
        public Response IsManager([FromBody] RoleCheckRequest request)
        {
            var response = _userService.manager(request.AccessToken, request.StoreId);
            return response;
        }

        [HttpPost("isGuest")]
        public Response IsGuest([FromBody] RoleCheckRequest request)
        {
            var response = _userService.guest(request.AccessToken);
            return response;
        }

        [HttpPost("isSubscriber")]
        public Response IsSubscriber([FromBody] RoleCheckRequest request)
        {
            var response = _userService.subscriber(request.AccessToken);
            return response;
        }

        [HttpPost("isAdmin")]
        public Response IsAdmin([FromBody] RoleCheckRequest request)
        {
            var response = _userService.admin(request.AccessToken);
            return response;
        }

        [HttpPost("search_product")]
        public Response SearchProduct([FromBody] FuncStoreDTO storeData)
        {
            var response = _storeService.search_product_by(storeData.doc);
            return response;
        }

        [HttpPost("process_store_order")]
        public Response ProcessStoreOrder([FromBody] Basket data)
        {
            var response = _storeService.calculate_products_prices(data);
            return response;
        }

        [HttpPost("get_shoping_cart")]
        public Response getShopingCart([FromBody] UIuserDTOAPI user)
        {
            Dictionary<string, string> doc = new Dictionary<string, string>()
            {
                ["token"] = user.AccessToken
            };
            var response = _userService.cart_by_token(doc);
            return response;
        }

        [HttpPost("get_owned_stores")]
        public Response getOwnedStores([FromBody] UIuserDTOAPI user)
        {
            var response = _userService.GetMyOwnedStores(user.AccessToken);
            return response;
        }

        [HttpPost("get_managed_stores")]
        public Response getManagedStores([FromBody] UIuserDTOAPI user)
        {
            var response = _userService.GetMyManagedStores(user.AccessToken);
            return response;
        }

        [HttpPost("get_stores_by_id")]

        public Response getStoresById([FromBody] FuncStoreDTO store)
        {
            var response = _storeService.store_by_id(store.storeId);
            return response;
        }

        [HttpPost("create_store")]
        public Response CreateStore([FromBody] UIStoreDTOAPI storeDTO)
        {
            var response = _storeService.create_store(storeDTO.AccessToken, storeDTO.Name, storeDTO.Email, storeDTO.PhoneNumber, storeDTO.StoreDescription, storeDTO.Address);
            return response;
        }
        [HttpPost("remove_from_cart")]
        public Response removeFromCart([FromBody] RemoveFromCartRequest request)
        {
            var response = _userService.cart_remove_product(new ProductDTO(request.ProductId), request.Token);
            return response;
        }
        [HttpPost("completePurchase")]
        public Response CompletePurchase([FromBody] PurchaseDTO purchaseDetails)
        {
            var response = _userService.CompletePurchase(purchaseDetails.AccessToken, purchaseDetails.DestinationAddress, purchaseDetails.CreditCardInfo);
            return response;
        }
    }
}