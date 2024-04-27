using ArbiDataLib.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ArbiReader.ModelBinders
{
    public class ArbiFilterModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(ArbiFilter))
            {
                return new ArbiFilterModelBinder();
            }
            return null;
        }
    }
    public class ArbiFilterModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext, nameof(bindingContext));

            var model = new ArbiFilter();
            bool parsed = false;
            if (TryParseDoubleValue(bindingContext.ValueProvider, "price", out double minPrice))
            {
                model.MinPrice = minPrice;
                parsed = true;
            }

            if (TryParseDoubleValue(bindingContext.ValueProvider, "askVol", out double askVol))
            {
                model.MinAskVolumeUSDT = askVol;
                parsed = true;
            }

            if (TryParseDoubleValue(bindingContext.ValueProvider, "bidVol", out double bidVol))
            {
                model.MinBidVolumeUSDT = bidVol;
                parsed = true;
            }

            if (TryParseDoubleValue(bindingContext.ValueProvider, "askDayVol", out double askDayVol))
            {
                model.MinAskDayVolumeUSDT = askDayVol;
                parsed = true;
            }

            if (TryParseDoubleValue(bindingContext.ValueProvider, "bidDayVol", out double bidDayVol))
            {
                model.MinBidDayVolumeUSDT = bidDayVol;
                parsed = true;
            }

            if (TryParseDoubleValue(bindingContext.ValueProvider, "minP", out double minP))
            {
                model.MinPercent = minP;
                parsed = true;
            }

            if (TryParseDoubleValue(bindingContext.ValueProvider, "maxP", out double maxP))
            {
                model.MaxPercent = maxP;
                parsed = true;
            }

            if (TryParseIntValue(bindingContext.ValueProvider, "num", out int num))
            {
                model.Amount = num;
                parsed = true;
            }

            if (TryParseStringValue(bindingContext.ValueProvider, "buy", out string buy))
            {
                model.ForbiddenBuy = buy;
                parsed = true;
            }

            if (TryParseStringValue(bindingContext.ValueProvider, "sell", out string sell))
            {
                model.ForbiddenSell = sell;
                parsed = true;
            }

            if(parsed)
            {
                bindingContext.Result = ModelBindingResult.Success(model);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }

        private bool TryParseStringValue(IValueProvider valueProvider, string key, out string result)
        {
            var value = valueProvider.GetValue(key);
            if (value == ValueProviderResult.None)
            {
                result = string.Empty;
                return false;
            }

            string? res = value.FirstValue;
            if(string.IsNullOrEmpty(res))
            {
                result = string.Empty;
                return false;
            }
            result = res;
            return true;
        }


        private bool TryParseIntValue(IValueProvider valueProvider, string key, out int result)
        {
            var value = valueProvider.GetValue(key);
            if (value == ValueProviderResult.None)
            {
                result = 0;
                return false;
            }

            if (!int.TryParse(value.FirstValue, out result))
            {
                return false;
            }

            return true;
        }

        private bool TryParseDoubleValue(IValueProvider valueProvider, string key, out double result)
        {
            var value = valueProvider.GetValue(key);
            if (value == ValueProviderResult.None)
            {
                result = 0.0;
                return false;
            }

            if (!double.TryParse(value.FirstValue, out result))
            {
                return false;
            }

            return true;
        }
    }
}
