using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UniqChat.Data;
using UniqChat.Models;

namespace UniqChat.MapService
{
    public class CMS
    {
        //CONNECTION MAPPING SERVICE, MAPS JWTTOKEN TO THE CONNECTION ID 
        public interface IConnectionMappingService
        {
            void AddConnection(string jwtToken, string connectionId);
            IEnumerable<string> GetConnections(string jwtToken);
            void RemoveConnection(string jwtToken, string connectionId);
            IEnumerable<string> GetAllConnectedUsernames();
        }
        public class ConnectionMappingService : IConnectionMappingService
        {
            private readonly ConcurrentDictionary<string, HashSet<string>> _connections =
            new ConcurrentDictionary<string, HashSet<string>>();
            public List<string> _usernames = new List<string>();
            private readonly DatabaseContext _db;
            public ConnectionMappingService(DatabaseContext db)
            {
                _db = db;
            }
            //ADD CONNECTIONS
            public void AddConnection(string jwtToken, string connectionId)
            {

                _connections.AddOrUpdate(jwtToken, new HashSet<string> { connectionId }, (_, connections) =>
                {
                    connections.Add(connectionId);
                    return connections;
                });
            }
            //REMOVE CONNECTIONS
            public void RemoveConnection(string jwtToken, string connectionId)
            {
                _connections.AddOrUpdate(jwtToken, new HashSet<string>(), (_, connections) =>
                {
                    connections.Remove(connectionId);

                    return connections;
                });
            }
            //GET CONNECTIONS
            public IEnumerable<string> GetConnections(string jwtToken)
            {
                _connections.TryGetValue(jwtToken, out var connections);
                return connections ?? new HashSet<string>();
            }
            //GET CONNECTED USERS TO LIST
            public IEnumerable<string> GetAllConnectedUsernames()
            {
                foreach (var kvp in _connections)
                {
                    string jwtToken = kvp.Key;
                    string ConnectionId = kvp.Value.Single();
                    try
                    {
                        // Parse the JWT token to extract the username
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadJwtToken(jwtToken);
                        var nameClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                        if (nameClaim != null)
                        {
                            var username = nameClaim.Value;
                            // Check if the username already exists in the database
                            var existingUser = _db.AddAllConnectedUsers.FirstOrDefault(u => u.username == username);
                            if (existingUser == null)
                            {
                                _db.AddAllConnectedUsers.Add(new AddAllConnectedUsers
                                {
                                    username = username,
                                    connectionId = ConnectionId,
                                    jwtToken = jwtToken
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing token: {ex.Message}");
                    }
                }
                _db.SaveChanges(); 
                // Add all usernames from the database to _usernames
                _usernames.AddRange(_db.AddAllConnectedUsers.Select(user => user.username));
                return _usernames;
            }
        }
    }
}
