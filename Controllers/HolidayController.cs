using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using GM.Data.View.Static;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class HolidayController : BaseController
    {
        // GET: Holiday
        StaticEntities api_static = new StaticEntities();
        static int add = 0;
        static string year = string.Empty;
        static string cur = string.Empty;

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            ResultWithModel<HolidayResult> result = new ResultWithModel<HolidayResult>();

            HolidayViewModel holidayViewModel = new HolidayViewModel();
            holidayViewModel.FormAction = new HolidayModel();
            holidayViewModel.FormSearch = new HolidayModel();
            holidayViewModel.FormSearch.year = DateTime.Parse(DateTime.Now.ToString(), new CultureInfo("en-US")).Year.ToString();
            holidayViewModel.FormSearch.yearCalendar = holidayViewModel.FormSearch.year;
            holidayViewModel.FormSearch.yearEvent = holidayViewModel.FormSearch.year;
            holidayViewModel.FormSearch.cur = "THB";
            holidayViewModel.FormAction.cur = "THB";
            cur = "THB";
            api_static.Holiday.GetHolidayList(holidayViewModel.FormSearch, p =>
            {
                result = p;
            });
            holidayViewModel.FormSearch.TotalEventDays = result.Data.HolidayResultModel.Count();
            holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav table-content tab-pane in active";
            holidayViewModel.FormSearch.ModeEvent = "box-content have-nav tab-pane in";
            holidayViewModel.FormSearch.Tap = true;
            holidayViewModel.FormCalendar = PrepareCalendar(Convert.ToInt32(holidayViewModel.FormSearch.year), result.Data.HolidayResultModel);
            holidayViewModel.FormEvent = PrepareEvent(result.Data.HolidayResultModel);
            holidayViewModel.FormAction.HolidayType = true;
            year = holidayViewModel.FormSearch.year;
            holidayViewModel.FormCalendar.Tap = true;
            holidayViewModel.FormEvent.Tap = false;
            return View(holidayViewModel);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index(HolidayViewModel holidayViewModel, FormCollection collection)
        {
            try
            {
                holidayViewModel.FormAction = new HolidayModel();
                ResultWithModel<HolidayResult> result = new ResultWithModel<HolidayResult>();

                //Start Sanhalak(MAX) Check Active tab when select currency 10/09/2018
                if (holidayViewModel.FormCalendar.Tap)
                {
                    holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav table-content tab-pane in active";
                    holidayViewModel.FormSearch.ModeEvent = "box-content have-nav tab-pane in";
                }
                else
                {
                    holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav tab-pane in";
                    holidayViewModel.FormSearch.ModeEvent = "box-content have-nav table-content tab-pane in active";

                }
                //End Sanhalak(MAX) Check Active tab when select currency 10/09/2018

                if (collection["FormSearch.cur"] != null && cur != holidayViewModel.FormSearch.cur)
                {
                    holidayViewModel.FormAction.cur = holidayViewModel.FormSearch.cur;
                    cur = holidayViewModel.FormSearch.cur;
                }
                else if (collection["btnNext1"] != null)
                {
                    add += 1;
                    holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav table-content tab-pane in active";
                    holidayViewModel.FormSearch.ModeEvent = "box-content have-nav tab-pane in";

                    holidayViewModel.FormCalendar.Tap = true;
                    holidayViewModel.FormEvent.Tap = false;
                }
                else if (collection["btnPrv1"] != null)
                {
                    add -= 1;
                    holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav table-content tab-pane in active";
                    holidayViewModel.FormSearch.ModeEvent = "box-content have-nav tab-pane in";

                    holidayViewModel.FormCalendar.Tap = true;
                    holidayViewModel.FormEvent.Tap = false;
                }
                else if (collection["btnNext2"] != null)
                {
                    add += 1;
                    holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav tab-pane in";
                    holidayViewModel.FormSearch.ModeEvent = "box-content have-nav table-content tab-pane in active";


                    holidayViewModel.FormCalendar.Tap = false;
                    holidayViewModel.FormEvent.Tap = true;
                }
                else if (collection["btnPrv2"] != null)
                {
                    add -= 1;
                    holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav tab-pane in";
                    holidayViewModel.FormSearch.ModeEvent = "box-content have-nav table-content tab-pane in active";

                    holidayViewModel.FormCalendar.Tap = false;
                    holidayViewModel.FormEvent.Tap = true;
                }
                else
                {
                    if (holidayViewModel.FormEvent.Tap)
                    {
                        holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav tab-pane in";
                        holidayViewModel.FormSearch.ModeEvent = "box-content have-nav table-content tab-pane in active";

                        holidayViewModel.FormCalendar.Tap = false;
                        holidayViewModel.FormEvent.Tap = true;
                    }
                    else
                    {
                        holidayViewModel.FormSearch.ModeCalendar = "box-content have-nav table-content tab-pane in active";
                        holidayViewModel.FormSearch.ModeEvent = "box-content have-nav tab-pane in";

                        holidayViewModel.FormCalendar.Tap = true;
                        holidayViewModel.FormEvent.Tap = false;
                    }

                    //if (holidayViewModel.FormSearch.yearCalendar == holidayViewModel.FormSearch.year
                    //    && holidayViewModel.FormSearch.yearEvent != holidayViewModel.FormSearch.year)
                    //{
                    //    holidayViewModel.FormSearch.year = holidayViewModel.FormSearch.yearCalendar;
                    //    add = 0;
                    //}
                    //else if (holidayViewModel.FormSearch.yearEvent == holidayViewModel.FormSearch.year
                    //    && holidayViewModel.FormSearch.yearCalendar != holidayViewModel.FormSearch.year)
                    //{
                    //    holidayViewModel.FormSearch.year = holidayViewModel.FormSearch.yearEvent;
                    //    add = 0;
                    //}
                    //else
                    if (holidayViewModel.FormSearch.year == year)
                    {
                        add = 0;
                    }

                    //holidayViewModel.FormSearch.year = (Convert.ToInt32(holidayViewModel.FormSearch.year) + add).ToString();
                    //year = holidayViewModel.FormSearch.year;
                    //holidayViewModel.FormSearch.yearCalendar = holidayViewModel.FormSearch.year;
                    //holidayViewModel.FormSearch.yearEvent = holidayViewModel.FormSearch.year;

                    api_static.Holiday.GetHolidayList(holidayViewModel.FormSearch, p =>
                    {
                        result = p;
                    });

                    holidayViewModel.FormSearch.TotalEventDays = result.Data.HolidayResultModel.Count();
                    holidayViewModel.FormCalendar = PrepareCalendar(Convert.ToInt32(holidayViewModel.FormSearch.year), result.Data.HolidayResultModel, holidayViewModel.FormCalendar.Tap);
                    holidayViewModel.FormEvent = PrepareEvent(result.Data.HolidayResultModel, holidayViewModel.FormEvent.Tap);
                    return View(holidayViewModel);
                }

                //holidayViewModel.FormSearch.year = (Convert.ToInt32(holidayViewModel.FormSearch.year) + add).ToString();
                //year = holidayViewModel.FormSearch.year;
                //holidayViewModel.FormSearch.yearCalendar = holidayViewModel.FormSearch.year;
                //holidayViewModel.FormSearch.yearEvent = holidayViewModel.FormSearch.year;

                api_static.Holiday.GetHolidayList(holidayViewModel.FormSearch, p =>
                {
                    result = p;
                });
                holidayViewModel.FormSearch.TotalEventDays = result.Data.HolidayResultModel.Count();
                holidayViewModel.FormCalendar = PrepareCalendar(Convert.ToInt32(holidayViewModel.FormSearch.year), result.Data.HolidayResultModel, holidayViewModel.FormCalendar.Tap);
                holidayViewModel.FormEvent = PrepareEvent(result.Data.HolidayResultModel, holidayViewModel.FormEvent.Tap);
                return View(holidayViewModel);
            }
            catch (Exception)
            {
                return View(holidayViewModel);
            }
        }

        public class Data
        {
            public string cur { get; set; }
            public string Year { get; set; }
        }

        public ActionResult FillCur(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Holiday.GetDDLCurrency(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        string[] MonthsOfTheYear = new string[] { "January", "February", "March",
            "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        private HolidayModel PrepareCalendar(int year, List<HolidayModel> result, bool active = false)
        {
            HolidayModel model = new HolidayModel();
            model.year = year.ToString();
            model.Tap = active;
            EventYear eventYear = new EventYear();

            for (int i = 0; i < 12; i++)
            {
                EventMonth month = new EventMonth();
                month.Name = MonthsOfTheYear[i];
                for (int x = 0; x < 6; x++)
                {
                    EventWeek week = new EventWeek();
                    for (int y = 0; y < 7; y++)
                    {
                        week.Days.Add(new EventDay());
                    }
                    month.Weeks.Add(week);
                }
                eventYear.Months.Add(month);
            }

            for (int month = 1; month < 13; month++)
            {
                foreach (var day in Enumerable.Range(1, DateTime.DaysInMonth(year, month)))
                {
                    DateTime dt = new DateTime(year, month, day);
                    int weekOfMonth = dt.GetWeekOfMonth() - 1;
                    int dayOfWeek = ((int)dt.DayOfWeek);
                    int monthOfYear = month - 1;
                    if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        eventYear.Months[monthOfYear].Weeks[weekOfMonth].Days[dayOfWeek].Mode = "mute";
                    }
                    else
                    {
                        HolidayModel tmpResuult = result.Where(x => x.holiday_date == dt).FirstOrDefault();
                        if (tmpResuult != null)
                        {
                            eventYear.Months[monthOfYear].Weeks[weekOfMonth].Days[dayOfWeek].Mode = "active show-popover";
                            eventYear.Months[monthOfYear].Weeks[weekOfMonth].Days[dayOfWeek].Description = tmpResuult.holiday_desc;

                        }
                        else
                        {
                            eventYear.Months[monthOfYear].Weeks[weekOfMonth].Days[dayOfWeek].Mode = "show-popover";
                        }
                        eventYear.Months[monthOfYear].Weeks[weekOfMonth].Days[dayOfWeek].Date = dt.Date.ToString("dd MMMM yyyy", new CultureInfo("en-US"));
                        eventYear.Months[monthOfYear].Weeks[weekOfMonth].Days[dayOfWeek].Key = dt.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                    }
                    eventYear.Months[monthOfYear].Weeks[weekOfMonth].Days[dayOfWeek].Day = dt.Day.ToString();
                }
            }

            model.EventYear = eventYear;
            return model;
        }

        private HolidayModel PrepareEvent(List<HolidayModel> result, bool active = false)
        {
            HolidayModel model = new HolidayModel();
            model.year = year.ToString();
            model.Tap = active;
            EventYear eventYear = new EventYear();
            EventMonth eventMonth;
            if (result.Count > 0)
            {
                result = result.OrderBy(o => o.holiday_date).ToList();
                for (int i = 1; i < 13; i++)
                {
                    eventMonth = new EventMonth();
                    foreach (var tmp in result.Where(x => x.holiday_date.Value.Month == i))
                    {
                        EventDay eventDay = new EventDay();
                        eventDay.Date = tmp.holiday_date.Value.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                        eventDay.Description = tmp.holiday_desc;
                        eventDay.Key = tmp.holiday_date.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                        eventMonth.Name = MonthsOfTheYear[i - 1];
                        eventMonth.Days.Add(eventDay);
                    }
                    if (eventMonth.Days.Count() > 0)
                    {
                        eventYear.Months.Add(eventMonth);
                    }
                }
            }
            model.EventYear = eventYear;
            return model;
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(HolidayViewModel view)
        {
            var rwm = new ResultWithModel<HolidayResult>();
            view.FormAction.create_by = HttpContext.User.Identity.Name;
            List<HolidayModel> listInsert = new List<HolidayModel>();
            view.FormAction.cur = cur;
            if (view.FormAction.holiday_date == null && view.FormAction.HolidayEnd != null && view.FormAction.HolidayStart != null)
            {
                int loop = view.FormAction.HolidayEnd.Value.DayOfYear - view.FormAction.HolidayStart.Value.DayOfYear;
                for (int i = 0; i <= loop; i++)
                {
                    HolidayModel model = new HolidayModel();
                    model.cur = view.FormAction.cur;
                    model.holiday_date = view.FormAction.HolidayStart.Value.AddDays(i);
                    model.holiday_desc = view.FormAction.holiday_desc;
                    model.create_by = view.FormAction.create_by;
                    if ((int)model.holiday_date.Value.DayOfWeek != 0 && (int)model.holiday_date.Value.DayOfWeek != 6)
                    {
                        listInsert.Add(model);
                    }
                }
            }
            else
            {
                if (view.FormAction.holiday_date != null && (int)view.FormAction.holiday_date.Value.DayOfWeek != 0
                    && (int)view.FormAction.holiday_date.Value.DayOfWeek != 6)
                {
                    listInsert.Add(view.FormAction);
                }
            }

            try
            {
                if (listInsert.Count() > 0)
                {
                    if (ModelState.IsValid)
                    {
                        api_static.Holiday.CreateHoliday(listInsert, p =>
                        {
                            rwm = p;
                        });

                        if (!rwm.Success)
                        {
                            ModelState.AddModelError("", rwm.Message);
                        }
                    }
                    else
                    {
                        var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                        Models.ForEach(field =>
                        {
                            field.Errors.ToList().ForEach(error =>
                            {
                                rwm.Message += error.ErrorMessage;
                            });
                        });
                    }
                }
                else
                {
                    rwm.Message = "Not Insert Saturday And Sunday";
                }
            }
            catch (Exception ex)
            {
                rwm.Message = ex.Message;
            }

            return Json(rwm, JsonRequestBehavior.AllowGet);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string date)
        {
            HolidayModel model = new HolidayModel();
            ResultWithModel<HolidayResult> rwm = new ResultWithModel<HolidayResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.holiday_date = DateTime.ParseExact(date, "dd/MM/yyyy", new CultureInfo("en-US"));

            model.cur = cur;
            api_static.Holiday.GetHolidayEdit(model, p =>
            {
                rwm = p;
            });
            return Json((rwm.Data.HolidayResultModel.Count > 0 ? rwm.Data.HolidayResultModel[0] : new HolidayModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(HolidayViewModel view)
        {
            var rwm = new ResultWithModel<HolidayResult>();
            view.FormAction.update_by = HttpContext.User.Identity.Name;

            try
            {
                if (ModelState.IsValid)
                {
                    api_static.Holiday.UpdateHoliday(view.FormAction, p =>
                    {
                        rwm = p;
                    });
                    if (!rwm.Success)
                    {
                        ModelState.AddModelError("", rwm.Message);
                    }
                }
                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                    Models.ForEach(field =>
                    {
                        field.Errors.ToList().ForEach(error =>
                        {
                            rwm.Message += error.ErrorMessage;
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                rwm.Message = ex.Message;
            }
            return Json(rwm, JsonRequestBehavior.AllowGet);
        }

        [RoleScreen(RoleScreen.DELETE)]
        public ActionResult Delete(string date)
        {
            HolidayModel model = new HolidayModel();
            ResultWithModel<HolidayResult> rwm = new ResultWithModel<HolidayResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.holiday_date = DateTime.ParseExact(date, "dd/MM/yyyy", new CultureInfo("en-US"));
            model.cur = cur;
            api_static.Holiday.GetHolidayEdit(model, p =>
            {
                rwm = p;
            });
            return Json((rwm.Data.HolidayResultModel.Count > 0 ? rwm.Data.HolidayResultModel[0] : new HolidayModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public ActionResult Delete(HolidayViewModel view)
        {
            var rwm = new ResultWithModel<HolidayResult>();
            view.FormAction.update_by = HttpContext.User.Identity.Name;
            try
            {
                if (ModelState.IsValid)
                {
                    api_static.Holiday.DeleteHoliday(view.FormAction, p =>
                    {
                        rwm = p;
                    });
                    if (!rwm.Success)
                    {
                        ModelState.AddModelError("", rwm.Message);
                    }
                }
                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                    Models.ForEach(field =>
                    {
                        field.Errors.ToList().ForEach(error =>
                        {
                            rwm.Message += error.ErrorMessage;
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                rwm.Message = ex.Message;
            }
            return Json(rwm, JsonRequestBehavior.AllowGet);
        }
    }

    static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}