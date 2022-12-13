using System.Web;
using System.Web.Optimization;

namespace GM.WaTuPak.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Clear();
            bundles.ResetAll();

            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/treefilter").Include(
                    "~/Vendor/treefilter/treefilter.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                    "~/Vendor/datatable/datatables.min.js",
                    "~/Vendor/datatable/Select-1.2.4/js/dataTables.select.min.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/application").Include(
                    "~/Scripts/jquery.loading-overlay.min.js",
                    "~/Scripts/jquery.cookie.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/bootstrap-datetimepicker.js",
                    "~/Scripts/webui_popover.js",
                    "~/Scripts/GM.Main.js",
                    "~/Scripts/GM.Utility.js"
                  ));



            bundles.Add(new ScriptBundle("~/bundles/selectlist").Include(
                "~/Vendor/select2/select2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/chartjs").Include(
                    "~/vendor/chartjs/chartjs.min.js",
                    "~/vendor/chartjs/chartjs-extra.js"
            ));

            bundles.Add(new StyleBundle("~/content/cssapp").Include(
                "~/Content/bootstrap.css",
                "~/Content/alert-messag.css",
                "~/Vendor/normalize/normalize.css",
                "~/Vendor/feather_icon/css/feather_icon.css",
                "~/Vendor/treefilter/treefilter.css",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/css/main_scss.css",
                "~/Content/css/main.css"
           ));


            bundles.Add(new StyleBundle("~/content/layout2").Include(
                       "~/Content/vendors/normalize/normalize.css",

                       //<!-- Bootstrap CSS -->
                       "~/Content/vendors/bootstrap3/css/bootstrap.css",

                       //<!-- Font-icon -->
                       "~/Content/vendors/feather_icon/css/feather_icon.css",

                       //<!-- Plugin -->
                       "~/Content/vendors/treefilter/treefilter.css",
                       "~/Content/vendors/datatable/datatables.min.css",
                       "~/Content/vendors/date_time_picker/bootstrap-datetimepicker.min.css",

                       "~/Content/vendors/webui_popver/webui_popover.css",
                       "~/Content/alert-messag.css",
                       //<!-- Main -->
                       "~/Content/resources/css/main_scss.css",
                       "~/Content/resources/css/main.css"
                  ));


            //<link rel="stylesheet" href="~/Content/vendors/normalize/normalize.css">
            //<!-- Bootstrap CSS -->
            //<link rel="stylesheet" href="~/Content/vendors/bootstrap3/css/bootstrap.css">
            //<!-- Font-icon -->
            //<link rel="stylesheet" href="~/Content/vendors/feather_icon/css/feather_icon.css">
            //<!-- Plugin -->
            //<link rel="stylesheet" href="~/Content/vendors/treefilter/treefilter.css">
            //<link rel="stylesheet" href="~/Content/vendors/datatable/datatables.min.css">
            //<link rel="stylesheet" href="~/Content/vendors/date_time_picker/bootstrap-datetimepicker.min.css">
            //<!-- Main -->
            //<link rel="stylesheet" href="~/Content/resources/css/main_scss.css">
            //<link rel="stylesheet" href="~/Content/resources/css/main.css">



            bundles.Add(new StyleBundle("~/content/datatable").Include(
                 "~/Vendor/datatable/datatables.min.css",
                 "~/Vendor/datatable/Select-1.2.4/css/select.dataTables.min.css"
            ));

            bundles.Add(new StyleBundle("~/content/selectlist").Include("~/Vendor/select2/select2.min.css"));
            bundles.Add(new StyleBundle("~/bundles/fontawesome").Include(
              "~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform()
            ));

            bundles.Add(new StyleBundle("~/bundles/SweetAlertCss").Include(
             "~/Vendor/sweetalert/dist/sweetalert.css"
         ));
            bundles.Add(new StyleBundle("~/bundles/SweetAlert").Include(
                "~/Vendor/sweetalert/dist/sweetalert.min.js"
            ));


            //Module
            bundles.Add(new ScriptBundle("~/module/security").Include("~/Scripts/GM.Security.js"));

            //Module
            bundles.Add(new ScriptBundle("~/module/counterparty").Include("~/Scripts/GM.CounterParty.js"));

            //Module Screen
            bundles.Add(new ScriptBundle("~/module/screen").Include("~/Scripts/GM.Screen.js"));

            //Module Desk Group
            bundles.Add(new ScriptBundle("~/module/deskgroup").Include("~/Scripts/GM.DeskGroup.js"));

            //Module Book
            bundles.Add(new ScriptBundle("~/module/book").Include("~/Scripts/GM.Book.js"));

            //Module Trader
            bundles.Add(new ScriptBundle("~/module/trader").Include("~/Scripts/GM.Trader.js"));

            //Module User
            bundles.Add(new ScriptBundle("~/module/user").Include("~/Scripts/GM.User.js"));

            //Module Counter Party Index
            bundles.Add(new ScriptBundle("~/module/counterpartyindex").Include("~/Scripts/GM.CounterPartyIndex.js"));

            //Module Counter Party Add
            bundles.Add(new ScriptBundle("~/module/counterpartyadd").Include("~/Scripts/GM.CounterPartyAdd.js"));

            //Module Counter Party Add
            bundles.Add(new ScriptBundle("~/module/counterpartyedit").Include("~/Scripts/GM.CounterPartyEdit.js"));

            //Module Security Index
            bundles.Add(new ScriptBundle("~/module/securityindex").Include("~/Scripts/GM.SecurityIndex.js"));

            //Module Security Add
            bundles.Add(new ScriptBundle("~/module/securityadd").Include("~/Scripts/GM.SecurityAdd.js"));

            //Module Issuer Index
            bundles.Add(new ScriptBundle("~/module/issuerindex").Include("~/Scripts/GM.IssuerIndex.js"));

            //Module Issuer Add
            bundles.Add(new ScriptBundle("~/module/issueradd").Include("~/Scripts/GM.IssuerAdd.js"));

            //Module Issuer Edit
            bundles.Add(new ScriptBundle("~/module/issueredit").Include("~/Scripts/GM.IssuerEdit.js"));

            //Module Counter Party Fund Index
            bundles.Add(new ScriptBundle("~/module/counterpartyfundindex").Include("~/Scripts/GM.CounterPartyFundIndex.js"));

            //Module Counter Party Fund Add
            bundles.Add(new ScriptBundle("~/module/counterpartyfundadd").Include("~/Scripts/GM.CounterPartyFundAdd.js"));

            //Module Rp Referece
            bundles.Add(new ScriptBundle("~/module/rpreferece").Include("~/Scripts/GM.RPReferece.js"));

            //Module paymentmethod
            bundles.Add(new ScriptBundle("~/module/paymentmethod").Include("~/Scripts/GM.PaymentMethod.js"));

            //Module RPDealEntry Index
            bundles.Add(new ScriptBundle("~/module/rpdealentryindex").Include("~/Scripts/GM.RPDealEntryIndex.js"));

            //Module RPDealEntry Add
            bundles.Add(new ScriptBundle("~/module/rpdealentryadd").Include("~/Scripts/GM.RPDealEntryAdd.js"));

            //Module RPDealEntry Cancel
            bundles.Add(new ScriptBundle("~/module/rpdealentrycancel").Include("~/Scripts/GM.RPDealEntryCancel.js"));

            //Module RPDealVerify Index
            bundles.Add(new ScriptBundle("~/module/rpdealverifyindex").Include("~/Scripts/GM.RPDealVerifyIndex.js"));

            //Module RPDealVerify Approve
            bundles.Add(new ScriptBundle("~/module/rpdealverifyapprove").Include("~/Scripts/GM.RPDealVerifyApprove.js"));

            //Module RPDealApprove Index
            bundles.Add(new ScriptBundle("~/module/rpdealapproveindex").Include("~/Scripts/GM.RPDealApproveIndex.js"));

            //Module RPDealApprove Approve
            bundles.Add(new ScriptBundle("~/module/rpdealapproveapprove").Include("~/Scripts/GM.RPDealApproveApprove.js"));

            //Module RPDealSettlement Index
            bundles.Add(new ScriptBundle("~/module/rpdealsettlementindex").Include("~/Scripts/GM.RPDealSettlementIndex.js"));

            //Module RPDealSettlement Approve
            bundles.Add(new ScriptBundle("~/module/rpdealsettlementapprove").Include("~/Scripts/GM.RPDealSettlementApprove.js"));

            //Module RPDealMaturity Index
            bundles.Add(new ScriptBundle("~/module/rpdealmaturityindex").Include("~/Scripts/GM.RPDealMaturityIndex.js"));

            //Module RPDealMaturity Approve
            bundles.Add(new ScriptBundle("~/module/rpdealmaturityapprove").Include("~/Scripts/GM.RPDealMaturityApprove.js"));

            //Module RPDealSummary Index
            bundles.Add(new ScriptBundle("~/module/rpdealsummaryindex").Include("~/Scripts/GM.RPDealSummaryIndex.js"));

            //Module RPDealSummary Approve
            bundles.Add(new ScriptBundle("~/module/rpdealsummaryapprove").Include("~/Scripts/GM.RPDealSummaryApprove.js"));

            //Module Country
            bundles.Add(new ScriptBundle("~/module/country").Include("~/Scripts/GM.Country.js"));

            //Module Currency
            bundles.Add(new ScriptBundle("~/module/currency").Include("~/Scripts/GM.Currency.js"));

            //Module Role
            bundles.Add(new ScriptBundle("~/module/role").Include("~/Scripts/GM.Role.js"));

            //Module RoleScreenMapping
            bundles.Add(new ScriptBundle("~/module/rolescreenmapping").Include("~/Scripts/GM.RoleScreenMapping.js"));

            //Module Holiday
            bundles.Add(new ScriptBundle("~/module/holiday").Include("~/Scripts/GM.Holiday.js"));

            //Module RPCouponIndex
            bundles.Add(new ScriptBundle("~/module/rpcouponindex").Include("~/Scripts/GM.RPCouponIndex.js"));

            //Module RPCouponAdd
            bundles.Add(new ScriptBundle("~/module/rpcouponadd").Include("~/Scripts/GM.RPCouponAdd.js"));

            //Module RPRreleaseMessage Index
            bundles.Add(new ScriptBundle("~/module/rpreleasemsgindex").Include("~/Scripts/GM.RPRreleaseMsgIndex.js"));

            //Module RPRreleaseMessage Rrelease
            bundles.Add(new ScriptBundle("~/module/rpreleasemsgrelease").Include("~/Scripts/GM.RPRreleaseMsgRelease.js"));

            //Module RPRreleaseMessage Coupon Index
            bundles.Add(new ScriptBundle("~/module/rpreleasemsgcouponindex").Include("~/Scripts/GM.RPRreleaseMsgCouponIndex.js"));

            //Module RPRreleaseMessage Coupon Rrelease
            bundles.Add(new ScriptBundle("~/module/rpreleasemsgcouponrelease").Include("~/Scripts/GM.RPRreleaseMsgCouponRelease.js"));

            //Module RPRreleaseMessage Call Margin Index
            bundles.Add(new ScriptBundle("~/module/RPRreleaseMsgMarginIndex").Include("~/Scripts/GM.RPRreleaseMsgMarginIndex.js"));

            //Module RPRreleaseMessage Rrelease Pti Index
            bundles.Add(new ScriptBundle("~/module/RPReleaseMsgPtiIndex").Include("~/Scripts/GM.RPReleaseMsgPtiIndex.js"));

            //Module RPRreleaseMessage Rrelease Net Index
            bundles.Add(new ScriptBundle("~/module/RPReleaseMsgIndexNet").Include("~/Scripts/GM.RPReleaseMsgIndexNet.js"));
            
            //Module RPRreleaseMessage Call Margin Index
            bundles.Add(new ScriptBundle("~/module/RPRreleaseMsgInterestMarginIndex").Include("~/Scripts/GM.RPRreleaseMsgInterestMarginIndex.js"));



            //Module RPRreleaseMsg Confirm Index
            bundles.Add(new ScriptBundle("~/module/AmendConfirm").Include("~/Scripts/GM.AmendConfirm.js"));
            bundles.Add(new ScriptBundle("~/module/EarlyConfirm").Include("~/Scripts/GM.EarlyConfirm.js"));
            bundles.Add(new ScriptBundle("~/module/AmendDeal").Include("~/Scripts/GM.AmendDeal.js"));


            //Module Exchange Rate Index
            bundles.Add(new ScriptBundle("~/module/exchangerateindex").Include("~/Scripts/GM.ExchangeRateIndex.js"));
            bundles.Add(new ScriptBundle("~/module/exchangerateform").Include("~/Scripts/GM.ExchangeRateForm.js"));

            //Module Trader Limit
            bundles.Add(new ScriptBundle("~/module/traderlimit").Include("~/Scripts/GM.TraderLimit.js"));

            //Module RP Report TBMA
            bundles.Add(new ScriptBundle("~/module/rpreporttbma").Include("~/Scripts/GM.RPReportTBMA.js"));

            //Module Custodian
            bundles.Add(new ScriptBundle("~/module/custodianform").Include("~/Scripts/GM.CustodianForm.js"));

            //Module RPCallMargin Add
            bundles.Add(new ScriptBundle("~/module/rpcallmargin").Include("~/Scripts/GM.RPCallMargin.js"));

            //Module RPConfirmation Index
            bundles.Add(new ScriptBundle("~/module/rpconfirmationindex").Include("~/Scripts/GM.RPConfirmationIndex.js"));

            //Module Floating Index
            bundles.Add(new ScriptBundle("~/module/floatingindex").Include("~/Scripts/GM.FloatingIndex.js"));

            //Module RPMarginInterestindex
            bundles.Add(new ScriptBundle("~/module/RPMarginInterestindex").Include("~/Scripts/GM.RPMarginInterestIndex.js"));

            //Module RPMarginInterestedit
            bundles.Add(new ScriptBundle("~/module/RPMarginInterestedit").Include("~/Scripts/GM.RPMarginInterestEdit.js"));

            //Module GLAccountCode
            bundles.Add(new ScriptBundle("~/module/glaccountcode").Include("~/Scripts/GM.GLAccountCode.js"));

            //Module GLGenerate
            bundles.Add(new ScriptBundle("~/module/glgenerate").Include("~/Scripts/GM.GLGenerate.js"));

            //Module RPMarginInterestedit
            bundles.Add(new ScriptBundle("~/module/MarketPriceBBG").Include("~/Scripts/GM.MarketPriceBBG.js"));

            //support lt IE9 browser
            bundles.Add(new ScriptBundle("~/content/oldbrowser").Include(
                   "~/Vendor/html5shiv/html5shiv.js",
                   "~/Vendor/respond/respond.min.js"
                ));
            //BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/module/policyrate").Include("~/Scripts/GM.PolicyRate.js"));

            bundles.Add(new ScriptBundle("~/module/EODReconcile").Include("~/Scripts/GM.EODReconcile.js"));

            bundles.Add(new ScriptBundle("~/module/StockReconcile").Include("~/Scripts/GM.StockReconcile.js"));

            bundles.Add(new ScriptBundle("~/module/TraderLimitConfig").Include("~/Scripts/GM.TraderLimitConfig.js"));

            bundles.Add(new ScriptBundle("~/module/RPMarginInterestFCY").Include("~/Scripts/GM.RPMarginInterestFCY.js"));

            bundles.Add(new ScriptBundle("~/module/Activity").Include("~/Scripts/GM.Activity.js"));

            bundles.Add(new ScriptBundle("~/module/ThorRate").Include("~/Scripts/GM.ThorRate.js"));

            bundles.Add(new ScriptBundle("~/module/ThorIndex").Include("~/Scripts/GM.ThorIndex.js"));

            bundles.Add(new StyleBundle("~/content/dassboard").Include("~/Content/scss/_dashboard.css"));

            //Validation
            bundles.Add(new ScriptBundle("~/module/validation").Include("~/Scripts/GM.Validation.js"));

            //Constant
            bundles.Add(new ScriptBundle("~/module/Constant").Include("~/Scripts/GM.Constant.js"));

        }
    }
}
