using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PokerPlanner.Entities.DTO;

namespace PokerPlanner.API.Hubs
{
    public class GameHub : Hub
    {
        // User Joins
        public Task PlayerJoined(UserDto userDto = null)
        {
            // Check if guest
            if (userDto == null)
            {
                // User is a guest, there will be no creator

            }
        }
    }
}