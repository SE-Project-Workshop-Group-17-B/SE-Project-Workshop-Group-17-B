using Microsoft.AspNetCore.Mvc;
using Sadna_17_B_API.Models;
using Sadna_17_B;
using Sadna_17_B_Frontend;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B_Frontend.Controllers;

namespace Sadna_17_B_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestAPIController : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory;
        private readonly IUserService _userService;
        private readonly StoreService _storeService;

        public RestAPIController(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _userService = serviceFactory.UserService;
            _storeService = serviceFactory.StoreService;
        }

        [HttpPost("entry")]
        public Response Entry()
        {
            var response = _userService.GuestEntry();
            return response;
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] UIuserDTOAPI userDto)
        {
            // Validate userDto (consider adding data annotations to your UserDto)

            var response = _userService.CreateSubscriber(userDto.Username, userDto.Password);

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

            var response = _userService.Login(userDto.Username, userDto.Password);

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
            var response = _userService.Logout(user.AccessToken);
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
            var response = _userService.IsFounder(request.AccessToken, request.StoreId);
            return response;
        }

        [HttpPost("isOwner")]
        public Response IsOwner([FromBody] RoleCheckRequest request)
        {
            var response = _userService.IsOwner(request.AccessToken, request.StoreId);
            return response;
        }

        [HttpPost("isManager")]
        public Response IsManager([FromBody] RoleCheckRequest request)
        {
            var response = _userService.IsManager(request.AccessToken, request.StoreId);
            return response;
        }

        [HttpPost("isGuest")]
        public Response IsGuest([FromBody] RoleCheckRequest request)
        {
            var response = _userService.IsGuest(request.AccessToken);
            return response;
        }

        [HttpPost("isSubscriber")]
        public Response IsSubscriber([FromBody] RoleCheckRequest request)
        {
            var response = _userService.IsSubscriber(request.AccessToken);
            return response;
        }

        [HttpPost("isAdmin")]
        public Response IsAdmin([FromBody] RoleCheckRequest request)
        {
            var response = _userService.IsAdmin(request.AccessToken);
            return response;
        }

        [HttpPost("search_product")]
        public Response SearchProduct([FromBody] FuncStoreDTO storeData)
        {
            var response = _storeService.search_product_by(storeData.doc);
            return response;
        }

        [HttpPost("process_store_order")]
        public Response ProcessStoreOrder([FromBody] FuncStoreDTO data)
        {
            var response = _storeService.calculate_products_prices(data.storeId, data.quantities);
            return response;
        }

        [HttpPost("get_shoping_cart")]
        public Response getShopingCart([FromBody] UIuserDTOAPI user)
        {
            var response = _userService.GetShoppingCart(user.AccessToken);
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
    }
}