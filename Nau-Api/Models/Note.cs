using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api.Models
{
    public class Note
    {
        public string note_id { get; set; }
        public DateTime created_at { get; set; }
        public string content { get; set; }
    }
}