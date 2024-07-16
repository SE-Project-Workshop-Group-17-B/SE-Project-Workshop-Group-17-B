using Microsoft.AspNetCore.Mvc;
using Sadna_17_B_API.Models;
using Sadna_17_B;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B_API.Models;

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

        [HttpGet("get_product_by_id/{productId}")]
        public Response GetProductById(int productId)
        {
            var response = _storeService.get_product_by_id(productId);
            return response;
        }

        [HttpPost("add_product_to_cart")]
        public Response AddProductToCart([FromBody] AddToCartDTO addToCartDTO)
        {
            var response = _userService.cart_add_product(addToCartDTO.Doc, addToCartDTO.Change);
            return response;
        }

        [HttpPost("search_product")]
        public Response SearchProduct([FromBody] ProductSearchDTO searchDTO)
        {
            var response = _storeService.search_product_by(searchDTO.SearchCriteria);
            if (response.Success)
            {
                var products = response.Data as List<Product>;
                var productDTOs = products.Select(p => new ProductDTOAPI
                {
                    ID = p.ID,
                    name = p.name,
                    price = p.price,
                    description = p.description,
                    category = p.category,
                    amount = p.amount,
                    storeId = p.storeId,
                    rating = p.rating,
                    // Map other properties
                }).ToList();
                return new Response("Products found", true, productDTOs);
            }
            else
            {
                return new Response("Error while searching for products", false);
            }
        }


        public static T GetPropertyValue<T>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null && property.PropertyType == typeof(T))
            {
                return (T)property.GetValue(obj);
            }
            return default(T);
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
        [HttpPost("get_store_details")]
        public Response getStoreDetails([FromBody] int storeId)
        {
            var response = _storeService.get_store_info(storeId);
            return response;
        }
        [HttpPost("get_store_name")]
        public Response getStoreName([FromBody] int storeId)
        {
            var response = _storeService.get_store_name(storeId);
            return response;
        }

        [HttpPost("get_store_rating_by_id")]
        public Response getStoreRatingById([FromBody] int storeId)
        {
            var response = _storeService.get_store_rating(storeId);
            return response;
        }

        [HttpPost("add_store_rating")]
        public Response addStoreRating([FromBody] tempDTO storeDTO)
        {
            var response = _storeService.add_store_rating(storeDTO.ID, double.Parse(storeDTO.Data));
            return response;
        }

        [HttpPost("add_store_complaint")]
        public Response addStoreComplaint([FromBody] tempDTO storeDTO)
        {
            var response = _storeService.add_store_complaint(storeDTO.ID, storeDTO.Data);
            return response;
        }

        [HttpPost("add_store_review")]
        public Response addStoreReview([FromBody] tempDTO storeDTO)
        {
            var response = _storeService.add_store_review(storeDTO.ID, storeDTO.Data);
            return response;
        }

        [HttpPost("get_product_by_id")]
        public Response getProductById([FromBody] int productId)
        {
            var response = _storeService.get_product_by_id(productId);
            return response;
        }

        [HttpPost("cart_add_product")]
        public Response cartAddProduct([FromBody] AddToCartDTO dto)
        {
            var response = _userService.cart_add_product(dto.Doc, dto.Change);
            return response;
        }

        [HttpPost("search_product_by")]
        public Response searchProductBy([FromBody] Dictionary<string, string> doc)
        {
            var response = _storeService.search_product_by(doc);
            return response;
        }

        [HttpPost("get_store_reviews_by_ID")]
        public Response seargetStoreReviewsByIdchProductBy([FromBody] int storeId)
        {
            var response = _storeService.get_store_reviews_by_ID(storeId);
            return response;
        }
    }

}