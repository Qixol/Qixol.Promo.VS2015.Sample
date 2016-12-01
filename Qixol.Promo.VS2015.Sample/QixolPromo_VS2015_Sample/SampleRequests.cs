using Qixol.Promo.Integration.Lib.Basket;
using Qixol.Promo.Integration.Lib.Export;
using Qixol.Promo.Integration.Lib.Import;
using System;
using System.Collections.Generic;

namespace QixolPromo_VS2015_Sample
{
    public static class SampleRequests
    {

        /// <summary>
        /// Return a new basket request.  The basket will have two lines in it, for three items.  Set required and common basket attributes in the header too.
        /// </summary>
        /// <param name="companyKey">The company key which indicates to Promo which company to create the product within.  
        /// Company keys are visible in the Administration Portal, within Company Management under Configuration</param>
        /// <returns>A new BasketRequest.</returns>
        public static BasketRequest GetBasketRequest(string companyKey, bool getMissedPromotions)
        {
            return new BasketRequest()
            {
                // Required attributes
                CompanyKey = companyKey,
                SaleDateTime = DateTime.UtcNow,               
                Id = Guid.NewGuid().ToString(),     // Each basket must have a unique ID (though where a basket is submitted multiple times it should maintain the same ID).

                // Indicate whether we want to get missed promotions
                GetMissedPromotions  = getMissedPromotions,

                // Additional basket attributes
                DeliveryPrice = 10.99M,
                DeliveryMethod = "NEXTDAY",

                CustomerGroup = "NORMAL",
                Channel = "RETAIL",
                StoreGroup = "LONDON",
                Store = "KINGSTON",

                // We must have one or mote items in the basket.  A basket item can have a quantity greater than one if required.
                Items = new List<BasketRequestItem>
                {
                    new BasketRequestItem()
                    {
                        // We must always have a line number, price and quantity, and then a ProductCode and optionally a VariantCode.
                        Id = 1,
                        Price = 15.00M,
                        Quantity = 1,
                        ProductCode = "PR-29"       // This is the 'Custom T-Shirt' from the sample product set.
                    },

                    new BasketRequestItem()
                    {
                        Id = 2,
                        Price = 24.00M,
                        Quantity = 2,
                        ProductCode = "PR-28"       // This is the 'Oversized Women T-Shirt' from the sample product set.
                    }
                }
            };
        }

        /// <summary>
        /// Return a new product import request.  This provides an example of the creation of a product, which has two attributes, the first is the 
        /// category (a product attribute will be created for 'category' if it does not already exist).  The second product attribute helps us to see
        /// the product being updated with each subsequent import request, by storing the current date and time in the product attribute's value.
        /// </summary>
        /// <param name="companyKey">The company key which indicates to Promo which company to create the product within.  
        /// Company keys are visible in the Administration Portal, within Company Management under Configuration</param>
        /// <returns>A new ProductImportRequest</returns>
        public static ProductImportRequest GetProductImportRequest(string companyKey)
        {
            return new ProductImportRequest()
            {
                // Always set the company key.
                CompanyKey = companyKey,

                // Now provide a list of products to be inserted, updated or deleted.
                Products = new List<ProductImportRequestItem>
                {
                       new ProductImportRequestItem()
                       {
                            ProductCode = "EXAMPLE_IMPORTED_PRODUCT_1",
                            Description = "Example imported product",

                            Price = 109.99M,     // We need a price 

                            // For the product we can provide a list of attributes.  We can then use these attributes
                            // when creating promotions to create promotions which are applied based on products in the basket
                            // that have the specified attributes.
                            Attributes = new List<ProductImportRequestAttributeItem>
                            {

                                new ProductImportRequestAttributeItem()
                                {
                                    Name = "category",
                                    Value = "imported_items"
                                },

                                new ProductImportRequestAttributeItem()
                                {
                                    Name = "last_update_info",
                                    Value = DateTime.UtcNow.ToString()
                                }

                            }
                        }
                  }
            };
        }

        /// <summary>
        /// Return a new GetPromotionDetailsRequest, populating the known basket attributes.  Note custom attributes are supported in the request too.
        /// </summary>
        /// <param name="companyKey"></param>
        /// <returns></returns>
        public static PromotionDetailsRequest GetPromotionDetailsRequest(string companyKey)
        {
            return new PromotionDetailsRequest()
            {
                // Required items
                ValidationDate = DateTime.UtcNow,
                CompanyKey = companyKey,
                
                // Indicate that we want to ignore the time part of the [ValidationDate] specified above, 
                //  so the repsonse should include all promotions for the day.
                ValidateForTime = false,

                // Standard basket attributes.  These are optional and when specified
                //  will impact which promotions are returned based on their defined Criteria.
                CustomerGroup = "NORMAL",
                Channel = "RETAIL",
                StoreGroup = "LONDON",
                Store = "KINGSTON"
            };
        }
    }
}
