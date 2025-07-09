using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OledbSyncronization
{
    public class ChangesModel
    {
        public string ColumnName { get; set; }
        string OldValue {  get; set; }  
        string NewValue { get; set; }
    }
}
