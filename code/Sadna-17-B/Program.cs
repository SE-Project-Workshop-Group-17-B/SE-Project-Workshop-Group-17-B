using Sadna_17_B.DataAccessLayer.store;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Sadna_17_B
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing database...");
            InitializeDatabase();

            ServiceFactory serviceFactory = new ServiceFactory();
            IUserService userService = serviceFactory.UserService;
            IStoreService storeService = serviceFactory.StoreService;
            Console.WriteLine("Welcome to the server of Group 17B's Workshop Project");
            Console.ReadKey();
        }

        static void InitializeDatabase()
        {
            InitializeStoreTable();
            InitializeProductTable();
            InitializeDiscountTable();
            InitializeDiscountPolicyTable();
            InitializePurchaseTable();
            InitializePurchasePolicyTable();
        }

        static void InitializeStoreTable()
        {
            try
            {
                StoreDAO storeDAO = new StoreDAO();
                storeDAO.CreateStoreTable();

                StoreDTO store = new StoreDTO
                {
                    Name = "Test Store",
                    Email = "teststore@example.com",
                    PhoneNumber = "123-456-7890",
                    Description = "This is a test store.",
                    Address = "123 Test Street",
                    Rating = 4.5,
                    Reviews = new List<string> { "Great store!", "Excellent service." },
                    Complaints = new List<string> { "None" }
                };

                storeDAO.AddStore(store);
                Console.WriteLine("Test store added successfully.");

                StoreDTO addedStore = storeDAO.GetStore(1);
                if (addedStore != null)
                {
                    Console.WriteLine($"Store retrieved: {addedStore.Name}, ID: {addedStore.ID}");
                    addedStore.Email = "updatedstore@example.com";
                    storeDAO.UpdateStore(addedStore);
                    Console.WriteLine("Store updated successfully.");
                }

                // delete works too 
                // storeDAO.DeleteStore(1);
                // StoreDTO deletedStore = storeDAO.GetStore(1);
                // Console.WriteLine(deletedStore == null ? "Store deleted successfully." : "Store deletion failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred with the Store table: " + ex.Message);
            }
        }

        static void InitializeProductTable()
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();
                productDAO.CreateProductTable();

                ProductDTO product = new ProductDTO
                {
                    StoreID = 1,
                    Amount = 10,
                    Name = "Test Product",
                    Price = 19.99,
                    Rating = 0,
                    Category = "Test Category",
                    Description = "This is a test product.",
                    Reviews = new List<string>(),
                    Locked = false
                };

                productDAO.AddProduct(product);
                Console.WriteLine("Test product added successfully.");

                ProductDTO addedProduct = productDAO.GetProduct(1);
                if (addedProduct != null)
                {
                    Console.WriteLine($"Product retrieved: {addedProduct.Name}, ID: {addedProduct.ID}");
                    addedProduct.Price = 24.99;
                    productDAO.UpdateProduct(addedProduct);
                    Console.WriteLine("Product updated successfully.");
                }

                // delete works too 
                // productDAO.DeleteProduct(1);
                // ProductDTO deletedProduct = productDAO.GetProduct(1);
                // Console.WriteLine(deletedProduct == null ? "Product deleted successfully." : "Product deletion failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred with the Product table: " + ex.Message);
            }
        }

        static void InitializeDiscountTable()
        {
            try
            {
                DiscountDAO discountDAO = new DiscountDAO();
                discountDAO.CreateDiscountTable();

                DiscountDTO discount = new DiscountDTO
                {
                    DiscountID = 1,
                    StartDate = DateTime.Parse("2024-01-01"),
                    EndDate = DateTime.Parse("2024-12-31"),
                    Strategy = "Flat",
                    DiscountType = "Seasonal",
                    Relevant = "All Products",
                    Conditions = "No Conditions"
                };

                discountDAO.AddDiscount(discount);
                Console.WriteLine("Test discount added successfully.");

                DiscountDTO addedDiscount = discountDAO.GetDiscount(1);
                if (addedDiscount != null)
                {
                    Console.WriteLine($"Discount retrieved: {addedDiscount.DiscountID}");
                    addedDiscount.Strategy = "Percentage";
                    discountDAO.UpdateDiscount(addedDiscount);
                    Console.WriteLine("Discount updated successfully.");
                }

                // delete works too 
                // discountDAO.DeleteDiscount(1);
                // DiscountDTO deletedDiscount = discountDAO.GetDiscount(1);
                // Console.WriteLine(deletedDiscount == null ? "Discount deleted successfully." : "Discount deletion failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred with the Discount table: " + ex.Message);
            }
        }

        static void InitializeDiscountPolicyTable()
        {
            try
            {
                DiscountPolicyDAO discountPolicyDAO = new DiscountPolicyDAO();
                discountPolicyDAO.CreateDiscountPolicyTable();

                DiscountPolicyDTO discountPolicy = new DiscountPolicyDTO
                {
                    StoreID = 1,
                    DiscountID = 1
                };

                discountPolicyDAO.AddDiscountPolicy(discountPolicy);
                Console.WriteLine("Test discount policy added successfully.");

                DiscountPolicyDTO addedDiscountPolicy = discountPolicyDAO.GetDiscountPolicy(1, 1);
                if (addedDiscountPolicy != null)
                {
                    Console.WriteLine($"Discount policy retrieved: StoreID: {addedDiscountPolicy.StoreID}, DiscountID: {addedDiscountPolicy.DiscountID}");
                }

                // delete works too 
                // discountPolicyDAO.DeleteDiscountPolicy(1, 1);
                // DiscountPolicyDTO deletedDiscountPolicy = discountPolicyDAO.GetDiscountPolicy(1, 1);
                // Console.WriteLine(deletedDiscountPolicy == null ? "Discount policy deleted successfully." : "Discount policy deletion failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred with the DiscountPolicy table: " + ex.Message);
            }
        }

        static void InitializePurchaseTable()
        {
            try
            {
                PurchaseDAO purchaseDAO = new PurchaseDAO();
                purchaseDAO.CreatePurchaseTable();

                PurchaseDTO purchase = new PurchaseDTO
                {
                    Id = 1,
                    Name = "Test Purchase",
                    AggregationRule = "Sum",
                    Conditions = "None"
                };

                purchaseDAO.AddPurchase(purchase);
                Console.WriteLine("Test purchase added successfully.");

                PurchaseDTO addedPurchase = purchaseDAO.GetPurchase(1);
                if (addedPurchase != null)
                {
                    Console.WriteLine($"Purchase retrieved: {addedPurchase.Name}, ID: {addedPurchase.Id}");
                    addedPurchase.AggregationRule = "Average";
                    purchaseDAO.UpdatePurchase(addedPurchase);
                    Console.WriteLine("Purchase updated successfully.");
                }

                // delete works too 
                // purchaseDAO.DeletePurchase(1);
                // PurchaseDTO deletedPurchase = purchaseDAO.GetPurchase(1);
                // Console.WriteLine(deletedPurchase == null ? "Purchase deleted successfully." : "Purchase deletion failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred with the Purchase table: " + ex.Message);
            }
        }

        static void InitializePurchasePolicyTable()
        {
            try
            {
                PurchasePolicyDAO purchasePolicyDAO = new PurchasePolicyDAO();
                purchasePolicyDAO.CreatePurchasePolicyTable();

                PurchasePolicyDTO purchasePolicy = new PurchasePolicyDTO
                {
                    StoreID = 1,
                    PurchaseID = 1
                };

                purchasePolicyDAO.AddPurchasePolicy(purchasePolicy);
                Console.WriteLine("Test purchase policy added successfully.");

                PurchasePolicyDTO addedPurchasePolicy = purchasePolicyDAO.GetPurchasePolicy(1, 1);
                if (addedPurchasePolicy != null)
                {
                    Console.WriteLine($"Purchase policy retrieved: StoreID: {addedPurchasePolicy.StoreID}, PurchaseID: {addedPurchasePolicy.PurchaseID}");
                }

                // delete works too 
                // purchasePolicyDAO.DeletePurchasePolicy(1, 1);
                // PurchasePolicyDTO deletedPurchasePolicy = purchasePolicyDAO.GetPurchasePolicy(1, 1);
                // Console.WriteLine(deletedPurchasePolicy == null ? "Purchase policy deleted successfully." : "Purchase policy deletion failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred with the PurchasePolicy table: " + ex.Message);
            }
        }
    }
}
