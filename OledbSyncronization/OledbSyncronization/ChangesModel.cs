using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OledbSyncronization
{
    public class ChangesModel
    {
      // public string GUID { get; set; }
       public string Direction { get; set; }
       public string ColumnName { get; set; }
       public string OldValue {  get; set; }  
       public string NewValue { get; set; }
       public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
