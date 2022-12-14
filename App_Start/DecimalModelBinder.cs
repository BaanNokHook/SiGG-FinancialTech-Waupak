using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace GM.WaTuPak.Web.App_Start
{
    public class DecimalModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == null)
            {
                return 0;
            }
            else
            {
                ModelState modelState = new ModelState { Value = valueResult };

                string[] arr = ((IEnumerable)valueResult.RawValue).Cast<object>()
                                    .Select(x => x.ToString())
                                    .ToArray();

                object actualValue = null;

                if (arr[0].Replace(",", "") != string.Empty)
                {
                    try
                    {
                        actualValue = Convert.ToDecimal(arr[0].Replace(",", ""), CultureInfo.CurrentCulture);
                    }
                    catch (FormatException e)
                    {
                        modelState.Errors.Add(e);
                    }
                    catch (Exception ex)
                    {
                        modelState.Errors.Add(ex);
                    }
                }

                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
                return actualValue;
            }
        }
    }

    // OLD
    //public class DecimalModelBinder : System.Web.Mvc.DefaultModelBinder
    //{
    //    public override object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
    //    {
    //        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
    //        var modelState = new System.Web.Mvc.ModelState { Value = valueResult };
    //        decimal actualValue = 0;

    //        try
    //        {
    //            actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
    //                CultureInfo.CurrentCulture);
    //        }
    //        catch (FormatException e)
    //        {
    //            if (bindingContext.ModelMetadata.IsRequired)
    //            modelState.Errors.Add(e);
    //        }

    //        bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
    //        return actualValue;
    //    }
    //}
}