using API.DTOs;
using API.Entities;
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
    public class MessageRepository : IMessageRespository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(group => group.Connections)
                .Where(group => group.Connections.Any(connection => connection.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(message => message.Sender)
                .Include(message => message.Recipient)
                .SingleOrDefaultAsync(message => message.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(group => group.Connections)
                .FirstOrDefaultAsync(group => group.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(message => message.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            // filter

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName,
            string recipientUserName)
        {
            var messages = await _context.Messages
                .Where(message => message.Recipient.UserName == currentUserName && message.RecipientDeleted == false
                    && message.Sender.UserName == recipientUserName
                    || message.Recipient.UserName == recipientUserName
                    && message.Sender.UserName == currentUserName && message.SenderDeleted == false)
                .OrderBy(message => message.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var unreadMessages = messages.Where(message => message.DateRead == null
                && message.RecipientUserName == currentUserName)
                .ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
    }
}
