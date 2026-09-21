using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Models
{
    public record PagedResult<T>(int Page, int PageSize, int TotalCount, List<T> Items)
    {
        // Математический трюк для округления вверх при целочисленном делении: 
        // Вместо Math.Ceiling((double)TotalCount / PageSize) используется формула (TotalCount + PageSize - 1) / PageSize. 
        // Это работает быстрее и не требует приведения типов к double/float
        public int TotalPages => PageSize > 0 ? (TotalCount + PageSize - 1) / PageSize : 0;
    }
}
