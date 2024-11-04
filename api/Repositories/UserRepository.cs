﻿using api.Data;
using api.Dtos.Product;
using api.Dtos.User;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Utilities;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;
        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<UserDTO>> GetAllAsync(UserQueryDTO query)
        {
            IQueryable<User> users = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                users = users.Where(p => p.Name.Contains(query.Name));
            }

            int totalElements = await users.CountAsync();

            int recordsSkipped = (query.PageNumber - 1) * query.PageSize;

            var result = await users.Skip(recordsSkipped).Take(query.PageSize).ToListAsync();
            var resultDto = result.Select(p => p.ToUserDto()).ToList();

            return new PaginatedResponse<UserDTO>(resultDto, query.PageNumber, query.PageSize, totalElements);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<User?> UpdateAsync(int id, UpdateUserRequestDTO userDTO)
        {
            User? existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (existingUser == null || (existingUser != null && existingUser.Status == false))
            {
                return null;
            }

            existingUser.Email = userDTO.Name;
            existingUser.Password = userDTO.Password;
            existingUser.Name = userDTO.Name;
            existingUser.Address = userDTO.Address;
            existingUser.PhoneNumber = userDTO.PhoneNumber;

            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<User?> DeleteAsync(int id)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null || (user != null && user.Status == false))
            {
                return null;
            }

            //_context.Products.Remove(product);
            user.Status = false;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
