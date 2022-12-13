using GM.Helper;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Globalization;
using System.Web.Mvc;

namespace GM.WaTuPak.Web.App_Start
{
    public class DateTimeModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var displayFormat = bindingContext.ModelMetadata.DisplayFormatString;
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            AppSettingsReader settingsReader;

            string DefaultDateFormat = "{0:dd/MM/yyyy}";
            string DefaultDateTimeFormat = "{0:dd/MM/yyyy HH:mm:ss}";

            settingsReader = new AppSettingsReader();

            try
            {
                DefaultDateFormat = (string)settingsReader.GetValue("DefaultDateFormat", typeof(String));
            }
            catch (Exception ex) { Log.Error(ex.Message); }

            try
            {
                DefaultDateTimeFormat = (string)settingsReader.GetValue("DefaultDateTimeFormat", typeof(String));
            }
            catch (Exception ex) { Log.Error(ex.Message); }

            if (value != null && !string.IsNullOrEmpty(value.AttemptedValue))
            {
                if (string.IsNullOrEmpty(displayFormat))
                {
                    DateTime date;

                    //displayFormat = "{0:dd/MM/yyyy HH:mm:ss}";
                    //displayFormat = displayFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);
                    DefaultDateTimeFormat = DefaultDateTimeFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);

                    // use the format specified in the DisplayFormat attribute to parse the date
                    if (DateTime.TryParseExact(value.AttemptedValue, DefaultDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        return date;
                    }
                    else
                    {
                        //displayFormat   = "{0:dd/MM/yyyy}";
                        //displayFormat   = displayFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);
                        DefaultDateFormat = DefaultDateFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);

                        if (DateTime.TryParseExact(value.AttemptedValue, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            return date;
                        }
                        else
                        {

                            if (value.AttemptedValue.Contains("Date"))
                            {
                                return JsonConvert.DeserializeObject<DateTime>(@"""" + value.AttemptedValue + @"""").AddDays(1);
                            }

                            if (value.AttemptedValue == "Invalid date")
                            {
                                return new DateTime();
                            }

                            if (!string.IsNullOrEmpty(value.AttemptedValue)
                                && !value.AttemptedValue.Contains("undefined"))
                            {
                                string[] tmp = value.AttemptedValue.Split('/');
                                int dd = System.Convert.ToInt32(tmp[1]);
                                int mm = System.Convert.ToInt32(tmp[0]);
                                int yyyy = System.Convert.ToInt32(tmp[2][0].ToString() + tmp[2][1].ToString() + tmp[2][2].ToString() + tmp[2][3].ToString());
                                return new DateTime(yyyy, mm, dd);
                            }
                            bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("{0} is an invalid date format", value.AttemptedValue));
                        }
                    }
                }

                else
                {
                    DateTime date;
                    displayFormat = displayFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);
                    // use the format specified in the DisplayFormat attribute to parse the date
                    if (DateTime.TryParseExact(value.AttemptedValue, displayFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        return date;
                    }
                    else
                    {

                        bindingContext.ModelState.AddModelError(
                            bindingContext.ModelName,
                            string.Format("{0} is an invalid date format", value.AttemptedValue)
                        );
                    }
                }


            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}