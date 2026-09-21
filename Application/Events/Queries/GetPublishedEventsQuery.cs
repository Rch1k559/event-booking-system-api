using Application.Common.Models;
using Application.Events.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Queries
{
    public record GetPublishedEventsQuery(string? SearchTerm, string? Category, decimal? MinPrice, decimal? MaxPrice, int Page = 1, int PageSize = 10) : IRequest<PagedResult<EventListItemDto>>;
}
