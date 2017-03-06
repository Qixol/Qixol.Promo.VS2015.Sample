using Qixol.Promo.Integration.Lib.Basket;
using Qixol.Promo.Integration.Lib.Coupon;
using Qixol.Promo.Integration.Lib.Export;
using Qixol.Promo.Integration.Lib.Import;
using Qixol.Promo.Integration.Lib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QixolPromo_VS2015_Sample
{
    public static class DisplayHelpers
    {

        /// <summary>
        /// Build the menu text
        /// </summary>
        /// <param name="lastResultMessage"></param>
        /// <returns></returns>
        public static string GetMenuDisplayText(string lastResultMessage)
        {
            var textOut = new StringBuilder();        
            textOut.AppendLine("--------  Qixol Promo - Integration Sample  --------");        
            textOut.AppendLine();
            textOut.AppendLine(" 1 - Submit basket");
            textOut.AppendLine(" 2 - Create/Update Product");
            textOut.AppendLine(" 3 - Validate Coupon Code");
            textOut.AppendLine(" 4 - Retrieve Basket Promotions for Day");
            textOut.AppendLine(" 5 - Retrieve Promotions for Day by Product");
            textOut.AppendLine(" 6 - Retrieve Missed Promotions with Basket");
            textOut.AppendLine(" 7 - Push coupon codes up to Promo for specified coupon");
            textOut.AppendLine(" 8 - Retrieve coupon codes from Promo for specified coupons");
            textOut.AppendLine();

            if (!string.IsNullOrEmpty(lastResultMessage))
                textOut.AppendLine(lastResultMessage);

            textOut.AppendLine(" Select an option... Or any other key to quit.");
            textOut.AppendLine();        
            return textOut.ToString();
        }

        /// <summary>
        /// Return text to be displayed when the app.config has not been fully completed.
        /// </summary>
        /// <returns></returns>
        public static string GetWarningMessage(List<string> messages)
        {
            var textOut = new StringBuilder();
            textOut.AppendLine(" --------  Qixol Promo - Integration Sample  --------");
            textOut.AppendLine();
            textOut.AppendLine(" In order to run this sample project - please provide:");
            textOut.AppendLine();
            messages.ForEach(m => textOut.AppendLine(m));
            textOut.AppendLine();
            textOut.AppendLine(" When you register to evaluate Qixol Promo (which is free!)");
            textOut.AppendLine(" you are assigned a Company Key.");
            textOut.AppendLine();
            textOut.AppendLine(" You can find your company key by logging into");
            textOut.AppendLine(" The Qixol Promo Administration Portal,");
            textOut.AppendLine(" selecting 'Configuration' from the navigation menu");
            textOut.AppendLine(" then clicking 'Manage Company'.");
            textOut.AppendLine();
            textOut.AppendLine(" ----------------------------------------------------");
            textOut.AppendLine(" Press any key to quit...");
            return textOut.ToString();
        }

        /// <summary>
        /// Return text to be displayed when a company key has not been specified.
        /// </summary>
        /// <returns></returns>
        public static string GetCompanyWarningMessage()
        {
            return " your Company Key in the app.config file companyKey setting.";
        }

        /// <summary>
        /// Return text to be displayed when a coupon key has not been specified.
        /// </summary>
        /// <returns></returns>
        public static string GetCouponWarningMessage()
        {
            return " your Coupon Import Key in the app.config file couponImportKey setting.";
        }

        /// <summary>
        /// Get the display text for a ResponseSummary
        /// </summary>
        /// <param name="responseSummary"></param>
        /// <returns></returns>
        public static string GetErrorDisplayText(this ResponseSummary responseSummary)
        {
            return string.Concat(" ERROR - ", responseSummary.ToMessagesString());
        }

        /// <summary>
        /// Get the display text for an ImportResponseSummary
        /// </summary>
        /// <param name="responseSummary"></param>
        /// <returns></returns>
        public static string GetErrorDisplayText(this ImportResponseSummary responseSummary)
        {
            return string.Concat(" ERROR - ", responseSummary.ToMessagesString());
        }

        /// <summary>
        /// Get the display text for an ExportResponseSummary
        /// </summary>
        /// <param name="responseSummary"></param>
        /// <returns></returns>
        public static string GetErrorDisplayText(this ExportResponseSummary responseSummary)
        {
            return string.Concat(" ERROR - ", responseSummary.ToMessagesString());
        }

        /// <summary>
        /// Get some display text for a BASKET RESPONSE to be shown in the console.
        /// </summary>
        /// <param name="basketResponse"></param>
        /// <returns></returns>
        public static string GetDisplayText(this BasketResponse basketResponse)
        {
            var textOut = new StringBuilder();
            textOut.AppendLine("Basket Response Received.");

            basketResponse.Items.ForEach(responseItem =>
            {
                textOut.AppendLine(string.Format(" - Line [{0}] Promotions Applied: {1}", responseItem.Id, responseItem.AppliedPromotions.Count.ToString()));
                responseItem.AppliedPromotions.ForEach(appliedPromo =>
                {
                    // Get the promotion name from the summary, as this is not repeated beneath each line...
                    string promoName = basketResponse.Summary.AppliedPromotions.Where(summaryAppliedPromo => summaryAppliedPromo.PromotionId == appliedPromo.PromotionId).FirstOrDefault().PromotionName;
                    textOut.AppendLine(string.Format("     -> {0} ({1}) - {2}", promoName, appliedPromo.InstanceId.ToString(), appliedPromo.DiscountAmount.ToString("0.00")));
                });
            });

            textOut.AppendLine(string.Format(" - Basket total discount: {0}", basketResponse.TotalDiscount.ToString("0.00")));
            textOut.AppendLine(string.Format(" - Basket total points: {0}", basketResponse.TotalIssuedPoints.ToString("0.00")));
            textOut.AppendLine(string.Format(" - Basket coupons issued: {0}", basketResponse.Coupons.Where(coupon => coupon.Issued).Count().ToString()));
            textOut.AppendLine(string.Format(" - Total promotions applied: {0}", basketResponse.Summary.AppliedPromotions.Count.ToString()));

            textOut.AppendLine();

            if (basketResponse.Summary.AppliedPromotions.Count == 0)
            {
                textOut.AppendLine(" -> No promotions were applied,");
                textOut.AppendLine("    please check that the sample promotions have been published.");
            }
            else
            {
                textOut.AppendLine(" - Promotions Summary");

                basketResponse.Summary.AppliedPromotions.ForEach(appliedPromo =>
                {
                    textOut.Append("     -> ");
                    textOut.Append(appliedPromo.PromotionName);
                    textOut.Append(" ");
                    textOut.Append(appliedPromo.InstanceId.ToString());
                    textOut.Append(" - ");
                    textOut.AppendLine(appliedPromo.DiscountAmount.ToString("0.00"));
                    textOut.Append("     -> promotion type display: ");
                    textOut.AppendLine(appliedPromo.PromotionTypeDisplay);
                });
            }

            return textOut.ToString();
        }

        public static string GetDisplayText(this CouponCodesImportResponse couponCodesImportResponse)
        {
            var textOut = new StringBuilder();

            textOut.AppendLine("Coupon codes import response received.");

            return textOut.ToString();
        }

        public static string GetDisplayText(this CouponCodesExportResponse couponCodesExportResponse)
        {
            var textOut = new StringBuilder();

            textOut.AppendLine("Coupon codes export response received.");

            if (couponCodesExportResponse.Summary != null && couponCodesExportResponse.Summary.Messages != null && couponCodesExportResponse.Summary.Messages.Count > 0 )
            {
                foreach(var m in couponCodesExportResponse.Summary.Messages)
                {
                    textOut.AppendLine(m.Message);
                }
            }

            if (couponCodesExportResponse.Pagination != null)
            {
                textOut.AppendLine("Pagination response.");
                textOut.Append(" - Page: ");
                textOut.AppendLine(couponCodesExportResponse.Pagination.Page.ToString());
                textOut.Append(" - PageSize: ");
                textOut.AppendLine(couponCodesExportResponse.Pagination.PageSize.ToString());
                textOut.Append(" - Total Items (Codes) Count: ");
                textOut.AppendLine(couponCodesExportResponse.Pagination.TotalItemCount.ToString());
                textOut.Append(" - Total Page Count: ");
                textOut.AppendLine(couponCodesExportResponse.Pagination.TotalPageCount.ToString());

            }

            if (couponCodesExportResponse.Coupon != null)
            {
                textOut.AppendLine(couponCodesExportResponse.Coupon.GetDisplayText());
            }

            return textOut.ToString();
        }

        /// <summary>
        /// Get some display text for a BASKET RESPONSE MISSED PROMOTIONS to be shown in the console.
        /// </summary>
        /// <param name="basketResponse"></param>
        /// <returns></returns>
        public static string GetMissedPromotionsDisplayText(this BasketResponse basketResponse)
        {
            var textOut = new StringBuilder();
            textOut.AppendLine();
            textOut.AppendLine("Missed Promotions");

            if(basketResponse.MissedPromotions.Count > 0)
            {
                basketResponse.MissedPromotions.ForEach(missedPromotion =>
                {
                    textOut.AppendLine(string.Format(" - Missed Promotion [{0}, {1}, {2}]", missedPromotion.Category, missedPromotion.PromotionTypeDisplay, missedPromotion.PromotionName));

                    if(missedPromotion.Criteria.BasketAdditionalSpend > 0)
                        textOut.AppendLine(string.Format("            -> Basket Additional Spend: {0}", missedPromotion.Criteria.BasketAdditionalSpend.ToString()));

                    if (missedPromotion.Criteria.TotalAdditionalSpend > 0)
                        textOut.AppendLine(string.Format("            -> Total Additional Spend: {0}", missedPromotion.Criteria.TotalAdditionalSpend.ToString()));

                    missedPromotion.Criteria.CriteriaItems.ForEach(criteriaGroup =>
                    {
                        textOut.AppendLine(string.Format("            -> {0}, {1}{2}, Only Matched Items: {3}", criteriaGroup.Source.ToString(), 
                                                                                                                           criteriaGroup.AdditionalQuantity > 0 ? string.Concat("Additional Qty: ", criteriaGroup.AdditionalQuantity.ToString()) : string.Empty, 
                                                                                                                           criteriaGroup.AdditionalSpend > 0 ? string.Concat("Additional Spend: ", criteriaGroup.AdditionalSpend.ToString()) : string.Empty,
                                                                                                                           criteriaGroup.OnlyMatchedItems.ToString()));
                        criteriaGroup.Items.ForEach(criteriaItem =>
                        {
                            textOut.AppendLine(string.Format("               -- {0} : {1} {2} {3}", criteriaItem.ItemType.ToString(),
                                                                                                criteriaItem.ItemType == MissedPromotionDetailItemType.Product ? criteriaItem.ProductCode : criteriaItem.AttributeToken,
                                                                                                criteriaItem.ItemType == MissedPromotionDetailItemType.Product ? criteriaItem.VariantCode : criteriaItem.AttributeValue,
                                                                                                criteriaItem.IsMatched ? string.Format(" Matched Lines: {0}", string.Concat(criteriaItem.MatchedLineIds.Select(matchedLine => matchedLine.ToString()))) : string.Empty));
                        });
                    });

                    textOut.AppendLine(string.Format("            -> Apply to: {0}, Action: {1}, Save From: {2}", missedPromotion.Action.AppliesTo.ToString(), 
                                                                                                                  string.Concat(missedPromotion.Action.Details.ToString(),
                                                                                                                                missedPromotion.Action.Details == MissedPromotionActionDetailsDetail.PercentageDiscount ? string.Format(" [{0}%]", missedPromotion.Action.Percentage.ToString()) : string.Empty), 
                                                                                                                  missedPromotion.Action.SaveFrom.ToString()));
                });
            }
            else
            {
                textOut.AppendLine("There are no missed promotions for the submitted basket.");
            }

            return textOut.ToString();
        }

        /// <summary>
        /// Get some display text for a PRODUCT IMPORT RESPONSE to be shown in the console.
        /// </summary>
        /// <param name="productImportResponse"></param>
        /// <returns></returns>
        public static string GetDisplayText(this ProductImportResponse productImportResponse)
        {
            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendLine("Response:");
            messageBuilder.Append(" - Import Reference: ");
            messageBuilder.AppendLine(productImportResponse.Reference);

            messageBuilder.Append(" - ");
            messageBuilder.AppendLine(productImportResponse.Summary.ToMessagesString());
            messageBuilder.AppendLine(" - Please check the status by following ");
            messageBuilder.AppendLine("   Configuration / Import Log in the admin portal.");

            if (productImportResponse.Summary.Messages.Count > 0)
            {
                var resultMessage = productImportResponse.Summary.Messages.FirstOrDefault();
                if (resultMessage != null)
                {
                    if (string.Compare(resultMessage.Code, "IM105", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        messageBuilder.AppendLine("   then check under Configuration / Products");
                        messageBuilder.AppendLine("   and look for the \"Example imported product\"");
                    }
                }
            }
            return messageBuilder.ToString();
        }

        /// <summary>
        /// Get some display text for a COUPON VALIDATION RESPONSE to be shown in the console.
        /// </summary>
        /// <param name="coupon"></param>
        public static string GetDisplayText(this ValidatedCoupon coupon)
        {
            var textOut = new StringBuilder();
            textOut.AppendLine("Coupon Validation Response Received.");        
            textOut.AppendLine(string.Format(" - Coupon Type:         {0}", coupon.CouponType));
            textOut.AppendLine(string.Format(" - Coupon Name:         {0}", coupon.Name));
            textOut.AppendLine(string.Format(" - Generate on Demand?: {0}", coupon.GenerateOnDemand.ToString()));
            textOut.AppendLine(string.Format(" - Issue Only?:         {0}", coupon.IssueOnly.ToString()));
            textOut.AppendLine("   Codes:");
            coupon.Codes.ForEach(code =>
            {
                textOut.AppendLine(string.Format("    - Code: {0}", code.Code));
                textOut.AppendLine(string.Format("       - Status:       {0}", code.Status));
                textOut.AppendLine(string.Format("       - IsRedeemable: {0}", code.IsRedeemable.ToString()));
                textOut.AppendLine(string.Format("       - Valid From:   {0}", code.ValidFrom.ToString()));
                textOut.AppendLine(string.Format("       - Valid To:     {0}", code.ValidTo.ToString()));
                textOut.AppendLine(string.Format("       - Uses:         {0}", code.UsesCount.ToString()));
                textOut.AppendLine(string.Format("       - Status:       {0}", code.Status));
            });
            return textOut.ToString(); 
        }

        /// <summary>
        /// Get some display text for EXPORT PROMOTION DETAILS ITEMs to be shown in the console.
        /// </summary>
        /// <param name="promotionItems"></param>
        /// <returns></returns>
        public static string GetDisplayText(this List<ExportPromotionDetailsItem> promotionItems)
        {
            var textOut = new StringBuilder();
            textOut.AppendLine("Get Basket Level Promotions for Day Response Received.");              
            promotionItems.ForEach(promotionItem =>
            {
                textOut.AppendLine(string.Format(" - Promotion Name: {0}", promotionItem.PromotionName));
                textOut.AppendLine(string.Format("     - Type:                            {0}", promotionItem.PromotionType));
                textOut.AppendLine(string.Format("     - Valid From:                      {0}", promotionItem.ValidFrom.ToString()));
                textOut.AppendLine(string.Format("     - Valid To:                        {0}", promotionItem.ValidTo.ToString()));
                textOut.AppendLine(string.Format("     - HasAdditionalBasketRestrictions: {0}", promotionItem.HasAdditionalBasketRestrictions.ToString()));
                textOut.AppendLine(string.Format("     - HasCouponRestrictions:           {0}", promotionItem.HasCouponRestrictions.ToString()));
            });
            return textOut.ToString();
        }
    }
}
