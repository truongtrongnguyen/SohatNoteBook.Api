﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Entities.Dto.Generic
{
    public class PagedResult<T> : Result<List<T>>
    {
        public int Page { get; set; }
        public int ResultCount { get; set; }
        public int ResultPerPage { get; set; }
    }
}
