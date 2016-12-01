using Qixol.Promo.Integration.Lib.Basket;
using Qixol.Promo.Integration.Lib.Coupon;
using Qixol.Promo.Integration.Lib.Export;
using Qixol.Promo.Integration.Lib.Import;
using Qixol.Promo.Integration.Lib.Shared;
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
            textOut.AppendLine(" 5 - Retrieve Missed Promotions with Basket");
            textOut.AppendLine();

            if (!string.IsNullOrEmpty(lastResultMessage))
                textOut.AppendLine(lastResultMessage);

            textOut.AppendLine(" Select an option... Or any other key to quit.");
            textOut.AppendLine();        
            return textOut.ToString();
        }

        /// <summary>
        /// Return text to be displayed when a company key has not been specified.
        /// </summary>
        /// <returns></returns>
        public static string GetWarningMessage()
        {
            var textOut = new StringBuilder();
            textOut.AppendLine(" --------  Qixol Promo - Integration Sample  --------");
            textOut.AppendLine();
            textOut.AppendLine(" In order to run this sample project - please provide your Company Key in");
            textOut.AppendLine("  the Program.cs in the 'Sample_CompanyKey' variable.");
            textOut.AppendLine();
            textOut.AppendLine(" When you register to evaluate Qixol Promo (which is free!) - you are assigned a Company Key.");
            textOut.AppendLine();
            textOut.AppendLine(" You can find your company key by logging into The Qixol Promo Administration Portal and ");
            textOut.AppendLine("  selecting 'Configuration' from the navigation menu and then clicking 'Manage Company'.");
            textOut.AppendLine();
            textOut.AppendLine(" Press any key to quit...");
            return textOut.ToString();
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
        /// Get the display text for a ImportResponseSummary
        /// </summary>
        /// <param name="responseSummary"></param>
        /// <returns></returns>
        public static string GetErrorDisplayText(this ImportResponseSummary responseSummary)
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
                    textOut.AppendLine(string.Format("            -> {0} ({1}) - {2}", promoName, appliedPromo.InstanceId.ToString(), appliedPromo.DiscountAmount.ToString("0.00")));
                });
            });

            textOut.AppendLine(string.Format(" - Basket total discount: {0}", basketResponse.TotalDiscount.ToString("0.00")));
            textOut.AppendLine(string.Format(" - Basket total points: {0}", basketResponse.TotalIssuedPoints.ToString("0.00")));
            textOut.AppendLine(string.Format(" - Basket coupons issued: {0}", basketResponse.Coupons.Where(coupon => coupon.Issued).Count().ToString()));
            textOut.AppendLine(string.Format(" - Total promotions applied: {0}", basketResponse.Summary.AppliedPromotions.Count.ToString()));

            basketResponse.Summary.AppliedPromotions.ForEach(appliedPromo =>
            {
                textOut.AppendLine(string.Format("            -> {0} ({1}) - {2} - {3}", appliedPromo.PromotionName, appliedPromo.InstanceId.ToString(), appliedPromo.PromotionTypeDisplay, appliedPromo.DiscountAmount.ToString("0.00")));
            });

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
            return string.Format("Import Reference: {0}, {1}", productImportResponse.Reference, productImportResponse.Summary.ToMessagesString());
        }

        /// <summary>
        /// Get some display text for a COUPON VALIDATION RESPONSE to be shown in the console.
        /// </summary>
        /// <param name="coupon"></param>
        public static string GetDisplayText(this ValidatedCoupon coupon)
        {
            var textOut = new StringBuilder();
            textOut.AppendLine("Coupon Validation Response Received.");        
            textOut.AppendLine(string.Format(" - Coupon Type: {0}", coupon.CouponType));
            textOut.AppendLine(string.Format(" - Coupon Name: {0}", coupon.Name));
            textOut.AppendLine(string.Format(" - Generate on Demand?: {0}", coupon.GenerateOnDemand.ToString()));
            textOut.AppendLine(string.Format(" - Issue Only?: {0}", coupon.IssueOnly.ToString()));
            textOut.AppendLine("   Codes:");
            coupon.Codes.ForEach(code =>
            {
                textOut.AppendLine(string.Format("    - Code: {0}", code.Code));
                textOut.AppendLine(string.Format("    - Status: {0}", code.Status));
                textOut.AppendLine(string.Format("    - IsRedeemable: {0}", code.IsRedeemable.ToString()));
                textOut.AppendLine(string.Format("    - Valid From: {0}", code.ValidFrom.ToString()));
                textOut.AppendLine(string.Format("    - Valid To: {0}", code.ValidTo.ToString()));
                textOut.AppendLine(string.Format("    - Uses: {0}", code.UsesCount.ToString()));
                textOut.AppendLine(string.Format("    - Status: {0}", code.Status));
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
                textOut.AppendLine(string.Format(" - Type: {0}", promotionItem.PromotionType));
                textOut.AppendLine(string.Format(" - Valid From: {0}", promotionItem.ValidFrom.ToString()));
                textOut.AppendLine(string.Format(" - Valid To: {0}", promotionItem.ValidTo.ToString()));
                textOut.AppendLine(string.Format(" - HasAdditionalBasketRestrictions: {0}", promotionItem.HasAdditionalBasketRestrictions.ToString()));
                textOut.AppendLine(string.Format(" - HasCouponRestrictions: {0}", promotionItem.HasCouponRestrictions.ToString()));
            });
            return textOut.ToString();
        }
    }
}
