using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1.Models
{
    /// <summary>
    /// Stores Consultant data. Consultant is 'user' in the database.
    /// </summary>
    public class Consultant
    {
        public int Id;
        public string Name;

        public Consultant(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    
}
