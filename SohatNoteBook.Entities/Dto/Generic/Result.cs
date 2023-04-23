using SohatNoteBook.Entities.Dto.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Entities.Dto.Generic
{
    public class Result<T>      // Single item return
    {
        public T Content { get; set; }
        public Error Error { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    }
}
