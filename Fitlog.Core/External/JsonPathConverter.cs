using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Fitlog.External
{
    public class JsonPathConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var data = JObject.Load(reader);
            var @object = Activator.CreateInstance(objectType);

            foreach (var prop in @object.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
            {
                var attr = prop.GetCustomAttributes(false).FirstOrDefault();
                if (attr != null)
                {
                    var propName = ((JsonPropertyAttribute)attr).PropertyName;
                    if (!string.IsNullOrWhiteSpace(propName))
                    {
                        //split by the delimiter, and traverse recursevly according to the path
                        var conventions = propName.Split('.');
                        object propValue = null;
                        JToken token = null;
                        for (var i = 0; i < conventions.Length; i++)
                        {
                            if (token == null)
                            {
                                token = data[conventions[i]];
                            }
                            else
                            {
                                token = token[conventions[i]];
                            }
                            if (token == null)
                            {
                                //silent fail: exit the loop if the specified path was not found
                                break;
                            }
                            else
                            {
                                //store the current value
                                if (token is JValue)
                                {
                                    propValue = ((JValue)token).Value;
                                }
                            }
                        }

                        if (propValue != null)
                        {
                            //workaround for numeric values being automatically created as Int64 (long) objects.
                            if (propValue is long && prop.PropertyType == typeof(Int32))
                            {
                                prop.SetValue(@object, Convert.ToInt32(propValue));
                            }
                            if(propValue != null && (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?)))
                            {
                                prop.SetValue(@object, Convert.ToDecimal(propValue));
                            }
                            else
                            {
                                prop.SetValue(@object, propValue);
                            }
                        }
                    }
                }
            }
            return @object;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
