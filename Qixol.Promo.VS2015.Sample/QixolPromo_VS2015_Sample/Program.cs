using Qixol.Promo.Integration.Lib;
using System;
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


        // The company key must be specified here, this is assigned to you when you sign up to evaluate Qixol Promo, and can
        // be found under 'Company Management' under 'Configuration' in the Promo Administration Portal.
        private static readonly string Sample_CompanyKey = "<-- COMPANY KEY HERE -->";

        // Working variable for messages to be displayed.
        private static string _lastResultMessage = "";

        #region MAIN

        static void Main(string[] args)
        {
            if(Sample_CompanyKey == "<-- COMPANY KEY HERE -->")
            {
                Console.Write(DisplayHelpers.GetWarningMessage());
                Console.ReadKey();
            }
            else
            {
                bool continueLoop = true;

                do
                {
                    // Display the menu, and any text from previous activities, and then wait for a key.
                    // If we don't receive a key we're expecting, exit the loop.
                    Console.Clear();
                    Console.Write(DisplayHelpers.GetMenuDisplayText(_lastResultMessage));

                    // Clear down any previous messages
                    _lastResultMessage = string.Empty;

                    // Wait for a key to be processed and then call thru to the appropriate method.
                    var keyPressed = Console.ReadKey().Key;
                    try
                    {
                        switch (keyPressed)
                        {
                            case ConsoleKey.D1:
                            case ConsoleKey.NumPad1:
                                Sample_SubmitBasket();
                                break;

                            case ConsoleKey.D2:
                            case ConsoleKey.NumPad2:
                                Sample_CreateUpdateProduct();
                                break;

                            case ConsoleKey.D3:
                            case ConsoleKey.NumPad3:
                                Sample_ValidateCouponCode();
                                break;

                            case ConsoleKey.D4:
                            case ConsoleKey.NumPad4:
                                Sample_RetrieveBasketPromotionsForDay();
                                break;

                            case ConsoleKey.D5:
                            case ConsoleKey.NumPad5:
                                Sample_SubmitBasketForMissedPromotions();
                                break;

                            default:
                                // Not recognised
                                continueLoop = false;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _lastResultMessage = string.Concat("ERROR - ", ex.Message);
                    }

                } while (continueLoop);
            }
        }

        #endregion
    
        #region SUBMIT BASKET SAMPLE 

        /// <summary>
        /// Demonstrate submitting a basket and processing the response.
        /// </summary>
        private static void Sample_SubmitBasket()
        {
            // Create the basket service manager - targeting the evaluation services.
            var basketServiceManager = new BasketServiceManager(ServiceTarget.EvaluationServices);

            // Get the basket request details.
            var basketRequest = SampleRequests.GetBasketRequest(Sample_CompanyKey, false);

            // Submit the request, and get the response.
            var basketResponse = basketServiceManager.SubmitBasket(basketRequest);

            // Now look at the response and display some information in the console.
            if (basketResponse.Summary.ProcessingResult != true)
            {
                // Something went wrong with the validation of the basket, so show the message.
                _lastResultMessage = basketResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Response received...  Use the LOCAL GetDisplayText method to output some details to the console.
                _lastResultMessage = basketResponse.GetDisplayText();
            }                  
        }

        #endregion

        #region CREATE or UPDATE PRODUCT SAMPLE

        /// <summary>
        /// Attempt to insert a product.  If the product exists, it will just be updated
        /// </summary>
        private static void Sample_CreateUpdateProduct()
        {
            // Create the import service manager - targeting the evaluation services.
            var importService = new ImportServiceManager(ServiceTarget.EvaluationServices);

            // Get the request details.
            var productImportRequest = SampleRequests.GetProductImportRequest(Sample_CompanyKey);

            // Submit the request and get the response.
            var importResponse = importService.ImportProducts(productImportRequest);

            // Now look at the response and display some information in the console.
            if(importResponse.Summary.ProcessedSuccessfully != true)
            {
                // Something went wrong with the import, so show the message.
                _lastResultMessage = importResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Get some display text from the import result.
                _lastResultMessage = importResponse.GetDisplayText();
            }
        }

        #endregion

        #region VALIDATE COUPON CODE SAMPLE

        private static void Sample_ValidateCouponCode()
        {
            // Create the basket service manager - targeting the evaluation services.
            var basketService = new BasketServiceManager(ServiceTarget.EvaluationServices);

            // We need to pass the coupon code which is to be validated.
            string couponCodeToValidate = "2EPoy3Rv_0WTjRjkt0V24g";

            // Submit the coupon code validation and get the response.
            var validationResult = basketService.ValidateCouponCode(Sample_CompanyKey, couponCodeToValidate);

            // Now look at the response and display some information in the console.
            if (validationResult.Summary.ProcessingResult != true)
            {
                // Something went wrong, so show the message.
                _lastResultMessage = validationResult.Summary.GetErrorDisplayText();
            }
            else
            {
                // Display the coupon code details.
                _lastResultMessage = validationResult.Coupon.GetDisplayText();
            }
        }


        #endregion

        #region GET PROMOTIONS FOR DAY SAMPLE 

        private static void Sample_RetrieveBasketPromotionsForDay()
        {
            // Create the export service manager - targeting the evaluation services.
            var exportService = new ExportServiceManager(ServiceTarget.EvaluationServices);

            // Get the request details.
            var getPromotionsRequest = SampleRequests.GetPromotionDetailsRequest(Sample_CompanyKey);

            // Submit the request and get the response
            var promosResponse = exportService.ExportPromotionsForBasket(getPromotionsRequest);

            // Now look at the response and display some information in the console.
            if (promosResponse.Summary.ProcessingResult != true)
            {
                // Something went wrong, so show the message.
                _lastResultMessage = promosResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Display the promotion details.
                _lastResultMessage = promosResponse.Promotions.GetDisplayText();
            }        
        }

        #endregion

        #region SUBMIT BASKET - GET MISSED PROMOTIONS

        /// <summary>
        /// Demonstrate submitting a basket and reviewing any missed promotions returned.
        /// </summary>
        private static void Sample_SubmitBasketForMissedPromotions()
        {
            // Create the basket service manager - targeting the evaluation services.
            var basketServiceManager = new BasketServiceManager(ServiceTarget.EvaluationServices);

            // Get the basket request details.
            var basketRequest = SampleRequests.GetBasketRequest(Sample_CompanyKey, true);

            // Submit the request, and get the response.
            var basketResponse = basketServiceManager.SubmitBasket(basketRequest);

            // Now look at the response and display some information in the console.
            if (basketResponse.Summary.ProcessingResult != true)
            {
                // Something went wrong with the validation of the basket, so show the message.
                _lastResultMessage = basketResponse.Summary.GetErrorDisplayText();
            }
            else
            {
                // Response received...  Use the LOCAL GetDisplayText and GetMissedPromotionsDisplayText methods to output some details to the console.
                _lastResultMessage = basketResponse.GetDisplayText();
                _lastResultMessage += basketResponse.GetMissedPromotionsDisplayText();
            }
        }

        #endregion

    }
}
