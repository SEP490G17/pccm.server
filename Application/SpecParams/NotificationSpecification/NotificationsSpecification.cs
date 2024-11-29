

using Domain.Entity;

namespace Application.SpecParams.NotificationSpecification
{
    public class NotificationsSpecification : BaseSpecification<Notification>
    {
        public NotificationsSpecification(BaseSpecParam baseSpecParam, string userName) : base(
            x => x.AppUser.UserName.Equals(userName)
        )
        {
            ApplyPaging(baseSpecParam.Skip, baseSpecParam.PageSize);
            AddOrderByDescending(x => x.CreateAt);
        }
    }
}