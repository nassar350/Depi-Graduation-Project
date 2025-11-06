using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Eventify.APIs.DTOs.Tickets;
using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Eventify.Service.Interfaces;


namespace Eventify.Service.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repo;
        private readonly IMapper _mapper;
        private readonly EventifyContext _context;

        public TicketService(ITicketRepository repo, IMapper mapper, EventifyContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<TicketDto>> GetAllAsync()
        {
            var tickets = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }

        public async Task<TicketDetailsDto?> GetByIdAsync(int id)
        {
            var ticket = await _repo.GetByIdAsync(id);
            return _mapper.Map<TicketDetailsDto>(ticket);
        }

        public async Task<TicketDto> CreateAsync(CreateTicketDto dto)
        {
            var ticket = _mapper.Map<Ticket>(dto);
            await _repo.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return _mapper.Map<TicketDto>(ticket);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTicketDto dto)
        {
            var ticket = await _repo.GetByIdAsync(id);
            if (ticket == null) return false;

            _mapper.Map(dto, ticket);
            _repo.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _repo.GetByIdAsync(id);
            if (ticket == null) return false;

            _repo.Delete(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
