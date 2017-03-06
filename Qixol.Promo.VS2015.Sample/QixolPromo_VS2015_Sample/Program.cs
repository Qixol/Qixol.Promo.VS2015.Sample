using Qixol.Promo.Integration.Lib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace QixolPromo_VS2015_Sample
{
    class Program
    {
        /*
         *  This project demonstrates using the Qixol Promo Integration Library to interact with an account setup in Qixol Promo.
         *  
         *  To use this project effectively, please register to evaluate Qixol Promo, so that a new company account is created for you.
         *  Once you have an evaluation company set up, please copy the COMPANY KEY below before attempting to interact with your company. 
         *
         */

        private static readonly string COMPANY_KEY = "companyKey";
        private static readonly string COUPON_IMPORT_KEY = "couponImportKey";

        #region MAIN

        static void Main(string[] args)
        {
            #region read and validate configuration

            string companyKey = ReadAppSetting(COMPANY_KEY);
            string couponImportKey = ReadAppSetting(COUPON_IMPORT_KEY);

            List<string> warningMessages = new List<string>();

            if (string.IsNullOrEmpty(companyKey) || (string.Compare(companyKey, "## COMPANY KEY HERE ##", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                warningMessages.Add(DisplayHelpers.GetCompanyWarningMessage());
            }

            if (string.IsNullOrEmpty(couponImportKey) || (string.Compare(couponImportKey, "## COUPON IMPORT KEY HERE ##", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                warningMessages.Add(DisplayHelpers.GetCouponWarningMessage());
            }

            if (warningMessages.Count > 0)
            {
                Console.WriteLine(DisplayHelpers.GetWarningMessage(warningMessages));
                Console.ReadKey();
                return;
            }

            #endregion

            #region main loop

            bool continueLoop = true;
            string lastResultMessage = string.Empty;

            do
            {
                // Display the menu, and any text from previous activities, and then wait for a key.
                // If we don't receive a key we're expecting, exit the loop.
                Console.Clear();
                Console.Write(DisplayHelpers.GetMenuDisplayText(lastResultMessage));

                // Clear down any previous messages
                lastResultMessage = string.Empty;

                // Wait for a key to be processed and then call thru to the appropriate method.
                var keyPressed = Console.ReadKey().Key;
                try
                {
                    switch (keyPressed)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            lastResultMessage = Sample_SubmitBasket(companyKey);
                            break;

                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            lastResultMessage = Sample_CreateUpdateProduct(companyKey);
                            break;

                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            lastResultMessage = Sample_ValidateCouponCode(companyKey);
                            break;

                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            lastResultMessage = Sample_RetrieveBasketPromotionsForDay(companyKey);
                            break;

                        case ConsoleKey.D5:
                        case ConsoleKey.NumPad5:
                            lastResultMessage = Sample_RetrieveBasketPromotionsForDayByProduct(companyKey);
                            break;

                        case ConsoleKey.D6:
                        case ConsoleKey.NumPad6:
                            lastResultMessage = Sample_SubmitBasketForMissedPromotions(companyKey);
                            break;

                        case ConsoleKey.D7:
                        case ConsoleKey.NumPad7:
                            lastResultMessage = Sample_PushCouponCodesToPromo(companyKey, couponImportKey);
                            break;

                        case ConsoleKey.D8:
                        case ConsoleKey.NumPad8:
                            lastResultMessage = Sample_RetrieveCouponCodesFromPromo(companyKey, couponImportKey);
                            break;

                        default:
                            // Not recognised
                            continueLoop = false;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    lastResultMessage = string.Concat("ERROR - ", ex.Message);
                }

            } while (continueLoop);

            #endregion
        }

        #endregion

        #region SUBMIT BASKET SAMPLE 

        /// <summary>
        /// Demonstrate submitting a basket and processing the response.
        /// </summary>
        private static string Sample_SubmitBasket(string companyKey)
        {
            // Create the basket service manager - targeting the evaluation services.
            var basketServiceManager = new BasketServiceManager(ServiceTarget.EvaluationServices);

            // Get the basket request details.
            var basketRequest = SampleRequests.GetBasketRequest(companyKey, false);

            // Submit the request, and get the response.
            var basketResponse = basketServiceManager.SubmitBasket(basketRequest);

            // Now look at the response and display some information in the console.
            if (basketResponse.Summary.ProcessingResult != true)
            {
                // Something went wrong with the validation of the basket, so show the message.
                return basketResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Response received...  Use the LOCAL GetDisplayText method to output some details to the console.
                return basketResponse.GetDisplayText();
            }
        }

        #endregion

        #region CREATE or UPDATE PRODUCT SAMPLE

        /// <summary>
        /// Attempt to insert a product.  If the product exists, it will just be updated
        /// </summary>
        private static string Sample_CreateUpdateProduct(string companyKey)
        {
            // Create the import service manager - targeting the evaluation services.
            var importService = new ImportServiceManager(ServiceTarget.EvaluationServices);

            // Get the request details.
            var productImportRequest = SampleRequests.GetProductImportRequest(companyKey);

            // Submit the request and get the response.
            var importResponse = importService.ImportProducts(productImportRequest);

            // Now look at the response and display some information in the console.
            if (importResponse.Summary.ProcessedSuccessfully != true)
            {
                // Something went wrong with the import, so show the message.
                return importResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Get some display text from the import result.
                return importResponse.GetDisplayText();
            }
        }

        #endregion

        #region VALIDATE COUPON CODE SAMPLE

        private static string Sample_ValidateCouponCode(string companyKey)
        {
            // Create the basket service manager - targeting the evaluation services.
            var basketService = new BasketServiceManager(ServiceTarget.EvaluationServices);

            // We need to pass the coupon code which is to be validated.
            string couponCodeToValidate = "2EPoy3Rv_0WTjRjkt0V24g";

            // Submit the coupon code validation and get the response.
            var validationResult = basketService.ValidateCouponCode(companyKey, couponCodeToValidate);

            // Now look at the response and display some information in the console.
            if (validationResult.Summary.ProcessingResult != true)
            {
                // Something went wrong, so show the message.
                return validationResult.Summary.GetErrorDisplayText();
            }
            else
            {
                // Display the coupon code details.
                return validationResult.Coupon.GetDisplayText();
            }
        }


        #endregion

        #region GET PROMOTIONS FOR DAY SAMPLE 

        private static string Sample_RetrieveBasketPromotionsForDay(string companyKey)
        {
            // Create the export service manager - targeting the evaluation services.
            var exportService = new ExportServiceManager(ServiceTarget.EvaluationServices);

            // Get the request details.
            var getPromotionsRequest = SampleRequests.GetPromotionDetailsRequest(companyKey);

            // Submit the request and get the response
            var promosResponse = exportService.ExportPromotionsForBasket(getPromotionsRequest);

            // Now look at the response and display some information in the console.
            if (promosResponse.Summary.ProcessingResult != true)
            {
                // Something went wrong, so show the message.
                return promosResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Display the promotion details.
                return promosResponse.Promotions.GetDisplayText();
            }
        }

        #endregion

        #region GET PROMOTIONS FOR DAY BY PRODUCT SAMPLE 

        private static string Sample_RetrieveBasketPromotionsForDayByProduct(string companyKey)
        {
            // Create the export service manager - targeting the evaluation services.
            var exportService = new ExportServiceManager(ServiceTarget.EvaluationServices);

            // Get the request details.
            var getPromotionsRequest = SampleRequests.GetPromotionDetailsByProductRequest(companyKey);

            // Submit the request and get the response
            var promosResponse = exportService.ExportPromotionsForProducts(getPromotionsRequest);

            // Now look at the response and display some information in the console.
            if (promosResponse.Summary.ProcessingResult != true)
            {
                // Something went wrong, so show the message.
                return promosResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Display the promotion details.
                return promosResponse.Promotions.GetDisplayText();
            }
        }

        #endregion

        #region SUBMIT BASKET - GET MISSED PROMOTIONS

        /// <summary>
        /// Demonstrate submitting a basket and reviewing any missed promotions returned.
        /// </summary>
        private static string Sample_SubmitBasketForMissedPromotions(string companyKey)
        {
            // Create the basket service manager - targeting the evaluation services.
            var basketServiceManager = new BasketServiceManager(ServiceTarget.EvaluationServices);

            // Get the basket request details.
            var basketRequest = SampleRequests.GetBasketRequest(companyKey, true);

            // Submit the request, and get the response.
            var basketResponse = basketServiceManager.SubmitBasket(basketRequest);

            // Now look at the response and display some information in the console.
            if (basketResponse.Summary.ProcessingResult != true)
            {
                // Something went wrong with the validation of the basket, so show the message.
                return basketResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Response received...  Use the LOCAL GetDisplayText and GetMissedPromotionsDisplayText methods to output some details to the console.
                string message = basketResponse.GetDisplayText();
                message += basketResponse.GetMissedPromotionsDisplayText();
                return message;
            }
        }

        #endregion

        #region COUPONS

        /// <summary>
        /// Demonstrate pushing coupon codes into Promo
        /// </summary>
        /// <param name="companyKey"></param>
        /// <param name="couponImportKey"></param>
        /// <returns>result message</returns>
        private static string Sample_PushCouponCodesToPromo(string companyKey, string couponImportKey)
        {
            var importService = new ImportServiceManager(ServiceTarget.EvaluationServices);

            var couponCodeImportRequest = SampleRequests.GetCouponCodesImportRequest(companyKey, couponImportKey);

            var importResponse = importService.ImportCouponCodes(couponCodeImportRequest);

            if (!importResponse.Summary.ProcessedSuccessfully)
            {
                return importResponse.Summary.GetErrorDisplayText();

            }
            else
            {
                return importResponse.GetDisplayText();
            }
        }

        /// <summary>
        /// Demonstrate retriving coupon codes from Promo
        /// </summary>
        /// <param name="companyKey"></param>
        /// <param name="couponImportKey"></param>
        /// <returns>result message</returns>
        private static string Sample_RetrieveCouponCodesFromPromo(string companyKey, string couponImportKey)
        {
            var exportService = new ExportServiceManager(ServiceTarget.EvaluationServices);

            var couponCodeExportRequest = SampleRequests.GetCouponCodesExportRequest(companyKey, couponImportKey);

            var exportResponse = exportService.ExportCouponCodes(couponCodeExportRequest);

            // Now look at the response and display some information in the console.
            if (exportResponse.Summary.ProcessedSuccessfully != true)
            {
                // Something went wrong with the import, so show the message.
                return exportResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Get some display text from the import result.
                return exportResponse.GetDisplayText();
            }
        }

        #endregion

        #region helpers

        /// <summary>
        /// read setting from app.config
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string: null if error / not present, value if present</returns>
        private static string ReadAppSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch
            {
            }
            return null;
        }
        #endregion
    }
}
