using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
  public class CurrentUser
  {
    public class Query : IRequest<User> { }

    public class Handler : IRequestHandler<Query, User>
    {
      private readonly UserManager<AppUser> _userManager;
      private readonly IJwtGenerator _jwtGenerator;
      private readonly IUserAccessor _userAccessor;
      public Handler(UserManager<AppUser> userManager,
        IJwtGenerator jwtGenerator,
        IUserAccessor userAccessor)
      {
        _jwtGenerator = jwtGenerator;
        _userManager = userManager;
        _userAccessor = userAccessor;
      }

      public async Task<User> Handle(Query request,
        CancellationToken cancellationToken)
      {
        var user = await _userManager.FindByNameAsync(
            _userAccessor.GetCurrentUsername());

        return new User {
            DisplayName = user.DisplayName,
            Token = _jwtGenerator.CreateToken(user),
            Username = user.UserName,
            Image = null
        };
      }
    }
  }
}
