using Microsoft.AspNetCore.Mvc;
using Sadna_17_B_API.Models;
using Sadna_17_B;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Layer_Service.ServiceDTOs;
using System.Threading.Channels;

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


        [HttpPost("process_store_order")]
        public Response ProcessStoreOrder([FromBody] Basket data)
        {
            var response = _storeService.calculate_products_prices(data);
            return response;
        }

        [HttpPost("get_shoping_cart")]
        public IActionResult getShopingCart([FromBody] UIuserDTOAPI user)
        {
            Dictionary<string, string> doc = new Dictionary<string, string>()
            {
                ["token"] = user.AccessToken
            };
            var response = _userService.cart_by_token(doc);
            ShoppingCartDTO temp = response.Data as ShoppingCartDTO;
            //string temp2 = JsonConvert.SerializeObject(temp);

            List<ItemDTO> itemList = new List<ItemDTO>();
            foreach (KeyValuePair<int, ShoppingBasketDTO> entry in temp.ShoppingBaskets)
            {
                ShoppingBasketDTO basket = entry.Value;
                int storeId = entry.Key;
                foreach (KeyValuePair<ProductDTO, int> entry2 in basket.ProductQuantities)
                {
                    ProductDTO product = entry2.Key;
                    int amount = entry2.Value;
                    //create new obejct that contains the data we need

                    //product category, name, store_id,quantity, price
                    ItemDTO payload = new ItemDTO()
                    {
                        ID = product.Id,
                        Name = product.Name,
                        Amount = product.amount,
                        StoreId = product.store_id,
                        Price = product.Price,
                        Category = product.Category,
                        Quantity = amount
                    };

                    itemList.Add(payload);
                }
            }

            return Ok(itemList);
        }

        [HttpPost("cart_remove_product")]
        public Response cartRemoveProduct([FromBody] temp2DTO tempObj)
        {
            var response = _userService.cart_remove_product(tempObj.p, tempObj.AccessToken);
            return response;
        }        


        [HttpPost("remove_from_cart")]
        public Response removeFromCart([FromBody] RemoveFromCartRequest request)
        {
            //casting to product not working
            ItemDTO i = request.Item;
            ProductDTO product = new ProductDTO()
            {
                Id = i.ID,
                store_id = i.StoreId,
                amount = i.Amount,
                Price = i.Price,
                Category = i.Category,
                Name = i.Name,
                Description = "very good product",
                CustomerRate = 4.5
            };
            var response = _userService.cart_remove_product(product, request.Token);
            return response;
        }

        [HttpPost("cart_update_product")]
        public Response cartRemoveProduct([FromBody] Dictionary<string, string> doc)
        {
            var response = _userService.cart_update_product(doc);
            return response;
        }

        [HttpPost("get_owned_stores")]
        public Response getOwnedStores([FromBody] UIuserDTOAPI user)
        {
            var response = _userService.GetMyOwnedStores(user.AccessToken);
            return response;
        }

/*        [HttpPost("completePurchase")]
        public IActionResult CompletePurchase([FromBody] PurchaseDTO purchaseDetails)
        {
            //{creditCardInfo, DestInfo}
            var response = _userService.CompletePurchase(purchaseDetails.AccessToken, purchaseDetails.DestinationAddress, purchaseDetails.CreditCardInfo);
            return Ok(response);
        }*/

        [HttpPost("process_order")]
        public Response ProcessOrder([FromBody] PurchaseDTO userData)
        {
            string token = userData.AccessToken;
            var response = _userService.Process_order(token);
            return response;

        }

        [HttpPost("reduce_order")]
        public Response reduce_order(PurchaseDTO purchase_details)
        {
            var response = _userService.reduce_cart(purchase_details.AccessToken);
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
        [HttpPost("show_purchase_policy")]
        public Response ShowPurchasePolicy([FromBody] Dictionary<string, string> policy)
        {
            return _storeService.show_purchase_policy(policy);
        }
        [HttpPost("show_discount_policy")]
        public Response ShowDiscountPolicy([FromBody] Dictionary<string, string> policy)
        {
            return _storeService.show_discount_policy(policy);
        }
        [HttpPost("get_store_order_history")]
        public Response GetStoreOrderHistory([FromBody] orderHistoryDTO dto)
        {
            var response = _userService.GetStoreOrderHistory(dto.AccessToken, dto.OrderId);
            return response;
        }
        [HttpPost("add_store_product")]
        public Response AddStoreProduct([FromBody] AddProductDTO dto)
        {
            Response res = _storeService.add_product_to_store(dto.Token, dto.Sid, dto.Name, dto.Price, dto.Category, dto.Description, dto.Amount);
            return res;
        }
        [HttpPost("editProduct")]
        public Response EditProduct([FromBody] Dictionary<string, string> productDetails)
        {
            var response = _storeService.edit_product_in_store(productDetails);
            return response;
        }
        [HttpPost("createStore")]
        public Response CreateStore([FromBody] StoreDTOAPI storeDetails)
        {
            var response = _storeService.create_store(storeDetails.AccessToken, storeDetails.Name, storeDetails.Email, storeDetails.PhoneNumber, storeDetails.StoreDescription, storeDetails.Address);
            return response;
        }
        [HttpPost("offerManagerAppointment")]
        public Response OfferManagerAppointment([FromBody] ManagerAppointmentDTO appointmentDetails)
        {
            var response = _userService.OfferManagerAppointment(appointmentDetails.AccessToken, appointmentDetails.StoreId, appointmentDetails.UserName);
            return response;
        }
        [HttpPost("offerOwnerAppointment")]
        public Response OfferOwnerAppointment([FromBody] OwnerAppointmentDTO appointmentDetails)
        {
            var response = _userService.OfferOwnerAppointment(appointmentDetails.AccessToken, appointmentDetails.StoreId, appointmentDetails.UserName);
            return response;
        }
        [HttpPost("abandonOwnership")]
        public Response AbandonOwnership([FromBody] AbandonOwnershipDTO requestDetails)
        {
            string token = requestDetails.AccessToken;
            int storeId = requestDetails.StoreId;
            var response = _userService.AbandonOwnership(token, storeId);
            return response;
        }
        [HttpPost("closeStore")]
        public Response CloseStore([FromBody] CloseStoreDTO requestDetails)
        {
            string token = requestDetails.AccessToken;
            int storeId = requestDetails.StoreId;
            var response = _storeService.close_store(token, storeId);
            return response;
        }
        [HttpPost("addProductReview")]
        public Response AddProductReview([FromBody] ProductReviewDTO reviewDetails)
        {
            string token = reviewDetails.AccessToken;
            int storeID = reviewDetails.StoreID;
            int productID = reviewDetails.ProductID;
            string review = reviewDetails.Review;
            var response = _storeService.add_product_review(storeID, productID, review);
            return response;
        }
        [HttpPost("getProductRating")]
        public Response GetProductRating([FromBody] ProductRatingDTO requestDetails)
        {
            string token = requestDetails.AccessToken;
            int productId = requestDetails.ProductId;
            var response = _storeService.get_product_rating(productId);
            return response;
        }
        [HttpPost("addProductRating")]
        public Response AddProductRating([FromBody] AddProductRatingDTO ratingDetails)
        {
            string token = ratingDetails.AccessToken;
            int storeID = ratingDetails.StoreID;
            int productID = ratingDetails.ProductID;
            int rating = ratingDetails.Rating;
            var response = _storeService.add_product_rating(storeID, productID, rating);
            return response;
        }
        [HttpPost("get_user_order_history")]
        public Response GetUserOrderHistory([FromBody] UserOrderHistoryDTO dto)
        {
            Response res = _userService.GetUserOrderHistory(dto.Token, dto.Username);
            return res;
        }

    }

}