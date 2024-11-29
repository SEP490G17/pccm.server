
using Domain.Entity;

namespace Application.SpecParams.NotificationSpecification
{
    public class NotificationCountUnreadSpecification : BaseSpecification<Notification>
    {
        public NotificationCountUnreadSpecification(string userName) : base
        (
            x => x.AppUser.UserName.Equals(userName) && (x.ReadAt == null)
        )
        {
        }
    }
}