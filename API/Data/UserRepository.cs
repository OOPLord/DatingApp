using API.DTOs;
using API.Entity;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Data context
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">Data context</param>
        /// <param name="mapper">Mapper</param>
        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all users async.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Photos)
                .ToListAsync();
        }

        /// <summary>
        /// Returns a single user by if async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FindAsync(id);
        }

        /// <summary>
        /// Returns a single user by user name async
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(user => user.UserName.Equals(username));
        }

        /// <summary>
        /// Updates user state
        /// </summary>
        /// <param name="user"></param>
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        /// <summary>
        /// Returns a collection of <see cref="MemberDto"/> objects async
        /// </summary>
        /// <returns></returns>
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(user => user.UserName != userParams.CurrentUserName);
            query = query.Where(user => user.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddDays(-userParams.MinAge);

            query = query.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);

            // ToDo: filter

            return await PagedList<MemberDto>.CreateAsync(
                query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber,
                userParams.PageSize);
        }

        /// <summary>
        /// Returns a <see cref="MemberDto"/> object async
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(user => user.UserName.Equals(username))
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<string> GetUserGender(string userName)
        {
            return await _context.Users
                .Where(user => user.UserName == userName)
                .Select(user => user.Gender)
                .FirstOrDefaultAsync();
        }
    }
}
