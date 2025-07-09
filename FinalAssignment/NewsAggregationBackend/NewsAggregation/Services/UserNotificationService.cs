using AutoMapper;
using NewsAggregation.Configurations;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

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

        protected virtual int UserId => _requestContext.UserId!.Value;

        public async Task<UserNotificationDto> AddAsync(UserNotificationDto userNotificationDto)
        {
            if (userNotificationDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = _mapper.Map<UserNotification>(userNotificationDto);

            await _crudBaseRepository.AddAsync(entity);
            await _crudBaseRepository.SaveChangesAsync();

            userNotificationDto.UserNotificationId = entity.UserNotificationId;
            return userNotificationDto;
        }

        public async Task AddRangeAsync(IEnumerable<UserNotification> notifications)
        {
            if (notifications == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            await _userNotificationRepository.AddRangeAsync(notifications);
            await _crudBaseRepository.SaveChangesAsync();
        }

        public async Task<UserNotificationDto> UpdateAsync(int id, UserNotificationDto userNotificationDto)
        {
            if (userNotificationDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            _mapper.Map(userNotificationDto, entity);

            await _crudBaseRepository.UpdateAsync(entity);
            await _crudBaseRepository.SaveChangesAsync();

            userNotificationDto.UserNotificationId = entity.UserNotificationId;
            return userNotificationDto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            await _crudBaseRepository.DeleteAsync(id);
            await _crudBaseRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserNotificationDto>> GetAllUserNotificationAsync()
        {
            var entities = await _userNotificationRepository.GetAllUserNotificationAsync(UserId);
            if (entities == null || !entities.Any())
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            IEnumerable<UserNotificationDto> notifications = _mapper.Map<IEnumerable<UserNotificationDto>>(entities);
            await _userNotificationRepository.MarkAllUserNotificationsAsRead(UserId);
            return notifications;
        }
    }
}
