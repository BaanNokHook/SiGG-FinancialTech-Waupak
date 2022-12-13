using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace GM.WaTuPak.Web.App_Start
{
    public class IntModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult != null)
            {
                var modelState = new System.Web.Mvc.ModelState { Value = valueResult };
                object actualValue = null;
                string[] arr = ((IEnumerable)valueResult.RawValue).Cast<object>()
                                     .Select(x => x.ToString())
                                     .ToArray();

                string value = string.Empty;
                try
                {
                    if (arr[0] != "")
                    {
                        value = arr[0];
                        if (value.Contains("."))
                        {
                            value = value.Substring(0, valueResult.AttemptedValue.IndexOf('.'));
                        }
                        value = value.Replace(",", "");
                    }
                    else if (valueResult.AttemptedValue != "")
                    {
                        value = valueResult.AttemptedValue;
                        if (value.Contains("."))
                        {
                            value = value.Substring(0, valueResult.AttemptedValue.IndexOf('.'));
                        }
                        value = value.Replace(",", "");
                    }

                    //if (bindingContext.ModelMetadata.IsRequired)
                    //{
                    //    if (arr[0] == "" && valueResult.AttemptedValue == "")
                    //    {

                    //    }
                    //    else
                    //    {
                    //        actualValue = Convert.ToInt32(value, CultureInfo.CurrentCulture);
                    //    }
                    //}

                    //actualValue = Convert.ToInt32(value,CultureInfo.CurrentCulture);
                    actualValue = string.IsNullOrEmpty(value) ? bindingContext.Model : int.Parse(value);
                }
                catch (FormatException e)
                {
                    if (bindingContext.ModelMetadata.IsRequired)
                        modelState.Errors.Add(e);
                }
                catch (Exception ex)
                {
                    modelState.Errors.Add(ex);
                }

                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

                return actualValue;
            }
            else
                return 0;
        }
    }
}