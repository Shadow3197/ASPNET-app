using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using API.Interfaces;

namespace API.SignalR
{
  [Authorize]
  public class PresenceHub : Hub
  {
    private readonly PresenceTracker _tracker;
    private readonly IUnitOfWork _unitOfWork;
    public PresenceHub(PresenceTracker tracker, IUnitOfWork unitOfWork)
    {
      _tracker = tracker;
      _unitOfWork = unitOfWork;
    }

    public override async Task OnConnectedAsync()
    {
      var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

      var unreadMessages = await _unitOfWork.MessageRepository.GetUnReadMessageCount(Context.User.GetUsername());
      if(isOnline)
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
        await Clients.Caller.SendAsync("UnreadCount", unreadMessages.Count);
      var currentUsers = await _tracker.GetOnlineUsers();
      await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
      if(isOffline)
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

      await base.OnDisconnectedAsync(exception);
    }
  }
}