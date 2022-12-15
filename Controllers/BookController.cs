using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.UserAndScreen;
using GM.Data.Result.UserAndScreen;
using GM.Data.View.UserAndScreen;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers  
{
      [Authorize]  
      [Audit]  
      public class BookController : BaseController  
      {
            UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();

            [RoleScreen(RoleScreen.VIEW)]   
            public ActionResult Index()
            {
                  BookViewModel bookViewModel = new BookViewModel();  
                  bookViewModel.FormAction = new BookModel();  
                  return View(bookViewModel);   
            }  

            [HttpPost]  
            public ActionResult Search(DataTableAjaxPostModel model)   
            {
                  string searchvalue = Request["search[value]"];
                  ResultWithModel<BookResult> result = new ResultWithModel<BookResult>();  
                  BookModel bookModel = new BookModel();  
                  //Add Paging  
                  PagingModel paging = new PagingModel();   
                  paging.PageNumber = model.pageno;  
                  paging.RecordPerPage = model.length;  
                  bookModel.paging = paging;   

                  //Add Orderby  
                  var orders = new List<OrderByModel>();   

                  if (model.order != null)  
                  {
                        model.order.ForEach(o =>  
                        {
                              var col = model.columns[o.column];    
                              orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });   
                        });     
                  }   

                  bookModel.ordersby = orders;   

                  var columns = model.columns.Where(o => o.search.value != null).ToList();   
                  columns.ForEach(column => 
                  {
                        switch (column.data)   
                        {
                              case "user_id":   
                                 bookModel.user_id = column.search.value;   
                                 break;   
                              case "book_name_en":  
                                 bookModel.book_name_en = column.search.value;  
                                 break;  
                              case "port":  
                                 bookModel.port = column.search.value;   
                                 break;  
                              case "active_flag":
                                 bookModel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");    
                                 break;  
                        }
                });  

                api_userandscreen.Book.GetBookList(bookModel, p => {  
                    result = p;  
                });    
                return Json(new  
                {
                    draw = model.draw,  
                    recordsTotal = result.HowManyRecord,  
                    recordsFiltered = result.HowManyRecord,   
                    data = result.Data != null ? result.Data.BookResultModel : new List<BookModel>()   
                });   
            }

            [HttpPost]   
            [RoleScreen(RoleScreen.CREATE)]   
            public ActionResult Create(BookViewModel view)   
            {
                var rwm = new ResultWithModel<BookResult>();   
                view.FormAction.create_by = HttpContext.User.Identity.Name;   
                view.FormAction.user_id = HttpContext.User.Identity.Name;   
                try
                {
                    if (ModelState.IsValid)
                    {
                        api_userandscreen.Book.CreateBook(view.FormAction, p => {
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

        [RoleScreen(RoleScreen.VIEW)]   
        public ActionResult Edit(string id)   
        {
            BookModel model = new BookModel();   
         
            ResultWithModel<BookResult> result = new ResultWithModel<BookResult>();
            Paging.PageNumber = 0;  
            paging.RecordPerPage = 0;    
            model.paging = paging;  
            model.book_id = int.Parse(id);   
            model.create_by = HttpContext.User.identity.Name;   

            api_userandscreen.Book.GetBookEdit(model, p => { 
                result = p;
            });
            //return View(model);  
            return Json((result.Data.BookResultModel.Count > 0 ? result.Data.BookResultModel[0] : new BookModel()), JsonRequestBehavior.AllowGet);   
        }   

        [HttpPost]  
        [RoleScreen(RoleScreen.EDIT)]    
        public ActionResult Edit(BookViewModel view)   
        {
            var rwm = new ResultWithModel<BookResult>();   
            view.FormAction.create_by = HttpContext.User.Identity.Name;
            view.FormAction.user_id = HttpContext.User.Identity.Name;    
            try  
            {
                if (ModelState.IsValid)    
                {

                    api_userandscreen.Book.UpdateBook(view.FormAction, p => {
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

        public class Data  
        {
            public int bookid { get; set; }
        }  

        [HttpPost]   
        [RoleScreen(RoleScreen.DELETE)]  
        public JsonResult Deletes(Data data)   
        {
            var rwm = new ResultWithModel<BookResult>();   
            BookModel view = new BookModel();   
            view.create_by = HttpContext.User.Identity.Name;  
            view.book_id = data.bookid;     
            try
            {
                api_userandscreen.Book.DeleteBook(view, p =>  
                {
                    rwm = p;   
                });   

                if (rwm.Success)   
                {
                    return Json(new { success = true, responseText = "Your message successfully sent!" }, JsonRequestBehavior.AllowGet);    
                }  

                else  
                {
                    ModelState.AddModelError("", rwm.Message);   
                    return Json(new { success = false, responseText = rwm.Message }, JsonRequestBehavior.AllowGet);    
                }  
            }  
            catch (Exception ex)   
            {
                return Json(new { success = false, responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);    
                // return View();   
            }
            // return Json(rwm, JsonRequestBehavior.AllowGet);   
        }  

        #region Dropdown  
        public ActionResult FillPort(string datastr)  
        {
            List<DDLItemModel> res = new List<DDLItemModel>();   
            api_userandscreen.Book.GetDDLPortfolio(datastr, p => {
                if (p.Success)    
                {
                    res = p.Data.DDLItems;  
                }  
            });   

            return Json(res, JsonRequestBehavior.AllowGet);   
        }
        #endRegion
      }
}

