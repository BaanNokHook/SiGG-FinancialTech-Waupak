using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Model.UserAndScreen;
using GM.Data.Result.Static;
using GM.Data.Result.UserAndScreen;
using GM.Data.View.UserAndScreen;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web.Security;

namespace GM.Application.Web.Controllers
{

    [Authorize]
    [Audit]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (User == null)
            {
                return RedirectToAction("Index", "Login");
            }

            UserAndScreenEntities api = new UserAndScreenEntities();
            RoleTaskModel itemSearch = new RoleTaskModel();
            itemSearch.role_id = Convert.ToInt32(User.RoleId);
            itemSearch.BusinessDate = Getbusinessdate();
            ResultWithModel<RoleTaskResult> result = new ResultWithModel<RoleTaskResult>();

            api.RoleTask.GetRoleTaskList(itemSearch, p =>
            {
                result = p;
            });

            RoleTaskViewModel viewRoleTask = new RoleTaskViewModel();
            if (result.Data != null)
            {
                foreach (var resultItem in result.Data.RoleTaskResultModel)
                {
                    if (viewRoleTask.Datas.Exists(x => x.task_name == resultItem.task_name))
                    {
                        viewRoleTask.Datas.Where(x => x.task_name == resultItem.task_name)
                            .Select(S => { S.remaining += resultItem.remaining; S.complete += resultItem.complete; return S; }).ToList();
                    }
                    else
                    {
                        if (resultItem.remaining > 0 || resultItem.complete > 0)
                            viewRoleTask.Datas.Add(resultItem);
                    }
                }

                foreach (var item in viewRoleTask.Datas)
                {
                    if (item.task_enable)
                    {
                        if (item.task_condition.ToUpper() == "MO")
                        {
                            viewRoleTask.MO_REMAINING_JOB += item.remaining;
                            viewRoleTask.MO_COMPLETE_JOB += item.complete;
                        }
                        else if (item.task_name.ToUpper().StartsWith("FO"))
                        {
                            viewRoleTask.FO_REMAINING_JOB += item.remaining;
                            viewRoleTask.FO_COMPLETE_JOB += item.complete;
                        }
                        else
                        {
                            viewRoleTask.BO_REMAINING_JOB += item.remaining;
                            viewRoleTask.BO_COMPLETE_JOB += item.complete;
                        }
                    }
                }
            }

            return View(viewRoleTask);
        }

        public DateTime Getbusinessdate()
        {
            StaticEntities db = new StaticEntities();
            ResultWithModel<BusinessDateResult> rwm = new ResultWithModel<BusinessDateResult>();
            BusinessDateModel BusinessDateModel = new BusinessDateModel();
            DateTime Date = new DateTime();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = 1;
            paging.RecordPerPage = 20;
            BusinessDateModel.paging = paging;

            db.BusinessDate.GetBusinessDateList(BusinessDateModel, p =>
            {
                rwm = p;
            });

            if (rwm.Success)
            {
                string StrDate = string.Empty;
                List<BusinessDateModel> ListData = rwm.Data.BusinessDateResultModel;
                Date = rwm.Data.BusinessDateResultModel[0].business_date;
            }
            return Date;
        }
    }
}