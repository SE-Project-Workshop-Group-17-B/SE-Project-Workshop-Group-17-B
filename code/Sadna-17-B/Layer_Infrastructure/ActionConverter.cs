using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Web;
using Action = Sadna_17_B.Layer_Infrastructure.Config.Action;

namespace Sadna_17_B.Layer_Infrastructure
{


    public class ActionConverter : JsonConverter<Action>
    {
        public override Action Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var jsonObject = jsonDoc.RootElement;

                var actionName = jsonObject.GetProperty("action name").GetString();


                Action action;
                switch (actionName)
                {
                    case "register admin":
                        action = JsonSerializer.Deserialize<Config.Action_register>(jsonObject.GetRawText(), options);
                        break;
                    case "register subscriber":
                        action = JsonSerializer.Deserialize<Config.Action_register>(jsonObject.GetRawText(), options);
                        break;
                    case "open store":
                        action = JsonSerializer.Deserialize<Config.Action_open_store>(jsonObject.GetRawText(), options);
                        break;
                    case "add product":
                        action = JsonSerializer.Deserialize<Config.Action_add_product>(jsonObject.GetRawText(), options);
                        break;
                    case "assign manager":
                        action = JsonSerializer.Deserialize<Config.Action_assign_manager>(jsonObject.GetRawText(), options);
                        break;
                    case "assign owner":
                        action = JsonSerializer.Deserialize<Config.Action_assign_owner>(jsonObject.GetRawText(), options);
                        break;
                    default:
                        action = null;
                        break;
                }



                if (action == null)
                    throw new Exception("config error");

                return action;
            }
        }

        public override void Write(Utf8JsonWriter writer, Action value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }


}