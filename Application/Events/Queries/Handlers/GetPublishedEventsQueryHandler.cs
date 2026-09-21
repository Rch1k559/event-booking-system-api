using Application.Common.Intefaces;
using Application.Common.Models;
using Application.Events.DTOs;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Queries.Handlers
{
    public class GetPublishedEventsQueryHandler : IRequestHandler<GetPublishedEventsQuery, PagedResult<EventListItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPublishedEventsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<EventListItemDto>> Handle(GetPublishedEventsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Event> query = _context.Events.AsNoTracking().Where(e => e.Status == StatusEvent.Published);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(e => e.Title.Contains(request.SearchTerm) || e.Location.Contains(request.SearchTerm));
            }
            if (request.MinPrice.HasValue)
            {
                query = query.Where(e => e.TicketTypes.Any(tt => tt.Price >= request.MinPrice.Value));
            }
            if (request.MaxPrice.HasValue)
            {
                query = query.Where(e => e.TicketTypes.Any(tt => tt.Price <= request.MaxPrice.Value));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query.OrderBy(e => e.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new EventListItemDto(
                e.Id,
                e.Title,
                e.Date,
                e.Location,
                e.TicketTypes.Min(tt => tt.Price)
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<EventListItemDto>(
                Page: request.Page,
                PageSize: request.PageSize,
                TotalCount: totalCount,
                Items: items
                );
        }
    }
}
