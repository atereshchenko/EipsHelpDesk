using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Entities
{
    public class FirstFilter
    {
		public int id { get; set; }
        public string name { get; set; }
        public IEnumerable<FirstFilter> GetList()
        {
            List<FirstFilter> FirstFilters = new List<FirstFilter>();
            FirstFilters.Add(new FirstFilter { id = 0, name = "все" });
            FirstFilters.Add(new FirstFilter { id = 1, name = "я инициатор" });
            FirstFilters.Add(new FirstFilter { id = 2, name = "я исполнитель" });
            return FirstFilters;
        }
    }

    public class SecondFilter
    {
        public int id { get; set; }
        public string name { get; set; }
        public IEnumerable<SecondFilter> GetList()
        {
            List<SecondFilter> SecondFilters = new List<SecondFilter>();
            SecondFilters.Add(new SecondFilter { id = 0, name = "все" });
            SecondFilters.Add(new SecondFilter { id = 1, name = "новые" });
            SecondFilters.Add(new SecondFilter { id = 2, name = "в процессе" });
            SecondFilters.Add(new SecondFilter { id = 3, name = "выполненые" });
            SecondFilters.Add(new SecondFilter { id = 4, name = "отмененные" });
            SecondFilters.Add(new SecondFilter { id = 5, name = "закрытые" });
            SecondFilters.Add(new SecondFilter { id = 6, name = "просроченные" });            
            return SecondFilters;
        }
    }

    public class ThirdFilter
    {
        public int id { get; set; }
        public string name { get; set; }
        public IEnumerable<ThirdFilter> GetList()
        {
            List<ThirdFilter> ThirdFilters = new List<ThirdFilter>();
            ThirdFilters.Add(new ThirdFilter { id = 0, name = "от новых к старым" });
            ThirdFilters.Add(new ThirdFilter { id = 1, name = "от старых к новым" });
           
            return ThirdFilters;
        }
    }
}
