using Bison.Models;

using Microsoft.EntityFrameworkCore;

namespace Bison.Database;

public class DatabaseRepository(BisonDBContext dBContext) : IDatabaseRepository
{
    private readonly BisonDBContext _dBContext = dBContext;

    public async Task<User> CreateUser(string name)
    {
        var res = _dBContext.Users.Add(new User() { Name = name });
        await _dBContext.SaveChangesAsync();
        return res.Entity;
    }
    public async Task<List<User>> GetAllUsers()
    {
        var query = from user in _dBContext.Users
                    select user;
        return await query.ToListAsync();
    }

    public async Task<Observation?> GetObservation(int id)
    {
        var query = from obs in _dBContext.Obserations
                    where obs.Id == id
                    select obs;
        var res = await query.ToListAsync();

        return res.Count == 0 ? null : res[0];
    }
}
