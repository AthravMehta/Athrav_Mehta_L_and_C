using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using System.Collections.Generic;

namespace NewsAggregation.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly ICrudBaseRepository<UserNotification> _crudBaseRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly RequestContext _requestContext;
        private readonly IMapper _mapper;

        public UserNotificationService(
            ICrudBaseRepository<UserNotification> crudBaseRepository,
            IUserNotificationRepository userNotificationRepository,
            RequestContext requestContext,
            IMapper mapper)
        {
            _crudBaseRepository = crudBaseRepository ?? throw new ArgumentNullException(nameof(crudBaseRepository));
            _userNotificationRepository = userNotificationRepository ?? throw new ArgumentNullException(nameof(userNotificationRepository));
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserNotificationDto> AddAsync(UserNotificationDto userNotificationDto)
        {
            if (userNotificationDto == null)
                throw new ArgumentNullException(nameof(userNotificationDto));

            try
            {
                var entity = _mapper.Map<UserNotification>(userNotificationDto);

                await _crudBaseRepository.AddAsync(entity);
                await _crudBaseRepository.SaveChangesAsync();

                userNotificationDto.UserNotificationId = entity.UserNotificationId;
                return userNotificationDto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding user notification.", ex);
            }
        }

        public async Task AddRangeAsync(IEnumerable<UserNotification> notifications)
        {
            if (notifications == null)
                throw new ArgumentNullException(nameof(notifications));

            try
            {
                await _userNotificationRepository.AddRangeAsync(notifications);
                await _crudBaseRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding range of user notifications.", ex);
            }
        }

        public async Task<UserNotificationDto> UpdateAsync(int id, UserNotificationDto userNotificationDto)
        {
            if (userNotificationDto == null)
                throw new ArgumentNullException(nameof(userNotificationDto));

            try
            {
                var entity = await _crudBaseRepository.GetByIdAsync(id);
                if (entity == null)
                    throw new KeyNotFoundException($"UserNotification with id {id} not found.");

                _mapper.Map(userNotificationDto, entity);

                await _crudBaseRepository.UpdateAsync(entity);
                await _crudBaseRepository.SaveChangesAsync();

                userNotificationDto.UserNotificationId = entity.UserNotificationId;
                return userNotificationDto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating user notification with id {id}.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _crudBaseRepository.GetByIdAsync(id);
                if (entity == null)
                    throw new KeyNotFoundException($"UserNotification with id {id} not found.");

                await _crudBaseRepository.DeleteAsync(id);
                await _crudBaseRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting user notification with id {id}.", ex);
            }
        }

        public async Task<IEnumerable<UserNotificationDto>> GetAllUserNotificationAsync()
        {
            try
            {
                var entities = await _userNotificationRepository.GetAllUserNotificationAsync(_requestContext.UserId);
                IEnumerable < UserNotificationDto > notifications = _mapper.Map<IEnumerable<UserNotificationDto>>(entities);
                await _userNotificationRepository.MarkAllUserNotificationsAsRead(_requestContext.UserId);
                return notifications;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving user notifications.", ex);
            }
        }
    }
}
