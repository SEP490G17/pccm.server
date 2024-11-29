

using Domain.Entity;

namespace Application.SpecParams.NotificationSpecification
{
    public class NotificationCountSpecification : BaseSpecification<Notification>
    {
        public NotificationCountSpecification(string userName) : base
        (
            x => x.AppUser.UserName.Equals(userName)
        )
        {
        }
    }

}