using Qixol.Promo.Integration.Lib.Basket;
using Qixol.Promo.Integration.Lib.Export;
using Qixol.Promo.Integration.Lib.Import;
using Qixol.Promo.Integration.Lib.Shared;
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
            BasketRequest basketRequest = new BasketRequest()
            {
                // Required attributes
                CompanyKey = companyKey,
                SaleDateTime = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),     // Each basket must have a unique ID (though where a basket is submitted multiple times it should maintain the same ID).

                // Indicate whether we want to get missed promotions
                GetMissedPromotions = getMissedPromotions,

                // Additional basket attributes
                DeliveryPrice = 10.99M,
                DeliveryMethod = "NEXTDAY",

                CustomerGroup = "NORMAL",
                Channel = "RETAIL",
                StoreGroup = "LONDON",
                Store = "KINGSTON"
            };
            // We can add basket level custom attributes if needed
            basketRequest.AddCustomAttribute("testbasket", "true");

            // We must have one or mote items in the basket.  A basket item can have a quantity greater than one if required.

            BasketRequestItem basketRequestItem1 = new BasketRequestItem()
            {
                // We must always have a line number, price and quantity, and then a ProductCode and optionally a VariantCode.
                Id = 1,
                Price = 27.56M,
                Quantity = 2,
                ProductCode = "PR-25", // This is the 'adidas Consortium Campus 80s Running Shoes' from the sample product set.
            };
            basketRequest.AddItem(basketRequestItem1);

            BasketRequestItem basketRequestItem2 = new BasketRequestItem()
            {
                Id = 2,
                Price = 15.00M,
                Quantity = 1,
                ProductCode = "PR-29"       // This is the 'Custom T-Shirt' from the sample product set.
            };
            // We can also add custom attributes against an item in the basket if needed
            basketRequestItem1.AddCustomAttribute("message", "This is my t-shirt!");
            basketRequest.AddItem(basketRequestItem2);

            return basketRequest;
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

        /// <summary>
        /// Return a new GetPromotionDetailsRequest, populating the known basket attributes.  Note custom attributes are supported in the request too.
        /// </summary>
        /// <param name="companyKey"></param>
        /// <returns></returns>
        public static PromotionDetailsByProductRequest GetPromotionDetailsByProductRequest(string companyKey)
        {
            var promotionsByProductRequest = new PromotionDetailsByProductRequest()
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

            promotionsByProductRequest.AddProduct("EXAMPLE_IMPORTED_PRODUCT_1", string.Empty);

            return promotionsByProductRequest;
        }

        public static CouponCodesImportRequest GetCouponCodesImportRequest(string sample_CompanyKey, string sample_couponImportKey)
        {

            var couponCodesImportRequest = new CouponCodesImportRequest()
            {
                CompanyKey = sample_CompanyKey
                // CouponKey = sample_couponImportKey
                // Do not specify the couponkey at this level
                // we will specify it at code level so we can also create a new coupon during the import
            };

            // Add a code to an existing coupon using default values for all fields except the mandatory code
            couponCodesImportRequest.CouponCodes.Add(
                new CouponCodesImportRequestItem()
                {
                    CouponKey = sample_couponImportKey,
                    Code = "2EPoy3Rv_0WTjRjkt0V24g" // this is the same code that will be validated using option 3
                });

            // Add a single use code restricted to one week to an existing coupon
            couponCodesImportRequest.CouponCodes.Add(
                new CouponCodesImportRequestItem()
                {
                    CouponKey = sample_couponImportKey,
                    Code = "Single-Use",
                    MaximumUses = 1,
                    ValidFrom = DateTime.Now,
                    ValidTo = DateTime.Now.AddDays(7)
                });

            // Add a code for a new coupon
            couponCodesImportRequest.CouponCodes.Add(
                new CouponCodesImportRequestItem()
                {
                    CouponName = "Coupon from sample code",
                    Code = "sample-003"
                });

            // Add another code for the new coupon
            couponCodesImportRequest.CouponCodes.Add(
                new CouponCodesImportRequestItem()
                {
                    CouponName = "Coupon from sample code",
                    Code = "sample-004"
                });

            return couponCodesImportRequest;
        }

        public static CouponCodesExportRequest GetCouponCodesExportRequest(string sample_CompanyKey, string sample_couponExportKey)
        {
            var couponCodesExportRequest = new CouponCodesExportRequest()
            {
                CompanyKey = sample_CompanyKey,
                CouponKey = sample_couponExportKey
            };

            RequestPagination pagination = new RequestPagination()
            {
                Page = "1",
                PageSize = 500
            };

            couponCodesExportRequest.Pagination = pagination;

            couponCodesExportRequest.AddStatus(Qixol.Promo.Integration.Lib.Coupon.CouponCodeStatus.Active);
            couponCodesExportRequest.AddStatus(Qixol.Promo.Integration.Lib.Coupon.CouponCodeStatus.Created);
            couponCodesExportRequest.AddStatus(Qixol.Promo.Integration.Lib.Coupon.CouponCodeStatus.Expired);
            couponCodesExportRequest.AddStatus(Qixol.Promo.Integration.Lib.Coupon.CouponCodeStatus.Issued);
            couponCodesExportRequest.AddStatus(Qixol.Promo.Integration.Lib.Coupon.CouponCodeStatus.Redeemed);
            couponCodesExportRequest.AddStatus(Qixol.Promo.Integration.Lib.Coupon.CouponCodeStatus.Withdrawn);

            return couponCodesExportRequest;
        }
    }
}
