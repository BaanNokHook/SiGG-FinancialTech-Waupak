using System;
using System.Collections.Generic;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Report;
using GM.Data.Model.Static;
using GM.Data.Result.Static;

namespace GM.Application.Web.Areas.Report.Models
{
    public class ReportEntitiesModel
    {
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities api_report = new ReportEntities();

        public List<DDLItemModel> Getreportname(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Config.GetReportID(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });
            return res;
        }

        public List<ConfigModel> GetReportHeader(string item_code)
        {
            var res = new List<ConfigModel>();
            api_report.ReportData.ReportHeader("REPORT_HEADER", item_code, p =>
            {
                if (p.Success) res = p.Data.ConfigResultModel;
            });
            return res;
        }

        public bool CheckDateTradeSettleMaturity(ReportCriteriaModel reportCriteriaModel)
        {
            if (string.IsNullOrEmpty(reportCriteriaModel.trade_date_from_string) &&
                string.IsNullOrEmpty(reportCriteriaModel.settlement_date_from_string) &&
                string.IsNullOrEmpty(reportCriteriaModel.maturity_date_from_string)) return false;
            return true;
        }

        public DateTime? Getbusinessdate()
        {
            try
            {
                var db = new StaticEntities();
                var rwm = new ResultWithModel<BusinessDateResult>();
                var BusinessDateModel = new BusinessDateModel();
                DateTime? Date = new DateTime();

                //Add Paging
                var paging = new PagingModel();
                paging.PageNumber = 1;
                paging.RecordPerPage = 20;
                BusinessDateModel.paging = paging;

                db.BusinessDate.GetBusinessDateList(BusinessDateModel, p => { rwm = p; });

                if (rwm.Success)
                {
                    var StrDate = string.Empty;
                    var ListData = rwm.Data.BusinessDateResultModel;
                    Date = rwm.Data.BusinessDateResultModel[0].business_date;
                }
                else
                {
                    Date = null;
                }

                return Date;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}