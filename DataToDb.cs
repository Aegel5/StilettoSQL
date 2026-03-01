using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL;

// Сконвертированные данные, которые отправляются в провайдер

public class DataToDb {
    public enum CustomType { None, Json }
    public object data;
    public CustomType customType = CustomType.None;
}
