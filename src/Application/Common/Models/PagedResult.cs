using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Models
{
    public record PagedResult<T>(int Page, int PageSize, int TotalCount, List<T> Items)
    {
        // Integer division ceiling formula: 
        // Uses (TotalCount + PageSize - 1) / PageSize instead of Math.Ceiling((double)TotalCount / PageSize). 
        // Avoids floating-point conversions and allocation overhead while rounding up correctly.
        public int TotalPages => PageSize > 0 ? (TotalCount + PageSize - 1) / PageSize : 0;
    }
}
